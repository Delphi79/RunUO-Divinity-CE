using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a harpy corpse" )]
	public class Harpy : BaseCreature
	{
		[Constructable]
        public Harpy()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 30;
            Name = "a harpy";
            SetStr(110, 125);
            SetHits(96, 120);
            SetDex(86, 110);
            SetStam(86, 110);
            SetInt(51, 75);
            SetMana(51, 75);

            Fame = 2500;
            Karma = -2500;

            BaseSoundID = 402;
            SetSkill(SkillName.Wrestling, 60.1, 80);
            SetSkill(SkillName.Parry, 75.1, 90);
            SetSkill(SkillName.Tactics, 70.1, 80);
            SetSkill(SkillName.MagicResist, 50.1, 65);

            VirtualArmor = 14;
            SetDamage(3, 9);
			BardLevel = 70;
        }

        public override void GenerateLoot()
        {
            PackGold(50, 75);
        }

		public override int GetAttackSound()
		{
			return 916;
		}

		public override int GetAngerSound()
		{
			return 916;
		}

		public override int GetDeathSound()
		{
			return 917;
		}

		public override int GetHurtSound()
		{
			return 919;
		}

		public override int GetIdleSound()
		{
			return 918;
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 4; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Feathers{ get{ return 50; } }

		public Harpy( Serial serial ) : base( serial )
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