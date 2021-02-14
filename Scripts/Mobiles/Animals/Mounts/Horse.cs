using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a horse corpse" )]
	[TypeAlias( "Server.Mobiles.BrownHorse", "Server.Mobiles.DirtyHorse", "Server.Mobiles.GrayHorse", "Server.Mobiles.TanHorse" )]
	public class Horse : BaseMount
	{
		private static int[] m_IDs = new int[]
			{
				0xC8, 0x3E9F,
				0xE2, 0x3EA0,
				0xE4, 0x3EA1,
				0xCC, 0x3EA2
			};

		[Constructable]
		public Horse() : this( "a horse" )
		{
		}

		[Constructable]
		public Horse( string name ) : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			int random = Utility.Random( 4 );

			Body = m_IDs[random * 2];
			ItemID = m_IDs[random * 2 + 1];
			BaseSoundID = 0xA8;

			Name = "a horse";
			SetStr( 44, 120 );
			SetHits( 44, 120 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 6, 10 );
			SetMana( 0 );
			Tamable = true;
			MinTameSkill = 45;
			SetSkill( SkillName.Wrestling, 29.3, 44 );
			SetSkill( SkillName.Parry, 35.1, 45 );
			SetSkill( SkillName.Tactics, 29.3, 44 );
			SetSkill( SkillName.MagicResist, 25.1, 30 );

			VirtualArmor = 9;
			SetDamage( 4, 12 );


            Fame = 300;
            Karma = 300;
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Horse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}