using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a cow corpse" )]
	public class Cow : BaseCreature
	{
		[Constructable]
        public Cow()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = Utility.RandomList(216, 231);
            Name = "a cow";
            SetStr(42, 76);
            SetHits(45, 85);
            SetDex(26, 45);
            SetStam(32, 56);
            SetInt(2, 10);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 30;
            BaseSoundID = 120;
            SetSkill(SkillName.Wrestling, 27.6, 45);
            SetSkill(SkillName.Parry, 22.6, 35);
            SetSkill(SkillName.Tactics, 27.6, 45);
            SetSkill(SkillName.MagicResist, 17.6, 25);

            VirtualArmor = 9;
            SetDamage(6, 12);

			BardLevel = 10;
        }

		public override int Meat{ get{ return 8; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick( from );

			int random = Utility.Random( 100 );

			if ( random < 5 )
				Tip();
			else if ( random < 20 )
				PlaySound( 120 );
			else if ( random < 40 )
				PlaySound( 121 );
		}

		public void Tip()
		{
			PlaySound( 121 );
			Animate( 8, 0, 3, true, false, 0 );
		}

		public Cow(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}