using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a sewer rat corpse" )]
	public class Sewerrat : BaseCreature
	{
		[Constructable]
        public Sewerrat()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 238;
            Name = "a sewer rat";
            Hue = 443;
            SetStr(11, 19);
            SetHits(11, 19);
            SetDex(36, 45);
            SetStam(36, 45);
            SetInt(6, 10);
            SetMana(0);

            Karma = -300;

            Tamable = true;
            MinTameSkill = 20;
            BaseSoundID = 204;
            SetSkill(SkillName.Wrestling, 5.1, 10);
            SetSkill(SkillName.Parry, 35.1, 45);
            SetSkill(SkillName.Tactics, 5.1, 10);
            SetSkill(SkillName.MagicResist, 5.1, 10);

            VirtualArmor = Utility.RandomMinMax(1, 3);
            SetDamage(1, 3);
        }

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Eggs | FoodType.FruitsAndVegies; } }

		public Sewerrat(Serial serial) : base(serial)
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