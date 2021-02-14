
using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Commands;

namespace Server.Misc
{
    public class Event
    {
        private static ArrayList m_Gates = new ArrayList();

        public static ArrayList Gates { get { return m_Gates; } }

        public static bool Open { get { return (m_Gates.Count > 0); } }

        public static void Register(EventGate gate)
        {
            m_Gates.Add(gate);
        }

        public static void Unregister(EventGate gate)
        {
            m_Gates.Remove(gate);
        }

        public static void Close()
        {
            ArrayList copy = new ArrayList(m_Gates);

            for (int i = 0; i < copy.Count; ++i)
                ((EventGate)copy[i]).Delete();
        }

        public static void Initialize()
        {
            CommandHandlers.Register("OpenEvent", AccessLevel.GameMaster, new CommandEventHandler(OpenEvent_OnCommand));
            CommandHandlers.Register("CloseEvent", AccessLevel.GameMaster, new CommandEventHandler(CloseEvent_OnCommand));

            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        [Usage("OpenEvent")]
        private static void OpenEvent_OnCommand(CommandEventArgs e)
        {
            if (Open)
            {
                e.Mobile.SendMessage(0x35, "Event gates are already open.");
            }
            else
            {
                e.Mobile.SendMessage(0x35, "Target gate destination.");
                e.Mobile.Target = new EventTarget();
            }
        }

        private static void EventSink_Login(LoginEventArgs e)
        {
            //if ( e.Mobile is PlayerMobile && (e.Mobile as PlayerMobile).EventBanned )
            //return;

            if (Open)
                e.Mobile.SendMessage(0x482, "There is currently an event in progress. If you would like to participate, check the nearest bank, town center, or shrine for a gate.");
        }

        private class EventTarget : Target
        {
            public EventTarget()
                : base(-1, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                    return;

                Spells.SpellHelper.GetSurfaceTop(ref p);

                m_Destination = new Point3D(p);
                m_Facet = from.Map;

                // Place New Gates

                // Unguarded
                //PlaceEventGate(2714, 2173, 0);  // Buccaneer's Den
                //PlaceEventGate(1377, 1504, 10);  // Britain Graveyard
                //PlaceEventGate(1827, 2770, 0);  // Trinsic Entrance
                //PlaceEventGate(2001, 2933, 0);  // Trinsic South Gate
                //PlaceEventGate(2901, 605, 0);   // North of Vesper Bank
                //PlaceEventGate(1367, 888, 0);   // Wind Entrance
                PlaceEventGate(1458, 844, 5);   // Chaos Shrine
                PlaceEventGate(1858, 875, -1);  // Compassion
                PlaceEventGate(4211, 564, 42);  // Honesty
                PlaceEventGate(1727, 3528, 3);  // Honor
                PlaceEventGate(4274, 3697, 0);  // Humility
                PlaceEventGate(1301, 634, 16);  // Justice
                PlaceEventGate(3355, 290, 4);   // Sacrifice
                PlaceEventGate(1600, 2489, 12); // Spirituality
                PlaceEventGate(2492, 3931, 5);  // Valor

                //PlaceEventGate(5349, 3957, 2);  // Delucia Pond

                PlaceEventGate(5274, 1162, 0); // Jail Cell 1

                // Guarded
                PlaceEventGate(1421, 1698, 0);  // Britain Bank
                PlaceEventGate(2286, 1200, 0);  // Cove
                PlaceEventGate(3731, 2169, 20); // Magincia
                PlaceEventGate(2522, 558, 0);   // Minoc
                PlaceEventGate(4462, 1176, 0);  // Moonglow Bank
                PlaceEventGate(4479, 1124, 0);  // Moonglow Counselor's Guild
                PlaceEventGate(3676, 2508, 0);  // Ocllo
                PlaceEventGate(2880, 3465, 15); // Serps
                PlaceEventGate(2900, 692, 0);   // Vesper Bank
                PlaceEventGate(5354, 90, 15);   // Wind Bank
                PlaceEventGate(535, 992, 0);    // Center of Yew
                PlaceEventGate(5273, 3994, 37); // Delucia Bank

                // Notify Clients That Gates Are Open
                foreach (NetState state in NetState.Instances)
                {
                    Mobile m = state.Mobile;

                    //if ( m is PlayerMobile && (m as PlayerMobile).EventBanned )
                    //continue;

                    if (m != null)
                        m.SendGump(new EventNoticeGump());
                }

                // Notify IRC Users
                IRCBot.SendMessage(IRCBot.Channel, String.Format("[EVENT] An event has been opened by {0}. Check the nearest bank, town center, or shrine for a gate.", from.Name));
            }

            private Point3D m_Destination;
            private Map m_Facet;

            private void PlaceEventGate(int x, int y, int z)
            {
                new EventGate(new Point3D(x, y, z), Map.Felucca, m_Destination, m_Facet);
            }
        }

        [Usage("CloseEvent")]
        private static void CloseEvent_OnCommand(CommandEventArgs e)
        {
            if (!Open)
            {
                e.Mobile.SendMessage(0x35, "Event gates are already closed.");
            }
            else
            {
                Close();

                // Cancel Notifications (afk, ...)
                foreach (NetState state in NetState.Instances)
                {
                    Mobile m = state.Mobile;

                    if (m != null)
                        m.CloseGump(typeof(EventNoticeGump));
                }
            }
        }

        private class EventNoticeGump : Gump
        {
            public string Center(string text)
            {
                return String.Format("<CENTER>{0}</CENTER>", text);
            }

            public string Color(string text, int color)
            {
                return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
            }

            private const int LabelColor32 = 0xFFFFFF;

            public EventNoticeGump()
                : base((640 - 378) / 2, (480 - 135) / 2)
            {
                Closable = false;

                AddPage(0);

                int height = 135;

                AddBackground(1, 1, 378, height - 2, 3600);
                AddAlphaRegion(16, 15, 349, height - 31);

                AddImage(-89, -59, 0xEE40);

                AddHtml(20, 20, 312, 20, Color(Center("Public Event"), LabelColor32), false, false);
                AddHtml(74, 50, 282, 60, Color("An event has been opened. Check the nearest bank, town center, or shrine for a gate.", 0x99CC66), false, false);

                AddButton(296, height - 44, 247, 248, 1, GumpButtonType.Reply, 0);
            }
        }
    }
}

namespace Server.Items
{
    public class EventGate : Moongate
    {
        public EventGate(Point3D loc, Map map, Point3D dest, Map targetMap)
            : base(loc, targetMap)
        {
            Name = "an event gate";
            Dispellable = false;
            Target = dest;
            Hue = 0x485;

            /*			GumpWidth = 300;
                        GumpHeight = 150;
                        MessageColor = 0xFFC000;
                        MessageString = "Do you wish to enter the event?";
                        TitleColor = 0x7800;
                        TitleNumber = 1062051; // Gate Warning
            */

            MoveToWorld(loc, map);
            Effects.PlaySound(loc, map, 0x20E);

            Event.Register(this);
        }

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;

				if ( m.Player && pm != null && !pm.EventBanned )
					CheckGate( m, 0 );
				else if ( pm.EventBanned )
					pm.LocalOverheadMessage( MessageType.Regular, 0x22, false, "You are banned from entering events." );
			}
			

			return true;
		}

        public override void OnDelete()
        {
            base.OnDelete();

            Effects.PlaySound(GetWorldLocation(), Map, 0x209);
            Event.Unregister(this);
        }

        public EventGate(Serial serial)
            : base(serial)
        {
            Event.Register(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}