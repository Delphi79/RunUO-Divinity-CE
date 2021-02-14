using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Commands.Generic;
using Server.Targeting;
using Server.Regions;
using Server.Guilds;

namespace Server.Battle
{
    public class PlayerCommands
    {
        public const int ChaosItemHue = 0x4AA;
        public const int OrderItemHue = 0x47E;

        public static void Initialize()
        {
            CommandSystem.Register("BattleScore", AccessLevel.Player, new CommandEventHandler(OnBattleScore));
            CommandSystem.Register("HueItem", AccessLevel.Administrator, new CommandEventHandler(OnHueItem));
        }

        [Description( "Shows the score of the current battle." )]
        public static void OnBattleScore(CommandEventArgs args)
        {
            Region baseReg = args.Mobile.Region;
            VirtueGuardedRegion reg = baseReg.GetRegion(typeof(VirtueGuardedRegion)) as VirtueGuardedRegion;

            if (reg == null || reg.Battle == null || !reg.Battle.InProgress)
            {
                args.Mobile.SendMessage("You are not currently in a battle region.");
            }
            else
            {
                int h, m, s = (int)reg.Battle.TimeLeft.TotalSeconds;
                
                h = (s / 60) / 60;
                m = (s / 60)%60;
                s = s % 60;

                args.Mobile.SendMessage("The battle will end in {0}h {1}m {2}s. Current Scores:", h,m,s );
                args.Mobile.SendMessage("Order : {0}", reg.Battle.GetActualScore(1) );
                args.Mobile.SendMessage("Chaos : {0}", reg.Battle.GetActualScore(0) );
            }
        }

        [Description("Allows you to hue items according to your virtue.")]
        public static void OnHueItem(CommandEventArgs args)
        {
            Mobile from = args.Mobile;

            if (from.Guild == null || from.Guild.Type == GuildType.Regular)
            {
                from.SendMessage("You must be in a virtue guild to use this command.");
                return;
            }

            from.SendMessage("Select the item you wish to hue.");
            from.BeginTarget(3, false, TargetFlags.None, new TargetCallback(OnHueTarget));
        }

        public static void OnHueTarget(Mobile from, object targeted)
        {
            if (from.Guild == null || from.Guild.Type == GuildType.Regular)
            {
                from.SendMessage("You must be in a virtue guild to use this command.");
                return;
            }

            if (!(targeted is BaseClothing || targeted is BaseArmor || targeted is BaseWeapon))
            {
                from.SendMessage("You may only hue clothing, weapons, and armor.");
                return;
            }

            Item item = (Item)targeted;

            if (item.Parent != from)
            {
                from.SendMessage("You must be wearing the item in order to hue it.");
                return;
            }

            if ( !(item.Hue == 0 || (item.Hue > 1 && item.Hue <= 999)) || 
                ((item is Sandals || item is Boots || item is ThighBoots || !(item is BaseClothing)) && item.Hue != 0) )
            {
                from.SendMessage( "That item already has a special hue.  You would be wise to keep it as it is." );
                return;
            }

            switch (from.Guild.Type)
            {
                case GuildType.Chaos:
                    item.Hue = ChaosItemHue;
                    break;
                case GuildType.Order:
                    item.Hue = OrderItemHue;
                    break;
            }

            from.SendMessage("The item has been hued.  It will loose this hue if you unequip it for any reason.");
        }
    }

    public class AddToBuildingCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register( new AddToBuildingCommand() );
        }

        public AddToBuildingCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Area | CommandSupport.Single;
            Commands = new string[] { "AddToBuilding" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "AddToBuilding";
            Description = "Adds targeted items to a targeted 'BattleBuilding' controller";
            ListOptimized = true;
        }

        public override void ExecuteList( CommandEventArgs e, ArrayList list )
        {
            BattleBuilding bb = null;

            foreach ( Item item in list )
            {
                if ( item is BattleBuilding )
                {
                    if ( bb != null )
                    {
                        LogFailure( "You can only select one BattleBuilding at a time!" );
                        return;
                    }

                    bb = item as BattleBuilding;
                }
            }

            if ( bb == null )
            {
                e.Mobile.SendAsciiMessage("Select the building to add the items to.");
                e.Mobile.BeginTarget(18, false, TargetFlags.None, new TargetStateCallback(OnSelectBuilding), list);
                return;
            }
            else
            {
                OnSelectBuilding(e.Mobile, bb, list);
            }
        }

        public void OnSelectBuilding(Mobile from, object target, object state)
        {
            BattleBuilding bb = target as BattleBuilding;
            ArrayList list = state as ArrayList;

            if (bb == null)
            {
                from.SendAsciiMessage(0x25, "Select the building to add the items to!");
                from.BeginTarget(18, false, TargetFlags.None, new TargetStateCallback(OnSelectBuilding), state);
                return;
            }

            int count = 0;

            foreach (Item i in list)
            {
                if (i != bb && !bb.Decorations.Contains(i))
                {
                    bb.Decorations.Add(i);
                    count++;
                }
            }

            from.SendAsciiMessage(count.ToString() + " items added to building.");
        }
    }

    public class AddToVirtueCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register( new AddToVirtueCommand() );
        }

        public AddToVirtueCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Area | CommandSupport.Single;
            Commands = new string[] { "AddToVirtue" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "AddToVirtue";
            Description = "Adds targeted items to a targeted 'BattleVirtue' controller";
            ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            BattleVirtue bv = null;

            foreach (Item item in list)
            {
                if (item is BattleVirtue)
                {
                    if (bv != null)
                    {
                        LogFailure("You can only select one BattleVirtue at a time!");
                        return;
                    }

                    bv = item as BattleVirtue;
                }
            }

            if (bv == null)
            {
                e.Mobile.SendAsciiMessage("Select the BattleVirtue to add the items to.");
                e.Mobile.BeginTarget(18, false, TargetFlags.None, new TargetStateCallback(OnSelectVirtue), list);
            }
            else
            {
                OnSelectVirtue(e.Mobile, bv, list);
            }
        }

        public void OnSelectVirtue(Mobile from, object target, object state)
        {
            BattleVirtue bv = target as BattleVirtue;
            ArrayList list = state as ArrayList;

            if (bv == null)
            {
                from.SendAsciiMessage(0x25, "Select the BattleVirtue to add the items to!");
                from.BeginTarget(18, false, TargetFlags.None, new TargetStateCallback(OnSelectVirtue), state);
                return;
            }

            int count = 0;
            foreach (Item i in list)
            {
                if (i != bv && !bv.Decorations.Contains(i))
                {
                    bv.Decorations.Add(i);
                    count++;
                }
            }

            from.SendAsciiMessage(count.ToString() + " items added to virtue.");
        }
    }
}
