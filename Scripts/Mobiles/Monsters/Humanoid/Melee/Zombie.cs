using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a rotting corpse" )]
	public class Zombie : BaseCreature
	{
		[Constructable]
        public Zombie()
            : base(AIType.AI_Melee, FightMode.Closest, 112, 1, 0.4, 0.6)
        {
            Body = 3;
            Name = "a zombie";
            SetStr(46, 70);
            SetHits(46, 70);
            SetDex(31, 50);
            SetStam(31, 50);
            SetInt(26, 40);
            SetMana(26, 40);

            Fame = 600;
            Karma = -600;

            BaseSoundID = 471;
            SetSkill(SkillName.Wrestling, 35.1, 50);
            SetSkill(SkillName.Parry, 20.1, 30);
            SetSkill(SkillName.Tactics, 35.1, 50);
            SetSkill(SkillName.MagicResist, 15.1, 40);

            VirtualArmor = 9;
            SetDamage(3, 9);
			BardLevel = 45;
        }

        public override void GenerateLoot()
        {
			AddLoot(LootPack.Poor);

            Item item = null;
            if (Utility.RandomBool())
                PackGem();
            
            switch (Utility.Random(15))
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
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public Zombie( Serial serial ) : base( serial )
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