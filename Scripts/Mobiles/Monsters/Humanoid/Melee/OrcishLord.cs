using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcishLord : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
        public OrcishLord()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 7;
            Name = "An Orcish Lord";
            SetStr(140, 150);
            SetHits(111, 145);
            SetDex(94, 190);
            SetStam(101, 135);
            SetInt(64, 160);
            SetMana(86, 110);

            Fame = 2500;
            Karma = -2500;

            BaseSoundID = 432;
            SetSkill(SkillName.Swords, 60.1, 85);
            SetSkill(SkillName.Parry, 60.1, 85);
            SetSkill(SkillName.Tactics, 75.1, 90);
            SetSkill(SkillName.MagicResist, 70.1, 85);
            SetSkill(SkillName.Magery, 70.2, 95);

            VirtualArmor = 15;
            SetDamage(8, 24);
			BardLevel = 82;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich); 

            if (Utility.RandomDouble() > 0.95)
            {
                AddItem(new OrcishKinMask());
            }

            PackItem(new OrcHelm());
            PackItem(new ThighBoots());
            PackItem(new RingmailChest());

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }
        }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
				return false;

			return base.IsEnemy( m );
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is OrcishKinMask )
			{
				AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x307 );
			}
		}

		public OrcishLord( Serial serial ) : base( serial )
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