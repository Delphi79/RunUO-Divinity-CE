using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a giant rat corpse" )]
	[TypeAlias( "Server.Mobiles.Giantrat" )]
	public class GiantRat : BaseCreature
	{
		[Constructable]
        public GiantRat()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 215;
            Name = "a giant rat";
            SetStr(32, 74);
            SetHits(22, 64);
            SetDex(46, 65);
            SetStam(46, 65);
            SetInt(16, 30);
            SetMana(0);

            Fame = 300;
            Karma = -300;

            Tamable = true;
            MinTameSkill = 45;
            BaseSoundID = 389;
            SetSkill(SkillName.Wrestling, 29.3, 44);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 29.3, 44);
            SetSkill(SkillName.MagicResist, 25.1, 30);

            VirtualArmor = 9;
            SetDamage(3, 9);
        }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.Meat | FoodType.FruitsAndVegies | FoodType.Eggs; } }

		public GiantRat(Serial serial) : base(serial)
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