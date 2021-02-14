using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an eagle corpse" )]
	public class Eagle : BaseCreature
	{
		[Constructable]
        public Eagle()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 5;
            Name = "an eagle";
            SetStr(31, 47);
            SetHits(30, 40);
            SetDex(36, 60);
            SetStam(60, 110);
            SetInt(8, 20);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 35;
            BaseSoundID = 143;
            SetSkill(SkillName.Wrestling, 20.1, 30);
            SetSkill(SkillName.Parry, 25.1, 40);
            SetSkill(SkillName.Tactics, 18.1, 37);
            SetSkill(SkillName.MagicResist, 15.3, 30);

            VirtualArmor = 11;
            SetDamage(4, 10);

            Fame = 300;
        }

		public override int Meat{ get{ return 1; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Feathers{ get{ return 36; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public Eagle(Serial serial) : base(serial)
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