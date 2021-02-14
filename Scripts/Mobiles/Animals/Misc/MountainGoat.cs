using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a mountain goat corpse" )]
	public class MountainGoat : BaseCreature
	{
        [Constructable]
        public MountainGoat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 209;
            Name = "a mountain goat";
            SetStr(22, 64);
            SetHits(22, 64);
            SetDex(56, 75);
            SetStam(56, 75);
            SetInt(16, 30);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 20;
            BaseSoundID = 153;
            SetSkill(SkillName.Wrestling, 29.3, 44);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 29.3, 44);
            SetSkill(SkillName.MagicResist, 25.1, 30);

            VirtualArmor = 8;
            SetDamage(3, 7);

            Fame = 300;
            Karma = 0;

			BardLevel = 25;
        }

		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay | FoodType.FruitsAndVegies; } }

		public MountainGoat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}