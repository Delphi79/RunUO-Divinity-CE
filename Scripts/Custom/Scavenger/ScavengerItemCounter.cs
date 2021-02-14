//*********************************************************************
//*	Scavenger Hunt File: ScavengerItemCounter.cs
//*
//*	Author: FrayedString
//*
//*
//*
//*	Description: Tracks the number of scavenger items in the world.
//*		Also used in deleting the remaining items at the end of the event,
//*		by storing them in their own collection.
//*     Intended for GM use to determine when to end the event (based on
//*     the number of remaining items).
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
using Server;

namespace Server.Items
{
	public class ScavengerItemCounter : Item
	{
		private DateTime lastused = DateTime.Now;
		private static TimeSpan delay = TimeSpan.FromSeconds( 2 );

		public override string DefaultName { get { return "Scavenger Item Counter"; } }

		[Constructable]
		public ScavengerItemCounter()
			: base( 6254 )
		{
			Weight = 0;

			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( from.AccessLevel == AccessLevel.Player && !ScavengerSignup.allowCountStones )
				this.Delete();  //NO CHEATING ALLOWED

			if( IsChildOf( from.Backpack ) )
			{
				if( lastused + delay > DateTime.Now )
					return;

				lastused = DateTime.Now;

				if( !ScavengerSignup.Started )
				{
					from.SendMessage( "The hunt hasn't started yet!" );
				}
				else if( ScavengerSignup.ScavengerItems.Count <= 0 )
				{
					from.SendMessage( "All of the items have been found" );
				}
				else if( ScavengerSignup.ScavengerItems.Count == 1 )
				{
					from.SendMessage( "There is 1 item remaining" );
				}
				else
				{
					from.SendMessage( "There are " + ScavengerSignup.ScavengerItems.Count.ToString() + " items remaining" );
				}
			}
		}

		public ScavengerItemCounter( Serial serial )
			: base( serial )
		{
			lastused = DateTime.Now;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}