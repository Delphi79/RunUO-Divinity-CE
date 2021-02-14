//*********************************************************************
//*	Scavenger Hunt File: ScavengerSignup.cs
//*
//*	Author: FrayedString
//*
//*
//*
//*	Description: Doubleclicking this "signup" stone when enabled will give
//*		the player a signup gump which promps them about entry into the event.
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
using System.Collections.Generic;
using Server;
using Server.Gumps;

namespace Server.Items
{
	public class ScavengerSignup : Item
	{
		//For future reference, these should be private but with public accessors/properties. But ASayre is too lazy to change :P
		public static bool Started = false;
		public static bool signupEnabled = false;
		public static bool allowCountStones = false;

		public static int firstPlacePay;
		public static int secondPlacePay;
		public static int thirdPlacePay;
		public static int baseItemPay;

		public static List<ScavengerBasket> ScavengerBaskets = new List<ScavengerBasket>();
		public static List<ScavengerItem> ScavengerItems = new List<ScavengerItem>();

		public override string DefaultName { get { return "Scavenger Hunt Signup Stone"; } }

		[Constructable]
		public ScavengerSignup()
			: base( 3805 )
		{
			Weight = 0;
			Movable = false;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowCountStones
		{
			get { return allowCountStones; }
			set { allowCountStones = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool SignupEnabled
		{
			get { return signupEnabled; }
			set { signupEnabled = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PerItemPayout
		{
			get { return baseItemPay; }
			set
			{
				if( value >= 0 && value <= 10000 )
					baseItemPay = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int firstPlaceBasePay
		{
			get { return firstPlacePay; }
			set
			{
				if( value >= 0 && value <= 100000 )
				{
					firstPlacePay = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int secondPlaceBasePay
		{
			get { return secondPlacePay; }
			set
			{
				if( value >= 0 && value <= 100000 )
				{
					secondPlacePay = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int thirdPlaceBasePay
		{
			get { return thirdPlacePay; }
			set
			{
				if( value >= 0 && value <= 100000 )
				{
					thirdPlacePay = value;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( (from.AccessLevel == AccessLevel.Player || from.AccessLevel > AccessLevel.Counselor) && from.InRange( this, 2 ) && signupEnabled )
			{
				if( from.Backpack.FindItemByType( typeof( ScavengerBasket ) ) == null )
				{
					if( !from.HasGump( typeof( ScavengerSignupGump ) ) )
						from.SendGump( new ScavengerSignupGump() );
				}
				else
				{
					from.SendMessage( "You are already entered in the scavenger hunt." );
				}
			}
			else if( !from.InRange( this, 2 ) )
			{
				from.SendLocalizedMessage( 500312 ); //You cannot reach that
			}
			else if( !signupEnabled )
			{
				from.SendMessage( "Signup for the scavenger hunt is not currently enabled" );
			}
			else if( from.AccessLevel > AccessLevel.Player )
			{
				from.SendMessage( "Staff are not allowed to participate in the scavenger hunt" );
			}
			else
			{
				from.SendMessage( "For some unforseen reason you are unable to sign up for this event. (notify developer to investigate)" );
			}
		}

		public ScavengerSignup( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );

			writer.Write( Started );
			writer.Write( signupEnabled );
			writer.Write( allowCountStones );

			writer.Write( firstPlacePay );
			writer.Write( secondPlacePay );
			writer.Write( thirdPlacePay );
			writer.Write( baseItemPay );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			Started = reader.ReadBool();
			signupEnabled = reader.ReadBool();
			allowCountStones = reader.ReadBool();

			firstPlacePay = reader.ReadInt();
			secondPlacePay = reader.ReadInt();
			thirdPlacePay = reader.ReadInt();
			baseItemPay = reader.ReadInt();
		}
	}
}
