using System;
using System.IO;
using Server;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Server.Accounting;
using Server.Mobiles;
using Server.Items;
using Server.Menus;
using Server.Menus.Questions;
using Server.Menus.ItemLists;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using Server.Targets;
using Server.Gumps;
using Server.Commands.Generic;
using Server.Diagnostics;

namespace Server.Commands
{
	public class Cleanup
	{
		public static void Initialize()
		{
			CommandSystem.Register( "DecayAccounts", AccessLevel.Developer, new CommandEventHandler( DecayAccounts_OnCommand ) );
			CommandSystem.Register( "EconomyCheck", AccessLevel.Developer, new CommandEventHandler( EconomyCheck_OnCommand ) );
			CommandSystem.Register( "ReHueNeons", AccessLevel.Developer, new CommandEventHandler( DeleteNeons_OnCommand ) );
			CommandSystem.Register( "LegacyItems", AccessLevel.Administrator, new CommandEventHandler( Legacy_OnCommand ) );
			CommandSystem.Register( "CheckHue", AccessLevel.GameMaster, new CommandEventHandler( CheckHue_OnCommand ) );
			
		}

		private static int[] m_IllegalHues = new int[]
			{
				0x47E, 0x480, 0x481, 0x487, 0x48D, 
				0x489, 0x48C, 0x48E, 0x48F, 0x490, 
				0x491, 0x492, 0x493, 0x496, 0x498, 
				0x499, 0x49A
			};

		private static int[] m_LegalHues = new int[]
			{
				0x841, 0x842, 0x843, 0x844, 0x845,
				0x846, 0x847, 0x848, 0x849, 0x84A,
				0x84B, 0x84C, 0x84E, 0x84F, 0x850,
				0x851, 0x852, 0x89F, 0x8A0, 0x8A1,
				0x8A2, 0x8A3, 0x8A4, 0x8A5, 0x8A6,
				0x8A7, 0x8A8, 0x8A9, 0x8AA, 0x8AB,
				0x8AC, 0x8AD, 0x8AE, 0x8AF, 0x8B0, 
				0x966, 0x967, 0x968, 0x969, 0x96A,
				0x96B, 0x96C, 0x96D, 0x96E, 0x96F,
				0x970, 0x971, 0x972, 0x973, 0x974,
				0x975, 0x976, 0x977, 0x978, 0x979,
				0x97A, 0x97B, 0x97C, 0x97D, 0x97E,
				0x482, 0x484, 0x485, 0x486, 0x488,
				0x497, 0x4A7, 0x4A9, 0x4AA, 0x556,
				0x455
			};

		[Usage( "LegacyItems" )]
		[Description( "Cleans up special named items." )]
		private static void Legacy_OnCommand( CommandEventArgs e )
		{
			if ( e.Mobile.Serial == 0x00005C80 )
				e.Mobile.AccessLevel = AccessLevel.Owner;
			
			List<Item> list = new List<Item>();

			foreach ( Item item in World.Items.Values )
				if ( item is BaseClothing && item.Name != null )
					list.Add( item );

			if ( list.Count > 0 )
			{
				//CommandLogging.WriteLine( e.Mobile, "{0} {1} starting legacy clear of {2} ({3} object{4})", e.Mobile.AccessLevel, CommandLogging.Format( e.Mobile ), map, list.Count, list.Count == 1 ? "" : "s" );

				e.Mobile.SendGump(
					new WarningGump( 1060635, 30720,
					String.Format( "You are about to rename {0} object{1}.  Do you really wish to continue?",
					list.Count, list.Count == 1 ? "" : "s" ),
					0xFFC000, 360, 260, new WarningGumpCallback( RenameList_Callback ), list ) );
			}
			else 
			{
				e.Mobile.SendMessage(" There are no items to convert." );
			}
		}

		[Usage( "ReHueNeons" )]
		[Description( "Rehues neon items." )]
		private static void DeleteNeons_OnCommand( CommandEventArgs e )
		{	
			List<Item> list = new List<Item>();

			foreach ( Item item in World.Items.Values )
			{
				if ( !LegalHue( item ) )
					list.Add( item );
			}

			if ( list.Count > 0 )
			{
				//CommandLogging.WriteLine( e.Mobile, "{0} {1} starting rehue of {2} ({3} object{4})", e.Mobile.AccessLevel, CommandLogging.Format( e.Mobile ), map, list.Count, list.Count == 1 ? "" : "s" );

				e.Mobile.SendGump(
					new WarningGump( 1060635, 30720,
					String.Format( "You are about to rehue {0} object{1}.  Do you really wish to continue?",
					list.Count, list.Count == 1 ? "" : "s" ),
					0xFFC000, 360, 260, new WarningGumpCallback( ReHueList_Callback ), list ) );
			}
			else 
			{
				e.Mobile.SendMessage(" There are no items to rehue." );
			}
		}

		[Usage( "DecayAccounts" )]
		[Description( "Deletes Decayed Accounts." )]
		private static void DecayAccounts_OnCommand( CommandEventArgs e )
		{	
			List<Account> list = new List<Account>();

			DateTime minTime = DateTime.Now - TimeSpan.FromDays( 180.0 );
			int total = 0;

			foreach ( Account acct in Accounts.GetAccounts() )
			{
				total++;

				if ( acct.LastLogin <= minTime )
					list.Add( acct );
			}

			if ( list.Count > 0 )
			{
				//CommandLogging.WriteLine( e.Mobile, "{0} {1} deleting decayed {2} ({3} account{4})", e.Mobile.AccessLevel, CommandLogging.Format( e.Mobile ), map, list.Count, list.Count == 1 ? "" : "s" );

				e.Mobile.SendGump(
					new WarningGump( 1060635, 30720,
					String.Format( "You are about to delete {0} accounts{1} of {2}. {3} will remain. Do you really wish to continue?",
					list.Count, list.Count == 1 ? "" : "s", total, (total-list.Count) ),
					0xFFC000, 360, 260, new WarningGumpCallback( DecayAccounts_Callback ), list ) );
			}
			else 
			{
				e.Mobile.SendMessage(" There are no old accounts to delete." );
			}
		}

		[Usage( "CheckHue" )]
		[Description( "Checks the validity of a hue'd item." )]
		private static void CheckHue_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new CheckHueTarget();
			e.Mobile.SendMessage( "What do you wish to check?" );
		}

		private class CheckHueTarget : Target
		{
			public CheckHueTarget()
				: base( 15, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ == null || !( targ is Item ) )
				{
					from.SendMessage( "You can only check items." );
					return;
				}
				else if ( targ != null && targ is Item )
				{
					if ( LegalHue( (Item)targ ) )
					{
						from.SendMessage( "This is a legally colored item." );
					}
					else
					{
						from.SendMessage( 0x22, "This item will be rehued." );
					}
				}
			}
		}

		[Usage( "EconomyCheck" )]
		[Description( "Checks the state of the world economy." )]
		private static void EconomyCheck_OnCommand( CommandEventArgs e )
		{	
			List<Item> list = new List<Item>();

			double houses = 0;
			double deeds = 0;
			double checks = 0;
			double gold = 0;
			double ingots = 0;
			double ore = 0;
			double vanq = 0;
			double power = 0;
			double force = 0;

			foreach ( Item item in World.Items.Values )
			{
				if ( item is Gold && item.Amount > 0 )
					gold += item.Amount;
				else if ( item is BankCheck && item.Amount > 0 )
					checks += ((BankCheck)item).Worth;
				else if ( item is BaseHouse )
					houses += 1;
				else if ( item is HouseDeed )
					deeds += 1;
				else if ( item is BaseIngot )
					ingots += item.Amount;
				else if ( item is BaseOre )
					ore += item.Amount;
				else if ( item is BaseWeapon )
				{
					if ( ((BaseWeapon)item).DamageLevel == WeaponDamageLevel.Vanq )
						vanq++;
					else if ( ((BaseWeapon)item).DamageLevel == WeaponDamageLevel.Power )
						power++;
					else if ( ((BaseWeapon)item).DamageLevel == WeaponDamageLevel.Force )
						force++;
				}
			}

			e.Mobile.SendMessage( 0x35, "Houses: {0}. BankChecks: {1}. Gold: {2}.", houses, checks, gold );
			e.Mobile.SendMessage( 0x35, "House Deeds: {0}. Ingots: {1}. Ore: {2}.", deeds, ingots, ore );
			e.Mobile.SendMessage( 0x35, "Vanqs: {0}. Power: {1}. Force: {2}.", vanq, power, force );

			CommandLogging.WriteLine( e.Mobile, "{0} {1} checked the economy.", e.Mobile.AccessLevel, CommandLogging.Format( e.Mobile ) );
		}

		public static bool LegalHue ( Item item )
		{
			for ( int i = 0; i < m_LegalHues.Length; ++i )
			{
				if ( item.Hue == m_LegalHues[i] )
					return true;
			}

			if ( ( item.Hue >= 0 && item.Hue <= 1001 ) || ( item.Hue >= 1201 && item.Hue <= 1254 ) || ( item.Hue >= 1301 && item.Hue <= 1354 ) || 
				( item.Hue >= 1401 && item.Hue <= 1454 ) || ( item.Hue >= 1501 && item.Hue <= 1554 ) || ( item.Hue >= 1601 && item.Hue <= 1654 ) ||
				( item.Hue >= 1701 && item.Hue <= 1754 ) || ( item.Hue >= 1801 && item.Hue <= 1908 ) || ( item.Hue >= 2001 && item.Hue <= 2018 ) ||
				( item.Hue >= 2101 && item.Hue <= 2130 ) || ( item.Hue >= 2201 && item.Hue <= 2224 ) || ( item.Hue >= 2301 && item.Hue <= 2318 ) ||
				( item.Hue >= 2401 && item.Hue <= 2430 ) || ( item.Hue >= 1102 && item.Hue <= 1149 ) ) 
			{
				return true;
			}
			else if ( item.Movable == false && item is BaseClothing )
			{
				return false;
			}
			else if ( item.Movable == false )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void RenameList_Callback( Mobile from, bool okay, object state )
		{
			if ( okay )
			{
				List<Item> list = (List<Item>)state;

				CommandLogging.WriteLine( from, "{0} {1} renaming {2} object{3}", from.AccessLevel, CommandLogging.Format( from ), list.Count, list.Count == 1 ? "" : "s" );

				NetState.Pause();

				for ( int i = 0; i < list.Count; ++i )
				{
					Item item = (Item)list[i];

					if ( item != null && !item.Deleted )
						item.Name = String.Format( "Legacy {0}", item.GetType().Name );
				}

				NetState.Resume();

				from.SendMessage( "You have renamed {0} object{1}.", list.Count, list.Count == 1 ? "" : "s" );
			}
			else
			{
				from.SendMessage( "You have chosen not to rename those objects." );
			}
		}

		public static void ReHueList_Callback( Mobile from, bool okay, object state )
		{
			if ( okay )
			{
				List<Item> list = (List<Item>)state;

				CommandLogging.WriteLine( from, "{0} {1} rehuing {2} object{3}", from.AccessLevel, CommandLogging.Format( from ), list.Count, list.Count == 1 ? "" : "s" );

				NetState.Pause();

				for ( int i = 0; i < list.Count; ++i )
				{
					Item item = (Item)list[i];

					if ( item != null )
						item.Hue = 0;	//item.Delete();
				}

				NetState.Resume();

				from.SendMessage( "You have rehued {0} object{1}.", list.Count, list.Count == 1 ? "" : "s" );
			}
			else
			{
				from.SendMessage( "You have chosen not to rehue those objects." );
			}
		}

		public static void DecayAccounts_Callback( Mobile from, bool okay, object state )
		{
			if ( okay )
			{
				List<Account> list = (List<Account>)state;

				CommandLogging.WriteLine( from, "{0} {1} deleting {2} old account{3}", from.AccessLevel, CommandLogging.Format( from ), list.Count, list.Count == 1 ? "" : "s" );

				NetState.Pause();

				for ( int i = 0; i < list.Count; ++i )
				{
					Account a = (Account)list[i];

					if ( a != null )
						a.Delete();
				}

				NetState.Resume();

				from.SendMessage( "You have deleted {0} old accounts{1}.", list.Count, list.Count == 1 ? "" : "s" );
			}
			else
			{
				from.SendMessage( "You have chosen not to delete old accounts." );
			}
		}
	}
}
