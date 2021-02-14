//*********************************************************************
//*	Scavenger Hunt File: ScavengerCmd.cs
//*
//*	Author: FrayedString
//*
//*
//*
//*	Description: Creates the commands to start and stop the hunt.
//*		Starting makes all scavenger items in the world visible.
//*		Stopping deletes all remaining scavenger items, scavenger baskets,
//*     and issues rewards.
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
using Server.Commands;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;


namespace Server.Scripts.Commands
{
	public class ScavengerHuntCmds
	{
		public static void Initialize()
		{
			CommandSystem.Register( "StartScavengerHunt", AccessLevel.GameMaster, new CommandEventHandler( On_StartScavengerHunt ) );
			CommandSystem.Register( "StopScavengerHunt", AccessLevel.GameMaster, new CommandEventHandler( On_StopScavengerHunt ) );
		}

		[Usage( "StartScavengerHunt" )]
		[Description( "Sets the visibility of all the scavenger hunt items to true and starts the scavenger hunt" )]
		private static void On_StartScavengerHunt( CommandEventArgs e )
		{
			if( ScavengerSignup.ScavengerBaskets.Count > 2 && ScavengerSignup.ScavengerItems.Count > 0 && !ScavengerSignup.Started )
			{
				ScavengerSignup.Started = true;

				for( int i = 0; i < ScavengerSignup.ScavengerItems.Count; i++ )
				{
					ScavengerSignup.ScavengerItems[i].Visible = true;
				}

				World.Broadcast( 1153, true, "The scavenger hunt has begun!" );
			}
			else if( ScavengerSignup.Started )
			{
				e.Mobile.SendMessage( "A scavenger hunt is already in progress!" );
			}
			else
			{
				e.Mobile.SendMessage( "Either there are no scavenger items in the world, or there are not enough players (3 player minimum)" );
			}
		}

		[Usage( "StopScavengerHunt" )]
		[Description( "Removes all scavenger items and baskets then rewards the players" )]
		private static void On_StopScavengerHunt( CommandEventArgs e )
		{
			if( ScavengerSignup.Started )
			{
				ScavengerSignup.signupEnabled = false;  //No more basket handouts once the event is over

				string endMessage="";	//Should use String.Format instead of string concat.

				List<ScavengerItem> toDelete = new List<ScavengerItem>( ScavengerSignup.ScavengerItems );

				for( int i = 0; i < toDelete.Count; i++ )
				{
					toDelete[i].Delete();	//Will remove them from the ScavengerItems list.
				}

				endMessage += "The scavenger hunt is now finished\n";

				ScavengerSignup.ScavengerBaskets.Sort(
					delegate( ScavengerBasket x, ScavengerBasket y )
                    {
						if( y.ItemCount == x.ItemCount )
							return x.LastAcquiredItem.CompareTo( y.LastAcquiredItem );

						return y.ItemCount - x.ItemCount;
					} );

				List<ScavengerBasket> baskets = new List<ScavengerBasket>( ScavengerSignup.ScavengerBaskets );

				for( int i = 0; i < baskets.Count; i++ )
				{
					int payment = baskets[i].ItemCount * ScavengerSignup.baseItemPay;

					PlayerMobile owner = baskets[i].Owner;

					switch( i )
					{
						case 0:
							{
								payment += ScavengerSignup.firstPlacePay;
								endMessage += "First Place: " + owner.Name + "\n";
								break;
							}
						case 1:
							{
								payment += ScavengerSignup.secondPlacePay;
								endMessage += "Second Place: " + owner.Name + "\n";
								break;
							}
						case 2:
							{
								payment += ScavengerSignup.thirdPlacePay;
								endMessage += "Third Place: " + owner.Name;
								break;
							}
					}

					if( payment > 0 )
						owner.AddToBackpack( new BankCheck( payment ) );

					baskets[i].Delete(); //Removes from .ScavangerBaskets list
				}

				World.Broadcast( 1150, false, endMessage );

				ScavengerSignup.Started = false;
			}
		}
	}
}