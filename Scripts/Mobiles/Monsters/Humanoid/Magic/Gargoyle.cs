using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gargoyle corpse" )]
	public class Gargoyle : BaseCreature
	{
		[Constructable]
        public Gargoyle()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 4;
            Name = "a gargoyle";
            SetStr(146, 175);
            SetHits(146, 175);
            SetDex(76, 95);
            SetStam(76, 95);
            SetInt(81, 105);
            SetMana(81, 105);

            Fame = 3500;
            Karma = -3500;

            BaseSoundID = 372;
            SetSkill(SkillName.Wrestling, 40.1, 80);
            SetSkill(SkillName.Parry, 35.1, 45);
            SetSkill(SkillName.Tactics, 50.1, 70);
            SetSkill(SkillName.MagicResist, 70.1, 85);
            SetSkill(SkillName.Magery, 70.1, 85);

            VirtualArmor = 16;
            SetDamage(3, 18);
			BardLevel = 75;
        }

        public override void GenerateLoot()
        {
            PackGold(200, 250);
            PackScroll(4, 8);
            PackScroll(4, 8);

            AddLoot(LootPack.Gems);
        }


		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public Gargoyle( Serial serial ) : base( serial )
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