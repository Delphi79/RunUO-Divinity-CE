using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
	public class MagicReflectSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Magic Reflection", "In Jux Sanct",
				242,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MagicReflectSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public override void OnCast()
        {
			Caster.NextSpellTime = DateTime.Now + this.GetCastRecovery();// Spell.NextSpellDelay;

            // do this before check sequence so it doesn't take reags or mana
            if (Caster.MagicDamageAbsorb > 0)
            {
                DoFizzle();
            }
            else if (CheckSequence())
            {
                Caster.MagicDamageAbsorb = 1;

                Caster.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                Caster.PlaySound(0x1E9);
            }

            FinishSequence();
        }
	}
}
