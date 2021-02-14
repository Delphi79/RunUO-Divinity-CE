using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a leopard corpse" )]
	[TypeAlias( "Server.Mobiles.Snowleopard" )]
	public class SnowLeopard : BaseCreature
	{
		[Constructable]
        public SnowLeopard()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 214;
            Name = "a snow leopard";
            Hue = 2301;
            SetStr(56, 80);
            SetHits(56, 80);
            SetDex(66, 85);
            SetStam(66, 85);
            SetInt(26, 50);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 65;
            BaseSoundID = 186;
            SetSkill(SkillName.Wrestling, 40.1, 50);
            SetSkill(SkillName.Parry, 55.1, 65);
            SetSkill(SkillName.Tactics, 45.1, 60);
            SetSkill(SkillName.MagicResist, 25.1, 35);

            VirtualArmor = 12;
            SetDamage(2, 10);

            Fame = 450;
        }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 8; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Feline; } }

		public SnowLeopard(Serial serial) : base(serial)
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