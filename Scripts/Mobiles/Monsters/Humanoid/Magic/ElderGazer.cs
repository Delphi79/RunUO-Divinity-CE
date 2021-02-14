using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an elder gazer corpse" )]
	public class ElderGazer : BaseCreature
	{
		[Constructable]
        public ElderGazer()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 22;
            Name = "an elder gazer";
            SetStr(150, 170);
            SetHits(186, 215);
            SetDex(86, 105);
            SetStam(46, 65);
            SetInt(91, 185);
            SetMana(191, 285);

            Fame = 12500;
            Karma = -12500;

            BaseSoundID = 377;
            SetSkill(SkillName.Wrestling, 80.1, 100);
            SetSkill(SkillName.Parry, 55.1, 65);
            SetSkill(SkillName.Tactics, 80.1, 100);
            SetSkill(SkillName.MagicResist, 85.1, 100);
            SetSkill(SkillName.Magery, 90.1, 100);

            VirtualArmor = 25;
            SetDamage(3, 24);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
        }

		public override int TreasureMapLevel{ get{ return 1; } }

		public ElderGazer( Serial serial ) : base( serial )
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