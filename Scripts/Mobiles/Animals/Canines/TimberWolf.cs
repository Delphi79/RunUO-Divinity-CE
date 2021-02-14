using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a timber wolf corpse" )]
	[TypeAlias( "Server.Mobiles.Timberwolf" )]
	public class TimberWolf : BaseCreature
	{
		[Constructable]
		public TimberWolf() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			Body = 225;
			Name = "a timber wolf";
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 11, 25 );
			SetMana( 0 );
			Tamable = true;
			MinTameSkill = 40;
			BaseSoundID = 229;
			SetSkill( SkillName.Wrestling, 40.1, 60 );
			SetSkill( SkillName.Parry, 42.6, 55 );
			SetSkill( SkillName.Tactics, 30.1, 50 );
			SetSkill( SkillName.MagicResist, 27.6, 45 );

			VirtualArmor = 9;
			SetDamage( 4, 10 );

            Fame = 450;

			BardLevel = 25;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 5; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public TimberWolf(Serial serial) : base(serial)
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