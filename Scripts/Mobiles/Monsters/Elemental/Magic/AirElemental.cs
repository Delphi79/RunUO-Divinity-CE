using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an air elemental corpse" )]
	public class AirElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public AirElemental () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 13;
			Name = "an air elemental";
			SetStr( 126, 155 );
			SetHits( 190, 200 );
			SetDex( 166, 185 );
			SetStam( 166, 185 );
			SetInt( 71, 95 );
			SetMana( 151, 195 );

			BaseSoundID = 263;
			SetSkill( SkillName.Wrestling, 60.1, 80 );
			SetSkill( SkillName.Parry, 65.1, 75 );
			SetSkill( SkillName.Tactics, 60.1, 80 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Magery, 70, 85 );

			VirtualArmor = 25;
            SetDamage(10, 15);

            Fame = 4500;
            Karma = -4500;
			BardLevel = 65;
		}

		public override void GenerateLoot()
		{
            PackGold(300, 350);
            PackGem();
            PackScroll(4, 6);
            PackScroll(4, 6);
		}

        public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public AirElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 655;
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if ( !c.Deleted )
            {
                while ( c.Items.Count > 0 )
                    c.Items[0].MoveToWorld( c.Location, c.Map );
                c.Delete();
            }
        }
	}
}
