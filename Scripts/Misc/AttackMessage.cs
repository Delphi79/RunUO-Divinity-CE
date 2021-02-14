using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Misc
{
	public class AttackMessage
	{
		private const string AggressorFormat = "You are attacking {0}!";
		private const string AggressedFormat = "{0} is attacking you!";
		private const int Hue = 0x22;

		private static TimeSpan Delay = TimeSpan.FromMinutes( 1.0 );

		public static void Initialize()
		{
			EventSink.AggressiveAction += new AggressiveActionEventHandler( EventSink_AggressiveAction );
		}

		public static void EventSink_AggressiveAction( AggressiveActionEventArgs e )
		{
			Mobile aggressor = e.Aggressor;
			Mobile aggressed = e.Aggressed;

			if ( !CheckAggressions( aggressor, aggressed ) && !(aggressor is Mobiles.BaseGuard) )
			{
                if (aggressor.Player && aggressed.Player)
                {
                    aggressor.LocalOverheadMessage(MessageType.Regular, Hue, true, String.Format(AggressorFormat, aggressed.Name));
                    aggressed.LocalOverheadMessage(MessageType.Regular, Hue, true, String.Format(AggressedFormat, aggressor.Name));
                }

				/*Map map = aggressor.Map;

				if ( map == null || map == Map.Internal )
					return;

                IPooledEnumerable eable = map.GetClientsInRange( aggressor.Location, 15);
                Packet p = null;
                foreach (NetState ns in eable)
                {
                    Mobile m = ns.Mobile;
                    if (m != null && m.CanSee(aggressor) && m != aggressed && m != aggressor)
                    {
                        if (p == null)
                            p = Packet.Acquire(new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, 0x3b2, 3, "System", String.Format("You see {0} attacking {1}!", aggressor.Name, aggressed.Name)));
                        ns.Send(p);
                    }
                }
                eable.Free();
                Packet.Release(ref p);*/
			}
		}

		public static bool CheckAggressions( Mobile m1, Mobile m2 )
		{
            if ( m1 == m2 )
                return true;

			List<AggressorInfo> list = m1.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( info.Attacker == m2 && DateTime.Now < (info.LastCombatTime + Delay) )
					return true;
			}

			list = m2.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( info.Attacker == m1 && DateTime.Now < (info.LastCombatTime + Delay) )
					return true;
			}

			return false;
		}
	}
}