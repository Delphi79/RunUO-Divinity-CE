using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcCaptain : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
        public OrcCaptain()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 7;
            Name = NameList.RandomName("orc");
            SetStr(111, 145);
            SetHits(111, 145);
            SetDex(101, 135);
            SetStam(101, 135);
            SetInt(86, 110);
            SetMana(86, 110);

            Fame = 2500;
            Karma = -2500;

            BaseSoundID = 432;
            SetSkill(SkillName.Swords, 70.1, 95);
            SetSkill(SkillName.Parry, 70.1, 95);
            SetSkill(SkillName.Tactics, 85.1, 100);
            SetSkill(SkillName.MagicResist, 70.1, 85);
            SetSkill(SkillName.Magery, 60.1, 85);

            VirtualArmor = 17;
            SetDamage(6, 18);
			BardLevel = 80;
        }

        public override void GenerateLoot()
        {
            Item item = null;             
            
            if (Utility.RandomDouble() > 0.95)
            {
                AddItem(new OrcishKinMask());
            }

            AddLoot(LootPack.Average);

            item = new ThighBoots();
            PackItem(item);
            item = new OrcHelm();
            PackItem(item);

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }
        }

		public override bool CanRummageCorpses{ get{ return true; } }
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

		public OrcCaptain( Serial serial ) : base( serial )
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