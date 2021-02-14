using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gazer corpse" )]
	public class Gazer : BaseCreature
	{
		[Constructable]
        public Gazer()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 22;
            Name = "a gazer";
            SetStr(96, 125);
            SetHits(86, 115);
            SetDex(86, 105);
            SetStam(46, 65);
            SetInt(41, 65);
            SetMana(41, 55);

            Fame = 3500;
            Karma = -3500;

            BaseSoundID = 377;
            SetSkill(SkillName.Wrestling, 50.1, 70);
            SetSkill(SkillName.Parry, 55.1, 65);
            SetSkill(SkillName.Tactics, 50.1, 70);
            SetSkill(SkillName.MagicResist, 50.1, 65);
            SetSkill(SkillName.Magery, 50.1, 65);

            VirtualArmor = 19;
            SetDamage(3, 12);
			BardLevel = 65;
        }

        public override void GenerateLoot()
        {
            PackGold(100, 150);
            PackScroll(3, 6);
            PackScroll(3, 6);
        }

		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public Gazer( Serial serial ) : base( serial )
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