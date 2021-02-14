using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a grizzly bear corpse" )]
	[TypeAlias( "Server.Mobiles.Grizzlybear" )]
	public class GrizzlyBear : BaseCreature
	{
		[Constructable]
        public GrizzlyBear()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 212;
            Name = "a grizzly bear";
            SetStr(126, 155);
            SetHits(126, 155);
            SetDex(81, 105);
            SetStam(81, 105);
            SetInt(16, 40);
            SetMana(0);

            Fame = 1000;
            Karma = 0;

            Tamable = true;
            MinTameSkill = 70;
            BaseSoundID = 163;
            SetSkill(SkillName.Wrestling, 50.1, 65);
            SetSkill(SkillName.Parry, 70.1, 85);
            SetSkill(SkillName.Tactics, 70.1, 100);
            SetSkill(SkillName.MagicResist, 45.1, 60);

            VirtualArmor = 12;
            SetDamage(6, 15);

			BardLevel = 13;
        }

		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 16; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

        public override bool AlwaysAttackable
        {
            get
            {
                return true;
            }
        }

		public GrizzlyBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}