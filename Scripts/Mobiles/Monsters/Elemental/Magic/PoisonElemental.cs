using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a poison elementals corpse" )]
	public class PoisonElemental : BaseCreature
	{
		[Constructable]
		public PoisonElemental () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.3, 0.5 )
		{
            Body = 13;
            Hue = Utility.RandomMinMax(61, 79);

			Name = "a poison elemental";
			SetStr( 326, 415 );
			SetHits( 426, 515 );
			SetDex( 166, 185 );
			SetStam( 166, 185 );
			SetInt( 91, 165 );
			SetMana( 271, 500 );

			BaseSoundID = 263;
			SetSkill( SkillName.Wrestling, 70.1, 90 );
			SetSkill( SkillName.Parry, 75.1, 85 );
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 100, 105 );
			SetSkill( SkillName.Magery, 100, 120 );

			VirtualArmor = 35;
			SetDamage( 5, 25 );

            Fame = 12500;
			Karma = -12500;
			BardLevel = 65;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );

			PackGold( 100, 250 );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
        public override bool AutoDispel { get { return true; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{ get{ return 0.75; } }

		public override int TreasureMapLevel{ get{ return 5; } }

		public PoisonElemental( Serial serial ) : base( serial )
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