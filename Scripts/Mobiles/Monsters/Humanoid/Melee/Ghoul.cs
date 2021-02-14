using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a ghoulish corpse" )]
	public class Ghoul : BaseCreature
	{
		[Constructable]
        public Ghoul()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 26;
            switch (Utility.Random(3))
            {
                case 0: Name = "a spectre"; break;
                case 1: Name = "a shade"; break;
                case 2: Name = "a ghoul"; break;
            }
            SetStr(102, 112);
            SetHits(76, 100);
            SetDex(76, 95);
            SetStam(76, 95);
            SetInt(36, 60);
            SetMana(36, 60);

            Fame = 2500;
            Karma = -2500;

            BaseSoundID = 382;
            SetSkill(SkillName.Wrestling, 45.1, 55);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 45.1, 60);
            SetSkill(SkillName.MagicResist, 35.1, 50);

            VirtualArmor = 14;
            SetDamage(12, 18);
			BardLevel = 55;
        }

        public override void GenerateLoot()
        {
            if (Utility.RandomBool())
                PackGem();
            PackScroll(1, 8);
            AddLoot(LootPack.Average, 1);
        }


		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public Ghoul( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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