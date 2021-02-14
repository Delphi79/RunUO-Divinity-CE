using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class Skeleton : BaseCreature
	{
		[Constructable]
        public Skeleton()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = Utility.RandomList(50, 56);
            Name = "a skeleton";
            SetStr(56, 80);
            SetHits(75, 85);
            SetDex(56, 75);
            SetStam(56, 75);
            SetInt(16, 40);
            SetMana(0);

            Fame = 450;
            Karma = -450;

            BaseSoundID = 451;
            SetSkill(SkillName.Wrestling, 45.1, 55);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 45.1, 60);
            SetSkill(SkillName.MagicResist, 45.1, 60);

            VirtualArmor = 8;
            SetDamage(3, 9);
			BardLevel = 40;
        }

        public override void GenerateLoot()
        {
			AddLoot(LootPack.Poor);

            Item item = null;

            switch (Utility.Random(10))
            {
                case 0:
                    {
                        item = new BoneChest();
                        PackItem(item);
                        break;
                    }
                case 1:
                    {
                        item = new BoneLegs();
                        PackItem(item);
                        break;
                    }
                case 2:
                    {
                        item = new BoneArms();
                        PackItem(item);
                        break;
                    }
                case 3:
                    {
                        item = new BoneGloves();
                        PackItem(item);
                        break;
                    }
                case 4:
                    {
                        item = new BoneHelm();
                        PackItem(item);
                        break;
                    }
            }
        }

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }

		public Skeleton( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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
