using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an air elemental corpse" )]
	public class SummonedAirElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
        public SummonedAirElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 13;
            Name = "an air elemental";
            SetStr(116, 135);
            SetHits(116, 135);
            SetDex(56, 65);
            SetStam(56, 65);
            SetInt(61, 75);
            SetMana(61, 75);

            BaseSoundID = 263;
            SetSkill(SkillName.Wrestling, 60.1, 80);
            SetSkill(SkillName.Parry, 55.1, 65);
            SetSkill(SkillName.Tactics, 60.1, 80);
            SetSkill(SkillName.MagicResist, 60.1, 75);
            SetSkill(SkillName.Magery, 60.1, 75);

            VirtualArmor = 19;
            SetDamage(5, 13);
        }

		public SummonedAirElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 655;
		}
	}
}