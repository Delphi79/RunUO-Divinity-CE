using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Network;
using System.Collections;

namespace Server.SkillHandlers
{
    class SpiritSpeak
    {
        private static Hashtable m_Table = new Hashtable();
        public static void Initialize()
        {
            SkillInfo.Table[32].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            Timer t = m_Table[m] as Timer;
            if (t != null && t.Running)
                t.Stop();
            if (m.CheckSkill(SkillName.SpiritSpeak, 0, 100))
            {
                if (t == null)
                    m_Table[m] = t = new SpiritSpeakTimer(m);

                double secs = m.Skills[SkillName.SpiritSpeak].Base / 50;
                secs *= 90;
                if (secs < 10)
                    secs = 10;

                t.Delay = TimeSpan.FromSeconds(secs);//15 seconds to 3 minutes
                t.Start();
                m.CanHearGhosts = true;

                IPooledEnumerable eable = m.Map.GetMobilesInRange(m.Location, Core.GlobalMaxUpdateRange);
                // find all the dead people we can see in range and send them
                foreach (Mobile g in eable)
                {
                    if (!g.Alive && m.CanSee(g) && Utility.InUpdateRange(m.Location, g.Location))
                        m.Send(new MobileIncoming(m, g));
                }
                eable.Free();

                m.PlaySound(0x24A);
                m.SendLocalizedMessage(502444);//You contact the neitherworld.
            }
            else
            {
                m_Table.Remove(m);
                m.SendLocalizedMessage(502443);//You fail to contact the neitherworld.
                m.CanHearGhosts = false;
            }

            return TimeSpan.FromSeconds(10.0);
        }

        private class SpiritSpeakTimer : Timer
        {
            private Mobile m_Owner;
            public SpiritSpeakTimer(Mobile m)
                : base(TimeSpan.FromMinutes(2.0))
            {
                m_Owner = m;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Owner.CanHearGhosts = false;

                m_Table.Remove(m_Owner);

                IPooledEnumerable eable = m_Owner.Map.GetMobilesInRange(m_Owner.Location, Core.GlobalMaxUpdateRange);

                // find all the dead people we can't see and remove them
                foreach (Mobile m in eable)
                {
                    if ( !m.Alive && !m_Owner.CanSee(m) )
                        m_Owner.Send(m.RemovePacket);
                }

                eable.Free();

                m_Owner.SendLocalizedMessage(502445);//You feel your contact with the neitherworld fading.
            }
        }
    }
}
