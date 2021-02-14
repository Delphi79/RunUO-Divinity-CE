using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an earth elemental corpse" )]
	public class SummonedEarthElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
        public SummonedEarthElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 14;
            Name = "an earth elemental";
            SetStr(116, 135);
            SetHits(116, 135);
            SetDex(56, 65);
            SetStam(56, 65);
            SetInt(61, 75);
            SetMana(61, 75);

            BaseSoundID = 268;
            SetSkill(SkillName.Wrestling, 40.1, 80);
            SetSkill(SkillName.Parry, 40.2, 65);
            SetSkill(SkillName.Tactics, 60.1, 100);
            SetSkill(SkillName.MagicResist, 30.1, 75);

            VirtualArmor = 15;
            SetDamage(3, 18);
        }

		public SummonedEarthElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}