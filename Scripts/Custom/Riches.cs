using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;
using Server.Multis;
using Server.Items;

namespace Server.Misc
{
	public class Riches
	{

		public static void Initialize()
		{
			CommandSystem.Register( "riches", AccessLevel.Developer, new CommandEventHandler( Riches_OnCommand ) );
		}
		
		[Usage( "riches" )]
		public static void Riches_OnCommand( CommandEventArgs args )
		{
			try {
				Dictionary<int, long> table = new Dictionary<int, long>();

				long total = 0;

				foreach ( Item item in World.Items.Values ) {
					int worth;

					Gold gold = item as Gold;

					if ( gold != null ) {
						worth = gold.Amount;
					} else {
						BankCheck check = item as BankCheck;

						if ( check != null ) {
							worth = check.Worth;
						} else {
							continue;
						}
					}

					object obj = item.RootParent;

					if ( obj == null ) {
						obj = item;
					}

					BaseHouse house = null;

					if ( obj is Item ) {
						house = BaseHouse.FindHouseAt( obj as Item );
					} else if ( obj is Mobile ) {
						if ( (obj as Mobile).AccessLevel > AccessLevel.Player ) {
							continue;
						}
						//house = BaseHouse.FindHouseAt( obj as Mobile );
					}

					if ( house != null ) {
						obj = house;
					}

					if ( obj != null ) {
						int serial = ((IEntity)obj).Serial;

						long value;

						table.TryGetValue(serial, out value);
						table[serial] = value + worth;
					}
				}

				List<KeyValuePair<int, long>> list = new List<KeyValuePair<int, long>>( table );

				list.Sort( delegate( KeyValuePair<int, long> a, KeyValuePair<int, long> b ) {
					return -a.Value.CompareTo( b.Value );
				} );

				using ( StreamWriter op = new StreamWriter( "net-worth.log" ) ) {
					foreach ( KeyValuePair<int, long> kvp in list ) {
						Serial serial = kvp.Key;

						object obj;

						if ( serial.IsItem ) {
							obj = World.FindItem( serial );
						} else {
							obj = World.FindMobile( serial );
						}

						if ( serial.IsMobile ) {
							Mobile mob = (Mobile)obj;

							if ( mob.Account != null ) {
								Account acct = (Account)mob.Account;

								if ( acct.LoginIPs.Length > 0 ) {
									op.WriteLine( "{0}\t\t{1}\t\t{2}\t\t{3:N0}", obj, acct.Username, acct.LoginIPs[acct.LoginIPs.Length - 1], kvp.Value );
									continue;
								}
							}
						}

						op.WriteLine( "{0}\t{1:N0}", obj, kvp.Value );
					}
				}
			} catch {
			}
		}
	}
}