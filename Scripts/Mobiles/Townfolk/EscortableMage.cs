using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class EscortableMage : BaseEscortable
	{
		[Constructable]
		public EscortableMage()
		{
			Title = "the mage";

            SetStr(61, 75);
            SetDex(71, 85);
            SetInt(86, 100);
            Job = JobFragment.mage;

            SetSkill(SkillName.Wrestling, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Parry, 55, 77.5);
            SetSkill(SkillName.Tactics, 55, 77.5);
            SetSkill(SkillName.MagicResist, 65, 87.5);
            SetSkill(SkillName.Magery, 85.1, 100);
            SetSkill(SkillName.Inscribe, 50.1, 65);
		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } } // Do not display 'the mage' when single-clicking


		public override void InitOutfit()
		{
            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            if ( !Female )
                Utility.AssignRandomFacialHair(this, hairHue);

            item = new Robe();
            AddItem(item);
            item.Hue = Utility.RandomBlueHue();

            PackGold(15, 100);
		}

		public EscortableMage( Serial serial ) : base( serial )
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