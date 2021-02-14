using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fire elemental corpse" )]
	public class FireElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public FireElemental () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 15;
			Name = "a fire elemental";
			SetStr( 126, 155 );
			SetHits( 126, 155 );
			SetDex( 166, 185 );
			SetStam( 166, 185 );
			SetInt( 71, 95 );
			SetMana( 171, 195 );

            Fame = 4500;
			Karma = -4500;

			BaseSoundID = 273;
			SetSkill( SkillName.Wrestling, 70.1, 100 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Magery, 75, 85 );

			VirtualArmor = 25;
			SetDamage( 10, 15 );

			AddItem( new LightSource() );
			BardLevel = 55;
		}

		public override void GenerateLoot()
		{
            SulfurousAsh ash = new SulfurousAsh();
            ash.Amount = 3;

            PackItem(ash);
            PackGold(300, 350);
            PackGem(1);
            PackScroll(1, 6);
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public FireElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 274 )
				BaseSoundID = 838;
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (!c.Deleted)
            {
                while (c.Items.Count > 0)
                    c.Items[0].MoveToWorld(c.Location, c.Map);
                c.Delete();
            }
        }
	}
}
