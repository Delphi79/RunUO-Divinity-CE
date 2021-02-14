using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Guilds;
using Server.Network;
using Server.Factions;

namespace Server.Gumps
{
	public class GuildChangeTypeGump : Gump
	{
		private Mobile m_Mobile;
		private Guild m_Guild;

		public GuildChangeTypeGump( Mobile from, Guild guild ) : base( 20, 30 )
		{
			m_Mobile = from;
			m_Guild = guild;

			Dragable = false;

			AddPage( 0 );
			AddBackground( 0, 0, 550, 400, 5054 );
			AddBackground( 10, 10, 530, 380, 3000 );

			AddHtmlLocalized( 20, 15, 510, 30, 1013062, false, false ); // <center>Change Guild Type Menu</center>

			AddHtmlLocalized( 50, 50, 450, 30, 1013066, false, false ); // Please select the type of guild you would like to change to

			AddButton( 20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 85, 100, 300, 30, 1013063, false, false ); // Standard guild

            if (from.Kills < 5)
            {
                AddButton(20, 150, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddItem(50, 143, 7109);
                AddHtmlLocalized(85, 150, 300, 300, 1013064, false, false); // Order guild

                AddButton(20, 200, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddItem(45, 200, 7107);
                AddHtmlLocalized(85, 200, 300, 300, 1013065, false, false); // Chaos guild
            }

			AddButton( 300, 360, 4005, 4007, 4, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 335, 360, 150, 30, 1011012, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( GuildGump.BadLeader( m_Mobile, m_Guild ) )
				return;

			PlayerState pl = PlayerState.Find( m_Mobile );

			if ( pl != null )
			{
				m_Mobile.SendLocalizedMessage( 1010405 ); // You cannot change guild types while in a Faction!
			}
			else if ( m_Guild.TypeLastChange.AddDays( 7 ) > DateTime.Now )
			{
				//m_Mobile.SendLocalizedMessage( 1005292 ); // Your guild type will be changed in one week.
                m_Mobile.SendMessage("You can only change your guild type once every seven days.  Try again later.");
			}
			else
			{
				GuildType newType;

				switch ( info.ButtonID )
				{
					default: return; // Close
					case 1: newType = GuildType.Regular; break;
					case 2: newType = GuildType.Order;   break;
					case 3: newType = GuildType.Chaos;   break;
				}

                if (m_Guild.Type == newType)
                    return;

                if (newType != GuildType.Regular && !Guild.JoinVirtue(m_Mobile, newType))
                {
                    m_Mobile.SendAsciiMessage(0x25, "You cannot change this guild to be a that virtue right now.  Change to regular and then wait up to seven days.  Murderers cannot join virtue guilds, ever.");
                    return;
                }

				m_Guild.Type = newType;
				m_Guild.GuildMessage( 1018022, true, newType.ToString() ); // Guild Message: Your guild type has changed:

                if (newType != GuildType.Regular)
                {
                    List<Mobile> toRemove = new List<Mobile>();
                    foreach (Mobile m in m_Guild.Members)
                    {
                        if (!Guild.JoinVirtue(m, newType))
                            toRemove.Add(m);
                    }

                    foreach (Mobile m in toRemove)
                    {
                        m_Guild.RemoveMember(m);
                        m.SendAsciiMessage( 0x25, "You are a your guild has become a virtue guild, you cannot be in that virtue so you have been removed from the guild.  You must wait up to 7 days before you can join a guild of an opposing virtue.  Murderers may not join virtue guilds.");
                    }

                    m_Mobile.SendAsciiMessage( 0x25, "{0} murderers have been purged from the member list.", toRemove.Count);
                }
			}

			GuildGump.EnsureClosed( m_Mobile );
			m_Mobile.SendGump( new GuildmasterGump( m_Mobile, m_Guild ) );
		}
	}
}
