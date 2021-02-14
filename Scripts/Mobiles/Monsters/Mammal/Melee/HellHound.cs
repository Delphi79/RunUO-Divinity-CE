using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a hell hound corpse" )]
	public class HellHound : BaseCreature
	{
		[Constructable]
		public HellHound() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 225;
			Name = "a hell hound";
			Hue = Utility.RandomRedHue();
			SetStr( 96, 120 );
			SetHits( 220, 240 );
			SetDex( 81, 105 );
			SetStam( 91, 115 );
			SetInt( 36, 60 );
			SetMana( 71, 95 );

			BaseSoundID = 229;
			SetSkill( SkillName.Wrestling, 60.1, 80 );
			SetSkill( SkillName.Parry, 62.6, 75 );
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 57.6, 75 );

			VirtualArmor = 11;
			SetDamage( 6, 22 );

            Fame = 3400;
            Karma = -3400;

            Tamable = true;
            MinTameSkill = 92;
			//BardImmune = true;
			BardLevel = 50;
		}

		public override void GenerateLoot()
		{
			Item item = null;

			item = new SulfurousAsh( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );

			item = new SulfurousAsh( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );

			item = new SulfurousAsh( Utility.RandomMinMax( 1, 3 ) );
			PackItem( item );

			AddLoot( LootPack.Meager );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public HellHound( Serial serial ) : base( serial )
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