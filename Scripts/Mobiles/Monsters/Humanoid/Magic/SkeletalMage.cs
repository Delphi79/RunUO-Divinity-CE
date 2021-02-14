using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class SkeletalMage : BaseCreature
	{
		[Constructable]
        public SkeletalMage()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 50;
            Name = "a skeletal mage";
            Hue = Utility.RandomRedHue();
            SetStr(76, 100);
            SetHits(76, 100);
            SetDex(56, 75);
            SetStam(56, 75);
            SetInt(86, 110);
            SetMana(71, 150);

            Fame = 3000;
            Karma = -3000;

            BaseSoundID = 451;
            SetSkill(SkillName.Wrestling, 45.1, 55);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 45.1, 60);
            SetSkill(SkillName.MagicResist, 70.1, 75);
            SetSkill(SkillName.Magery, 70.1, 75);
            SetSkill(SkillName.EvalInt, 70.1, 75);
            SetSkill(SkillName.Meditation, 70.1, 75);

            VirtualArmor = 19;
            SetDamage(2, 8);
			BardLevel = 45;
        }

        public override void GenerateLoot()
        {
            /*if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Rich);
            }
            else
            {
                AddLoot(LootPack.Poor);
            }*/

            PackReg(1, 3);
            AddLoot(LootPack.Average);
            PackScroll(4, 8);
            PackScroll(4, 8);
        }

		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public SkeletalMage( Serial serial ) : base( serial )
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