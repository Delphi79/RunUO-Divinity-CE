using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Menus.Questions;

namespace Server.Gumps
{
	public enum ResurrectMessage
	{
		ChaosShrine=0,
		VirtueShrine=1,
		Healer=2,
		Generic=3,
	}

	public class ResurrectMenu : QuestionMenu
	{
		private static string[] m_Options = new string[] { "YES - You choose to try to come back to life now.", "NO - You prefer to remain a ghost for now." };

		private Mobile m_Healer;
		private Point3D m_Location;
		private bool m_Heal;

		private Timer m_Timer;

		public ResurrectMenu( Mobile owner ) : this( owner, owner, ResurrectMessage.Generic )
		{
		}

		public ResurrectMenu( Mobile owner, Mobile healer ) : this( owner, healer, ResurrectMessage.Generic )
		{
		}

		public ResurrectMenu( Mobile owner, ResurrectMessage msg ) : this( owner, owner, msg )
		{
		}

		private static void UnfreezeCallback( object state )
		{
			if ( state is Mobile )
				((Mobile)state).Frozen = false;
		}

		private static TimerStateCallback m_Unfreeze = new TimerStateCallback( UnfreezeCallback );

		public ResurrectMenu( Mobile owner, Mobile healer, ResurrectMessage msg ) : base( "", m_Options )
		{
			m_Location = owner.Location;
			m_Healer = healer;

			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), m_Unfreeze, owner );

			owner.Frozen = true;
			
			switch ( msg )
			{
				case ResurrectMessage.Healer:
					Question = "It is possible for you to be resurrected here by this healer. Do you wish to try?";
					break;
				case ResurrectMessage.VirtueShrine:
					m_Heal = true;
					Question = "It is possible for you to be resurrected at this Shrine to the Virtues. Do you wish to try?";
					break;
				case ResurrectMessage.ChaosShrine:
					m_Heal = true;
					Question = "It is possible for you to be resurrected here at the Chaos Shrine. Do you wish to try?";
					break;
				case ResurrectMessage.Generic:
				default:
					Question = "It is possible for you to be resurrected now. Do you wish to try?";
					break;
			}
		}

		public override void OnCancel(NetState state)
		{
			base.OnCancel (state);

			if ( state.Mobile != null && m_Timer != null && m_Timer.Running )
			{
				state.Mobile.Frozen = false;
				m_Timer.Stop();
			}
		}

		public override void OnResponse(NetState state, int index)
		{
			Mobile from = state.Mobile;

			if ( from != null && m_Timer != null && m_Timer.Running )
			{
				from.Frozen = false;
				m_Timer.Stop();
			}

			if ( index == 0 && from != null && !from.Alive )
			{
				if ( from.Location != m_Location )
				{
					from.SendAsciiMessage( "Thou hast wandered too far from the site of thy resurrection!" );
					return;
				}

				PlayerMobile pm = from as PlayerMobile;
				if ( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) || m_Location != from.Location )
				{
					from.SendAsciiMessage( "Thou can not be resurrected there!" );
					return;
				}
				/*else if ( pm != null && pm.SpiritCohesion <= 0 )
				{
					from.SendAsciiMessage( "Your spirit is too weak to return to corporeal form." );
					return;
				}*/

				from.PlaySound( 0x214 );
				from.FixedEffect( 0x376A, 10, 16 );

				from.Resurrect();

				if ( m_Heal )
				{
					if ( from.HitsMax > 10 )
						from.Hits = Utility.RandomMinMax( from.HitsMax/4, from.HitsMax );
					from.Stam = from.StamMax;
				}

				//Misc.Titles.AwardFame( from, -100, true ); // TODO: Proper fame loss

				if( from.Fame > 0 )
				{
					int amount = from.Fame / 10;
					
					if ( amount > 500 )
						amount = 500;
					else if ( amount < 100 )
						amount = 100;
					

					Misc.Titles.AwardFame( from, -amount, true );
				}
/* stat loss
				if( !Core.AOS && from.ShortTermMurders >= 5 )
				{
					//double loss = (100.0 - (4.0 + (from.ShortTermMurders / 5.0))) / 100.0; // 5 to 15% loss
					double loss = .95; // 5% loss
					int minstat = 80;
					int minskill = 80;

					if( from.RawStr * loss > minskill )
						from.RawStr = (int)(from.RawStr * loss);
					if( from.RawInt * loss > minskill )
						from.RawInt = (int)(from.RawInt * loss);
					if( from.RawDex * loss > minskill )
						from.RawDex = (int)(from.RawDex * loss);

					for( int s = 0; s < from.Skills.Length; s++ )
					{
						if( from.Skills[s].Base * loss > minstat )
							from.Skills[s].Base *= loss;
					}
				}

				//if( from.Alive && m_HitsScalar > 0 )
				//	from.Hits = (int)(from.HitsMax * m_HitsScalar);
*/
				Regions.HouseRegion hr = from.Region as Regions.HouseRegion;
				if ( hr != null && hr.House != null && !hr.House.Deleted && !hr.House.IsFriend( from ) )
					from.Location = hr.House.BanLocation;

				/*if ( pm != null )
				{
					pm.SpiritCohesion --;
					switch ( pm.SpiritCohesion )
					{
						case 0:
							from.SendAsciiMessage( "Your spirit returns to corporeal form, but is too weak to do so again for a while." );
							break;
						case 1:
							from.SendAsciiMessage( "Your spirit barely manages to return to corporeal form." );
							break;
						case 2:
							from.SendAsciiMessage( "With some effort your spirit returns to corporeal form." );
							break;
						case 3:
						default:
							from.SendAsciiMessage( "Your spirit easily returns to corporeal form." );
							break;
					}
				}*/
			}
		}
	}

	/*public class ResurrectGump : Gump
	{
		private Mobile m_Healer;
		private int m_Price;
		private bool m_FromSacrifice;
		private double m_HitsScalar;

		public ResurrectGump( Mobile owner )
			: this( owner, owner, ResurrectMessage.Generic, false )
		{
		}

		public ResurrectGump( Mobile owner, double hitsScalar )
			: this( owner, owner, ResurrectMessage.Generic, false, hitsScalar )
		{
		}

		public ResurrectGump( Mobile owner, bool fromSacrifice )
			: this( owner, owner, ResurrectMessage.Generic, fromSacrifice )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer )
			: this( owner, healer, ResurrectMessage.Generic, false )
		{
		}

		public ResurrectGump( Mobile owner, ResurrectMessage msg )
			: this( owner, owner, msg, false )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice )
			: this( owner, healer, msg, fromSacrifice, 0.0 )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice, double hitsScalar )
			: base( 100, 0 )
		{
			m_Healer = healer;
			m_FromSacrifice = fromSacrifice;
			m_HitsScalar = hitsScalar;

			AddPage( 0 );

			AddBackground( 0, 0, 400, 350, 2600 );

			AddHtmlLocalized( 0, 20, 400, 35, 1011022, false, false ); // <center>Resurrection</center>

			AddHtmlLocalized( 50, 55, 300, 140, 1011023 + (int)msg, true, true ); // It is possible for you to be resurrected here by this healer. Do you wish to try?<br>
																				  // CONTINUE - You chose to try to come back to life now.<br>
																				  // CANCEL - You prefer to remain a ghost for now.
																				  

			AddButton( 200, 227, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 235, 230, 110, 35, 1011012, false, false ); // CANCEL

			AddButton( 65, 227, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 100, 230, 110, 35, 1011011, false, false ); // CONTINUE
		}

		public ResurrectGump( Mobile owner, Mobile healer, int price )
			: base( 150, 50 )
		{
			m_Healer = healer;
			m_Price = price;

			Closable = false;

			AddPage( 0 );

			AddImage( 0, 0, 3600 );

			AddImageTiled( 0, 14, 15, 200, 3603 );
			AddImageTiled( 380, 14, 14, 200, 3605 );

			AddImage( 0, 201, 3606 );

			AddImageTiled( 15, 201, 370, 16, 3607 );
			AddImageTiled( 15, 0, 370, 16, 3601 );

			AddImage( 380, 0, 3602 );

			AddImage( 380, 201, 3608 );

			AddImageTiled( 15, 15, 365, 190, 2624 );

			AddRadio( 30, 140, 9727, 9730, true, 1 );
			AddHtmlLocalized( 65, 145, 300, 25, 1060015, 0x7FFF, false, false ); // Grudgingly pay the money

			AddRadio( 30, 175, 9727, 9730, false, 0 );
			AddHtmlLocalized( 65, 178, 300, 25, 1060016, 0x7FFF, false, false ); // I'd rather stay dead, you scoundrel!!!

			AddHtmlLocalized( 30, 20, 360, 35, 1060017, 0x7FFF, false, false ); // Wishing to rejoin the living, are you?  I can restore your body... for a price of course...

			AddHtmlLocalized( 30, 105, 345, 40, 1060018, 0x5B2D, false, false ); // Do you accept the fee, which will be withdrawn from your bank?

			AddImage( 65, 72, 5605 );

			AddImageTiled( 80, 90, 200, 1, 9107 );
			AddImageTiled( 95, 92, 200, 1, 9157 );

			AddLabel( 90, 70, 1645, price.ToString() );
			AddHtmlLocalized( 140, 70, 100, 25, 1023823, 0x7FFF, false, false ); // gold coins

			AddButton( 290, 175, 247, 248, 2, GumpButtonType.Reply, 0 );

			AddImageTiled( 15, 14, 365, 1, 9107 );
			AddImageTiled( 380, 14, 1, 190, 9105 );
			AddImageTiled( 15, 205, 365, 1, 9107 );
			AddImageTiled( 15, 14, 1, 190, 9105 );
			AddImageTiled( 0, 0, 395, 1, 9157 );
			AddImageTiled( 394, 0, 1, 217, 9155 );
			AddImageTiled( 0, 216, 395, 1, 9157 );
			AddImageTiled( 0, 0, 1, 217, 9155 );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if( info.ButtonID == 1 || info.ButtonID == 2 )
			{
				if( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) )
				{
					from.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					return;
				}

				if( m_Price > 0 )
				{
					if( info.IsSwitched( 1 ) )
					{
						if( Banker.Withdraw( from, m_Price ) )
						{
							from.SendLocalizedMessage( 1060398, m_Price.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
							from.SendLocalizedMessage( 1060022, Banker.GetBalance( from ).ToString() ); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
						}
						else
						{
							from.SendLocalizedMessage( 1060020 ); // Unfortunately, you do not have enough cash in your bank to cover the cost of the healing.
							return;
						}
					}
					else
					{
						from.SendLocalizedMessage( 1060019 ); // You decide against paying the healer, and thus remain dead.
						return;
					}
				}

				from.PlaySound( 0x214 );
				from.FixedEffect( 0x376A, 10, 16 );

				from.Resurrect();

				if( m_Healer != null && from != m_Healer )
				{
					VirtueLevel level = VirtueHelper.GetLevel( m_Healer, VirtueName.Compassion );

					switch( level )
					{
						case VirtueLevel.Seeker: from.Hits = AOS.Scale( from.HitsMax, 20 ); break;
						case VirtueLevel.Follower: from.Hits = AOS.Scale( from.HitsMax, 40 ); break;
						case VirtueLevel.Knight: from.Hits = AOS.Scale( from.HitsMax, 80 ); break;
					}
				}

				if( m_FromSacrifice && from is PlayerMobile )
				{
					((PlayerMobile)from).AvailableResurrects -= 1;

					Container pack = from.Backpack;
					Container corpse = from.Corpse;

					if( pack != null && corpse != null )
					{
						List<Item> items = new List<Item>( corpse.Items );

						for( int i = 0; i < items.Count; ++i )
						{
							Item item = items[i];

							if( item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.Movable )
								pack.DropItem( item );
						}
					}
				}

				if( from.Fame > 0 )
				{
					int amount = from.Fame / 10;

					Misc.Titles.AwardFame( from, -amount, true );
				}

				if( !Core.AOS && from.ShortTermMurders >= 5 )
				{
					double loss = (100.0 - (4.0 + (from.ShortTermMurders / 5.0))) / 100.0; // 5 to 15% loss

					if( loss < 0.85 )
						loss = 0.85;
					else if( loss > 0.95 )
						loss = 0.95;

					if( from.RawStr * loss > 10 )
						from.RawStr = (int)(from.RawStr * loss);
					if( from.RawInt * loss > 10 )
						from.RawInt = (int)(from.RawInt * loss);
					if( from.RawDex * loss > 10 )
						from.RawDex = (int)(from.RawDex * loss);

					for( int s = 0; s < from.Skills.Length; s++ )
					{
						if( from.Skills[s].Base * loss > 35 )
							from.Skills[s].Base *= loss;
					}
				}

				if( from.Alive && m_HitsScalar > 0 )
					from.Hits = (int)(from.HitsMax * m_HitsScalar);
			}
		}
	}*/
}
