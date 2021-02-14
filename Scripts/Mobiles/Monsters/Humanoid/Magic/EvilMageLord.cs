using System; 
using Server;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an evil mage lord corpse" )] 
	public class EvilMageLord : BaseCreature 
	{ 
		[Constructable] 
		public EvilMageLord() : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 0x0190;
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			SetStr( 100, 200 );
			SetDex( 91, 115 );
			SetInt( 150, 300 );
            SetHits(240, 260);
		
            Name = NameList.RandomName( "evil mage lord" );
			Fame = 10500;
			Karma = -10500;

			SetSkill( SkillName.Wrestling, 20.2, 60 );
			SetSkill( SkillName.Parry, 65, 87.5 );
			SetSkill( SkillName.Tactics, 65, 87.5 );
			SetSkill( SkillName.MagicResist, 95, 100 );
			SetSkill( SkillName.Magery, 100, 120 );

			VirtualArmor = 18;
			SetDamage( 3, 12 );

            Item item = null;

			int hairHue = Utility.RandomHairHue();
			Utility.AssignRandomHair( this, hairHue );

			Utility.AssignRandomFacialHair( this, hairHue );

			item = new Robe();
            item.Hue = Utility.RandomBlueHue();
			AddItem( item );

			item = new Sandals();
            item.Hue = Utility.RandomBlueHue();
			AddItem( item );

			//BardImmune = true;
			BardLevel = 90;
		}

		public override void GenerateLoot()
		{
            AddLoot(LootPack.Rich);
            PackGold(100, 150);
			PackScroll( 4, 8 );
			PackScroll( 4, 8 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int TreasureMapLevel{ get{ return Core.AOS ? 2 : 0; } }

		public EvilMageLord( Serial serial ) : base( serial ) 
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