using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class StrongMongbat : BaseCreature
	{
		[Constructable]
		public StrongMongbat() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 39;
			Name = "a mongbat";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 61, 80 );
			SetStam( 61, 80 );
			SetInt( 26, 50 );
			SetMana( 26, 50 );
		
			Fame = 150;
			Karma = -150;

			Tamable = true;
			MinTameSkill = 80;
			BaseSoundID = 422;
			SetSkill( SkillName.Wrestling, 20.1, 35 );
			SetSkill( SkillName.Parry, 50.1, 60 );
			SetSkill( SkillName.Tactics, 35.1, 50 );
			SetSkill( SkillName.MagicResist, 15.1, 30 );

			VirtualArmor = 10;
			SetDamage( 3, 9 );
			BardLevel = 35;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public StrongMongbat( Serial serial ) : base( serial )
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