using System;
using Server;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class WaterBreathingPotion : Item
	{

		[Constructable]
		public WaterBreathingPotion() : base()
		{
            Name = "a Water Breathing potion";
            Weight = 0.1;
            ItemID = 3850;
			Hue = 195;
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (!Movable)
                return;

            if (from.InRange(this.GetWorldLocation(), 1))
            {
                XmlAttach.AttachTo(from, new TemporaryQuestObject("CanBreathUnderwater", 30));
                from.SendAsciiMessage("You can breath underwater for 30 minutes now!");

                Consume();
            }
            else
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use
            }
        }

        public WaterBreathingPotion(Serial serial)
            : base(serial)
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