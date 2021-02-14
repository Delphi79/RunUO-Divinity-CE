using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Third
{
	public class PoisonSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Poison", "In Nox",
				203,
				9051,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public PoisonSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.NextSpellTime = DateTime.Now + this.GetCastRecovery();// Spell.NextSpellDelay;
		}

        private static TimeSpan PoisonTickDelay = TimeSpan.FromSeconds(2.5);

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				m.Paralyzed = false;

				if ( CheckResisted( m ) )
				{
					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}
				else
				{
					int level = 1;

                    if ( Caster.Skills[SkillName.Magery].Value < 75 )
                        level = 0;

                    if ( Utility.Random( 4 ) == 0 )
                        level = 2;

					if ( m.ApplyPoison( Caster, Poison.GetPoison( level ) ) == ApplyPoisonResult.Poisoned )
                    {
                        if ( m.PoisonTimer != null && m.PoisonTimer.Running && m.PoisonTimer.Delay > PoisonTickDelay )
                        {
                            m.PoisonTimer.Stop();
                            m.PoisonTimer.Delay = PoisonTickDelay;
                            m.PoisonTimer.Start();
                        }
                    }

                    if ( m.Spell is MagerySpell && ((MagerySpell)m.Spell).State == SpellState.Casting )
                        m.Spell.OnCasterHurt( (level + 1)*5 );
				}

				m.FixedParticles( 0x374A, 10, 15, 5021, EffectLayer.Waist );
				m.PlaySound( 0x474 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private PoisonSpell m_Owner;

			public InternalTarget( PoisonSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}