using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a ratman's corpse" )]
	public class Ratman : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Ratman; } }

		[Constructable]
        public Ratman()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = Utility.RandomList(42, 44, 45);
            Name = NameList.RandomName("ratman");
            SetStr(96, 120);
            SetHits(96, 150);
            SetDex(81, 100);
            SetStam(81, 100);
            SetInt(36, 60);
            SetMana(36, 60);

            Fame = 1500;
            Karma = -1500;

            BaseSoundID = 437;
            SetSkill(SkillName.Wrestling, 50.1, 75);
            SetSkill(SkillName.Parry, 50.1, 70);
            SetSkill(SkillName.Tactics, 50.1, 75);
            SetSkill(SkillName.MagicResist, 35.1, 60);

            VirtualArmor = 14;
            SetDamage(3, 10);
			BardLevel = 40;
        }

        public override void GenerateLoot()
        {
			AddLoot( LootPack.Meager );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Hides{ get{ return 8; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public Ratman( Serial serial ) : base( serial )
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