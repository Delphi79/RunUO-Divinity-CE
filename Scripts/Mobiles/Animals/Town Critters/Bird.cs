using System;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a bird corpse" )]
	public class Bird : BaseCreature
	{
		[Constructable]
		public Bird() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
            Body = 6;
            
			if ( Utility.RandomBool() )
			{
				Hue = 0x901;

				switch ( Utility.Random( 2 ) )
				{
                    case 0: Name = "a crow"; Karma = -100; break;
					case 1: Name = "a raven"; break;
				}

                SetStr(11, 17);
                SetHits(5, 15);
                SetDex(26, 35);
                SetStam(50, 100);
                SetInt(6, 10);
                SetMana(0);
                Tamable = true;
                MinTameSkill = 15;
                BaseSoundID = 125;
                SetSkill(SkillName.Wrestling, 9.2, 17);
                SetSkill(SkillName.Parry, 25.1, 35);
                SetSkill(SkillName.Tactics, 9.2, 17);
                SetSkill(SkillName.MagicResist, 5.1, 10);

                VirtualArmor = 1;
                SetDamage(1);
			}
			else
			{
				Hue = Utility.RandomBirdHue();
				Name = NameList.RandomName( "bird" );

                SetStr(1, 4);
                SetHits(3, 6);
                SetDex(26, 35);
                SetStam(50, 100);
                SetInt(1, 4);
                SetMana(0);
                Tamable = true;
                MinTameSkill = 10;
                BaseSoundID = 148;
                SetSkill(SkillName.Wrestling, 5.1, 10);
                SetSkill(SkillName.Parry, 15.1, 25);
                SetSkill(SkillName.Tactics, 5.1, 10);
                SetSkill(SkillName.MagicResist, 5.1, 10);

                VirtualArmor = 1;
                SetDamage(1);
			}
		}

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 25; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Bird( Serial serial ) : base( serial )
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

			if ( Hue == 0 )
				Hue = Utility.RandomBirdHue();
		} 
	}

	[CorpseName( "a bird corpse" )]
	public class TropicalBird : BaseCreature
	{
		[Constructable]
        public TropicalBird()
            : base(AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 6;
            Name = "a tropical bird";
            Hue = Utility.RandomBirdHue();
            SetStr(1, 4);
            SetHits(3, 6);
            SetDex(26, 35);
            SetStam(50, 100);
            SetInt(1, 4);
            SetMana(0);
            Tamable = true;
            MinTameSkill = 10;
            BaseSoundID = 191;
            SetSkill(SkillName.Wrestling, 5.1, 10);
            SetSkill(SkillName.Parry, 15.1, 25);
            SetSkill(SkillName.Tactics, 5.1, 10);
            SetSkill(SkillName.MagicResist, 5.1, 10);

            VirtualArmor = 1;
            SetDamage(1);
        }

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 25; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public TropicalBird( Serial serial ) : base( serial )
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