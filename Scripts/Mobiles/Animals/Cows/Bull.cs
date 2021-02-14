using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a bull corpse" )]
	public class Bull : BaseCreature
	{
		[Constructable]
        public Bull()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = Utility.RandomList(232, 233);
            Name = "a bull";
            SetStr(77, 111);
            SetHits(77, 111);
            SetDex(56, 75);
            SetStam(56, 75);
            SetInt(47, 75);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 80;
            BaseSoundID = 120;
            SetSkill(SkillName.Wrestling, 40.1, 57.5);
            SetSkill(SkillName.Parry, 42.6, 55);
            SetSkill(SkillName.Tactics, 67.6, 85);
            SetSkill(SkillName.MagicResist, 17.6, 25);

            VirtualArmor = 14;
            SetDamage(4, 9);

			BardLevel = 15;
        }

		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 15; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bull; } }

		public Bull(Serial serial) : base(serial)
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