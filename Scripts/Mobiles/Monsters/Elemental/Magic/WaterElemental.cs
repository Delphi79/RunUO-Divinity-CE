using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a water elemental corpse" )]
	public class WaterElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public WaterElemental () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 16;
			Name = "a water elemental";
            CanSwim = true;

			SetStr( 126, 155 );
			SetHits( 126, 155 );
			SetDex( 66, 85 );
			SetStam( 66, 85 );
			SetInt( 71, 95 );
			SetMana( 71, 150 );

			SetSkill( SkillName.Wrestling, 70.1, 90 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Magery, 70, 85 );

			VirtualArmor = 25;
			SetDamage( 10, 15 );

            Fame = 4500;
            Karma = -4500;

			BardLevel = 45;
		}

		public override void GenerateLoot()
		{
            PackGold(300, 350);

            BlackPearl pearl = new BlackPearl();
            pearl.Amount = 3;

            PackItem(pearl);
            PackGem(1);
            PackScroll(1, 6);
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public WaterElemental( Serial serial ) : base( serial )
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