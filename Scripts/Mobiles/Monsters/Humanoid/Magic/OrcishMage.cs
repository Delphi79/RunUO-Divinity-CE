using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a glowing corpse" )]
	public class OrcishMage : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
        public OrcishMage()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 17;
            Name = "An Orcish mage";
            Hue = Utility.RandomGreenHue();
            SetStr(96, 130);
            SetHits(111, 145);
            SetDex(91, 115);
            SetStam(101, 135);
            SetInt(61, 85);
            SetMana(186, 210);

            Fame = 3000;
            Karma = -3000;

            BaseSoundID = 432;
            SetSkill(SkillName.Swords, 60.1, 85);
            SetSkill(SkillName.Parry, 60.1, 85);
            SetSkill(SkillName.Tactics, 75.1, 90);
            SetSkill(SkillName.MagicResist, 70.1, 85);
            SetSkill(SkillName.Magery, 95, 100);

            VirtualArmor = 15;
            SetDamage(2, 16);
			BardLevel = 55;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() > 0.95)
            {
                AddItem(new OrcishKinMask());
            }

            PackScroll(4, 6);
            
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

		public OrcishMage( Serial serial ) : base( serial )
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
