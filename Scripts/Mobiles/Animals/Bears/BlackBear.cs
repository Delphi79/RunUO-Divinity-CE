using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a bear corpse" )]
	[TypeAlias( "Server.Mobiles.Bear" )]
	public class BlackBear : BaseCreature
	{
		[Constructable]
        public BlackBear()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 211;
            Name = "a black bear";
            SetStr(76, 100);
            SetHits(76, 100);
            SetDex(56, 75);
            SetStam(56, 75);
            SetInt(11, 14);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 50;
            BaseSoundID = 95;
            SetSkill(SkillName.Wrestling, 40.1, 60);
            SetSkill(SkillName.Parry, 25.1, 45);
            SetSkill(SkillName.Tactics, 40.1, 60);
            SetSkill(SkillName.MagicResist, 20.1, 40);

            VirtualArmor = 8;
            SetDamage(2, 12);

			BardLevel = 10;
        }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.Meat | FoodType.FruitsAndVegies; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public BlackBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}