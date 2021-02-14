using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an alligator corpse" )]
	public class Alligator : BaseCreature
	{
		[Constructable]
        public Alligator()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 202;
            Name = "an alligator";
            SetStr(76, 100);
            SetHits(76, 100);
            SetDex(6, 25);
            SetStam(46, 65);
            SetInt(11, 20);
            SetMana(0);

            Tamable = true;
            MinTameSkill = 60;
            BaseSoundID = 90;
            SetSkill(SkillName.Wrestling, 40.1, 60);
            SetSkill(SkillName.Parry, 37.6, 72.5);
            SetSkill(SkillName.Tactics, 40.1, 60);
            SetSkill(SkillName.MagicResist, 25.1, 40);

            VirtualArmor = 15;
            SetDamage(2, 18);

            Fame = 600;
            Karma = -600;

			BardLevel = 35;
        }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public Alligator(Serial serial) : base(serial)
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

			if ( BaseSoundID == 0x5A )
				BaseSoundID = 660;
		}
	}
}