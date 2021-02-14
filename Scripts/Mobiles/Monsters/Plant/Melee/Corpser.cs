using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a corpser corpse" )]
	public class Corpser : BaseCreature
	{
		[Constructable]
		public Corpser() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 8;
			Name = "a corpser";
			SetStr( 100, 112 );
			SetHits( 56, 80 );
			SetDex( 26, 45 );
			SetStam( 26, 50 );
			SetInt( 26, 40 );
			SetMana( 0 );

			BaseSoundID = 352;
			SetSkill( SkillName.Wrestling, 45.1, 60 );
			SetSkill( SkillName.Parry, 15.1, 25 );
			SetSkill( SkillName.Tactics, 45.1, 60 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );

			VirtualArmor = 9;
			SetDamage( 3, 6 );

            CantWalk = true;
            Frozen = true;
            Fame = 1000;
			Karma = -1000;
			BardLevel = 60;
		}

		public override void GenerateLoot()
		{
			PackGold( 50, 70 );

            Item item = new Board();
            item.Amount = 10;

            PackItem(item);
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override bool DisallowAllMoves{ get{ return true; } }

		public Corpser( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 352 )
				BaseSoundID = 684;

            Frozen = true;
		}
	}
}
