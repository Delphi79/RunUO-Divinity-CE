using System;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
    public class Poisoning
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Poisoning].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTargetPoison();

            m.SendLocalizedMessage(502137); // Select the poison you wish to use

            return TimeSpan.FromSeconds(10.0); // 10 second delay before beign able to re-use a skill
        }

        private class InternalTargetPoison : Target
        {
            public InternalTargetPoison()
                : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BasePoisonPotion)
                {
                    from.SendLocalizedMessage(502142); // To what do you wish to apply the poison?
                    from.Target = new InternalTarget((BasePoisonPotion)targeted);
                }
                else // Not a Poison Potion
                {
                    from.SendLocalizedMessage(502139); // That is not a poison potion.
                }
            }

            private class InternalTarget : Target
            {
                private BasePoisonPotion m_Potion;

                public InternalTarget(BasePoisonPotion potion)
                    : base(2, false, TargetFlags.None)
                {
                    m_Potion = potion;
                }

                protected override void OnTarget(Mobile from, object targeted)
                {
                    if (m_Potion.Deleted)
                        return;
                    BaseWeapon weapon = targeted as BaseWeapon;

                    if (targeted is Food ||
                        (weapon != null && weapon.Layer != Layer.TwoHanded && (weapon.Type == WeaponType.Slashing || weapon.Type == WeaponType.Piercing || weapon.Type == WeaponType.Axe)))
                    {
                        new InternalTimer(from, (Item)targeted, m_Potion).Start();

                        from.PlaySound(0x4F);

                        if (!Engines.ConPVP.DuelContext.IsFreeConsume(from))
                        {
                            m_Potion.Delete();

                            from.AddToBackpack(new Bottle());
                        }
                    }
                    else
                    {
                        from.SendAsciiMessage("You cannot poison that! You can only poison one handed bladed and piercing weapons and food.");
                    }
                }

                private class InternalTimer : Timer
                {
                    private Mobile m_From;
                    private Item m_Target;
                    private int m_Poison;
                    private double m_MinSkill, m_MaxSkill;

                    public InternalTimer(Mobile from, Item target, BasePoisonPotion potion)
                        : base(TimeSpan.FromSeconds(2.0))
                    {
                        m_From = from;
                        m_Target = target;
                        m_Poison = potion.Poison.Level;
                        m_MinSkill = potion.MinPoisoningSkill;
                        m_MaxSkill = potion.MaxPoisoningSkill;
                        Priority = TimerPriority.TwoFiftyMS;
                    }

                    protected override void OnTick()
                    {
                        if (m_From.CheckTargetSkill(SkillName.Poisoning, m_Target, m_MinSkill, m_MaxSkill))
                        {
                            if (m_From.Skills.Poisoning.Value < Utility.RandomDouble() * 100.0)
                            {
                                m_From.SendAsciiMessage("You apply a dose of poison to {0}.", m_Target.BuildSingleClick());
                                if (m_Poison > 0)
                                    m_Poison--;
                            }
                            else
                            {
                                m_From.SendAsciiMessage("You apply a strong dose of poison to {0}.", m_Target.BuildSingleClick());
                                if (m_Target is Food && m_Poison < 4)
                                    m_Poison++;
                            }

                            if (m_Target is Food)
                            {
                                ((Food)m_Target).Poison = Poison.GetPoison(m_Poison);
                            }
                            else if (m_Target is BaseWeapon)
                            {
                                BaseWeapon weapon = (BaseWeapon)m_Target;
                                weapon.Poison = Poison.GetPoison(m_Poison);
                                weapon.PoisonCharges = 20 - ((m_Poison + 1) * 2);
                                weapon.PoisonChance = Math.Max(0.2, (m_From.Skills.Poisoning.Value / 4.0) / 100.0);
                            }

                            Misc.Titles.AwardKarma(m_From, -20, true);
                        }
                        else // Failed
                        {
                            m_From.SendAsciiMessage("You fail apply a dose of poison to {0}.", m_Target.BuildSingleClick());
                        }
                    }
                }
            }
        }
    }
}