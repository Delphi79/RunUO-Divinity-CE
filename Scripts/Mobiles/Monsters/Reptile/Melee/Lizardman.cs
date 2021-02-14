using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a lizardman corpse" )]
	public class Lizardman : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Lizardman; } }

		[Constructable]
		public Lizardman() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = Utility.RandomList( 33,35,36 );
			Name = NameList.RandomName( "lizardman" );
			SetStr( 96, 120 );
			SetHits( 86, 110 );
			SetDex( 86, 105 );
			SetStam( 76, 95 );
			SetInt( 36, 60 );
			SetMana( 0 );

			Fame = 1500;
			Karma = -1500;

			BaseSoundID = 417;
			SetSkill( SkillName.Wrestling, 50.1, 70 );
			SetSkill( SkillName.Parry, 55.1, 75 );
			SetSkill( SkillName.Tactics, 55.1, 80 );
			SetSkill( SkillName.MagicResist, 35.1, 60 );

			VirtualArmor = 14;
			SetDamage( 5, 11 );
			BardLevel = 65;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
            PackItem( Loot.RandomWeapon() );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public Lizardman( Serial serial ) : base( serial )
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