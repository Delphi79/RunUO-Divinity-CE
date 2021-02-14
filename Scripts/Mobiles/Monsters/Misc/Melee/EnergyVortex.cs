using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "an energy vortex corpse" )]
	public class EnergyVortex : BaseCreature
	{
		public override bool DeleteCorpseOnDeath { get { return Summoned; } }
		public override bool AlwaysMurderer{ get{ return true; } } // Or Llama vortices will appear gray.

		public override double DispelDifficulty { get { return 80.0; } }
		public override double DispelFocus { get { return 20.0; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return ( m.Int + m.Skills[SkillName.Magery].Value ) / Math.Max( GetDistanceToSqrt( m ), 1.0 );
		}

		[Constructable]
		public EnergyVortex()
            : base( AIType.AI_Melee, FightMode.Smartest, 12, 1, 0.225, 0.225 )
		{
			Name = "an energy vortex";

            if (Utility.Random(100) == 17)
            { // Llama vortex!
                Body = 0xDC;
                Hue = 0x76;
            }
            else
            {
                Body = 0xD;
                Hue = 11;
            }

            SetStr(300);
            SetHits(900);
            SetDex(200);
            SetStam(0);
            SetInt(100);
            SetMana(0);
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.Parry, 100);
            SetSkill(SkillName.MagicResist, 100);

            SetDamage( 15, 30 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 50;

            ControlSlots = 2;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override TimeSpan ReacquireDelay
        {
            get
            {
                return TimeSpan.FromSeconds(0.25);
            }
        }
        public override Poison HitPoison { get { return Utility.RandomDouble() < 0.15 ? Poison.Deadly : Poison.Greater; } }

		public override int GetAngerSound()
		{
			return 0x15;
		}

		public override int GetAttackSound()
		{
			return 0x28;
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
					//TODO: Confim if it's the dispel with all the pretty effects or just a Deletion of it.
					Dispel( ( (Mobile) spirtsOrVortexes[index] ) );
					spirtsOrVortexes.RemoveAt( index );
				}
			}

			base.OnThink();
		}


		public EnergyVortex( Serial serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 0;
		}
	}
}