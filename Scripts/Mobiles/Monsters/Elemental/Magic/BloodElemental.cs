using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a blood elemental corpse" )]
	public class BloodElemental : BaseCreature
	{
		[Constructable]
		public BloodElemental () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.3, 0.5 )
		{
            Body = 16;
            Hue = Utility.RandomMinMax(33, 38);
			Name = "a blood elemental";
			SetStr( 326, 415 );
			SetHits( 326, 415 );
			SetDex( 66, 85 );
			SetStam( 66, 85 );
			SetInt( 91, 215 );
			SetMana( 91, 215 );

			Fame = 12500;
			Karma = -12500;

			SetSkill( SkillName.Wrestling, 80.1, 100 );
			SetSkill( SkillName.Parry, 85.1, 95 );
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 80.1, 95 );
			SetSkill( SkillName.Magery, 95, 100 );

			VirtualArmor = 30;
			SetDamage( 4, 28 );
			BardLevel = 55;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );

			PackGold( 100, 250 );
		}

        public override bool AutoDispel { get { return true; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public BloodElemental( Serial serial ) : base( serial )
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