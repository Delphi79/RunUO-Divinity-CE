using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a grey wolf corpse" )]
	[TypeAlias( "Server.Mobiles.Greywolf" )]
	public class GreyWolf : BaseCreature
	{
		[Constructable]
		public GreyWolf() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			Body = 225;
			Name = "a grey wolf";
			Hue = 946;
			SetStr( 56, 80 );
			SetHits( 56, 80 );
			SetDex( 56, 75 );
			SetStam( 56, 75 );
			SetInt( 31, 55 );
			SetMana( 0 );
			Tamable = true;
			MinTameSkill = 65;
			BaseSoundID = 229;
			SetSkill( SkillName.Wrestling, 45.1, 60 );
			SetSkill( SkillName.Parry, 45.1, 55 );
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 20.1, 35 );

			VirtualArmor = 9;
			SetDamage( 2, 8 );

			Fame = 450;
			Karma = 0;

			BardLevel = 30;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public GreyWolf(Serial serial) : base(serial)
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