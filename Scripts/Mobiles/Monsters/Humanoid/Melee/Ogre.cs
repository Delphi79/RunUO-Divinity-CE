using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an ogre corpse" )]
	public class Ogre : BaseCreature
	{
		[Constructable]
        public Ogre()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 1;
            Name = "an ogre";
            SetStr(166, 195);
            SetHits(166, 195);
            SetDex(46, 65);
            SetStam(46, 65);
            SetInt(46, 70);
            SetMana(0);

            Fame = 3000;
            Karma = -3000;

            BaseSoundID = 427;
            SetSkill(SkillName.Wrestling, 70.1, 80);
            SetSkill(SkillName.Parry, 45.1, 55);
            SetSkill(SkillName.Tactics, 60.1, 70);
            SetSkill(SkillName.MagicResist, 45.1, 60);

            VirtualArmor = Utility.RandomMinMax(5, 15);
            SetDamage(16);
			BardLevel = 65;
        }

        public override void GenerateLoot()
        {
            Item item = null;

            AddLoot(LootPack.Average);
            PackGold(25);

            item = new Club();
            AddItem(item);
        }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 2; } }

		public Ogre( Serial serial ) : base( serial )
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