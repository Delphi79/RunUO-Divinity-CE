using System;
using System.Collections;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
    public class ReactiveArmorSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Reactive Armor", "Flam Sanct",
                236,
                9011,
                Reagent.Garlic,
                Reagent.SpidersSilk,
                Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.First; } }

		public ReactiveArmorSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public override bool CheckCast()
        {
            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        private static Hashtable m_Registry = new Hashtable();

        public void Target(Mobile m)
        {
            if (CheckBSequence(m))
            {
                Timer t = m_Registry[m] as Timer;

                if (t != null)
                    t.Stop();

                m.MeleeDamageAbsorb = ((int)(10 + Caster.Skills[SkillName.Magery].Value / 2)) / 2;
                m.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
                m.PlaySound(0x1F2);

                m_Registry[m] = t = Timer.DelayCall( TimeSpan.FromSeconds(25 + (Caster.Skills[SkillName.Magery].Value / 2.0) ), new TimerStateCallback(OnExpire), m );
            }

            FinishSequence();
        }

        public static void OnExpire(object state)
        {
            Mobile m = state as Mobile;

            if (state != null)
            {
                m_Registry.Remove(m);
                m.MeleeDamageAbsorb = 0;
                m.PlaySound(92);
            }
        }

        public class InternalTarget : Target
        {
            private ReactiveArmorSpell m_Owner;

            public InternalTarget(ReactiveArmorSpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}