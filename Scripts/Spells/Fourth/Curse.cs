using System;
using System.Collections;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fourth
{
	public class CurseSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Curse", "Des Sanct",
				227,
				9031,
				Reagent.Nightshade,
				Reagent.Garlic,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public CurseSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.NextSpellTime = DateTime.Now + this.GetCastRecovery();// Spell.NextSpellDelay;
		}

		private static Hashtable m_UnderEffect = new Hashtable();

		public static void RemoveEffect( object state )
		{
			Mobile m = (Mobile)state;

			m_UnderEffect.Remove( m );

			m.UpdateResistances();
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_UnderEffect.Contains( m );
		}

        public static void PutUnderEffect(Mobile m, Timer t)
        {
            m_UnderEffect[m] = t;
        }

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

				bool success = SpellHelper.AddStatCurse( Caster, m, StatType.Str ); 
                SpellHelper.DisableSkillCheck = true;
                success = SpellHelper.AddStatCurse(Caster, m, StatType.Dex) || success;
                success = SpellHelper.AddStatCurse(Caster, m, StatType.Int) || success; 
                SpellHelper.DisableSkillCheck = false;

                m.Paralyzed = false;

                if ( success )
                {
				    Timer t = (Timer)m_UnderEffect[m];

				    if ( Caster.Player && m.Player /*&& Caster != m */ && t == null )	//On OSI you CAN curse yourself and get this effect.
				    {
					    TimeSpan duration = SpellHelper.GetDuration( Caster, m );
					    m_UnderEffect[m] = t = Timer.DelayCall( duration, new TimerStateCallback( RemoveEffect ), m );
					    m.UpdateResistances();
				    }

				    m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
				    m.PlaySound( 0x1EA );
                }
                else
                {
                    DoFizzle();
                }
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private CurseSpell m_Owner;

			public InternalTarget( CurseSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}