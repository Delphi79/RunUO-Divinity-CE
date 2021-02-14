using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a walrus corpse" )]
	public class Walrus : BaseCreature
	{
		[Constructable]
        public Walrus()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 221;
            Name = "a walrus";
            SetStr(21, 29);
            SetHits(21, 29);
            SetDex(46, 55);
            SetStam(46, 55);
            SetInt(16, 20);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 50;
            BaseSoundID = 224;
            SetSkill(SkillName.Wrestling, 19.2, 29);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 19.2, 29);
            SetSkill(SkillName.MagicResist, 15.1, 20);

            VirtualArmor = 9;
            SetDamage(3, 6);

            Fame = 150;

			BardLevel = 45;
        }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish; } }

		public Walrus(Serial serial) : base(serial)
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