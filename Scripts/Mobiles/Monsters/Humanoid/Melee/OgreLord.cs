using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "an ogre lord\'s corpse" )]
	public class OgreLord : BaseCreature
	{
		public override Faction FactionAllegiance { get { return Minax.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }

		[Constructable]
        public OgreLord()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 1;
            Name = "an ogre lord";
            SetStr(267, 445);
            SetHits(666, 755);
            SetDex(66, 75);
            SetStam(86, 175);
            SetInt(46, 70);
            SetMana(0);

            Fame = 15000;
            Karma = -15000;

            BaseSoundID = 427;
            SetSkill(SkillName.Wrestling, 90.1, 100);
            SetSkill(SkillName.Parry, 75.1, 85);
            SetSkill(SkillName.Tactics, 90.1, 100);
            SetSkill(SkillName.MagicResist, 65.1, 80);

            VirtualArmor = 25;
            SetDamage(10, 40);
			BardLevel = 85;
        }

        public override void GenerateLoot()
        {
            Item item = null;

            AddLoot(LootPack.FilthyRich);
            PackGold(200);

            item = new Club();
            PackItem(item);
        }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 2; } }

		public OgreLord( Serial serial ) : base( serial )
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