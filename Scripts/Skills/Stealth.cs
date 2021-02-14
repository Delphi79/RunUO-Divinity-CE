using System;
using Server;
using Server.Items;

namespace Server.SkillHandlers
{
    public class Stealth
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Stealth].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (!m.Hidden)
            {
                m.SendLocalizedMessage(502725); // You must hide first
            }
            else if (m.Skills[SkillName.Hiding].Base < 80.0)
            {
                m.SendLocalizedMessage(502726); // You are not hidden well enough.  Become better at hiding.
            }
            else if (GetArmorOffset(m) >= 15)
            {
                m.SendLocalizedMessage(502727); // You could not hope to move quietly wearing this much armor.
            }
            else if (m.CheckSkill(SkillName.Stealth, 0.0, 100.0))
            {
                int steps = (int)(m.Skills[SkillName.Stealth].Value / (Core.AOS ? 5.0 : 10.0));

                if (steps < 1)
                    steps = 1;

                m.AllowedStealthSteps = steps;

                m.SendLocalizedMessage(502730); // You begin to move quietly.

                return TimeSpan.FromSeconds(10.0);
            }
            else
            {
                m.SendLocalizedMessage(502731); // You fail in your attempt to move unnoticed.
                m.RevealingAction();
            }

            return TimeSpan.FromSeconds(10.0);
        }

        private static double GetArmorOffset(Mobile from)
        {
            double rating = 0.0;

            if (!Core.AOS)
                rating += GetArmorStealthValue(from.ShieldArmor as BaseArmor);

            rating += GetArmorStealthValue(from.NeckArmor as BaseArmor);
            rating += GetArmorStealthValue(from.HandArmor as BaseArmor);
            rating += GetArmorStealthValue(from.HeadArmor as BaseArmor);
            rating += GetArmorStealthValue(from.ArmsArmor as BaseArmor);
            rating += GetArmorStealthValue(from.LegsArmor as BaseArmor);
            rating += GetArmorStealthValue(from.ChestArmor as BaseArmor);

            return rating;
        }

        private static double GetArmorStealthValue(BaseArmor ar)
        {
            if (ar == null)
                return 0.0;

            //return ar.ArmorRatingScaled;
            return ar.BaseArmorRating * ar.ArmorScalar;
        }
    }
}