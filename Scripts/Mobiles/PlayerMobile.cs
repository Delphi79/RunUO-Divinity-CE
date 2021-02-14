using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Engines.Help;
using Server.ContextMenus;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Bushido;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Factions;
using Server.Regions;
using Server.Accounting;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Spells.Spellweaving;

namespace Server.Mobiles
{
	#region Enums
	[Flags]
	public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
	{
		None				= 0x00000000,
		Glassblowing		= 0x00000001,
		Masonry				= 0x00000002,
		SandMining			= 0x00000004,
		StoneMining			= 0x00000008,
		ToggleMiningStone	= 0x00000010,
		KarmaLocked			= 0x00000020,
		AutoRenewInsurance	= 0x00000040,
		UseOwnFilter		= 0x00000080,
		PublicMyRunUO		= 0x00000100,
		PagingSquelched		= 0x00000200,
		Young				= 0x00000400,
		AcceptGuildInvites	= 0x00000800,
		DisplayChampionTitle= 0x00001000
	}

	public enum NpcGuild
	{
		None,
		MagesGuild,
		WarriorsGuild,
		ThievesGuild,
		RangersGuild,
		HealersGuild,
		MinersGuild,
		MerchantsGuild,
		TinkersGuild,
		TailorsGuild,
		FishermensGuild,
		BardsGuild,
		BlacksmithsGuild
	}

	public enum SolenFriendship
	{
		None,
		Red,
		Black
	}
	#endregion

	public class PlayerMobile : Mobile, IHonorTarget
	{
		private class CountAndTimeStamp
		{
			private int m_Count;
			private DateTime m_Stamp;

			public CountAndTimeStamp()
			{
			}

			public DateTime TimeStamp { get{ return m_Stamp; } }
			public int Count 
			{ 
				get { return m_Count; } 
				set	{ m_Count = value; m_Stamp = DateTime.Now; } 
			}
		}

		private DesignContext m_DesignContext;

		private NpcGuild m_NpcGuild;
		private DateTime m_NpcGuildJoinTime;
		private TimeSpan m_NpcGuildGameTime;
		private PlayerFlag m_Flags;
		private int m_StepsTaken;
		private int m_Profession;
		private bool m_EventBanned;

		private DateTime m_LastOnline;
		private Server.Guilds.RankDefinition m_GuildRank;

		private int m_GuildMessageHue, m_AllianceMessageHue;

		[CommandProperty( AccessLevel.Counselor, AccessLevel.Owner )]
		public new Account Account
		{
			get { return base.Account as Account; }
			set { base.Account = value; }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool EventBanned
		{
			get{ return m_EventBanned; }
			set{ m_EventBanned = value; }
		}

		#region Getters & Setters
		public Server.Guilds.RankDefinition GuildRank
		{
			get
			{
				if( this.AccessLevel >= AccessLevel.GameMaster )
					return Server.Guilds.RankDefinition.Leader;
				else
					return m_GuildRank; 
			}
			set{ m_GuildRank = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int GuildMessageHue
		{
			get{ return m_GuildMessageHue; }
			set{ m_GuildMessageHue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int AllianceMessageHue
		{
			get { return m_AllianceMessageHue; }
			set { m_AllianceMessageHue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Profession
		{
			get{ return m_Profession; }
			set{ m_Profession = value; }
		}

		public int StepsTaken
		{
			get{ return m_StepsTaken; }
			set{ m_StepsTaken = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public NpcGuild NpcGuild
		{
			get{ return m_NpcGuild; }
			set{ m_NpcGuild = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NpcGuildJoinTime
		{
			get{ return m_NpcGuildJoinTime; }
			set{ m_NpcGuildJoinTime = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastOnline
		{
			get{ return m_LastOnline; }
			set{ m_LastOnline = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan NpcGuildGameTime
		{
			get{ return m_NpcGuildGameTime; }
			set{ m_NpcGuildGameTime = value; }
		}

		private int m_ToTItemsTurnedIn;

		[CommandProperty( AccessLevel.GameMaster )]
		public int ToTItemsTurnedIn
		{
			get { return m_ToTItemsTurnedIn; }
			set { m_ToTItemsTurnedIn = value; }
		}

		private int m_ToTTotalMonsterFame;

		[CommandProperty( AccessLevel.GameMaster )]
		public int ToTTotalMonsterFame
		{
			get { return m_ToTTotalMonsterFame; }
			set { m_ToTTotalMonsterFame = value; }
		}

		#endregion

		#region PlayerFlags
		public PlayerFlag Flags
		{
			get{ return m_Flags; }
			set{ m_Flags = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PagingSquelched
		{
			get{ return GetFlag( PlayerFlag.PagingSquelched ); }
			set{ SetFlag( PlayerFlag.PagingSquelched, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Glassblowing
		{
			get{ return GetFlag( PlayerFlag.Glassblowing ); }
			set{ SetFlag( PlayerFlag.Glassblowing, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Masonry
		{
			get{ return GetFlag( PlayerFlag.Masonry ); }
			set{ SetFlag( PlayerFlag.Masonry, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool SandMining
		{
			get{ return GetFlag( PlayerFlag.SandMining ); }
			set{ SetFlag( PlayerFlag.SandMining, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool StoneMining
		{
			get{ return GetFlag( PlayerFlag.StoneMining ); }
			set{ SetFlag( PlayerFlag.StoneMining, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ToggleMiningStone
		{
			get{ return GetFlag( PlayerFlag.ToggleMiningStone ); }
			set{ SetFlag( PlayerFlag.ToggleMiningStone, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool KarmaLocked
		{
			get{ return GetFlag( PlayerFlag.KarmaLocked ); }
			set{ SetFlag( PlayerFlag.KarmaLocked, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AutoRenewInsurance
		{
			get{ return GetFlag( PlayerFlag.AutoRenewInsurance ); }
			set{ SetFlag( PlayerFlag.AutoRenewInsurance, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool UseOwnFilter
		{
			get{ return GetFlag( PlayerFlag.UseOwnFilter ); }
			set{ SetFlag( PlayerFlag.UseOwnFilter, value ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PublicMyRunUO
		{
			get{ return GetFlag( PlayerFlag.PublicMyRunUO ); }
			set{ SetFlag( PlayerFlag.PublicMyRunUO, value ); InvalidateMyRunUO(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AcceptGuildInvites
		{
			get{ return GetFlag( PlayerFlag.AcceptGuildInvites ); }
			set{ SetFlag( PlayerFlag.AcceptGuildInvites, value ); }
		}
		#endregion


		public static Direction GetDirection4( Point3D from, Point3D to )
		{
			int dx = from.X - to.X;
			int dy = from.Y - to.Y;

			int rx = dx - dy;
			int ry = dx + dy;

			Direction ret;

			if ( rx >= 0 && ry >= 0 )
				ret = Direction.West;
			else if ( rx >= 0 && ry < 0 )
				ret = Direction.South;
			else if ( rx < 0 && ry < 0 )
				ret = Direction.East;
			else
				ret = Direction.North;

			return ret;
		}

		public override bool OnDroppedItemToWorld( Item item, Point3D location )
		{
			if ( !base.OnDroppedItemToWorld( item, location ) )
				return false;

			BounceInfo bi = item.GetBounce();

			if ( bi != null )
			{
				Type type = item.GetType();

				if ( type.IsDefined( typeof( FurnitureAttribute ), true ) || type.IsDefined( typeof( DynamicFlipingAttribute ), true ) )
				{
					object[] objs = type.GetCustomAttributes( typeof( FlipableAttribute ), true );

					if ( objs != null && objs.Length > 0 )
					{
						FlipableAttribute fp = objs[0] as FlipableAttribute;

						if ( fp != null )
						{
							int[] itemIDs = fp.ItemIDs;

							Point3D oldWorldLoc = bi.m_WorldLoc;
							Point3D newWorldLoc = location;

							if ( oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y )
							{
								Direction dir = GetDirection4( oldWorldLoc, newWorldLoc );

								if ( itemIDs.Length == 2 )
								{
									switch ( dir )
									{
										case Direction.North:
										case Direction.South: item.ItemID = itemIDs[0]; break;
										case Direction.East:
										case Direction.West: item.ItemID = itemIDs[1]; break;
									}
								}
								else if ( itemIDs.Length == 4 )
								{
									switch ( dir )
									{
										case Direction.South: item.ItemID = itemIDs[0]; break;
										case Direction.East: item.ItemID = itemIDs[1]; break;
										case Direction.North: item.ItemID = itemIDs[2]; break;
										case Direction.West: item.ItemID = itemIDs[3]; break;
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		public bool GetFlag( PlayerFlag flag )
		{
			return ( (m_Flags & flag) != 0 );
		}

		public void SetFlag( PlayerFlag flag, bool value )
		{
			if ( value )
				m_Flags |= flag;
			else
				m_Flags &= ~flag;
		}

		public DesignContext DesignContext
		{
			get{ return m_DesignContext; }
			set{ m_DesignContext = value; }
		}

		public static void Initialize()
		{
            Mobile.AutoManifestTimeout = TimeSpan.FromSeconds( 30.0 );

			if ( FastwalkPrevention )
				PacketHandlers.RegisterThrottler( 0x02, new ThrottlePacketCallback( MovementThrottle_Callback ) );

			EventSink.Login += new LoginEventHandler( OnLogin );
			EventSink.Logout += new LogoutEventHandler( OnLogout );
			EventSink.Connected += new ConnectedEventHandler( EventSink_Connected );
			EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
		}

		public override void OnSkillInvalidated( Skill skill )
		{
			if ( Core.AOS && skill.SkillName == SkillName.MagicResist )
				UpdateResistances();
		}

		public override int GetMaxResistance( ResistanceType type )
		{
			int max = base.GetMaxResistance( type );

			if ( type != ResistanceType.Physical && 60 < max && Spells.Fourth.CurseSpell.UnderEffect( this ) )
				max = 60;

			if( Core.ML && this.Race == Race.Elf && type == ResistanceType.Energy )
				max += 5; //Intended to go after the 60 max from curse

			return max;
		}

		protected override void OnRaceChange( Race oldRace )
		{
			ValidateEquipment();
			UpdateResistances();
		}

		public override int MaxWeight { get { return (((Core.ML && this.Race == Race.Human) ? 100 : 40) + (int)(3.5 * this.Str)); } }

		private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

		public override void OnNetStateChanged()
		{
			m_LastGlobalLight = -1;
			m_LastPersonalLight = -1;
		}

		public override void ComputeBaseLightLevels( out int global, out int personal )
		{
			global = LightCycle.ComputeLevelFor( this );

			bool racialNightSight = (Core.ML && this.Race == Race.Elf);

			if ( this.LightLevel < 21 && ( AosAttributes.GetValue( this, AosAttribute.NightSight ) > 0 || racialNightSight ))
				personal = 21;
			else
				personal = this.LightLevel;
		}

		public override void CheckLightLevels( bool forceResend )
		{
			NetState ns = this.NetState;

			if ( ns == null )
				return;

			int global, personal;

			ComputeLightLevels( out global, out personal );

			if ( !forceResend )
				forceResend = ( global != m_LastGlobalLight || personal != m_LastPersonalLight );

			if ( !forceResend )
				return;

			m_LastGlobalLight = global;
			m_LastPersonalLight = personal;

			ns.Send( GlobalLightLevel.Instantiate( global ) );
			ns.Send( new PersonalLightLevel( this, personal ) );
		}

		public override int GetMinResistance( ResistanceType type )
		{
			int magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
			int min = int.MinValue;

			if ( magicResist >= 1000 )
				min = 40 + ((magicResist - 1000) / 50);
			else if ( magicResist >= 400 )
				min = (magicResist - 400) / 15;

			if ( min > MaxPlayerResistance )
				min = MaxPlayerResistance;

			int baseMin = base.GetMinResistance( type );

			if ( min < baseMin )
				min = baseMin;

			return min;
		}

		private static void OnLogin( LoginEventArgs e )
		{
			Mobile from = e.Mobile;

			CheckAtrophies( from );

            if (from.AccessLevel > AccessLevel.Player)
            {
                PageEntry entry = (PageEntry)PageQueue.KeyedByHandler[from];

                if (entry != null)
                    from.SendGump(new PageEntryGump(from, entry));
                else if (PageQueue.List.Count > 0)
                    from.SendGump(new PageQueueGump());
                else
                    from.SendAsciiMessage( "The page queue is empty." );
            }

            if ( from is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)from;

                pm.CheckKillDecay();

                if ( pm.m_StatLoss.Count > 0 && (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished || pm.DuelPlayer.Eliminated))
                    pm.m_StatLoss[0].Resume();
            }

			if ( AccountHandler.LockdownLevel > AccessLevel.Player )
			{
				string notice;

				Accounting.Account acct = from.Account as Accounting.Account;

				if ( acct == null || !acct.HasAccess( from.NetState ) )
				{
					if ( from.AccessLevel == AccessLevel.Player )
						notice = "The server is currently under lockdown. No players are allowed to log in at this time.";
					else
						notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";

					Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Disconnect ), from );
				}
				else if ( from.AccessLevel >= AccessLevel.Administrator )
				{
					notice = "The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";
				}
				else
				{
					notice = "The server is currently under lockdown. You have sufficient access level to connect.";
				}

				from.SendGump( new NoticeGump( 1060637, 30720, notice, 0xFFC000, 300, 140, null, null ) );
			}

            if (from.Guild is Guilds.Guild && from.Guild.Type != Server.Guilds.GuildType.Regular)
            {
                Guilds.Guild g = (Guilds.Guild)from.Guild;

                if ( !Guilds.Guild.JoinVirtue(from, g.Type) )
                {
                    if (g.Leader == from)
                    {
                        from.SendAsciiMessage(0x25, "You cannot be in a virtue guild of that type right now.  Your guild's type has been automatically changed.");
                        g.Type = Guilds.GuildType.Regular;
                        g.GuildMessage(1018022, true, "Regular"); // Guild Message: Your guild type has changed:
                    }
                    else
                    {
                        g.RemoveMember(from);
                        from.SendAsciiMessage(0x25, "You have been removed from your virtue guild because you cannot be in a guild of that type right now.");
                    }
                }
            }
		}

		private bool m_NoDeltaRecursion;

		public void ValidateEquipment()
		{
			if ( m_NoDeltaRecursion || Map == null || Map == Map.Internal )
				return;

			if ( this.Items == null )
				return;

			m_NoDeltaRecursion = true;
			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ValidateEquipment_Sandbox ) );
		}

		private void ValidateEquipment_Sandbox()
		{
			try
			{
				if ( Map == null || Map == Map.Internal )
					return;

				List<Item> items = this.Items;

				if ( items == null )
					return;

				bool moved = false;

				int str = this.Str;
				int dex = this.Dex;
				int intel = this.Int;

				#region Factions
				int factionItemCount = 0;
				#endregion

				Mobile from = this;

				#region Ethics
				Ethics.Ethic ethic = Ethics.Ethic.Find( from );
				#endregion

				for ( int i = items.Count - 1; i >= 0; --i )
				{
					if ( i >= items.Count )
						continue;

					Item item = items[i];

					#region Ethics
					if ( ( item.SavedFlags & 0x100 ) != 0 )
					{
						if ( item.Hue != Ethics.Ethic.Hero.Definition.PrimaryHue )
						{
							item.SavedFlags &= ~0x100;
						}
						else if ( ethic != Ethics.Ethic.Hero )
						{
							from.AddToBackpack( item );
							moved = true;
							continue;
						}
					}
					else if ( ( item.SavedFlags & 0x200 ) != 0 )
					{
						if ( item.Hue != Ethics.Ethic.Evil.Definition.PrimaryHue )
						{
							item.SavedFlags &= ~0x200;
						}
						else if ( ethic != Ethics.Ethic.Evil )
						{
							from.AddToBackpack( item );
							moved = true;
							continue;
						}
					}
					#endregion

					if ( item is BaseWeapon )
					{
						BaseWeapon weapon = (BaseWeapon)item;

						bool drop = false;

						if( dex < weapon.DexRequirement )
							drop = true;
						else if( str < AOS.Scale( weapon.StrRequirement, 100 - weapon.GetLowerStatReq() ) )
							drop = true;
						else if( intel < weapon.IntRequirement )
							drop = true;
						else if( weapon.RequiredRace != null && weapon.RequiredRace != this.Race )
							drop = true;

						if ( drop )
						{
							string name = weapon.Name;

							if ( name == null )
								name = String.Format( "#{0}", weapon.LabelNumber );

							from.SendLocalizedMessage( 1062001, name ); // You can no longer wield your ~1_WEAPON~
							from.AddToBackpack( weapon );
							moved = true;
						}
					}
					else if ( item is BaseArmor )
					{
						BaseArmor armor = (BaseArmor)item;

						bool drop = false;

						if ( !armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster )
						{
							drop = true;
						}
						else if ( !armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster )
						{
							drop = true;
						}
						else if( armor.RequiredRace != null && armor.RequiredRace != this.Race )
						{
							drop = true;
						}
						else
						{
							int strBonus = armor.ComputeStatBonus( StatType.Str ), strReq = armor.ComputeStatReq( StatType.Str );
							int dexBonus = armor.ComputeStatBonus( StatType.Dex ), dexReq = armor.ComputeStatReq( StatType.Dex );
							int intBonus = armor.ComputeStatBonus( StatType.Int ), intReq = armor.ComputeStatReq( StatType.Int );

							if( dex < dexReq || (dex + dexBonus) < 1 )
								drop = true;
							else if( str < strReq || (str + strBonus) < 1 )
								drop = true;
							else if( intel < intReq || (intel + intBonus) < 1 )
								drop = true;
						}

						if ( drop )
						{
							string name = armor.Name;

							if ( name == null )
								name = String.Format( "#{0}", armor.LabelNumber );

							if ( armor is BaseShield )
								from.SendLocalizedMessage( 1062003, name ); // You can no longer equip your ~1_SHIELD~
							else
								from.SendLocalizedMessage( 1062002, name ); // You can no longer wear your ~1_ARMOR~

							from.AddToBackpack( armor );
							moved = true;
						}
					}
					else if ( item is BaseClothing )
					{
						BaseClothing clothing = (BaseClothing)item;

						bool drop = false;

						if ( !clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster )
						{
							drop = true;
						}
						else if ( !clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster )
						{
							drop = true;
						}
						else if( clothing.RequiredRace != null && clothing.RequiredRace != this.Race )
						{
							drop = true;
						}
						else
						{
							int strBonus = clothing.ComputeStatBonus( StatType.Str );
							int strReq = clothing.ComputeStatReq( StatType.Str );

							if( str < strReq || (str + strBonus) < 1 )
								drop = true;
						}

						if ( drop )
						{
							string name = clothing.Name;

							if ( name == null )
								name = String.Format( "#{0}", clothing.LabelNumber );

							from.SendLocalizedMessage( 1062002, name ); // You can no longer wear your ~1_ARMOR~

							from.AddToBackpack( clothing );
							moved = true;
						}
					}

					FactionItem factionItem = FactionItem.Find( item );

					if ( factionItem != null )
					{
						bool drop = false;

						Faction ourFaction = Faction.Find( this );

						if ( ourFaction == null || ourFaction != factionItem.Faction )
							drop = true;
						else if ( ++factionItemCount > FactionItem.GetMaxWearables( this ) )
							drop = true;

						if ( drop )
						{
							from.AddToBackpack( item );
							moved = true;
						}
					}
				}

				if ( moved )
					from.SendLocalizedMessage( 500647 ); // Some equipment has been moved to your backpack.
			}
			catch ( Exception e )
			{
				Console.WriteLine( e );
			}
			finally
			{
				m_NoDeltaRecursion = false;
			}
		}

		public override void Delta( MobileDelta flag )
		{
			base.Delta( flag );

			if ( (flag & MobileDelta.Stat) != 0 )
				ValidateEquipment();

			if ( (flag & (MobileDelta.Name | MobileDelta.Hue)) != 0 )
				InvalidateMyRunUO();
		}

		private static void Disconnect( object state )
		{
			NetState ns = ((Mobile)state).NetState;

			if ( ns != null )
				ns.Dispose();
		}

		private static void OnLogout( LogoutEventArgs e )
		{
            PlayerMobile pm = e.Mobile as PlayerMobile; 
            if (pm != null)
            {
                if (pm.m_StatLoss.Count > 0)
                    pm.m_StatLoss[0].Pause();
            }
		}

		private static void EventSink_Connected( ConnectedEventArgs e )
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			if ( pm != null )
			{
				pm.m_SessionStart = DateTime.Now;

				if ( pm.m_Quest != null )
					pm.m_Quest.StartTimer();

				pm.BedrollLogout = false;
				pm.LastOnline = DateTime.Now;
			}

			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ClearSpecialMovesCallback ), e.Mobile );
		}

		private static void ClearSpecialMovesCallback( object state )
		{
			Mobile from = (Mobile)state;

			SpecialMove.ClearAllMoves( from );
		}

		private static void EventSink_Disconnected( DisconnectedEventArgs e )
		{
			Mobile from = e.Mobile;
			DesignContext context = DesignContext.Find( from );

			if ( context != null )
			{
				/* Client disconnected
				 *  - Remove design context
				 *  - Eject all from house
				 *  - Restore relocated entities
				 */

				// Remove design context
				DesignContext.Remove( from );

				// Eject all from house
				from.RevealingAction();

				foreach ( Item item in context.Foundation.GetItems() )
					item.Location = context.Foundation.BanLocation;

				foreach ( Mobile mobile in context.Foundation.GetMobiles() )
					mobile.Location = context.Foundation.BanLocation;

				// Restore relocated entities
				context.Foundation.RestoreRelocatedEntities();
			}

			PlayerMobile pm = e.Mobile as PlayerMobile;

			if ( pm != null )
			{
				pm.m_GameTime += (DateTime.Now - pm.m_SessionStart);

				if ( pm.m_Quest != null )
					pm.m_Quest.StopTimer();

				pm.m_SpeechLog = null;
				pm.LastOnline = DateTime.Now;
			}
		}

		public override void RevealingAction()
		{
			if ( m_DesignContext != null )
				return;

			Spells.Sixth.InvisibilitySpell.RemoveTimer( this );

			base.RevealingAction();
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override bool Hidden
		{
			get
			{
				return base.Hidden;
			}
			set
			{
				base.Hidden = value;

				RemoveBuff( BuffIcon.Invisibility );	//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

				if( !Hidden )
				{
					RemoveBuff( BuffIcon.HidingAndOrStealth );
				}
				else// if( !InvisibilitySpell.HasTimer( this ) )
				{
					BuffInfo.AddBuff( this, new BuffInfo( BuffIcon.HidingAndOrStealth, 1075655 ) );	//Hidden/Stealthing & You Are Hidden
				}
			}
		}

		public override void OnSubItemAdded( Item item )
		{
			if ( AccessLevel < AccessLevel.GameMaster && item.IsChildOf( this.Backpack ) )
			{
				int maxWeight = WeightOverloading.GetMaxWeight( this );
				int curWeight = Mobile.BodyWeight + this.TotalWeight;

				if ( curWeight > maxWeight )
					this.SendLocalizedMessage( 1019035, true, String.Format( " : {0} / {1}", curWeight, maxWeight ) );
			}
		}

		public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
		{
			if ( m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null) )
				return false;

			if ( target is PlayerVendor || target is TownCrier )
			{
				if ( message )
				{
					if ( target.Title == null )
						SendMessage( "{0} the vendor cannot be harmed.", target.Name );
					else
						SendMessage( "{0} {1} cannot be harmed.", target.Name, target.Title );
				}

				return false;
			}

			return base.CanBeHarmful( target, message, ignoreOurBlessedness );
		}

		public override bool CanBeBeneficial( Mobile target, bool message, bool allowDead )
		{
			if ( m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null) )
				return false;

			return base.CanBeBeneficial( target, message, allowDead );
		}

		public override bool CheckContextMenuDisplay( IEntity target )
		{
            return base.CheckContextMenuDisplay( target ) && m_DesignContext == null;
		}

		public override void OnItemAdded( Item item )
		{
			base.OnItemAdded( item );

			if ( item is BaseArmor || item is BaseWeapon )
			{
				Hits=Hits; Stam=Stam; Mana=Mana;
			}

			if ( this.NetState != null )
				CheckLightLevels( false );

			InvalidateMyRunUO();
		}

		public override void OnItemRemoved( Item item )
		{
			base.OnItemRemoved( item );

			if ( item is BaseArmor || item is BaseWeapon )
			{
				Hits=Hits; Stam=Stam; Mana=Mana;
			}

			if ( this.NetState != null )
				CheckLightLevels( false );

			InvalidateMyRunUO();
		}

		public override double ArmorRating
		{
			get
			{
				//BaseArmor ar;
				double rating = 0.0;

				AddArmorRating( ref rating, NeckArmor );
				AddArmorRating( ref rating, HandArmor );
				AddArmorRating( ref rating, HeadArmor );
				AddArmorRating( ref rating, ArmsArmor );
				AddArmorRating( ref rating, LegsArmor );
				AddArmorRating( ref rating, ChestArmor );
				AddArmorRating( ref rating, ShieldArmor );

				return VirtualArmor + VirtualArmorMod + rating;
			}
		}

		private void AddArmorRating( ref double rating, Item armor )
		{
			BaseArmor ar = armor as BaseArmor;

			if( ar != null && ( !Core.AOS || ar.ArmorAttributes.MageArmor == 0 ))
				rating += ar.ArmorRatingScaled;
		}

		#region [Stats]Max
		[CommandProperty( AccessLevel.GameMaster )]
		public override int HitsMax
		{
			/*get
			{
				int strBase;
				int strOffs = GetStatOffset( StatType.Str );

				if ( Core.AOS )
				{
					strBase = this.Str;	//this.Str already includes GetStatOffset/str
					strOffs = AosAttributes.GetValue( this, AosAttribute.BonusHits );

					if ( AnimalForm.UnderTransformation( this, typeof( BakeKitsune ) ) || AnimalForm.UnderTransformation( this, typeof( GreyWolf ) ) )
						strOffs += 20;
				}
				else
				{
					strBase = this.RawStr;
				}

				return (strBase / 2) + 50 + strOffs;
			}*/
            get { return this.Str; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int StamMax
		{
			get{ return base.StamMax + AosAttributes.GetValue( this, AosAttribute.BonusStam ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ManaMax
		{
			get{ return base.ManaMax + AosAttributes.GetValue( this, AosAttribute.BonusMana ) + ((Core.ML && Race == Race.Elf) ? 20 : 0); }
		}
		#endregion

		#region Stat Getters/Setters

		[CommandProperty( AccessLevel.GameMaster )]
		public override int Str
		{
			get
			{
				if( Core.ML && this.AccessLevel == AccessLevel.Player )
					return Math.Min( base.Str, 150 );

				return base.Str;
			}
			set
			{
				base.Str = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int Int
		{
			get
			{
				if( Core.ML && this.AccessLevel == AccessLevel.Player )
					return Math.Min( base.Int, 150 );

				return base.Int;
			}
			set
			{
				base.Int = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int Dex
		{
			get
			{
				if( Core.ML && this.AccessLevel == AccessLevel.Player )
					return Math.Min( base.Dex, 150 );

				return base.Dex;
			}
			set
			{
				base.Dex = value;
			}
		}

		#endregion

		public override bool Move( Direction d )
		{
			NetState ns = this.NetState;

			/*if ( ns != null )
			{
				if ( HasGump( typeof( ResurrectGump ) ) ) {
					if ( Alive ) {
						CloseGump( typeof( ResurrectGump ) );
					} else {
						SendLocalizedMessage( 500111 ); // You are frozen and cannot move.
						return false;
					}
				}
			}*/

			TimeSpan speed = ComputeMovementSpeed( d );

			bool res;

			if ( !Alive )
				Server.Movement.MovementImpl.IgnoreMovableImpassables = true;

			res = base.Move( d );

			Server.Movement.MovementImpl.IgnoreMovableImpassables = false;

			if ( !res )
				return false;

			m_NextMovementTime += speed;

			return true;
		}

		public override bool CheckMovement( Direction d, out int newZ )
		{
			DesignContext context = m_DesignContext;

			if ( context == null )
				return base.CheckMovement( d, out newZ );

			HouseFoundation foundation = context.Foundation;

			newZ = foundation.Z + HouseFoundation.GetLevelZ( context.Level, context.Foundation );

			int newX = this.X, newY = this.Y;
			Movement.Movement.Offset( d, ref newX, ref newY );

			int startX = foundation.X + foundation.Components.Min.X + 1;
			int startY = foundation.Y + foundation.Components.Min.Y + 1;
			int endX = startX + foundation.Components.Width - 1;
			int endY = startY + foundation.Components.Height - 2;

			return ( newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map );
		}

		public override bool AllowItemUse( Item item )
		{
            #region Dueling
            if (m_DuelContext != null && !m_DuelContext.AllowItemUse(this, item))
                return false;
            #endregion

			return DesignContext.Check( this );
		}

		public SkillName[] AnimalFormRestrictedSkills{ get{ return m_AnimalFormRestrictedSkills; } }

		private SkillName[] m_AnimalFormRestrictedSkills = new SkillName[]
		{
			SkillName.ArmsLore,	SkillName.Begging, SkillName.Discordance, SkillName.Forensics,
			SkillName.Inscribe, SkillName.ItemID, SkillName.Meditation, SkillName.Peacemaking,
			SkillName.Provocation, SkillName.RemoveTrap, SkillName.SpiritSpeak, SkillName.Stealing,	
			SkillName.TasteID
		};

		public override bool AllowSkillUse( SkillName skill )
		{
			if ( AnimalForm.UnderTransformation( this ) )
			{
				for( int i = 0; i < m_AnimalFormRestrictedSkills.Length; i++ )
				{
					if( m_AnimalFormRestrictedSkills[i] == skill )
					{
						SendLocalizedMessage( 1070771 ); // You cannot use that skill in this form.
						return false;
					}
				}
			}

            #region Dueling
            if (m_DuelContext != null && !m_DuelContext.AllowSkillUse(this, skill))
                return false;
            #endregion

			return DesignContext.Check( this );
		}

		private bool m_LastProtectedMessage;
		private int m_NextProtectionCheck = 10;

		public virtual void RecheckTownProtection()
		{
			m_NextProtectionCheck = 10;

			Regions.GuardedRegion reg = (Regions.GuardedRegion) this.Region.GetRegion( typeof( Regions.GuardedRegion ) );
			bool isProtected = ( reg != null && !reg.IsDisabled() );

			if ( isProtected != m_LastProtectedMessage )
			{
				if ( isProtected )
					SendLocalizedMessage( 500112 ); // You are now under the protection of the town guards.
				else
					SendLocalizedMessage( 500113 ); // You have left the protection of the town guards.

				m_LastProtectedMessage = isProtected;
			}
		}

		public override void MoveToWorld( Point3D loc, Map map )
		{
			base.MoveToWorld( loc, map );

			RecheckTownProtection();
		}

		public override void SetLocation( Point3D loc, bool isTeleport )
		{
			if ( !isTeleport && AccessLevel == AccessLevel.Player )
			{
				// moving, not teleporting
				int zDrop = ( this.Location.Z - loc.Z );

				if ( zDrop > 20 ) // we fell more than one story
					Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
			}

			base.SetLocation( loc, isTeleport );

			if ( isTeleport || --m_NextProtectionCheck == 0 )
				RecheckTownProtection();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from == this )
			{
				/*if ( m_Quest != null )
					m_Quest.GetContextMenuEntries( list );

				if ( Alive && InsuranceEnabled )
				{
					list.Add( new CallbackEntry( 6201, new ContextCallback( ToggleItemInsurance ) ) );

					if ( AutoRenewInsurance )
						list.Add( new CallbackEntry( 6202, new ContextCallback( CancelRenewInventoryInsurance ) ) );
					else
						list.Add( new CallbackEntry( 6200, new ContextCallback( AutoRenewInventoryInsurance ) ) );
				}*/

				BaseHouse house = BaseHouse.FindHouseAt( this );

				if ( house != null )
				{
					//if ( Alive && house.InternalizedVendors.Count > 0 && house.IsOwner( this ) )
					//	list.Add( new CallbackEntry( 6204, new ContextCallback( GetVendor ) ) );

					if ( /*house.IsAosRules &&*/ !Region.IsPartOf( typeof( Engines.ConPVP.SafeZone ) ) ) // Dueling
						list.Add( new CallbackEntry( 6207, new ContextCallback( LeaveHouse ) ) );
				}
                /*
				if ( m_JusticeProtectors.Count > 0 )
					list.Add( new CallbackEntry( 6157, new ContextCallback( CancelProtection ) ) );

				if( Alive )
					list.Add( new CallbackEntry( 6210, new ContextCallback( ToggleChampionTitleDisplay ) ) );
                */
			}
		}

		private void CancelProtection()
		{
			for ( int i = 0; i < m_JusticeProtectors.Count; ++i )
			{
				Mobile prot = m_JusticeProtectors[i];

				string args = String.Format( "{0}\t{1}", this.Name, prot.Name );

				prot.SendLocalizedMessage( 1049371, args ); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
				this.SendLocalizedMessage( 1049371, args ); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
			}

			m_JusticeProtectors.Clear();
		}

		#region Insurance

		private void ToggleItemInsurance()
		{
			if ( !CheckAlive() )
				return;

			BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
			SendLocalizedMessage( 1060868 ); // Target the item you wish to toggle insurance status on <ESC> to cancel
		}

		private bool CanInsure( Item item )
		{
			if ( item is Container || item is BagOfSending || item is KeyRing )
				return false;

			if ( (item is Spellbook && item.LootType == LootType.Blessed)|| item is Runebook || item is PotionKeg || item is Sigil )
				return false;

			if ( item.Stackable )
				return false;

			if ( item.LootType == LootType.Cursed )
				return false;

			if ( item.ItemID == 0x204E ) // death shroud
				return false;

			return true;
		}

		private void ToggleItemInsurance_Callback( Mobile from, object obj )
		{
			if ( !CheckAlive() )
				return;

			Item item = obj as Item;

			if ( item == null || !item.IsChildOf( this ) )
			{
				BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
				SendLocalizedMessage( 1060871, "", 0x23 ); // You can only insure items that you have equipped or that are in your backpack
			}
			else if ( item.Insured )
			{
				item.Insured = false;

				SendLocalizedMessage( 1060874, "", 0x35 ); // You cancel the insurance on the item

				BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
				SendLocalizedMessage( 1060868, "", 0x23 ); // Target the item you wish to toggle insurance status on <ESC> to cancel
			}
			else if ( !CanInsure( item ) )
			{
				BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
				SendLocalizedMessage( 1060869, "", 0x23 ); // You cannot insure that
			}
			else if ( item.LootType == LootType.Blessed || item.LootType == LootType.Newbied || item.BlessedFor == from )
			{
				BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
				SendLocalizedMessage( 1060870, "", 0x23 ); // That item is blessed and does not need to be insured
				SendLocalizedMessage( 1060869, "", 0x23 ); // You cannot insure that
			}
			else
			{
				if ( !item.PayedInsurance )
				{
					if ( Banker.Withdraw( from, 600 ) )
					{
						SendLocalizedMessage( 1060398, "600" ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
						item.PayedInsurance = true;
					}
					else
					{
						SendLocalizedMessage( 1061079, "", 0x23 ); // You lack the funds to purchase the insurance
						return;
					}
				}

				item.Insured = true;

				SendLocalizedMessage( 1060873, "", 0x23 ); // You have insured the item

				BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
				SendLocalizedMessage( 1060868, "", 0x23 ); // Target the item you wish to toggle insurance status on <ESC> to cancel
			}
		}

		private void AutoRenewInventoryInsurance()
		{
			if ( !CheckAlive() )
				return;

			SendLocalizedMessage( 1060881, "", 0x23 ); // You have selected to automatically reinsure all insured items upon death
			AutoRenewInsurance = true;
		}

		private void CancelRenewInventoryInsurance()
		{
			if ( !CheckAlive() )
				return;

			if( Core.SE )
			{
				if( !HasGump( typeof( CancelRenewInventoryInsuranceGump ) ) )
					SendGump( new CancelRenewInventoryInsuranceGump( this ) );
			}
			else
			{
				SendLocalizedMessage( 1061075, "", 0x23 ); // You have cancelled automatically reinsuring all insured items upon death
				AutoRenewInsurance = false;
			}
		}

		private class CancelRenewInventoryInsuranceGump : Gump
		{
			private PlayerMobile m_Player;

			public CancelRenewInventoryInsuranceGump( PlayerMobile player ) : base( 250, 200 )
			{
				m_Player = player;

				AddBackground( 0, 0, 240, 142, 0x13BE );
				AddImageTiled( 6, 6, 228, 100, 0xA40 );
				AddImageTiled( 6, 116, 228, 20, 0xA40 );
				AddAlphaRegion( 6, 6, 228, 142 );

				AddHtmlLocalized( 8, 8, 228, 100, 1071021, 0x7FFF, false, false ); // You are about to disable inventory insurance auto-renewal.

				AddButton( 6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 40, 118, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

				AddButton( 114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 148, 118, 450, 20, 1071022, 0x7FFF, false, false ); // DISABLE IT!
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( !m_Player.CheckAlive() )
					return;

				if ( info.ButtonID == 1 )
				{
					m_Player.SendLocalizedMessage( 1061075, "", 0x23 ); // You have cancelled automatically reinsuring all insured items upon death
					m_Player.AutoRenewInsurance = false;
				}
				else
				{
					m_Player.SendLocalizedMessage( 1042021 ); // Cancelled.
				}
			}
		}
		#endregion

		private void GetVendor()
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( CheckAlive() && house != null && house.IsOwner( this ) && house.InternalizedVendors.Count > 0 )
			{
				CloseGump( typeof( ReclaimVendorGump ) );
				SendGump( new ReclaimVendorGump( house ) );
			}
		}

		private void LeaveHouse()
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null )
				this.Location = house.BanLocation;
		}

		private delegate void ContextCallback();

		private class CallbackEntry : ContextMenuEntry
		{
			private ContextCallback m_Callback;

			public CallbackEntry( int number, ContextCallback callback ) : this( number, -1, callback )
			{
			}

			public CallbackEntry( int number, int range, ContextCallback callback ) : base( number, range )
			{
				m_Callback = callback;
			}

			public override void OnClick()
			{
				if ( m_Callback != null )
					m_Callback();
			}
		}

		public override void DisruptiveAction()
		{
			if( Meditating )
			{
				RemoveBuff( BuffIcon.ActiveMeditation );
			}

			base.DisruptiveAction();
		}
		public override void OnDoubleClick( Mobile from )
		{
			if ( this == from && !Warmode )
			{
				IMount mount = Mount;

				if ( mount != null && !DesignContext.Check( this ) )
					return;
			}

			base.OnDoubleClick( from );
		}

		public override void DisplayPaperdollTo( Mobile to )
		{
			if ( DesignContext.Check( this ) )
				base.DisplayPaperdollTo( to );
		}

		private static bool m_NoRecursion;

		public override bool CheckEquip( Item item )
		{
			if ( !base.CheckEquip( item ) )
				return false;

            #region Dueling
            if (m_DuelContext != null && !m_DuelContext.AllowItemEquip(this, item))
                return false;
            #endregion

			#region Factions
			FactionItem factionItem = FactionItem.Find( item );

			if ( factionItem != null )
			{
				Faction faction = Faction.Find( this );

				if ( faction == null )
				{
					SendLocalizedMessage( 1010371 ); // You cannot equip a faction item!
					return false;
				}
				else if ( faction != factionItem.Faction )
				{
					SendLocalizedMessage( 1010372 ); // You cannot equip an opposing faction's item!
					return false;
				}
				else
				{
					int maxWearables = FactionItem.GetMaxWearables( this );

					for ( int i = 0; i < Items.Count; ++i )
					{
						Item equiped = Items[i];

						if ( item != equiped && FactionItem.Find( equiped ) != null )
						{
							if ( --maxWearables == 0 )
							{
								SendLocalizedMessage( 1010373 ); // You do not have enough rank to equip more faction items!
								return false;
							}
						}
					}
				}
			}
			#endregion

			if ( this.AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && this.HasTrade )
			{
				BounceInfo bounce = item.GetBounce();

				if ( bounce != null )
				{
					if ( bounce.m_Parent is Item )
					{
						Item parent = (Item) bounce.m_Parent;

						if ( parent == this.Backpack || parent.IsChildOf( this.Backpack ) )
							return true;
					}
					else if ( bounce.m_Parent == this )
					{
						return true;
					}
				}

				SendLocalizedMessage( 1004042 ); // You can only equip what you are already carrying while you have a trade pending.
				return false;
			}

			return true;
		}

		public override bool CheckTrade( Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			int msgNum = 0;

			if ( cont == null )
			{
				if ( to.Holding != null )
					msgNum = 1062727; // You cannot trade with someone who is dragging something.
				else if ( this.HasTrade )
					msgNum = 1062781; // You are already trading with someone else!
				else if ( to.HasTrade )
					msgNum = 1062779; // That person is already involved in a trade
			}

			if ( msgNum == 0 )
			{
				if ( cont != null )
				{
					plusItems += cont.TotalItems;
					plusWeight += cont.TotalWeight;
				}

				if ( this.Backpack == null || !this.Backpack.CheckHold( this, item, false, checkItems, plusItems, plusWeight ) )
					msgNum = 1004040; // You would not be able to hold this if the trade failed.
				else if ( to.Backpack == null || !to.Backpack.CheckHold( to, item, false, checkItems, plusItems, plusWeight ) )
					msgNum = 1004039; // The recipient of this trade would not be able to carry this.
				else
					msgNum = CheckContentForTrade( item );
			}

			if ( msgNum != 0 )
			{
				if ( message )
					this.SendLocalizedMessage( msgNum );

				return false;
			}

			return true;
		}

		private static int CheckContentForTrade( Item item )
		{
			//if ( item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None )
			//	return 1004044; // You may not trade trapped items.

			//if ( SkillHandlers.StolenItem.IsStolen( item ) )
			//	return 1004043; // You may not trade recently stolen items.

			if ( item is Container )
			{
				foreach ( Item subItem in item.Items )
				{
					int msg = CheckContentForTrade( subItem );

					if ( msg != 0 )
						return msg;
				}
			}

			return 0;
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			if ( !base.CheckNonlocalDrop( from, item, target ) )
				return false;

			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			Container pack = this.Backpack;
			if ( from == this && this.HasTrade && ( target == pack || target.IsChildOf( pack ) ) )
			{
				BounceInfo bounce = item.GetBounce();

				if ( bounce != null && bounce.m_Parent is Item )
				{
					Item parent = (Item) bounce.m_Parent;

					if ( parent == pack || parent.IsChildOf( pack ) )
						return true;
				}

				SendLocalizedMessage( 1004041 ); // You can't do that while you have a trade pending.
				return false;
			}

			return true;
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{
			CheckLightLevels( false );

            #region Dueling
            if (m_DuelContext != null)
                m_DuelContext.OnLocationChanged(this);
            #endregion

			DesignContext context = m_DesignContext;

			if ( context == null || m_NoRecursion )
				return;

			m_NoRecursion = true;

			HouseFoundation foundation = context.Foundation;

			int newX = this.X, newY = this.Y;
			int newZ = foundation.Z + HouseFoundation.GetLevelZ( context.Level, context.Foundation );

			int startX = foundation.X + foundation.Components.Min.X + 1;
			int startY = foundation.Y + foundation.Components.Min.Y + 1;
			int endX = startX + foundation.Components.Width - 1;
			int endY = startY + foundation.Components.Height - 2;

			if ( newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map )
			{
				if ( Z != newZ )
					Location = new Point3D( X, Y, newZ );

				m_NoRecursion = false;
				return;
			}

			Location = new Point3D( foundation.X, foundation.Y, newZ );
			Map = foundation.Map;

			m_NoRecursion = false;
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is BaseCreature && !((BaseCreature)m).Controlled )
				return false;

            #region Dueling
            if (Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)) && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                if (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished || pm.DuelPlayer.Eliminated)
                    return true;
            }
            #endregion

			return base.OnMoveOver( m );
		}

		public override bool CheckShove( Mobile shoved )
		{
			if( TransformationSpellHelper.UnderTransformation( this, typeof( WraithFormSpell ) ) )
				return true;
			else
				return base.CheckShove( shoved );
		}


		protected override void OnMapChange( Map oldMap )
		{
			if ( (Map != Faction.Facet && oldMap == Faction.Facet) || (Map == Faction.Facet && oldMap != Faction.Facet) )
				InvalidateProperties();

            #region Dueling
            if (m_DuelContext != null)
                m_DuelContext.OnMapChanged(this);
            #endregion

			DesignContext context = m_DesignContext;

			if ( context == null || m_NoRecursion )
				return;

			m_NoRecursion = true;

			HouseFoundation foundation = context.Foundation;

			if ( Map != foundation.Map )
				Map = foundation.Map;

			m_NoRecursion = false;
		}

		public override void OnBeneficialAction( Mobile target, bool isCriminal )
		{
			if ( m_SentHonorContext != null )
				m_SentHonorContext.OnSourceBeneficialAction( target );

			base.OnBeneficialAction( target, isCriminal );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			int disruptThreshold;

			if ( !Core.AOS )
				disruptThreshold = 0;
			else if ( from != null && from.Player )
				disruptThreshold = 18;
			else
				disruptThreshold = 25;

			if ( amount > disruptThreshold )
			{
				BandageContext c = BandageContext.GetContext( this );

				if ( c != null )
					c.Slip();
			}

			if( Confidence.IsRegenerating( this ) )
				Confidence.StopRegenerating( this );

			WeightOverloading.FatigueOnDamage( this, amount );

			if ( m_ReceivedHonorContext != null )
				m_ReceivedHonorContext.OnTargetDamaged( from, amount );
			if ( m_SentHonorContext != null )
				m_SentHonorContext.OnSourceDamaged( from, amount );

			base.OnDamage( amount, from, willKill );
		}

		public override void Resurrect()
		{
			bool wasAlive = this.Alive;

			base.Resurrect();

			if ( this.Alive && !wasAlive )
			{
			/*
                bool isNew = false;
				DeathRobe deathRobe = this.FindItemOnLayer( Layer.OuterTorso ) as DeathRobe;
                
                if ( deathRobe == null )
                {
                    if ( this.Backpack != null )
                        deathRobe = Backpack.FindItemByType( typeof( DeathRobe ) ) as DeathRobe;

                    if ( deathRobe == null )
                    {
                        deathRobe = new DeathRobe();
                        isNew = true;
                    }
                }

				if ( !EquipItem( deathRobe ) && isNew )
					deathRobe.Delete();
			*/

                SendEverything();

                if (m_StatLoss.Count > 0 && (DuelContext == null || DuelPlayer == null || !DuelContext.Started || DuelContext.Finished || DuelPlayer.Eliminated))
                    m_StatLoss[0].Resume();
			}
		}

		public override double RacialSkillBonus
		{
			get
			{
				if( Core.ML && this.Race == Race.Human )
					return 20.0;

				return 0;
			}
		}

		private Mobile m_InsuranceAward;
		private int m_InsuranceCost;
		private int m_InsuranceBonus;

		public override bool OnBeforeDeath()
		{
			m_InsuranceCost = 0;
			m_InsuranceAward = base.FindMostRecentDamager( false );

			if ( m_InsuranceAward is BaseCreature )
			{
				Mobile master = ((BaseCreature)m_InsuranceAward).GetMaster();

				if ( master != null )
					m_InsuranceAward = master;
			}

			if ( m_InsuranceAward != null && (!m_InsuranceAward.Player || m_InsuranceAward == this) )
				m_InsuranceAward = null;

			if ( m_InsuranceAward is PlayerMobile )
				((PlayerMobile)m_InsuranceAward).m_InsuranceBonus = 0;

			if ( m_ReceivedHonorContext != null )
				m_ReceivedHonorContext.OnTargetKilled();
			if ( m_SentHonorContext != null )
				m_SentHonorContext.OnSourceKilled();

            DropHolding();

			return base.OnBeforeDeath();
		}

		private bool CheckInsuranceOnDeath( Item item )
		{
			if ( InsuranceEnabled && item.Insured )
			{
                #region Dueling
                if (m_DuelPlayer != null && m_DuelContext != null && m_DuelContext.Registered && m_DuelContext.Started && !m_DuelPlayer.Eliminated)
                    return true;
                #endregion

				if ( AutoRenewInsurance )
				{
					int cost = ( m_InsuranceAward == null ? 600 : 300 );

					if ( Banker.Withdraw( this, cost ) )
					{
						m_InsuranceCost += cost;
						item.PayedInsurance = true;
					}
					else
					{
						SendLocalizedMessage( 1061079, "", 0x23 ); // You lack the funds to purchase the insurance
						item.PayedInsurance = false;
						item.Insured = false;
					}
				}
				else
				{
					item.PayedInsurance = false;
					item.Insured = false;
				}

				if ( m_InsuranceAward != null )
				{
					if ( Banker.Deposit( m_InsuranceAward, 300 ) )
					{
						if ( m_InsuranceAward is PlayerMobile )
							((PlayerMobile)m_InsuranceAward).m_InsuranceBonus += 300;
					}
				}

				return true;
			}

			return false;
		}

		public override DeathMoveResult GetParentMoveResultFor( Item item )
		{
			if ( CheckInsuranceOnDeath( item ) )
				return DeathMoveResult.MoveToBackpack;

			DeathMoveResult res = base.GetParentMoveResultFor( item );

			if ( res == DeathMoveResult.MoveToCorpse && item.Movable && this.Young )
				res = DeathMoveResult.MoveToBackpack;

			return res;
		}

		public override DeathMoveResult GetInventoryMoveResultFor( Item item )
		{
			if ( CheckInsuranceOnDeath( item ) )
				return DeathMoveResult.MoveToBackpack;

			DeathMoveResult res = base.GetInventoryMoveResultFor( item );

			if ( res == DeathMoveResult.MoveToCorpse && item.Movable && this.Young )
				res = DeathMoveResult.MoveToBackpack;

			return res;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			HueMod = -1;
			NameMod = null;
			SavagePaintExpiration = TimeSpan.Zero;

			SetHairMods( -1, -1 );

			PolymorphSpell.StopTimer( this );
			IncognitoSpell.StopTimer( this );
			DisguiseGump.StopTimer( this );

			EndAction( typeof( PolymorphSpell ) );
			EndAction( typeof( IncognitoSpell ) );

			MeerMage.StopEffect( this, false );
/*
            #region Temp Statloss
            if ( m_StatLoss.Count > 0 && m_StatLoss[0] != null )
                m_StatLoss[0].Pause();

            CheckKillDecay();

			CustomRegion cr = null;
			if ( Region is CustomRegion )
				cr = (CustomRegion)Region;

            if ( ShortTermMurders >= 5 && ( ( cr == null ) || ( cr != null && !cr.BattleGrounds ) ) && (DuelContext == null || DuelPlayer == null || !DuelContext.Started || DuelContext.Finished || DuelPlayer.Eliminated))
            {
				int time = ( ( ShortTermMurders / 5 ) * 30 );
				
				// 8 hour max
				if ( time > 480 )
					time = 480;
		
                bool start = m_StatLoss.Count == 0;
                StatLossInfo sli;

                double percent = ((double)ShortTermMurders) / 100.0;
                if ( percent > 0.25 )
                    percent = 0.25;

                sli = new StatLossInfo( this, percent, time );
                m_StatLoss.Add( sli );

                if ( start )
                {
                    sli.Start();
                    sli.Pause();
                }
            }
            #endregion
*/
            //SkillHandlers.StolenItem.ReturnOnDeath( this, c );

			if ( m_PermaFlags.Count > 0 )
			{
				m_PermaFlags.Clear();

				if ( c is Corpse )
					((Corpse)c).Criminal = true;

				if ( SkillHandlers.Stealing.ClassicMode )
					Criminal = true;
			}

			if ( this.Kills >= 5 && DateTime.Now >= m_NextJustAward )
			{
				Mobile m = FindMostRecentDamager( false );

				if( m is BaseCreature )
					m = ((BaseCreature)m).GetMaster();

				if ( m != null && m is PlayerMobile && m != this )
				{
					bool gainedPath = false;

					int pointsToGain = 0;

					pointsToGain += (int) Math.Sqrt( this.GameTime.TotalSeconds * 4 );
					pointsToGain *= 5;
					pointsToGain += (int) Math.Pow( this.Skills.Total / 250, 2 );

					if ( VirtueHelper.Award( m, VirtueName.Justice, pointsToGain, ref gainedPath ) )
					{
						if ( gainedPath )
							m.SendLocalizedMessage( 1049367 ); // You have gained a path in Justice!
						else
							m.SendLocalizedMessage( 1049363 ); // You have gained in Justice.

						m.FixedParticles( 0x375A, 9, 20, 5027, EffectLayer.Waist );
						m.PlaySound( 0x1F7 );

						m_NextJustAward = DateTime.Now + TimeSpan.FromMinutes( pointsToGain / 3 );
					}
				}
			}

			if ( m_InsuranceCost > 0 )
				SendLocalizedMessage( 1060398, m_InsuranceCost.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

			if ( m_InsuranceAward is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m_InsuranceAward;

				if ( pm.m_InsuranceBonus > 0 )
					pm.SendLocalizedMessage( 1060397, pm.m_InsuranceBonus.ToString() ); // ~1_AMOUNT~ gold has been deposited into your bank box.
			}

			Mobile killer = this.FindMostRecentDamager( true );

			if ( killer is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)killer;

				Mobile master = bc.GetMaster();
				if( master != null )
					killer = master;
			}

			if ( this.Young )
			{
				if ( YoungDeathTeleport() )
					Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), new TimerCallback( SendYoungDeathNotice ) );
			}

			Faction.HandleDeath( this, killer );

			Server.Guilds.Guild.HandleDeath( this, killer );

            #region Dueling
            if (m_DuelContext != null)
                m_DuelContext.OnDeath(this, c);
            #endregion

			if( m_BuffTable != null )
			{
				List<BuffInfo> list = new List<BuffInfo>();

				foreach( BuffInfo buff in m_BuffTable.Values )
				{
					if( !buff.RetainThroughDeath )
					{
						list.Add( buff );
					}
				}

				for( int i = 0; i < list.Count; i++ )
				{
					RemoveBuff( list[i] );
				}
			}

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, Core.GlobalMaxUpdateRange);

            foreach (Mobile m in eable)
            {
                // find all the living people we can't see and remove them
                if (m.Alive && !CanSee(m))
                    Send(m.RemovePacket);
                // find all the dead people we can see and send them
                else if (!m.Alive && CanSee(m))
                    Send(new MobileIncoming(this, m));
            }

            eable.Free();
		}

		private List<Mobile> m_PermaFlags;
		private List<Mobile> m_VisList;
		private Hashtable m_AntiMacroTable;
		private TimeSpan m_GameTime;
		private TimeSpan m_ShortTermElapse;
		private TimeSpan m_LongTermElapse;
		private DateTime m_SessionStart;
		private DateTime m_LastEscortTime;
		private DateTime m_NextSmithBulkOrder;
		private DateTime m_NextTailorBulkOrder;
		private DateTime m_SavagePaintExpiration;
		private SkillName m_Learning = (SkillName)(-1);

		public SkillName Learning
		{
			get{ return m_Learning; }
			set{ m_Learning = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan SavagePaintExpiration
		{
			get
			{
				TimeSpan ts = m_SavagePaintExpiration - DateTime.Now;

				if ( ts < TimeSpan.Zero )
					ts = TimeSpan.Zero;

				return ts;
			}
			set
			{
				m_SavagePaintExpiration = DateTime.Now + value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan NextSmithBulkOrder
		{
			get
			{
				TimeSpan ts = m_NextSmithBulkOrder - DateTime.Now;

				if ( ts < TimeSpan.Zero )
					ts = TimeSpan.Zero;

				return ts;
			}
			set
			{
				try{ m_NextSmithBulkOrder = DateTime.Now + value; }
				catch{}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan NextTailorBulkOrder
		{
			get
			{
				TimeSpan ts = m_NextTailorBulkOrder - DateTime.Now;

				if ( ts < TimeSpan.Zero )
					ts = TimeSpan.Zero;

				return ts;
			}
			set
			{
				try{ m_NextTailorBulkOrder = DateTime.Now + value; }
				catch{}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastEscortTime
		{
			get{ return m_LastEscortTime; }
			set{ m_LastEscortTime = value; }
		}

		public PlayerMobile()
		{
			m_VisList = new List<Mobile>();
			m_PermaFlags = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();

			m_BOBFilter = new Engines.BulkOrders.BOBFilter();

			m_GameTime = TimeSpan.Zero;
            m_ShortTermElapse = ShortTermMurderDecay;
            m_LongTermElapse = LongTermMurderDecay;

			m_JusticeProtectors = new List<Mobile>();
			m_GuildRank = Guilds.RankDefinition.Lowest;

			m_ChampionTitles = new ChampionTitleInfo();

            FollowersMax = 5;

			InvalidateMyRunUO();
		}

        public override int GetDeathSound()
        {
            if (Body.IsHuman)
            {
                return Utility.Random(Female ? 336 : 346, 4);
            }
            else
            {
                return base.GetDeathSound();
            }
        }

		public override bool MutateSpeech( List<Mobile> hears, ref string text, ref object context )
		{
			if ( Alive )
				return false;

			if ( Core.AOS )
			{
				for ( int i = 0; i < hears.Count; ++i )
				{
					Mobile m = hears[i];

					if ( m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0 )
						return false;
				}
			}

			return base.MutateSpeech( hears, ref text, ref context );
		}

		/*public override void DoSpeech( string text, int[] keywords, MessageType type, int hue )
		{
			if( Guilds.Guild.NewGuildSystem && (type == MessageType.Guild || type == MessageType.Alliance) )
			{
				Guilds.Guild g = this.Guild as Guilds.Guild;
				if( g == null )
				{
					SendLocalizedMessage( 1063142 ); // You are not in a guild!
				}
				else if( type == MessageType.Alliance )
				{
					if( g.Alliance != null && g.Alliance.IsMember( g ) )
					{
						//g.Alliance.AllianceTextMessage( hue, "[Alliance][{0}]: {1}", this.Name, text );
						g.Alliance.AllianceChat( this, text );
						SendToStaffMessage( this, "[Alliance]: {0}", text );

						m_AllianceMessageHue = hue;
					}
					else
					{
						SendLocalizedMessage( 1071020 ); // You are not in an alliance!
					}
				}
				else	//Type == MessageType.Guild
				{
					m_GuildMessageHue = hue;

					g.GuildChat( this, text );
					SendToStaffMessage( this, "[Guild]: {0}", text );
				}
			}
			else
			{
				base.DoSpeech( text, keywords, type, hue );
			}
		}*/

        public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
        {
            if ( type == MessageType.Guild || type == MessageType.Alliance )
            {
                GuildMessage(text, hue);
            }
            else
            {
                base.DoSpeech(text, keywords, type, hue);
            }
        }

        public void GuildMessage(string text, int hue)
        {
            m_GuildMessageHue = hue;

            GuildMessage( text );
        }

        public virtual void GuildMessage( string text )
        {
            Guilds.Guild g = this.Guild as Guilds.Guild;
            if ( g == null )
            {
                SendLocalizedMessage(1063142); // You are not in a guild!
            }
            else
            {
                g.GuildChat( this, text );
                SendToStaffMessage(this, "[Guild]: {0}", text);
            }
        }

		public static void SendToStaffMessage( Mobile from, string text )
		{
			Packet p = null;

			foreach( NetState ns in from.GetClientsInRange( 8 ) )
			{
				Mobile mob = ns.Mobile;

				if( mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel )
				{
					if( p == null )
						p = Packet.Acquire( new UnicodeMessage( from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text ) );

					ns.Send( p );
				}
			}

			Packet.Release( p );
		}
		
        public static void SendToStaffMessage( Mobile from, string format, params object[] args )
		{
			SendToStaffMessage( from, String.Format( format, args ) );
		}

		public override void Damage( int amount, Mobile from )
		{
			if ( Spells.Necromancy.EvilOmenSpell.CheckEffect( this ) )
				amount = (int)(amount * 1.25);

			Mobile oath = Spells.Necromancy.BloodOathSpell.GetBloodOath( from );

			if ( oath == this )
			{
				amount = (int)(amount * 1.1);
				from.Damage( amount, from );
			}

			base.Damage( amount, from );
		}

		#region Poison
		public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
		{
			if ( !Alive )
				return ApplyPoisonResult.Immune;

			if ( Spells.Necromancy.EvilOmenSpell.CheckEffect( this ) )
				poison = PoisonImpl.IncreaseLevel( poison );

			ApplyPoisonResult result = base.ApplyPoison( from, poison );

			if ( from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer )
				(PoisonTimer as PoisonImpl.PoisonTimer).From = from;

			return result;
		}

		public override bool CheckPoisonImmunity( Mobile from, Poison poison )
		{
			if ( this.Young )
				return true;

			return base.CheckPoisonImmunity( from, poison );
		}

		public override void OnPoisonImmunity( Mobile from, Poison poison )
		{
			if ( this.Young )
				SendLocalizedMessage( 502808 ); // You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
			else
				base.OnPoisonImmunity( from, poison );
		}
		#endregion

		public PlayerMobile( Serial s ) : base( s )
		{
			m_VisList = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();
			InvalidateMyRunUO();

            m_PermaFlags = new List<Mobile>();
           
            m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            m_GameTime = TimeSpan.Zero;
            m_ShortTermElapse = ShortTermMurderDecay;
            m_LongTermElapse = LongTermMurderDecay;

            m_JusticeProtectors = new List<Mobile>();
            m_GuildRank = Guilds.RankDefinition.Lowest;

            m_ChampionTitles = new ChampionTitleInfo();
		}

		public List<Mobile> VisibilityList
		{
			get{ return m_VisList; }
		}

		public List<Mobile> PermaFlags
		{
			get{ return m_PermaFlags; }
		}

		public override int Luck{ get{ return AosAttributes.GetValue( this, AosAttribute.Luck ); } }

		public override bool IsHarmfulCriminal( Mobile target )
		{
			if ( SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0 )
			{
				int noto = Notoriety.Compute( this, target );

				if ( noto == Notoriety.Innocent )
					target.Delta( MobileDelta.Noto );

				return false;
			}

			if ( target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled )
				return false;

			return base.IsHarmfulCriminal( target );
		}

		public bool AntiMacroCheck( Skill skill, object obj )
		{
			if ( obj == null || m_AntiMacroTable == null || this.AccessLevel != AccessLevel.Player )
				return true;

			Hashtable tbl = (Hashtable)m_AntiMacroTable[skill];
			if ( tbl == null )
				m_AntiMacroTable[skill] = tbl = new Hashtable();

			CountAndTimeStamp count = (CountAndTimeStamp)tbl[obj];
			if ( count != null )
			{
				if ( count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.Now )
				{
					count.Count = 1;
					return true;
				}
				else
				{
					++count.Count;
					if ( count.Count <= SkillCheck.Allowance )
						return true;
					else
						return false;
				}
			}
			else
			{
				tbl[obj] = count = new CountAndTimeStamp();
				count.Count = 1;

				return true;
			}
		}

		private void RevertHair()
		{
			SetHairMods( -1, -1 );
		}

		private Engines.BulkOrders.BOBFilter m_BOBFilter;

		public Engines.BulkOrders.BOBFilter BOBFilter
		{
			get{ return m_BOBFilter; }
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 28:
				{
					m_EventBanned = reader.ReadBool();
					goto case 27;
				}
                case 27:
                {
                    int count = reader.ReadInt();

                    m_StatLoss = new List<StatLossInfo>( count );
                    for (int i = 0; i < count; i++)
                    {
                        StatLossInfo sli = new StatLossInfo( this );
                        sli.Deserialize( reader );
                        m_StatLoss.Add( sli );
                    }

                    if ( m_StatLoss.Count > 0 )
                    {
                        // start it to add the mods, then just pause it until they log in
                        m_StatLoss[0].Start();
                        m_StatLoss[0].Pause();
                    }

                    goto case 26;
                }
				case 26:
				{
					m_NewbKills = reader.ReadInt();
					goto case 25;
				}
				case 25:
				{
					int recipeCount = reader.ReadInt();

					if( recipeCount > 0 )
					{
						m_AcquiredRecipes = new Dictionary<int, bool>();

						for( int i = 0; i < recipeCount; i++ )
						{
							int r = reader.ReadInt();
							if( reader.ReadBool() )	//Don't add in recipies which we haven't gotten or have been removed
								m_AcquiredRecipes.Add( r, true );
						}
					}
					goto case 24;
				}
				case 24:
				{
					m_LastHonorLoss = reader.ReadDeltaTime();
					goto case 23;
				}
				case 23:
				{
					m_ChampionTitles = new ChampionTitleInfo( reader );
					goto case 22;
				}
				case 22:
				{
					m_LastValorLoss = reader.ReadDateTime();
					goto case 21;
				}
				case 21:
				{
					m_ToTItemsTurnedIn = reader.ReadEncodedInt();
					m_ToTTotalMonsterFame = reader.ReadInt();
					goto case 20;
				}
				case 20:
				{
					m_AllianceMessageHue = reader.ReadEncodedInt();
					m_GuildMessageHue = reader.ReadEncodedInt();

					goto case 19;
				}
				case 19:
				{
					int rank = reader.ReadEncodedInt();
					int maxRank = Guilds.RankDefinition.Ranks.Length -1;
					if( rank > maxRank )
						rank = maxRank;

					m_GuildRank = Guilds.RankDefinition.Ranks[rank];
					m_LastOnline = reader.ReadDateTime();
					goto case 18;
				}
				case 18:
				{
					m_SolenFriendship = (SolenFriendship) reader.ReadEncodedInt();

					goto case 17;
				}
				case 17: // changed how DoneQuests is serialized
				case 16:
				{
					m_Quest = QuestSerializer.DeserializeQuest( reader );

					if ( m_Quest != null )
						m_Quest.From = this;

					int count = reader.ReadEncodedInt();

					if ( count > 0 )
					{
						m_DoneQuests = new List<QuestRestartInfo>();

						for ( int i = 0; i < count; ++i )
						{
							Type questType = QuestSerializer.ReadType( QuestSystem.QuestTypes, reader );
							DateTime restartTime;

							if ( version < 17 )
								restartTime = DateTime.MaxValue;
							else
								restartTime = reader.ReadDateTime();

							m_DoneQuests.Add( new QuestRestartInfo( questType, restartTime ) );
						}
					}

					m_Profession = reader.ReadEncodedInt();
					goto case 15;
				}
				case 15:
				{
					m_LastCompassionLoss = reader.ReadDeltaTime();
					goto case 14;
				}
				case 14:
				{
					m_CompassionGains = reader.ReadEncodedInt();

					if ( m_CompassionGains > 0 )
						m_NextCompassionDay = reader.ReadDeltaTime();

					goto case 13;
				}
				case 13: // just removed m_PayedInsurance list
				case 12:
				{
					m_BOBFilter = new Engines.BulkOrders.BOBFilter( reader );
					goto case 11;
				}
				case 11:
				{
					if ( version < 13 )
					{
						List<Item> payed = reader.ReadStrongItemList();

						for ( int i = 0; i < payed.Count; ++i )
							payed[i].PayedInsurance = true;
					}

					goto case 10;
				}
				case 10:
				{
					if ( reader.ReadBool() )
					{
						m_HairModID = reader.ReadInt();
						m_HairModHue = reader.ReadInt();
						m_BeardModID = reader.ReadInt();
						m_BeardModHue = reader.ReadInt();

						// We cannot call SetHairMods( -1, -1 ) here because the items have not yet loaded
						Timer.DelayCall( TimeSpan.Zero, new TimerCallback( RevertHair ) );
					}

					goto case 9;
				}
				case 9:
				{
					SavagePaintExpiration = reader.ReadTimeSpan();

					if ( SavagePaintExpiration > TimeSpan.Zero )
					{
						BodyMod = ( Female ? 184 : 183 );
						HueMod = 0;
					}

					goto case 8;
				}
				case 8:
				{
					m_NpcGuild = (NpcGuild)reader.ReadInt();
					m_NpcGuildJoinTime = reader.ReadDateTime();
					m_NpcGuildGameTime = reader.ReadTimeSpan();
					goto case 7;
				}
				case 7:
				{
					m_PermaFlags = reader.ReadStrongMobileList();
					goto case 6;
				}
				case 6:
				{
					NextTailorBulkOrder = reader.ReadTimeSpan();
					goto case 5;
				}
				case 5:
				{
					NextSmithBulkOrder = reader.ReadTimeSpan();
					goto case 4;
				}
				case 4:
				{
					m_LastJusticeLoss = reader.ReadDeltaTime();
					m_JusticeProtectors = reader.ReadStrongMobileList();
					goto case 3;
				}
				case 3:
				{
					m_LastSacrificeGain = reader.ReadDeltaTime();
					m_LastSacrificeLoss = reader.ReadDeltaTime();
					m_AvailableResurrects = reader.ReadInt();
					goto case 2;
				}
				case 2:
				{
					m_Flags = (PlayerFlag)reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					m_LongTermElapse = reader.ReadTimeSpan();
					m_ShortTermElapse = reader.ReadTimeSpan();
					m_GameTime = reader.ReadTimeSpan();
					goto case 0;
				}
				case 0:
				{
					break;
				}
			}

			// Professions weren't verified on 1.0 RC0
			if ( !CharacterCreation.VerifyProfession( m_Profession ) )
				m_Profession = 0;

			if ( m_PermaFlags == null )
				m_PermaFlags = new List<Mobile>();

			if ( m_JusticeProtectors == null )
				m_JusticeProtectors = new List<Mobile>();

			if ( m_BOBFilter == null )
				m_BOBFilter = new Engines.BulkOrders.BOBFilter();

			if( m_GuildRank == null )
				m_GuildRank = Guilds.RankDefinition.Member;	//Default to member if going from older verstion to new version (only time it should be null)

			if( m_LastOnline == DateTime.MinValue && Account != null )
				m_LastOnline = ((Account)Account).LastLogin;

			if( m_ChampionTitles == null )
				m_ChampionTitles = new ChampionTitleInfo();

			List<Mobile> list = this.Stabled;

			for ( int i = 0; i < list.Count; ++i )
			{
				BaseCreature bc = list[i] as BaseCreature;

				if ( bc != null )
					bc.IsStabled = true;
			}

			CheckAtrophies( this );


			if( Hidden )	//Hiding is the only buff where it has an effect that's serialized.
				AddBuff( new BuffInfo( BuffIcon.HidingAndOrStealth, 1075655 ) );
		}

        public void CheckKillDecay()
        {
            //decay our kills
			while ( m_ShortTermElapse < this.GameTime )
			{
                m_ShortTermElapse += ShortTermMurderDecay;
				if ( ShortTermMurders > 0 )
					--ShortTermMurders;
			}

			while ( m_LongTermElapse < this.GameTime )
			{
                m_LongTermElapse += LongTermMurderDecay;
				if ( Kills > 0 )
					--Kills;
			}
        }
		
		public override void Serialize( GenericWriter writer )
		{
			//cleanup our anti-macro table 
			foreach ( Hashtable t in m_AntiMacroTable.Values )
			{
				ArrayList remove = new ArrayList();
				foreach ( CountAndTimeStamp time in t.Values )
				{
					if ( time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.Now )
						remove.Add( time );
				}

				for (int i=0;i<remove.Count;++i)
					t.Remove( remove[i] );
			}

			//decay our kills
			CheckKillDecay();

			CheckAtrophies( this );

			base.Serialize( writer );
			
			writer.Write( (int) 28 ); // version

			writer.Write( (bool)m_EventBanned );

            writer.Write( (int)m_StatLoss.Count );
            foreach ( StatLossInfo sli in m_StatLoss )
                sli.Serialize( writer );

			writer.Write( (int) m_NewbKills );

			if( m_AcquiredRecipes == null )
			{
				writer.Write( (int)0 );
			}
			else
			{
				writer.Write( m_AcquiredRecipes.Count );

				foreach( KeyValuePair<int, bool> kvp in m_AcquiredRecipes )
				{
					writer.Write( kvp.Key );
					writer.Write( kvp.Value );
				}
			}

			writer.WriteDeltaTime( m_LastHonorLoss );

			ChampionTitleInfo.Serialize( writer, m_ChampionTitles );

			writer.Write( m_LastValorLoss );
			writer.WriteEncodedInt( m_ToTItemsTurnedIn );
			writer.Write( m_ToTTotalMonsterFame );	//This ain't going to be a small #.

			writer.WriteEncodedInt( m_AllianceMessageHue );
			writer.WriteEncodedInt( m_GuildMessageHue );

			writer.WriteEncodedInt( m_GuildRank.Rank );
			writer.Write( m_LastOnline );

			writer.WriteEncodedInt( (int) m_SolenFriendship );

			QuestSerializer.Serialize( m_Quest, writer );

			if ( m_DoneQuests == null )
			{
				writer.WriteEncodedInt( (int) 0 );
			}
			else
			{
				writer.WriteEncodedInt( (int) m_DoneQuests.Count );

				for ( int i = 0; i < m_DoneQuests.Count; ++i )
				{
					QuestRestartInfo restartInfo = m_DoneQuests[i];

					QuestSerializer.Write( (Type) restartInfo.QuestType, QuestSystem.QuestTypes, writer );
					writer.Write( (DateTime) restartInfo.RestartTime );
				}
			}

			writer.WriteEncodedInt( (int) m_Profession );

			writer.WriteDeltaTime( m_LastCompassionLoss );

			writer.WriteEncodedInt( m_CompassionGains );

			if ( m_CompassionGains > 0 )
				writer.WriteDeltaTime( m_NextCompassionDay );

			m_BOBFilter.Serialize( writer );

			bool useMods = ( m_HairModID != -1 || m_BeardModID != -1 );

			writer.Write( useMods );

			if ( useMods )
			{
				writer.Write( (int) m_HairModID );
				writer.Write( (int) m_HairModHue );
				writer.Write( (int) m_BeardModID );
				writer.Write( (int) m_BeardModHue );
			}

			writer.Write( SavagePaintExpiration );

			writer.Write( (int) m_NpcGuild );
			writer.Write( (DateTime) m_NpcGuildJoinTime );
			writer.Write( (TimeSpan) m_NpcGuildGameTime );

			writer.Write( m_PermaFlags, true );

			writer.Write( NextTailorBulkOrder );

			writer.Write( NextSmithBulkOrder );

			writer.WriteDeltaTime( m_LastJusticeLoss );
			writer.Write( m_JusticeProtectors, true );

			writer.WriteDeltaTime( m_LastSacrificeGain );
			writer.WriteDeltaTime( m_LastSacrificeLoss );
			writer.Write( m_AvailableResurrects );

			writer.Write( (int) m_Flags );

			writer.Write( m_LongTermElapse );
			writer.Write( m_ShortTermElapse );
			writer.Write( this.GameTime );
		}



		public static void CheckAtrophies( Mobile m )
		{
			SacrificeVirtue.CheckAtrophy( m );
			JusticeVirtue.CheckAtrophy( m );
			CompassionVirtue.CheckAtrophy( m );
			ValorVirtue.CheckAtrophy( m );
			HonorVirtue.CheckAtrophy( m );

			if( m is PlayerMobile )
				ChampionTitleInfo.CheckAtrophy( (PlayerMobile)m );
		}

        public static readonly TimeSpan ShortTermMurderDecay = TimeSpan.FromHours( 8.0 );
        public static readonly TimeSpan LongTermMurderDecay = TimeSpan.FromHours( 40.0 );

		public void ResetKillTime()
		{
			m_ShortTermElapse = this.GameTime + ShortTermMurderDecay;
			m_LongTermElapse = this.GameTime + LongTermMurderDecay;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime SessionStart
		{
			get{ return m_SessionStart; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan GameTime
		{
			get
			{
				if ( NetState != null )
					return m_GameTime + (DateTime.Now - m_SessionStart);
				else
					return m_GameTime;
			}
            set
            {
                m_GameTime = value;
            }
		}

        public override void OnWarmodeChanged()
        {
            base.OnWarmodeChanged();

            if (!Alive)
            {
                IPooledEnumerable eable = Map.GetMobilesInRange(Location, Core.GlobalMaxUpdateRange);

                foreach (Mobile m in eable)
                {
                    if (Warmode)
                    {
                        // find all the living people we can see in range and send them
                        if (m.Alive && CanSee(m) && Utility.InUpdateRange(Location, m.Location))
                            Send(new MobileIncoming(this, m));
                    }
                    else
                    {
                        // find all the living people we can't see and remove them
                        if (m.Alive && !CanSee(m))
                            Send(m.RemovePacket);
                    }
                }

                eable.Free();
            }
        }

		public override bool CanSee( Mobile m )
		{
            PlayerMobile pmTarg = m as PlayerMobile;
            if (pmTarg != null)
            {
                if (pmTarg.m_VisList.Contains(this))
                    return true;

                // if you are in a duel (  and it has started and you have not been eliminated ) and you are looking at someone NOT in a duel who is not staff, you can't see them.
                if (pmTarg.DuelContext == null && 
                    pmTarg.AccessLevel == AccessLevel.Player &&
                    DuelContext != null && 
                    (DuelContext.StartedBeginCountdown || DuelContext.StartedReadyCountdown || DuelContext.Started) &&
                    !DuelContext.Finished && DuelPlayer != null && !DuelPlayer.Eliminated)
                {
                    return false;
                }
            }

            // if we are dead and they are not and we are not in warmode we cant see them (unless they are a healer or staff)
            if ( !Alive && m.Alive && !Warmode && !(m is BaseHealer) && !(m.AccessLevel > AccessLevel.Player) )
                return false;

            // If we can hear ghosts and they are a ghost and we have access and are on the same map, we can see them
            if (CanHearGhosts && !m.Alive && AccessLevel >= m.AccessLevel && Map == m.Map)
                return true;

			return base.CanSee( m );
		}

		public override bool CanSee( Item item )
		{
			if ( m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer( item ) )
				return false;

			return base.CanSee( item );
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Faction faction = Faction.Find( this );

			if ( faction != null )
				faction.RemoveMember( this );

			BaseHouse.HandleDeletion( this );
		}

		public override bool NewGuildDisplay { get { return Server.Guilds.Guild.NewGuildSystem; } }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( Map == Faction.Facet )
			{
				PlayerState pl = PlayerState.Find( this );

				if ( pl != null )
				{
					Faction faction = pl.Faction;

					if ( faction.Commander == this )
						list.Add( 1042733, faction.Definition.PropName ); // Commanding Lord of the ~1_FACTION_NAME~
					else if ( pl.Sheriff != null )
						list.Add( 1042734, "{0}\t{1}", pl.Sheriff.Definition.FriendlyName, faction.Definition.PropName ); // The Sheriff of  ~1_CITY~, ~2_FACTION_NAME~
					else if ( pl.Finance != null )
						list.Add( 1042735, "{0}\t{1}", pl.Finance.Definition.FriendlyName, faction.Definition.PropName ); // The Finance Minister of ~1_CITY~, ~2_FACTION_NAME~
					else if ( pl.MerchantTitle != MerchantTitle.None )
						list.Add( 1060776, "{0}\t{1}", MerchantTitles.GetInfo( pl.MerchantTitle ).Title, faction.Definition.PropName ); // ~1_val~, ~2_val~
					else
						list.Add( 1060776, "{0}\t{1}", pl.Rank.Title, faction.Definition.PropName ); // ~1_val~, ~2_val~
				}
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Map == Faction.Facet )
			{
				PlayerState pl = PlayerState.Find( this );

				if ( pl != null )
				{
					string text;
					bool ascii = false;

					Faction faction = pl.Faction;

					if ( faction.Commander == this )
						text = String.Concat( this.Female ? "(Commanding Lady of the " : "(Commanding Lord of the ", faction.Definition.FriendlyName, ")" );
					else if ( pl.Sheriff != null )
						text = String.Concat( "(The Sheriff of ", pl.Sheriff.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")" );
					else if ( pl.Finance != null )
						text = String.Concat( "(The Finance Minister of ", pl.Finance.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")" );
					else
					{
						ascii = true;

						if ( pl.MerchantTitle != MerchantTitle.None )
							text = String.Concat( "(", MerchantTitles.GetInfo( pl.MerchantTitle ).Title.String, ", ", faction.Definition.FriendlyName, ")" );
						else
							text = String.Concat( "(", pl.Rank.Title.String, ", ", faction.Definition.FriendlyName, ")" );
					}

					int hue = ( Faction.Find( from ) == faction ? 98 : 38 );

					PrivateOverheadMessage( MessageType.Label, hue, ascii, text, from.NetState );
				}
			}

			base.OnSingleClick( from );
		}

		protected override bool OnMove( Direction d )
		{
			if( !Core.SE )
				return base.OnMove( d );

			if( AccessLevel != AccessLevel.Player )
				return true;

			if( Hidden && DesignContext.Find( this ) == null )	//Hidden & NOT customizing a house
			{
				if( !Mounted && Skills.Stealth.Value >= 25.0 )
				{
					bool running = (d & Direction.Running) != 0;

					if( running )
					{
						if( (AllowedStealthSteps -= 2) <= 0 )
							RevealingAction();
					}
					else if( AllowedStealthSteps-- <= 0 )
					{
						Server.SkillHandlers.Stealth.OnUse( this );
					}			
				}
				else
				{
					RevealingAction();
				}
			}

			return true;
		}

		private bool m_BedrollLogout;

		public bool BedrollLogout
		{
			get{ return m_BedrollLogout; }
			set{ m_BedrollLogout = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override bool Paralyzed
		{
			get
			{
				return base.Paralyzed;
			}
			set
			{
				base.Paralyzed = value;

				if( value )
					AddBuff( new BuffInfo( BuffIcon.Paralyze, 1075827 ) );	//Paralyze/You are frozen and can not move
				else
					RemoveBuff( BuffIcon.Paralyze );
			}
		}

		#region Ethics
		private Ethics.Player m_EthicPlayer;

		[CommandProperty( AccessLevel.GameMaster )]
		public Ethics.Player EthicPlayer
		{
			get { return m_EthicPlayer; }
			set { m_EthicPlayer = value; }
		}
		#endregion

		#region Factions
		private PlayerState m_FactionPlayerState;

		public PlayerState FactionPlayerState
		{
			get{ return m_FactionPlayerState; }
			set{ m_FactionPlayerState = value; }
		}
		#endregion

        #region Dueling
        private Engines.ConPVP.DuelContext m_DuelContext;
        private Engines.ConPVP.DuelPlayer m_DuelPlayer;

        public Engines.ConPVP.DuelContext DuelContext
        {
            get { return m_DuelContext; }
        }

        public Engines.ConPVP.DuelPlayer DuelPlayer
        {
            get { return m_DuelPlayer; }
            set
            {
                bool wasInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

                m_DuelPlayer = value;

                if (m_DuelPlayer == null)
                    m_DuelContext = null;
                else
                    m_DuelContext = m_DuelPlayer.Participant.Context;

                bool isInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

                if (wasInTourny != isInTourny)
                    SendEverything();
            }
        }
        #endregion

		#region Quests
		private QuestSystem m_Quest;
		private List<QuestRestartInfo> m_DoneQuests;
		private SolenFriendship m_SolenFriendship;

		public QuestSystem Quest
		{
			get{ return m_Quest; }
			set{ m_Quest = value; }
		}

		public List<QuestRestartInfo> DoneQuests
		{
			get{ return m_DoneQuests; }
			set{ m_DoneQuests = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SolenFriendship SolenFriendship
		{
			get{ return m_SolenFriendship; }
			set{ m_SolenFriendship = value; }
		}
		#endregion

		#region MyRunUO Invalidation
		private bool m_ChangedMyRunUO;

		public bool ChangedMyRunUO
		{
			get{ return m_ChangedMyRunUO; }
			set{ m_ChangedMyRunUO = value; }
		}

		public void InvalidateMyRunUO()
		{
			if ( !Deleted && !m_ChangedMyRunUO )
			{
				m_ChangedMyRunUO = true;
				Engines.MyRunUO.MyRunUO.QueueMobileUpdate( this );
			}
		}

		public override void OnKillsChange( int oldValue )
		{
			if ( this.Young && this.Kills > oldValue )
			{
				Account acc = this.Account as Account;

				if ( acc != null )
					acc.RemoveYoungStatus( 0 );
			}

            if (Kills > oldValue && Kills >= 5)
            {
                // becoming a murderer

                if (Guild != null && Guild is Guilds.Guild && Guild.Type != Server.Guilds.GuildType.Regular)
                {
                    Guilds.Guild g = (Guilds.Guild)Guild;

                    if (g.Leader == this)
                    {
                        this.SendAsciiMessage(0x25, "Your guild's type has changed because you are now a murderer.");
                        g.Type = Guilds.GuildType.Regular;
                        g.GuildMessage(1018022, true, "Regular"); // Guild Message: Your guild type has changed:
                    }
                    else
                    {
                        this.SendAsciiMessage(0x25, "You have been removed from your virtue guild because you are now a murderer.");
                        g.RemoveMember(this);
                    }
                }
            }

			InvalidateMyRunUO();
		}

		public override void OnGenderChanged( bool oldFemale )
		{
			InvalidateMyRunUO();
		}

		public override void OnGuildChange( Server.Guilds.BaseGuild oldGuild )
		{
			InvalidateMyRunUO();
		}

		public override void OnGuildTitleChange( string oldTitle )
		{
			InvalidateMyRunUO();
		}

		public override void OnKarmaChange( int oldValue )
		{
			InvalidateMyRunUO();
		}

		public override void OnFameChange( int oldValue )
		{
			InvalidateMyRunUO();
		}

		public override void OnSkillChange( SkillName skill, double oldBase )
		{
			if ( this.Young && this.SkillsTotal >= 4500 )
			{
				Account acc = this.Account as Account;

				if ( acc != null )
					acc.RemoveYoungStatus( 1019036 ); // You have successfully obtained a respectable skill level, and have outgrown your status as a young player!
			}

			InvalidateMyRunUO();
		}

		public override void OnAccessLevelChanged( AccessLevel oldLevel )
		{
			InvalidateMyRunUO();
		}

		public override void OnRawStatChange( StatType stat, int oldValue )
		{
			InvalidateMyRunUO();
		}

		public override void OnDelete()
		{
			if ( m_ReceivedHonorContext != null )
				m_ReceivedHonorContext.Cancel();
			if ( m_SentHonorContext != null )
				m_SentHonorContext.Cancel();

			InvalidateMyRunUO();
		}
		#endregion

		#region Fastwalk Prevention
		private static bool FastwalkPrevention = true; // Is fastwalk prevention enabled?
		private static TimeSpan FastwalkThreshold = TimeSpan.FromSeconds( 0.1 ); 

		private DateTime m_NextMovementTime;

		public virtual bool UsesFastwalkPrevention{ get{ return ( AccessLevel < AccessLevel.Counselor ); } }

		public override TimeSpan ComputeMovementSpeed( Direction dir, bool checkTurning )
		{
			if ( checkTurning && (dir & Direction.Mask) != (this.Direction & Direction.Mask) )
                return Mobile.RunMount;	// We are NOT actually moving (just a direction change)

			TransformContext context = TransformationSpellHelper.GetContext( this );

			if( context != null && context.Type == typeof( ReaperFormSpell ) )
				return Mobile.WalkFoot;

			bool running = ( (dir & Direction.Running) != 0 );

			bool onHorse = ( this.Mount != null );

			AnimalFormContext animalContext = AnimalForm.GetContext( this );

			if( onHorse || (animalContext != null && animalContext.SpeedBoost) )
				return ( running ? Mobile.RunMount : Mobile.WalkMount );

			return ( running ? Mobile.RunFoot : Mobile.WalkFoot );
		}

		public static bool MovementThrottle_Callback( NetState ns )
		{
			PlayerMobile pm = ns.Mobile as PlayerMobile;

			if ( pm == null || !pm.UsesFastwalkPrevention )
				return true;

			if ( pm.m_NextMovementTime == DateTime.MinValue )
			{
				// has not yet moved
				pm.m_NextMovementTime = DateTime.Now;
				return true;
			}

			TimeSpan ts = pm.m_NextMovementTime - DateTime.Now;

			if ( ts <= TimeSpan.Zero )
			{
				// been a while since we've last moved
				pm.m_NextMovementTime = DateTime.Now;
				return true;
			}

			return ( ts <= FastwalkThreshold );
		}
		#endregion

		#region Enemy of One
		private Type m_EnemyOfOneType;
		private bool m_WaitingForEnemy;

		public Type EnemyOfOneType
		{
			get{ return m_EnemyOfOneType; }
			set
			{
				Type oldType = m_EnemyOfOneType;
				Type newType = value;

				if ( oldType == newType )
					return;

				m_EnemyOfOneType = value;

				DeltaEnemies( oldType, newType );
			}
		}

		public bool WaitingForEnemy
		{
			get{ return m_WaitingForEnemy; }
			set{ m_WaitingForEnemy = value; }
		}

		private void DeltaEnemies( Type oldType, Type newType )
		{
			foreach ( Mobile m in this.GetMobilesInRange( 18 ) )
			{
				Type t = m.GetType();

				if ( t == oldType || t == newType ) {
					NetState ns = this.NetState;

					if ( ns != null ) {
						if ( ns.IsPost7000 ) {
							ns.Send( new MobileMoving( m, Notoriety.Compute( this, m ) ) );
						} else {
							ns.Send( new MobileMovingOld( m, Notoriety.Compute( this, m ) ) );
						}
					}
				}
			}
		}
		#endregion

		#region Hair and beard mods
		private int m_HairModID = -1, m_HairModHue;
		private int m_BeardModID = -1, m_BeardModHue;

		public void SetHairMods( int hairID, int beardID )
		{
			if ( hairID == -1 )
				InternalRestoreHair( true, ref m_HairModID, ref m_HairModHue );
			else if ( hairID != -2 )
				InternalChangeHair( true, hairID, ref m_HairModID, ref m_HairModHue );

			if ( beardID == -1 )
				InternalRestoreHair( false, ref m_BeardModID, ref m_BeardModHue );
			else if ( beardID != -2 )
				InternalChangeHair( false, beardID, ref m_BeardModID, ref m_BeardModHue );
		}

		private void CreateHair( bool hair, int id, int hue )
		{
			if( hair )
			{
				//TODO Verification?
				HairItemID = id;
				HairHue = hue;
			}
			else
			{
				FacialHairItemID = id;
				FacialHairHue = hue;
			}
		}

		private void InternalRestoreHair( bool hair, ref int id, ref int hue )
		{
			if ( id == -1 )
				return;

			if ( hair )
				HairItemID = 0;
			else
				FacialHairItemID = 0;

			//if( id != 0 )
			CreateHair( hair, id, hue );

			id = -1;
			hue = 0;
		}

		private void InternalChangeHair( bool hair, int id, ref int storeID, ref int storeHue )
		{
			if ( storeID == -1 )
			{
				storeID = hair ? HairItemID : FacialHairItemID;
				storeHue = hair ? HairHue : FacialHairHue;
			}
			CreateHair( hair, id, 0 );
		}
		#endregion

		#region Virtues
		private DateTime m_LastSacrificeGain;
		private DateTime m_LastSacrificeLoss;
		private int m_AvailableResurrects;

		public DateTime LastSacrificeGain{ get{ return m_LastSacrificeGain; } set{ m_LastSacrificeGain = value; } }
		public DateTime LastSacrificeLoss{ get{ return m_LastSacrificeLoss; } set{ m_LastSacrificeLoss = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AvailableResurrects{ get{ return m_AvailableResurrects; } set{ m_AvailableResurrects = value; } }

		private DateTime m_NextJustAward;
		private DateTime m_LastJusticeLoss;
		private List<Mobile> m_JusticeProtectors;

		public DateTime LastJusticeLoss{ get{ return m_LastJusticeLoss; } set{ m_LastJusticeLoss = value; } }
		public List<Mobile> JusticeProtectors { get { return new List<Mobile>(); } set { m_JusticeProtectors = value; } }

		private DateTime m_LastCompassionLoss;
		private DateTime m_NextCompassionDay;
		private int m_CompassionGains;

		public DateTime LastCompassionLoss{ get{ return m_LastCompassionLoss; } set{ m_LastCompassionLoss = value; } }
		public DateTime NextCompassionDay{ get{ return m_NextCompassionDay; } set{ m_NextCompassionDay = value; } }
		public int CompassionGains{ get{ return 0; } set{ m_CompassionGains = value; } }

		private DateTime m_LastValorLoss;

		public DateTime LastValorLoss { get { return m_LastValorLoss; } set { m_LastValorLoss = value; } }

		private DateTime m_LastHonorLoss;
		private DateTime m_LastHonorUse;
		private bool m_HonorActive;
		private HonorContext m_ReceivedHonorContext;
		private HonorContext m_SentHonorContext;

		public DateTime LastHonorLoss{ get{ return m_LastHonorLoss; } set{ m_LastHonorLoss = value; } }
		public DateTime LastHonorUse{ get{ return m_LastHonorUse; } set{ m_LastHonorUse = value; } }
		public bool HonorActive{ get{ return m_HonorActive; } set{ m_HonorActive = value; } }
		public HonorContext ReceivedHonorContext{ get{ return m_ReceivedHonorContext; } set{ m_ReceivedHonorContext = value; } }
		public HonorContext SentHonorContext{ get{ return m_SentHonorContext; } set{ m_SentHonorContext = value; } }
		#endregion

		#region Young system
		private int m_NewbKills;

		[CommandProperty( AccessLevel.Counselor, AccessLevel.Developer )]
		public int NewbKills
		{
			get { return m_NewbKills; }
			set { m_NewbKills = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Young
		{
            get { return false;/* GetFlag(PlayerFlag.Young); */ }
			set{ /*SetFlag( PlayerFlag.Young, value ); InvalidateProperties();*/ }
		}

		public override TimeSpan GetLogoutDelay()
		{
            if ( Criminal && AccessLevel == AccessLevel.Player )
                return Region.DefaultLogoutDelay;

			if ( Young || BedrollLogout /*|| TestCenter.Enabled*/ )
				return TimeSpan.Zero;

			return base.GetLogoutDelay();
		}

		public bool CheckYoungProtection( Mobile from )
		{
            return false;
		}

		public bool CheckYoungHealTime()
		{
			return false;
		}

		public bool YoungDeathTeleport()
		{
            return false;
		}

		private void SendYoungDeathNotice()
		{
			//this.SendGump( new YoungDeathNotice() );
		}
		#endregion

		#region Speech log
		private SpeechLog m_SpeechLog;

		public SpeechLog SpeechLog{ get{ return m_SpeechLog; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( SpeechLog.Enabled && this.NetState != null )
			{
				if ( m_SpeechLog == null )
					m_SpeechLog = new SpeechLog();

				m_SpeechLog.Add( e.Mobile, e.Speech );
			}
		}
		#endregion

        #region Speech while hidden
        private class SpeechItem : Item
        {
            private static Dictionary<Mobile, SpeechItem> m_Table = new Dictionary<Mobile, SpeechItem>();

            public static SpeechItem Get( Mobile m )
            {
                SpeechItem si = null;
                if (!m_Table.ContainsKey(m))
                {
                    m_Table.Add(m, si = new SpeechItem(m));
                }
                else
                {
                    si = m_Table[m];
                    si.ResetDecay();
                }
                si.MoveToWorld( m.Location, m.Map );
                si.ProcessDelta();
                return si;
            }

            private DecayTimer m_DecayTimer;
            private Mobile m_Owner;

            private SpeechItem(Mobile m)
                : base(0x2198) // no draw tile
            {
                Movable = false;

                m_Owner = m;

                m_DecayTimer = new DecayTimer(this);
                m_DecayTimer.Start();
            }

            public SpeechItem(Serial s)
                : base(s)
            {
                // decay immediately on server up
                m_DecayTimer = new DecayTimer(this);
                m_DecayTimer.Delay = TimeSpan.Zero;
                m_DecayTimer.Start();
            }

            public void ResetDecay()
            {
                m_DecayTimer.Stop();
                m_DecayTimer.Start();
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

            public override void OnSingleClick(Mobile from)
            {
            }

            private class DecayTimer : Timer
            {
                private SpeechItem m_SI;
                public DecayTimer(SpeechItem si)
                    : base(TimeSpan.FromSeconds(30.0))
                {
                    m_SI = si;
                }

                protected override void OnTick()
                {
                    try
                    {
                        if (m_SI.m_Owner != null)
                            m_Table.Remove(m_SI.m_Owner);

                        m_SI.Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        public override void OnSaid(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if ( from.Hidden && from.Alive && from.AccessLevel == AccessLevel.Player )
            {
                e.Handled = true;
                e.Blocked = true;

                int range = 15;

                switch ( e.Type )
                {
                    case MessageType.Whisper:
					    range = 1;
					    break;
				    case MessageType.Yell:
					    range = 18;
                        break;
                }

                if (from.Map != null)
                {
                    List<Mobile> hears = new List<Mobile>();
                    IPooledEnumerable eable = from.Map.GetMobilesInRange(from.Location, range);

                    foreach (Mobile heard in eable)
                    {
                        if (Mobile.NoSpeechLOS || heard == this || !heard.Player || heard.InLOS(this))
                        {
                            if (heard.NetState != null)
                                hears.Add(heard);
                        }
                    }

                    eable.Free();

                    object mutateContext = null;
                    string mutatedText = e.Speech;
                    SpeechEventArgs mutatedArgs = null;

                    if (MutateSpeech(hears, ref mutatedText, ref mutateContext))
                        mutatedArgs = new SpeechEventArgs(this, mutatedText, e.Type, e.Hue, new int[0]);

                    SpeechItem si = null;
                    Packet regp = null, regpSI = null;
                    Packet mutp = null, mutpSI = null;

                    for (int i = 0; i < hears.Count; ++i)
                    {
                        Mobile heard = hears[i];

                        if (mutatedArgs == null || !CheckHearsMutatedSpeech(heard, mutateContext))
                        {
                            heard.OnSpeech(e);

                            NetState ns = heard.NetState;

                            if (ns != null)
                            {
                                if (this != heard && !heard.CanSee(this))
                                {
                                    if (regpSI == null)
                                    {
                                        if (si == null)
                                            si = SpeechItem.Get(this);
                                        regpSI = Packet.Acquire(new AsciiMessage(si.Serial, si.ItemID, e.Type, e.Hue, 3, Name, e.Speech));
                                    }
                                    ns.Send(regpSI);
                                }
                                else
                                {
                                    if (regp == null)
                                        regp = Packet.Acquire(new UnicodeMessage(Serial, Body, e.Type, e.Hue, 3, Language, Name, e.Speech));

                                    ns.Send(regp);
                                }
                            }
                        }
                        else
                        {
                            heard.OnSpeech(mutatedArgs);

                            NetState ns = heard.NetState;

                            if (ns != null)
                            {
                                if (this != heard && !heard.CanSee(this))
                                {
                                    if (mutpSI == null)
                                    {
                                        if (si == null)
                                            si = SpeechItem.Get(this);

                                        mutpSI = Packet.Acquire(new AsciiMessage(si.Serial, si.ItemID, e.Type, e.Hue, 3, Name, mutatedText));
                                    }
                                    ns.Send(mutpSI);
                                }
                                else
                                {
                                    if (mutp == null)
                                        mutp = Packet.Acquire(new UnicodeMessage(Serial, Body, e.Type, e.Hue, 3, Language, Name, mutatedText));

                                    ns.Send(mutp);
                                }
                            }
                        }
                    }

                    Packet.Release(regp);
                    Packet.Release(regpSI);
                    Packet.Release(mutp);
                    Packet.Release(mutpSI);
                }
            }
            else
            {
                base.OnSaid(e);
            }
        }
        #endregion

        #region Champion Titles
        [CommandProperty( AccessLevel.GameMaster )]
		public bool DisplayChampionTitle
		{
			get { return GetFlag( PlayerFlag.DisplayChampionTitle ); }
			set { SetFlag( PlayerFlag.DisplayChampionTitle, value ); }
		}

		private ChampionTitleInfo m_ChampionTitles;

		[CommandProperty( AccessLevel.GameMaster )]
		public ChampionTitleInfo ChampionTitles { get { return m_ChampionTitles; } set { } }

		private void ToggleChampionTitleDisplay()
		{
			if( !CheckAlive() )
				return;

			if( DisplayChampionTitle )
				SendLocalizedMessage( 1062419, "", 0x23 ); // You have chosen to hide your monster kill title.
			else
				SendLocalizedMessage( 1062418, "", 0x23 ); // You have chosen to display your monster kill title.

			DisplayChampionTitle = !DisplayChampionTitle;
		}

		[PropertyObject]
		public class ChampionTitleInfo
		{
			public static TimeSpan LossDelay = TimeSpan.FromDays( 1.0 );
			public const int LossAmount = 90;

			private class TitleInfo
			{
				private int m_Value;
				private DateTime m_LastDecay;

				public int Value { get { return m_Value; } set { m_Value = value; } }
				public DateTime LastDecay { get { return m_LastDecay; } set { m_LastDecay = value; } }

				public TitleInfo()
				{
				}

				public TitleInfo( GenericReader reader )
				{
					int version = reader.ReadEncodedInt();

					switch( version )
					{
						case 0:
						{
							m_Value = reader.ReadEncodedInt();
							m_LastDecay = reader.ReadDateTime();
							break;
						}
					}
				}

				public static void Serialize( GenericWriter writer, TitleInfo info )
				{
					writer.WriteEncodedInt( (int)0 ); // version

					writer.WriteEncodedInt( info.m_Value );
					writer.Write( info.m_LastDecay );
				}

			}
			private TitleInfo[] m_Values;

			private int m_Harrower;	//Harrower titles do NOT decay


			public int GetValue( ChampionSpawnType type )
			{
				return GetValue( (int)type );
			}

			public void SetValue( ChampionSpawnType type, int value )
			{
				SetValue( (int)type, value );
			}

			public void Award( ChampionSpawnType type, int value )
			{
				Award( (int)type, value );
			}

			public int GetValue( int index )
			{
				if( m_Values == null || index < 0 || index >= m_Values.Length )
					return 0;

				if( m_Values[index] == null )
					m_Values[index] = new TitleInfo();

				return m_Values[index].Value;
			}

			public DateTime GetLastDecay( int index )
			{
				if( m_Values == null || index < 0 || index >= m_Values.Length )
					return DateTime.MinValue;

				if( m_Values[index] == null )
					m_Values[index] = new TitleInfo();

				return m_Values[index].LastDecay;
			}

			public void SetValue( int index, int value )
			{
				if( m_Values == null )
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

				if( value < 0 )
					value = 0;

				if( index < 0 || index >= m_Values.Length )
					return;

				if( m_Values[index] == null )
					m_Values[index] = new TitleInfo();

				m_Values[index].Value = value;
			}

			public void Award( int index, int value )
			{
				if( m_Values == null )
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

				if( index < 0 || index >= m_Values.Length || value <= 0 )
					return;

				if( m_Values[index] == null )
					m_Values[index] = new TitleInfo();

				m_Values[index].Value += value;
			}

			public void Atrophy( int index, int value )
			{
				if( m_Values == null )
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

				if( index < 0 || index >= m_Values.Length || value <= 0 )
					return;

				if( m_Values[index] == null )
					m_Values[index] = new TitleInfo();

				int before = m_Values[index].Value;

				if( (m_Values[index].Value - value) < 0 )
					m_Values[index].Value = 0;
				else
					m_Values[index].Value -= value;

				if( before != m_Values[index].Value )
					m_Values[index].LastDecay = DateTime.Now;
			}

			public override string ToString()
			{
				return "...";
			}

			[CommandProperty( AccessLevel.GameMaster )]
			public int Abyss { get { return GetValue( ChampionSpawnType.Abyss ); } set { SetValue( ChampionSpawnType.Abyss, value ); } }

			[CommandProperty( AccessLevel.GameMaster )]
			public int Arachnid { get { return GetValue( ChampionSpawnType.Arachnid ); } set { SetValue( ChampionSpawnType.Arachnid, value ); } }

			[CommandProperty( AccessLevel.GameMaster )]
			public int ColdBlood { get { return GetValue( ChampionSpawnType.ColdBlood ); } set { SetValue( ChampionSpawnType.ColdBlood, value ); } }

			[CommandProperty( AccessLevel.GameMaster )]
			public int ForestLord { get { return GetValue( ChampionSpawnType.ForestLord ); } set { SetValue( ChampionSpawnType.ForestLord, value ); } }

			[CommandProperty( AccessLevel.GameMaster )]
			public int SleepingDragon { get { return GetValue( ChampionSpawnType.SleepingDragon ); } set { SetValue( ChampionSpawnType.SleepingDragon, value ); } }

			[CommandProperty( AccessLevel.GameMaster )]
			public int UnholyTerror { get { return GetValue( ChampionSpawnType.UnholyTerror ); } set { SetValue( ChampionSpawnType.UnholyTerror, value ); } }

			[CommandProperty( AccessLevel.GameMaster )]
			public int VerminHorde { get { return GetValue( ChampionSpawnType.VerminHorde ); } set { SetValue( ChampionSpawnType.VerminHorde, value ); } }
			
			[CommandProperty( AccessLevel.GameMaster )]
			public int Harrower { get { return m_Harrower; } set { m_Harrower = value; } }

			public ChampionTitleInfo()
			{
			}

			public ChampionTitleInfo( GenericReader reader )
			{
				int version = reader.ReadEncodedInt();

				switch( version )
				{
					case 0:
					{
						m_Harrower = reader.ReadEncodedInt();

						int length = reader.ReadEncodedInt();
						m_Values = new TitleInfo[length];

						for( int i = 0; i < length; i++ )
						{
							m_Values[i] = new TitleInfo( reader );
						}

						if( m_Values.Length != ChampionSpawnInfo.Table.Length )
						{
							TitleInfo[] oldValues = m_Values;
							m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

							for( int i = 0; i < m_Values.Length && i < oldValues.Length; i++ )
							{
								m_Values[i] = oldValues[i];
							}
						}
						break;
					}
				}
			}

			public static void Serialize( GenericWriter writer, ChampionTitleInfo titles )
			{
				writer.WriteEncodedInt( (int)0 ); // version

				writer.WriteEncodedInt( titles.m_Harrower );

				int length = titles.m_Values.Length;
				writer.WriteEncodedInt( length );

				for( int i = 0; i < length; i++ )
				{
					if( titles.m_Values[i] == null )
						titles.m_Values[i] = new TitleInfo();

					TitleInfo.Serialize( writer, titles.m_Values[i] );
				}
			}

			public static void CheckAtrophy( PlayerMobile pm )
			{
				ChampionTitleInfo t = pm.m_ChampionTitles;
				if( t == null )
					return;

				if( t.m_Values == null )
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

				for( int i = 0; i < t.m_Values.Length; i++ )
				{
					if( (t.GetLastDecay( i ) + LossDelay) < DateTime.Now )
					{
						t.Atrophy( i, LossAmount );
					}
				}
			}

			public static void AwardHarrowerTitle( PlayerMobile pm )	//Called when killing a harrower.  Will give a minimum of 1 point.
			{
				ChampionTitleInfo t = pm.m_ChampionTitles;
				if( t == null )
					return;

				if( t.m_Values == null )
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

				int count = 1;

				for( int i = 0; i < t.m_Values.Length; i++ )
				{
					if( t.m_Values[i].Value > 900 )
						count++;
				}

				t.m_Harrower = Math.Max( count, t.m_Harrower );	//Harrower titles never decay.
			}
		}
		#endregion

		#region Recipes

		private Dictionary<int, bool> m_AcquiredRecipes;
		
		public virtual bool HasRecipe( Recipe r )
		{
			if( r == null ) 
				return false;

			return HasRecipe( r.ID );
		}

		public virtual bool HasRecipe( int recipeID )
		{
			if( m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey( recipeID ) )
				return m_AcquiredRecipes[recipeID];

			return false;
		}

		public virtual void AcquireRecipe( Recipe r )
		{
			if( r != null )
				AcquireRecipe( r.ID );
		}

		public virtual void AcquireRecipe( int recipeID )
		{
			if( m_AcquiredRecipes == null )
				m_AcquiredRecipes = new Dictionary<int, bool>();

			m_AcquiredRecipes[recipeID] = true;
		}

		public virtual void ResetRecipes()
		{
			m_AcquiredRecipes = null;
		}
	
		[CommandProperty( AccessLevel.GameMaster )]
		public int KnownRecipes
		{
			get 
			{
				if( m_AcquiredRecipes == null )
					return 0;

				return m_AcquiredRecipes.Count;
			}
		}
	

		#endregion

		#region Buff Icons

		public void ResendBuffs()
		{
			if( !BuffInfo.Enabled || m_BuffTable == null )
				return;

			NetState state = this.NetState;

			if( state != null && state.Version >= BuffInfo.RequiredClient )
			{
				foreach( BuffInfo info in m_BuffTable.Values )
				{
					state.Send( new AddBuffPacket( this, info ) );
				}
			}
		}

		private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

		public void AddBuff( BuffInfo b )
		{
			if( !BuffInfo.Enabled || b == null )
				return;

			RemoveBuff( b );	//Check & subsequently remove the old one.

			if( m_BuffTable == null )
				m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();

			m_BuffTable.Add( b.ID, b );

			NetState state = this.NetState;

			if( state != null && state.Version >= BuffInfo.RequiredClient )
			{
				state.Send( new AddBuffPacket( this, b ) );
			}
		}

		public void RemoveBuff( BuffInfo b )
		{
			if( b == null )
				return;

			RemoveBuff( b.ID );
		}

		public void RemoveBuff( BuffIcon b )
		{
			if( m_BuffTable == null || !m_BuffTable.ContainsKey( b ) )
				return;

			BuffInfo info = m_BuffTable[b];

			if( info.Timer != null && info.Timer.Running )
				info.Timer.Stop();

			m_BuffTable.Remove( b );

			NetState state = this.NetState;

			if( state != null && state.Version >= BuffInfo.RequiredClient )
			{
				state.Send( new RemoveBuffPacket( this, b ) );
			}

			if( m_BuffTable.Count <= 0 )
				m_BuffTable = null;
		}
		#endregion

        #region Temp Stat loss for reds
        private List<StatLossInfo> m_StatLoss = new List<StatLossInfo>();

        public List<StatLossInfo> TempStatLoss { get { return m_StatLoss; } }

        public class StatLossInfo
        {
            private PlayerMobile _Mob;
            private double _Perc;
            private TimeSpan _Duration;

            private Timer _Timer;
            private DateTime _Started;
            private List<SkillMod> _SkillMods = new List<SkillMod>();

            private static TimerStateCallback<StatLossInfo> _Callback = new TimerStateCallback<StatLossInfo>( OnTimer );

            public StatLossInfo(PlayerMobile m)
            {
                _Mob = m;

                _Started = DateTime.MinValue;
            }

            public StatLossInfo(PlayerMobile m, double percent, double minutes)
            {
                _Mob = m;
                _Perc = percent;
                _Duration = TimeSpan.FromMinutes( minutes );

                _Started = DateTime.MinValue;
            }

            public void Serialize( GenericWriter writer )
            {
                writer.Write((int)0); // version

                writer.Write(_Perc);
                writer.Write(this.Duration);
            }

            public void Deserialize(GenericReader reader)
            {
                int version = reader.ReadInt();

                _Perc = reader.ReadDouble();
                _Duration = reader.ReadTimeSpan();
            }

            public void Start()
            {
                _Mob.AddStatMod(new StatMod(StatType.Str, "TempStatLossStr", -(int)Math.Round(_Mob.RawStr * _Perc), TimeSpan.Zero));
                _Mob.AddStatMod(new StatMod(StatType.Dex, "TempStatLossDex", -(int)Math.Round(_Mob.RawDex * _Perc), TimeSpan.Zero));
                _Mob.AddStatMod(new StatMod(StatType.Int, "TempStatLossInt", -(int)Math.Round(_Mob.RawInt * _Perc), TimeSpan.Zero));

                for (int s = 0; s < (int)SkillName.Spellweaving; s++)
                {
                    SkillMod sm = new DefaultSkillMod((SkillName)s, true, -(_Mob.Skills[s].Base * _Perc));
                    _SkillMods.Add( sm );
                    _Mob.AddSkillMod( sm );
                }

                Resume();
            }

            public void Stop()
            {
                if (_Timer != null && _Timer.Running)
                    _Timer.Stop();
                _Timer = null;

                _Mob.RemoveStatMod("TempStatLossStr");
                _Mob.RemoveStatMod("TempStatLossDex");
                _Mob.RemoveStatMod("TempStatLossInt");
                foreach (SkillMod sm in _SkillMods)
                    _Mob.RemoveSkillMod(sm);
            }

            public void Resume()
            {
                if ( _Timer != null && _Timer.Running )
                    _Timer.Stop();

                _Started = DateTime.Now;

                _Timer = Timer.DelayCall<StatLossInfo>(_Duration, _Callback, this);
            }

            public void Pause()
            {
                if ( _Timer != null && _Timer.Running )
                    _Timer.Stop();
                _Timer = null;

                if (_Started != DateTime.MinValue)
                {
                    _Duration -= DateTime.Now - _Started;
                    if (_Duration.TotalSeconds < 1)
                        _Duration = TimeSpan.Zero;

                    _Started = DateTime.MinValue;
                }
            }

            private static void OnTimer( StatLossInfo info )
            {
                info.Stop();

                info._Mob.OnTempStatLossExpired( info );
            }

            public bool Running { get { return _Timer != null && _Timer.Running; } }

            public double Percent { get { return _Perc; } }

            public TimeSpan Duration
            {
                get
                {
                    TimeSpan dur = _Duration;

                    if (_Started != DateTime.MinValue)
                        dur -= DateTime.Now - _Started;

                    return dur;
                }
            }
        }

        private void OnTempStatLossExpired(StatLossInfo info)
        {
            m_StatLoss.Remove( info );

            if ( m_StatLoss.Count > 0 )
                m_StatLoss[0].Start();
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasTempStatLoss
        {
            get { return m_StatLoss.Count > 0; }
            set
            {
                if ( !value )
                {
                    if ( m_StatLoss.Count > 0 )
                        m_StatLoss[0].Stop();
                    m_StatLoss.Clear();
                }
            }
        }

        public void ConsiderSins()
        {
            CheckKillDecay();

            SendMessage("Short Term Murders : {0}", ShortTermMurders);
            SendMessage("Long Term Murders : {0}", Kills);

            foreach ( StatLossInfo sli in m_StatLoss )
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                TimeSpan dur = sli.Duration;
                bool hasDur = false;

                sb.AppendFormat( "For your sins you shall receive {0}% stat and skill loss for", sli.Percent*100.0 );
                if ( dur.Days != 0 )
                {
                    sb.AppendFormat( " {0} day{1}", dur.Days, dur.Days != 1 ? "s" : "" );
                    hasDur = true;
                }
                
                if ( dur.Hours != 0 )
                {
                    if ( hasDur )
                        sb.Append( "," );
                    sb.AppendFormat( " {0} hour{1}", dur.Hours, dur.Hours != 1 ? "s" : "" );
                    hasDur = true;
                }

                if ( dur.Minutes != 0 )
                {
                    if ( hasDur )
                    {
                        if ( dur.Seconds == 0 )
                            sb.Append( " and" );
                        else
                            sb.Append( "," );
                    }
                    sb.AppendFormat( " {0} minute{1}", dur.Minutes, dur.Minutes != 1 ? "s" : "" );
                    hasDur = true;
                }

                if ( dur.Seconds != 0 )
                {
                    if ( hasDur )
                        sb.Append( " and" );
                    sb.AppendFormat( " {0} second{1}", dur.Seconds, dur.Seconds != 1 ? "s" : "" );
                }

                sb.Append(".");

                SendMessage( 0x20, sb.ToString() );
            }
/*
            if (ShortTermMurders >= 5)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                bool hasDur = false;
                double percent = ShortTermMurders;
                if (percent > 25)
                    percent = 25;
				
				int time = (int)(ShortTermMurders / 5.0) * 30;

                TimeSpan dur = TimeSpan.FromMinutes((ShortTermMurders / 5.0 * 30) > 480 ? 480 : time );

                sb.AppendFormat("If you were to die now you would receive {0}% stat and skill loss for ", percent);
                if ( dur.Days != 0 )
                {
                    sb.AppendFormat( " {0} day{1}", dur.Days, dur.Days != 1 ? "s" : "" );
                    hasDur = true;
                }
                
                if ( dur.Hours != 0 )
                {
                    if ( hasDur )
                        sb.Append( "," );
                    sb.AppendFormat( " {0} hour{1}", dur.Hours, dur.Hours != 1 ? "s" : "" );
                    hasDur = true;
                }

                if ( dur.Minutes != 0 )
                {
                    if ( hasDur )
                    {
                        if ( dur.Seconds == 0 )
                            sb.Append( " and" );
                        else
                            sb.Append( "," );
                    }
                    sb.AppendFormat( " {0} minute{1}", dur.Minutes, dur.Minutes != 1 ? "s" : "" );
                    hasDur = true;
                }

                if ( dur.Seconds != 0 )
                {
                    if ( hasDur )
                        sb.Append( " and" );
                    sb.AppendFormat( " {0} second{1}", dur.Seconds, dur.Seconds != 1 ? "s" : "" );
                }

                sb.Append(".");

                SendMessage(sb.ToString());
            }	
*/
        }

        #endregion
    }
}


