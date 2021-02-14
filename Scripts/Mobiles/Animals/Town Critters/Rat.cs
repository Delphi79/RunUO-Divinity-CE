using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a rat corpse" )]
	public class Rat : BaseCreature
	{
		[Constructable]
		public Rat() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			Body = 238;
			Name = "a rat";
			Hue = 443;
			SetStr( 11, 17 );
			SetHits( 9, 17 );
			SetDex( 16, 25 );
			SetStam( 40, 70 );
			SetInt( 6, 10 );
			SetMana( 0 );

			Tamable = true;
			MinTameSkill = 20;
			BaseSoundID = 204;
			SetSkill( SkillName.Wrestling, 9.2, 17 );
			SetSkill( SkillName.Parry, 15.1, 25 );
			SetSkill( SkillName.Tactics, 9.2, 17 );
			SetSkill( SkillName.MagicResist, 5.1, 10 );

			VirtualArmor = 5;
			SetDamage( 1, 4 );
		
			Karma = -150;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.GrainsAndHay; } }

		public Rat(Serial serial) : base(serial)
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