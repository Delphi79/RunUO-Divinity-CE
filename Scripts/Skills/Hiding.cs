using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;
using Server.Mobiles;

namespace Server.SkillHandlers
{
    public class Hiding
    {
        public static bool CombatOverride = false;
        public static void Initialize()
        {
            SkillInfo.Table[21].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (m.Target != null || m.Spell != null)
            {
                m.SendLocalizedMessage(501238); // You are busy doing something else and cannot hide.
                return TimeSpan.FromSeconds(1.0);
            }

            double bonus = 0.0;

            BaseHouse house = BaseHouse.FindHouseAt(m);

            if (house != null && house.IsFriend(m))
            {
                bonus = 100.0;
            }

            int range = 18 - (int)(m.Skills[SkillName.Hiding].Value / 10);

            bool badCombat = (m.Combatant != null && m.InRange(m.Combatant.Location, range) && m.Combatant.InLOS(m));
            bool ok = ( (!badCombat || CombatOverride) && m.CheckSkill(SkillName.Hiding, 0.0 - bonus, 100.0 - bonus));

            if ( ok )
            {
                foreach ( Mobile check in m.GetMobilesInRange( range ) )
                {
                    if ( check.InLOS( m ) && check.Combatant == m )
                    {
                        badCombat = true;
                        ok = false;
                        break;
                    }
                }

                ok = ( !badCombat && m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus ) );
            }

            if (badCombat)
            {
                m.RevealingAction();

                m.LocalOverheadMessage(MessageType.Regular, 0x22, true, "You can't seem to hide right now."); // 501237

                return TimeSpan.FromSeconds(5.0);
            }
            else
            {
                if (ok)
                {
                    //if (m is PlayerMobile)
                    //    ((PlayerMobile)m).OnBeforeHide();

                    m.Hidden = true;

                    m.LocalOverheadMessage(MessageType.Regular, 0x1F4, true, "You have hidden yourself well."); // 501240
                }
                else
                {
                    m.RevealingAction();

                    m.LocalOverheadMessage(MessageType.Regular, 0x22, true, "You can't seem to hide here."); // 501241
                }

                return TimeSpan.FromSeconds(10.0);
            }
        }
    }
}

