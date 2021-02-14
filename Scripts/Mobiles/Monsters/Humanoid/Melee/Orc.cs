using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class Orc : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
        public Orc()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 17;
            Name = NameList.RandomName("orc");
            SetStr(96, 101);
            SetHits(120, 150);
            SetDex(81, 105);
            SetStam(91, 115);
            SetInt(36, 60);
            SetMana(71, 95);

            Fame = 1500;
            Karma = -1500;

            BaseSoundID = 432;
            SetSkill(SkillName.Wrestling, 50.1, 70);
            SetSkill(SkillName.Parry, 50.1, 75);
            SetSkill(SkillName.Tactics, 55.1, 80);
            SetSkill(SkillName.MagicResist, 50.1, 75);
            SetSkill(SkillName.Magery, 50.1, 75);

            VirtualArmor = 14;
            SetDamage(3, 9);
			BardLevel = 70;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            PackGold(35);
            if (Utility.RandomDouble() > 0.95)
            {
                AddItem(new OrcishKinMask());
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

		public Orc( Serial serial ) : base( serial )
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
