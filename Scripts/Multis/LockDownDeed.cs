using Server;
using System;
using System.Collections;
using Server.Multis;
using Server.Targeting;
using Server.Items;

namespace Server.Multis.Deeds
{
	public class LockDownDeed : Item
	{
		[Constructable]
		public LockDownDeed() : base( 0x14F0 )
		{
			Name = "+25 lockdown addon deed";
			Weight = 1.0;
			LootType = LootType.Newbied;
		}

		public LockDownDeed( Serial serial ) : base( serial )
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

			switch ( version )
			{
				case 0:
				{
					break;
				}
			}

			if ( Weight == 0.0 )
				Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( from );

			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( house == null || !house.IsOwner( from ) )
				from.SendMessage( "You must use this item in a house that you own." );
			else if ( house.MaxLockDowns >= 1500 )
				from.SendMessage( "You have way too much time to have this many lockdowns. Get a new house." );
			else
			{
				from.SendMessage( "You have added 25 extra lockdowns to your house. If you redeed there will be NO REFUND." );
				house.MaxLockDowns += 25;
				this.Delete();
			}

			
		}
	}
}