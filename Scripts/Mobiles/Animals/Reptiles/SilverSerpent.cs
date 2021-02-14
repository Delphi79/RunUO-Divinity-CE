using System;
using Server.Mobiles;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName("a silver serpent corpse")]
	[TypeAlias( "Server.Mobiles.Silverserpant" )]
	public class SilverSerpent : BaseCreature
	{
		public override Faction FactionAllegiance { get { return TrueBritannians.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Hero; } }

		[Constructable]
		public SilverSerpent() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 52;
			Name = "a silver serpent";
			SetStr( 61, 80 );
			SetHits( 26, 45 );
			SetDex( 51, 65 );
			SetStam( 46, 55 );
			SetInt( 11, 20 );
			SetMana( 26, 40 );
			Tamable = true;
			MinTameSkill = 60;
			BaseSoundID = 219;
			SetSkill( SkillName.Wrestling, 40.1, 55 );
			SetSkill( SkillName.Parry, 45.1, 60 );
			SetSkill( SkillName.Tactics, 40.1, 45 );
			SetSkill( SkillName.MagicResist, 25.1, 40 );

			VirtualArmor = 9;
			SetDamage( 1, 8 );
		
			Fame = 5000;
			Karma = -5000;

			BardLevel = 50;
		}

		public override void GenerateLoot()
		{
		}

		public override bool DeathAdderCharmable{ get{ return true; } }

		public override int Meat{ get{ return 1; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public SilverSerpent(Serial serial) : base(serial)
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

			if ( BaseSoundID == -1 )
				BaseSoundID = 219;
		}
	}
}