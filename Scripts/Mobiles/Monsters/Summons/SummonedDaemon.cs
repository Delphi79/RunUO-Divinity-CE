using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class SummonedDaemon : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
        public SummonedDaemon()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 9;
            Name = NameList.RandomName("daemon");
            SetStr(200);
            SetHits(166, 185);
            SetDex(266, 275);
            SetStam(66, 75);
            SetInt(291, 305);
            SetMana(91, 105);

            BaseSoundID = 357;
            SetSkill(SkillName.Wrestling, 60.1, 80);
            SetSkill(SkillName.Parry, 65.1, 75);
            SetSkill(SkillName.Tactics, 70.1, 80);
            SetSkill(SkillName.MagicResist, 70.1, 80);
            SetSkill(SkillName.Magery, 70.1, 80);

            VirtualArmor = Utility.RandomMinMax(3, 18);
            SetDamage(30);

            ControlSlots = 2;
        }

		public override Poison PoisonImmune{ get{ return Poison.Regular; } } // TODO: Immune to poison?

		public SummonedDaemon( Serial serial ) : base( serial )
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