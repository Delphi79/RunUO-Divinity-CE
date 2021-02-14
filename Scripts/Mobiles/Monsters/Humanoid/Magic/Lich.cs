using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class Lich : BaseCreature
	{
		[Constructable]
        public Lich()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 24;
            Name = "a lich";
            SetStr(106, 135);
            SetHits(106, 135);
            SetDex(66, 85);
            SetStam(66, 85);
            SetInt(176, 205);
            SetMana(276, 375);

            Fame = 8000;
            Karma = -8000;

            BaseSoundID = 412;
            SetSkill(SkillName.Parry, 55.1, 65);
            SetSkill(SkillName.Tactics, 75.1, 83);
            SetSkill(SkillName.Wrestling, 75.1, 83);
            SetSkill(SkillName.MagicResist, 70.1, 90);
            SetSkill(SkillName.Magery, 80, 100);

            VirtualArmor = 25;
            SetDamage(15, 25);

			BardLevel = 75;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );

            if (Utility.RandomBool())
                PackGem();

            PackGold(25, 75);
            PackScroll(1, 8);
            PackScroll(4, 8);
            PackScroll(4, 8);
        }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public Lich( Serial serial ) : base( serial )
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