using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a giant spider corpse" )]
	public class GiantSpider : BaseCreature
	{
		[Constructable]
		public GiantSpider() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 28;
			Name = "a giant spider";
			SetStr( 76, 100 );
			SetHits( 76, 100 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 36, 60 );

			Tamable = true;
			MinTameSkill = 70;
			BaseSoundID = 387;
			SetSkill( SkillName.Wrestling, 50.1, 65 );
			SetSkill( SkillName.Parry, 35.1, 45 );
			SetSkill( SkillName.Tactics, 35.1, 50 );
			SetSkill( SkillName.MagicResist, 25.1, 40 );

			VirtualArmor = 8;
			SetDamage( 3, 15 );
			BardLevel = 60;

			Fame = 600;
            Karma = -600;
		}

		public override void GenerateLoot()
		{
            PackItem(new SpidersSilk(Utility.RandomMinMax( 1, 4 )));
            //AddLoot( LootPack.Poor ); // not valid?
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }

		public GiantSpider( Serial serial ) : base( serial )
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