using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a dog corpse" )]
	public class Dog : BaseCreature
	{
		[Constructable]
        public Dog()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 217;
            Name = "a dog";
            Hue = Utility.RandomAnimalHue();
            SetStr(27, 37);
            SetHits(28, 37);
            SetDex(28, 43);
            SetStam(31, 49);
            SetInt(29, 37);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 3;
            BaseSoundID = 133;
            SetSkill(SkillName.Wrestling, 19.2, 31);
            SetSkill(SkillName.Parry, 28.1, 53);
            SetSkill(SkillName.Tactics, 19.2, 31);
            SetSkill(SkillName.MagicResist, 22.1, 47);

            VirtualArmor = 6;
            SetDamage(4, 7);

            Karma = 150;
        }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public Dog(Serial serial) : base(serial)
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