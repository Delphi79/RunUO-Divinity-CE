using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a polar bear corpse" )]
	[TypeAlias( "Server.Mobiles.Polarbear" )]
	public class PolarBear : BaseCreature
	{
		[Constructable]
		public PolarBear() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			Body = 213;
			Name = "a polar bear";
			Hue = 2301;
			SetStr( 116, 140 );
			SetHits( 116, 140 );
			SetDex( 81, 105 );
			SetStam( 81, 105 );
			SetInt( 26, 50 );
			SetMana( 0 );
			Tamable = true;
			MinTameSkill = 50;
			BaseSoundID = 95;
			SetSkill( SkillName.Wrestling, 60.1, 90 );
			SetSkill( SkillName.Parry, 70.1, 85 );
			SetSkill( SkillName.Tactics, 70.1, 100 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );

			VirtualArmor = 9;
			SetDamage( 5, 14 );

            Fame = 1500;
			Karma = 0;

			BardLevel = 35;
		}

		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 16; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

		public PolarBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}