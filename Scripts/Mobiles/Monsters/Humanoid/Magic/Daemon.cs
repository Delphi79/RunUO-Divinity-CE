using System;
using Server;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class Daemon : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }

		public override Faction FactionAllegiance { get { return Shadowlords.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }

		[Constructable]
		public Daemon () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 9;
            Name = NameList.RandomName("daemon");
			SetStr( 276, 305 );
			SetHits( 276, 305 );
			SetDex( 76, 95 );
			SetStam( 76, 95 );
			SetInt( 101, 125 );
			SetMana( 376, 400 );

            Fame = 15000;
            Karma = -15000;

			BaseSoundID = 357;
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Parry, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.Magery, 100);

			VirtualArmor = Utility.RandomMinMax( 3, 24 );
			SetDamage( 30 );

			//BardImmune = true;
			BardLevel = 85;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			PackScroll( 4, 8 );
			PackScroll( 4, 8 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }

		public Daemon( Serial serial ) : base( serial )
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
