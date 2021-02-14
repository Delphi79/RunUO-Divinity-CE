using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a troll corpse" )]
	public class Troll : BaseCreature
	{
		[Constructable]
		public Troll () : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = Utility.RandomList( 54,53,55 );
			Name = "a troll";
			SetStr( 176, 205 );
			SetHits( 176, 205 );
			SetDex( 46, 65 );
			SetStam( 46, 65 );
			SetInt( 46, 70 );
			SetMana( 0 );

            Fame = 3500;
            Karma = -3500;

			BaseSoundID = 461;
			SetSkill( SkillName.Wrestling, 75, 100 );
			SetSkill( SkillName.Parry, 45.1, 60 );
			SetSkill( SkillName.Tactics, 50.1, 70 );
			SetSkill( SkillName.MagicResist, 45.1, 60 );

			VirtualArmor = 20;
			SetDamage( 15, 20 );
			BardLevel = 55;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
            PackGold(75);
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 2; } }

		public Troll( Serial serial ) : base( serial )
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