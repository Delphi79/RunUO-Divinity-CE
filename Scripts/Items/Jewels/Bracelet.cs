using System;

namespace Server.Items
{
    public abstract class BaseBracelet : BaseClothing
	{
		//public override int BaseGemTypeNumber{ get{ return 1044221; } } // star sapphire bracelet

		public BaseBracelet( int itemID ) : base( itemID, Layer.Bracelet )
		{
		}

		public BaseBracelet( Serial serial ) : base( serial )
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

		public override bool Dye( Mobile from, DyeTub sender )
		{
			return false;
		}

		public override bool Scissor( Mobile from, Scissors scissors )
		{
			return false;
		}
	}

    public class WristWatch : Clock
    {
        [Constructable]
        public WristWatch()
            : base(4230)
        {
            Weight = 1.0;
            Layer = Layer.Bracelet;
            Name = "a wrist watch";
        }

        public WristWatch(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

	public class GoldBracelet : BaseBracelet
	{
		[Constructable]
		public GoldBracelet() : base( 0x1086 )
		{
			Weight = 0.1;
		}

		public GoldBracelet( Serial serial ) : base( serial )
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

	public class SilverBracelet : BaseBracelet
	{
		[Constructable]
		public SilverBracelet() : base( 0x1F06 )
		{
			Weight = 0.1;
		}

		public SilverBracelet( Serial serial ) : base( serial )
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
