using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a deer corpse" )]
	public class Hind : BaseCreature
	{
		[Constructable]
        public Hind()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 237;
            Name = "a hind";
            SetStr(21, 51);
            SetHits(31, 49);
            SetDex(47, 77);
            SetStam(41, 53);
            SetInt(17, 47);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 40;
            BaseSoundID = 128;
            SetSkill(SkillName.Wrestling, 26.2, 38);
            SetSkill(SkillName.Parry, 22.7, 34.5);
            SetSkill(SkillName.Tactics, 19.2, 31);
            SetSkill(SkillName.MagicResist, 15.2, 27);

            VirtualArmor = 8;
            SetDamage(4, 11);

			BardLevel = 10;
        }

		public override int Meat{ get{ return 5; } }
		public override int Hides{ get{ return 8; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Hind(Serial serial) : base(serial)
		{
		}

		public override int GetAttackSound() 
		{ 
			return 0x82; 
		} 

		public override int GetHurtSound() 
		{ 
			return 0x83; 
		} 

		public override int GetDeathSound() 
		{ 
			return 0x84; 
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