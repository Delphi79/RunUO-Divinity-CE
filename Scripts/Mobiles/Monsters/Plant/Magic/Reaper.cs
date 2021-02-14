using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a reapers corpse" )]
	public class Reaper : BaseCreature
	{
		[Constructable]
		public Reaper() : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 47;
			Name = "a reaper";
			SetStr( 66, 80 );
			SetHits( 146, 160 );
			SetDex( 66, 75 );
			SetStam( 0 );
			SetInt( 110, 120 );
			SetMana( 110 );

			SetSkill( SkillName.Wrestling, 50.1, 60 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 35.1, 50 );
			SetSkill( SkillName.Magery, 40.1, 50 );

			VirtualArmor = 16;
			SetDamage( 5, 15 );

			Fame = 3500;
			Karma = -3500;
			BardLevel = 75;

            CantWalk = true;
            //Frozen = true;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
            Item item = null;

            item = new Log(Utility.RandomMinMax(1, 10));
            PackItem(item);
		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int TreasureMapLevel{ get{ return 2; } }
		public override bool DisallowAllMoves{ get{ return true; } }

		public Reaper( Serial serial ) : base( serial )
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
            //Frozen = true;
		}
	}
}