using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a hare corpse" )]
	public class Rabbit : BaseCreature
	{
		[Constructable]
        public Rabbit()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 205;
            Name = "a rabbit";
            SetStr(6, 10);
            SetHits(4, 8);
            SetDex(26, 38);
            SetStam(40, 70);
            SetInt(6, 14);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 5;
            SetSkill(SkillName.Wrestling, 5.1, 10);
            SetSkill(SkillName.Parry, 25.1, 38);
            SetSkill(SkillName.Tactics, 5.1, 10);
            SetSkill(SkillName.MagicResist, 5.1, 14);

            VirtualArmor = Utility.RandomMinMax(2, 7);
            SetDamage(1, 2);
        }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies; } }

		public Rabbit(Serial serial) : base(serial)
		{
		}

		public override int GetAttackSound() 
		{ 
			return 0xC9; 
		} 

		public override int GetHurtSound() 
		{ 
			return 0xCA; 
		} 

		public override int GetDeathSound() 
		{ 
			return 0xCB; 
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