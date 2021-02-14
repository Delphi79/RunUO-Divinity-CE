using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an ettins corpse" )]
	public class Ettin : BaseCreature
	{
		[Constructable]
        public Ettin()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 18;
            Name = "an ettin";
            SetStr(136, 165);
            SetHits(136, 165);
            SetDex(56, 75);
            SetStam(56, 75);
            SetInt(31, 55);
            SetMana(0);

            Fame = 3000;
            Karma = -3000;

            BaseSoundID = 367;
            SetSkill(SkillName.Wrestling, 50.1, 60);
            SetSkill(SkillName.Parry, 50.1, 60);
            SetSkill(SkillName.Tactics, 50.1, 70);
            SetSkill(SkillName.MagicResist, 40.1, 55);

            VirtualArmor = 19;
            SetDamage(10, 20);
			BardLevel = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            PackGold(35);
        }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 4; } }

		public Ettin( Serial serial ) : base( serial )
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