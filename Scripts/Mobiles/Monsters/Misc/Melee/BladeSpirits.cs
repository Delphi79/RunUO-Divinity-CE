using System;
using System.Collections;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a blade spirit corpse" )]
	public class BladeSpirits : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return true; } }
		public override bool IsHouseSummonable { get { return true; } }

		public override double DispelDifficulty { get { return 0.0; } }
		public override double DispelFocus { get { return 20.0; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Str + m.Skills[SkillName.Tactics].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public BladeSpirits()
			: base( AIType.AI_Melee, FightMode.Fastest, 10, 1, 0.4, 0.4 )
		{
			Name = "a blade spirit";
			Body = 574;

            SetStr(75);
            SetHits(200);
            SetDex(75);
            SetStam(0);
            SetInt(100);
            SetMana(0);
            SetSkill(SkillName.Parry, 25, 47.5);
            SetSkill(SkillName.MagicResist, 25, 47.5);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            
            SetDamage(5, 10);

			Fame = 0;
			Karma = 0;

			VirtualArmor = 50;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
        //public override TimeSpan ReaquireDelay { get { return TimeSpan.FromSeconds(0.5); } }
        public override Poison HitPoison { get { return Poison.Regular; } }

		public override int GetAngerSound()
		{
			return 0x23A;
		}

		public override int GetAttackSound()
		{
			return 0x3B8;
		}

		public override int GetHurtSound()
		{
			return 0x23A;
		}

		public override void OnThink()
		{
			if ( Core.SE && Summoned )
			{
				ArrayList spirtsOrVortexes = new ArrayList();

				foreach ( Mobile m in GetMobilesInRange( 5 ) )
				{
					if ( m is EnergyVortex || m is BladeSpirits )
					{
						if ( ( (BaseCreature) m ).Summoned )
							spirtsOrVortexes.Add( m );
					}
				}

				while ( spirtsOrVortexes.Count > 6 )
				{
					int index = Utility.Random( spirtsOrVortexes.Count );
					Dispel( ( (Mobile) spirtsOrVortexes[index] ) );
					spirtsOrVortexes.RemoveAt( index );
				}
			}

			base.OnThink();
		}

		public BladeSpirits( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}