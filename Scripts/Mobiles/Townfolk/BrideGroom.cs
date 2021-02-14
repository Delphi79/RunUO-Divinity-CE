using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class BrideGroom : BaseEscortable
	{
		[Constructable]
		public BrideGroom()
		{
			if ( Female )
			    Title = "the bride";
			else
			    Title = "the groom";
           
            SetStr(36, 50);
            SetDex(31, 45);
            SetInt(36, 50);
            Job = JobFragment.laborer;
		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } } // Do not display 'the groom' when single-clicking

		public override void InitOutfit()
		{
            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new Shirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = new Skirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            AddLoot(LootPack.Poor);
		}

		public BrideGroom( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}