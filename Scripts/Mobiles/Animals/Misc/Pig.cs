using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a pig corpse" )]
	public class Pig : BaseCreature
	{
		[Constructable]
        public Pig()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 203;
            Name = "a pig";
            SetStr(22, 64);
            SetHits(23, 65);
            SetDex(22, 64);
            SetStam(23, 65);
            SetInt(26, 33);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 30;
            BaseSoundID = 196;
            SetSkill(SkillName.Wrestling, 19.3, 34);
            SetSkill(SkillName.Parry, 19.3, 34);
            SetSkill(SkillName.Tactics, 19.3, 34);
            SetSkill(SkillName.MagicResist, 25.1, 33);

            VirtualArmor = 6;
            SetDamage(2, 6);

            Fame = 150;

			BardLevel = 10;
        }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Pig(Serial serial) : base(serial)
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