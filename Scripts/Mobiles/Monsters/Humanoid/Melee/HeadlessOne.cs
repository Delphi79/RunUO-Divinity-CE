using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a headless corpse" )]
	public class HeadlessOne : BaseCreature
	{
		[Constructable]
        public HeadlessOne()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 31;
            Name = "a headless one";
            Hue = Utility.RandomSkinHue();
            SetStr(26, 50);
            SetHits(26, 50);
            SetDex(36, 55);
            SetStam(36, 55);
            SetInt(16, 30);
            SetMana(16, 30);

            Fame = 500;
            Karma = -500;

            BaseSoundID = 407;
            SetSkill(SkillName.Wrestling, 25.1, 40);
            SetSkill(SkillName.Parry, 35.1, 45);
            SetSkill(SkillName.Tactics, 25.1, 40);
            SetSkill(SkillName.MagicResist, 15.1, 20);

            VirtualArmor = 9;
            SetDamage(3, 12);
			BardLevel = 45;
        }

        public override void GenerateLoot()
        {
			AddLoot(LootPack.Poor);
        }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }

		public HeadlessOne( Serial serial ) : base( serial )
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