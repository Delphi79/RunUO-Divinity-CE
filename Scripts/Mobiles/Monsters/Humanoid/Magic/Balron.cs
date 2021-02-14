using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a balron corpse" )]
	public class Balron : BaseCreature
	{
		[Constructable]
		public Balron () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.3, 0.5 )
		{
			Body = 10;
			Name = NameList.RandomName( "balron" );
			SetStr( 386, 585 );
			SetHits( 401, 800 );
			SetDex( 77, 155 );
			SetStam( 156, 255 );
			SetInt( 226, 525 );
			SetMana( 451, 550 );

            Fame = 24000;
            Karma = -24000;

            Hue = Utility.RandomMinMax(1106, 1110);

			BaseSoundID = 357;
			SetSkill( SkillName.Wrestling, 90.1, 100 );
			SetSkill( SkillName.Parry, 90.1, 100 );
			SetSkill( SkillName.Tactics, 90.1, 100 );
			SetSkill( SkillName.MagicResist, 90.1, 100 );
			SetSkill( SkillName.Magery, 90.1, 100 );

			VirtualArmor = Utility.RandomMinMax( 18, 33 );
			SetDamage( 25 );

			//BardImmune = true;
			BardLevel = 97.0;
		}

        private static int GetRandomOldBonus()
        {
            int rnd = Utility.RandomMinMax(0, 100);

            if (50 > rnd)
                return 1;
            else
                rnd -= 50;

            if (25 > rnd)
                return 2;
            else
                rnd -= 25;

            if (14 > rnd)
                return 3;
            else
                rnd -= 14;

            if (8 > rnd)
                return 4;

            return 5;
        }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Meager);
            PackScroll(4, 8);
            PackScroll(4, 8);
		}

        public override bool AutoDispel { get { return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 1; } }

		public Balron( Serial serial ) : base( serial )
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