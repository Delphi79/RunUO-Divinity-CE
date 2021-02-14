//*********************************************************************
//*	Scavenger Hunt File: ScavengerBasket.cs
//*
//*	Author: FrayedString
//*
//*
//*
//*	Description: Item script for creating Scavenger Hunt baskets.
//*		By double clicking the basket and targeting a scavenger
//*		hunt item, the item is placed in the player's basket. 
//*
//*	Scavenger Hunt includes the following files:
//*		- ScavengerBasket.cs		- ScavengerCmd.cs
//*		- ScavengerItemCounter.cs	- ScangerItems.cs
//*		- ScavengerSignup.cs		- ScavengerSignupGump.cs
//*     - ScavengerREADME.txt       - ScavengerLicense.txt
//*     - ScavengerCHANGELOG.txt
//*
//*********************************************************************

using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class ScavengerBasket : Item
	{
		private PlayerMobile m_Owner;
		private int m_items;
		private DateTime m_LastAcquiredItem;

		private DateTime lastused = DateTime.Now;
		private static TimeSpan delay = TimeSpan.FromSeconds( 2 );

		public override string DefaultName { get { return "A Scavenger Basket"; } }

		public ScavengerBasket( PlayerMobile owner )
			: base( 0x24DA )
		{
			Weight = 0;
			Hue = 31;
			LootType = LootType.Blessed;
			Movable = false;
			m_Owner = owner;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( lastused + delay > DateTime.Now )
				return;

			//ASayre note: Don't need the else cause it'll drop down into here anyways

			lastused = DateTime.Now;

			if( IsChildOf( from.Backpack ) )
			{
				from.Target = new InternalTarget( this );
                from.RevealingAction();
			}
			else
			{
				from.SendLocalizedMessage( 1047012 );	//This must be in your backpack bla bla
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public PlayerMobile Owner
		{
			get { return m_Owner; }
		}

		public DateTime LastAcquiredItem
		{
			get { return m_LastAcquiredItem; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ItemCount
		{
			get { return m_items; }
			set
			{
				m_items = value;
				InvalidateProperties();
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1060658, "Scavenger Items Found\t{0}", m_items );
		}

		public override void OnDelete()
		{
			if( ScavengerSignup.ScavengerBaskets.Contains( this ) )
				ScavengerSignup.ScavengerBaskets.Remove( this );
		}


		public ScavengerBasket( Serial serial )
			: base( serial )
		{
			//No need for the conditional since we KNOW the List isn't persisted across restarts.
			//if(!ScavengerSignup.ScavengerBaskets.Contains(this))
			ScavengerSignup.ScavengerBaskets.Add( this );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 );

			writer.Write( m_LastAcquiredItem );
			writer.WriteMobile<PlayerMobile>( m_Owner );
			writer.Write( m_items );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch( version )
			{
				case 1:
					{
						m_LastAcquiredItem = reader.ReadDateTime();
						goto case 0;
					}
				case 0:
					{
						m_Owner = reader.ReadMobile<PlayerMobile>();
						m_items = reader.ReadInt();
						break;
					}
			}
		}


		private class InternalTarget : Target
		{
			private ScavengerBasket theBasket;

			public InternalTarget( ScavengerBasket Basket )
				: base( 2, false, TargetFlags.None )
			{
				//Redundant
				//CheckLOS = true;

				theBasket = Basket;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				if( target != null )
				{
					if( target is ScavengerItem )
					{
						from.SendMessage( "You put the item in your basket for safekeeping" );
						((ScavengerItem)target).Delete();

						theBasket.m_items++;
						theBasket.InvalidateProperties();
						theBasket.m_LastAcquiredItem = DateTime.Now;

                        from.RevealingAction();
					}
					else
					{
						from.SendMessage( "That's not a scavenger hunt item!" );
					}
				}
			}
		}
	}
}
