using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class EarthquakeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Earthquake", "In Vas Por",
				233,
				9012,
				false,
				Reagent.Bloodmoss,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		public EarthquakeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public override void OnCast()
		{
			Caster.NextSpellTime = DateTime.Now + this.GetCastRecovery();// Spell.NextSpellDelay;

			if ( SpellHelper.CheckTown( Caster, Caster ) && CheckSequence() )
			{
				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
					foreach ( Mobile m in Caster.GetMobilesInRange( 1 + (int)(Caster.Skills[SkillName.Magery].Value / 15.0) ) )
						if ( Caster != m && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) && (!Core.AOS || Caster.InLOS( m )) )
							targets.Add( m );

				Caster.PlaySound( 0x2F3 );

                new AnimTimer(targets).Start();

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = targets[i];

					int damage;

					damage = (int)(m.Hits * 0.25);

				    if ( damage < 1 )
						damage = 1;
					else if ( damage > 50 )
						damage = 50;

					Caster.DoHarmful( m );
					SpellHelper.Damage( TimeSpan.Zero, m, Caster, damage, 100, 0, 0, 0, 0 );
				}
			}

			FinishSequence();
		}

        private class AnimTimer : Timer
        {
            private List<Mobile> m_List;
            public AnimTimer(List<Mobile> list)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1), 2)
            {
                Priority = TimerPriority.FiftyMS;
                m_List = list;
            }

            protected override void OnTick()
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    Mobile m = m_List[i];
                    int offset = 0;
                    if (m.Body.IsMonster)
                        offset = 2;
                    else if (m.Body.IsSea || m.Body.IsAnimal)
                        offset = 8;
                    else if (m.Body.IsHuman)
                        offset = 21;

                    if (offset != 0)
                        m.Animate(offset + ((((int)m.Direction) >> 7) & 0x1), 5, 1, true, false, 0);
                }
            }
        }
	}
}