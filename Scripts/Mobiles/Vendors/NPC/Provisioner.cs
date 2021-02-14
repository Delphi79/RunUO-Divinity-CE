using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Provisioner : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBProvisioner() );

			if ( IsTokunoVendor )
				m_SBInfos.Add( new SBSEHats() );
		}

		public Provisioner( Serial serial ) : base( serial )
		{
		}

        [Constructable]
        public Provisioner()
            : base("the provisioner")
        {
            Job = JobFragment.laborer;

            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Parry, 45, 67.5);
            SetSkill(SkillName.Tactics, 45, 67.5);
            SetSkill(SkillName.MagicResist, 45, 67.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);
        }

        public override void InitOutfit()
        {
            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new Shirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = new Shoes();
            AddItem(item);
            item.Hue = Utility.RandomNeutralHue();

            PackGold(15, 100);

            if (!Female)
            {
                Utility.AssignRandomFacialHair(this, hairHue);

                item = new ShortPants();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
            else
            {
                item = new Skirt();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
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