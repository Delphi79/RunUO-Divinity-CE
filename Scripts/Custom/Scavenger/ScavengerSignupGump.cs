//*********************************************************************
//*	Scavenger Hunt File: ScavengerSignupGump.cs
//*
//*	Author: FrayedString
//*
//*
//*
//*	Description: The gump displayed when a player double clicks on 
//*	     the scavenger signup stone.  Promps for entry into the event.
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
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class ScavengerSignupGump : Gump
	{
		public ScavengerSignupGump()
			: base( 200, 200 )
		{
			this.Closable = false;
			this.Disposable = true;
			this.Dragable = false;
			this.Resizable = false;
			this.AddPage( 0 );
			this.AddBackground( 0, 0, 420, 215, 9270 );
			this.AddAlphaRegion( 10, 10, 400, 195 );

			this.AddLabel( 135, 10, 1153, "Scavenger Hunt Signup" );

			this.AddLabel( 15, 38, 56, "You will be supplied with a Scavenger Basket." );
			this.AddLabel( 15, 58, 56, "Use the basket to gather the items that have been" );
			this.AddLabel( 15, 78, 56, "scattered throughout the world.  When the event is completed" );
			this.AddLabel( 15, 98, 56, "you will be rewarded based on the number of items you found." );

			this.AddRadio( 20, 155, 209, 208, true, (int)Buttons.rdoYesJoin );
			this.AddLabel( 45, 155, 1153, "Yes, I want to participate." );
			this.AddRadio( 20, 180, 209, 208, false, (int)Buttons.rdoNoJoin );
			this.AddLabel( 45, 180, 1153, "No, I do not want to participate." );

			this.AddButton( 344, 180, 247, 248, (int)Buttons.exit, GumpButtonType.Reply, 0 );

		}

		public enum Buttons
		{
			exit,
			rdoYesJoin,
			rdoNoJoin
		}


		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if( info.IsSwitched( (int)Buttons.rdoYesJoin ) )
			{
				if( ScavengerSignup.signupEnabled && from is PlayerMobile )
				{
					ScavengerBasket newBasket = new ScavengerBasket( (PlayerMobile)from );
					if( !from.Backpack.CheckHold( from, newBasket, false ) )
					{
						from.SendMessage( "Your backpack is too full to even consider entering this event!" );
						newBasket.Delete();
						return;
					}

					from.AddToBackpack( newBasket );
					ScavengerSignup.ScavengerBaskets.Add( newBasket );
					from.SendMessage( "You are now entered in the scavenger hunt. Use the supplied basket to gather the scavenger items!" );
				}
				else
				{
					from.SendMessage( "You have waited too long and signup for the scavenger hunt has ended" );
				}
			}
			else
			{
				from.SendMessage( "You have not been entered into the scavenger hunt event" );
			}
		}
	}
}