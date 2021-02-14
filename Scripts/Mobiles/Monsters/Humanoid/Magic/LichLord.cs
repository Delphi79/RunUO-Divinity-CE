using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class LichLord : BaseCreature
	{
		[Constructable]
        public LichLord()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 24;
            Name = "a lich lord";
            SetStr(416, 505);
            SetHits(416, 505);
            SetDex(96, 115);
            SetStam(96, 115);
            SetInt(566, 655);
            SetMana(566, 855);

            Fame = 18000;
            Karma = -18000;

            BaseSoundID = 412;
            SetSkill(SkillName.Wrestling, 83.1, 90);
            SetSkill(SkillName.Parry, 55.1, 65);
            SetSkill(SkillName.Tactics, 83.1, 90);
            SetSkill(SkillName.MagicResist, 90.1, 100);
            SetSkill(SkillName.Magery, 95, 110);

            VirtualArmor = 26;
            SetDamage(6, 18);

			//BardImmune = true;
			BardLevel = 90.0;
        }

        public override void GenerateLoot()
        {
            PackGold(65, 85);

            if (Utility.RandomBool())
                PackGem();

            PackScroll(1, 8);

            //AddLoot(LootPack.MagicWeapons);
            AddLoot(LootPack.FilthyRich);

            PackScroll(4, 8);
            PackScroll(4, 8);
        }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

        public override bool AutoDispel { get { return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 4; } }

		public LichLord( Serial serial ) : base( serial )
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