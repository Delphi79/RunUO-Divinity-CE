using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Accounting;
using Server.Targeting;
using Server.Network;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Server.Misc
{
    public class SyncDetection
    {
        private static PacketHandler m_Target, m_Target6017;
        private static PacketHandler m_Equip, m_Equip6017;
        public static void Initialize()
        {
            m_Target = PacketHandlers.GetHandler(0x6C);
            m_Target6017 = PacketHandlers.Get6017Handler(0x6C);

            m_Equip = PacketHandlers.GetHandler(0x13);
            m_Equip6017 = PacketHandlers.Get6017Handler(0x13);

            if (m_Target6017 == null)
                m_Target6017 = m_Target;

            if (m_Equip6017 == null)
                m_Equip6017 = m_Equip;

            PacketHandlers.Register(0x6C, 19, true, new OnPacketReceive(TargetResponse));
            PacketHandlers.Register6017(0x6C, 19, true, new OnPacketReceive(TargetResponse6017));
            PacketHandlers.Register(0x13, 10, true, new OnPacketReceive(EquipReq));
            PacketHandlers.Register6017(0x13, 10, true, new OnPacketReceive(EquipReq6017));
        }

        private class TargetInfo
        {
            private Mobile _From, _Target;
            private Type _Type;
            private DateTime _Time;
            private List<TargetInfo> _Matches;

            public Mobile From { get { return _From; } }
            public Mobile Target { get { return _Target; } }
            public Type Type { get { return _Type; } }
            public DateTime Time { get { return _Time; } }
            public List<TargetInfo> Matches { get { return _Matches; } }

            public TargetInfo(Mobile from, Mobile target, Type type)
            {
                _From = from;
                _Target = target;
                _Type = type;
                _Time = DateTime.Now;
                _Matches = new List<TargetInfo>();
            }
        }

        private static List<TargetInfo> m_Targs = new List<TargetInfo>();
        private static readonly TimeSpan ExpireTime = TimeSpan.FromSeconds( 0.33333 );

        private static Regex IgnoreRegex = new Regex("ShepherdsCrook|Provocation|EvalInt|Anatomy",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static void HandleTarget(TargetInfo info)
        {
            bool add = true;
            int i = m_Targs.Count - 1;
            
            while (i > 0)
            {
                TargetInfo check = m_Targs[i];
                if (check.Time + ExpireTime >= DateTime.Now)
                {
                    if ( check.Target == info.Target && check.From != info.From )
                    {
                        check.Matches.Add(info);
                        add = false;
                        break;
                    }
                }
                else
                {
                    m_Targs.RemoveAt(i);

                    if ( CheckLog( check ) ) // only log 3+ people syncing
                        LogSync( check );
                }

                i--;
            }

            if ( add )
                m_Targs.Add( info );
        }

        private class EquipInfo
        {
            private Mobile _From;
            private Item _Item;
            private DateTime _Time;
            
            public Mobile From { get { return _From; } }
            public Item Item { get { return _Item; } }
            public DateTime Time { get { return _Time; } }

            public EquipInfo(Mobile from, Item item)
            {
                _From = from;
                _Item = item;
                _Time = DateTime.Now;
            }
        }

        private static List<EquipInfo> m_Equips = new List<EquipInfo>();

        private static void HandleEquip(EquipInfo info)
        {
            while (m_Equips.Count > 0)
            {
                if ((m_Equips[0].Time + (ExpireTime)) <= DateTime.Now)
                    m_Equips.RemoveAt(0);
                else
                    break;
            }

            m_Equips.Add( info );
        }

        private static bool CheckLog(TargetInfo sync)
        {
            if (sync.Matches.Count <= 1)
            {
                return false;
            }
            else if (sync.Matches.Count == 1)
            {
                if (sync.Matches[0].Time - sync.Time > TimeSpan.FromSeconds(0.15))
                    return false;

                for (int e = 0; e < m_Equips.Count; e++)
                {
                    if (m_Equips[e].Time > sync.Time)
                        break;

                    if (m_Equips[e].From == sync.From || m_Equips[e].From == sync.Matches[0].From)
                        return true;
                }

                return false;
            }
            else if (sync.Matches.Count == 2)
            {
                return sync.Matches[1].Time - sync.Time < TimeSpan.FromSeconds(0.25);
            }
            else
            {
                return true;
            }
        }

        private static void LogSync(TargetInfo sync)
        {
            EquipInfo eq = null;

            for (int e = 0; e < m_Equips.Count; e++)
            {
                if ( m_Equips[e].Time > sync.Time )
                    break;

                if (m_Equips[e].From == sync.From)
                {
                    eq = m_Equips[e];
                    m_Equips.RemoveAt(e);
                    break;
                }

                for (int i = 0; i < sync.Matches.Count; i++)
                {
                    if (sync.Matches[i].From == m_Equips[e].From)
                    {
                        eq = m_Equips[e];
                        m_Equips.RemoveAt(e);
                        break;
                    }
                }

                if (eq != null)
                    break;
            }

            try
            {
                StringBuilder msg = new StringBuilder("Sync Notice ");
                
                msg.AppendFormat( "(on '{0}'): ", sync.Target.Name);

                using (System.IO.StreamWriter w = new System.IO.StreamWriter("Syncers.log", true, System.Text.Encoding.ASCII))
                {
                    w.WriteLine("Sync on '{0}' ({1}):", sync.Target.Name, sync.Target.Account == null ? "<npc>" : sync.Target.Account.Username);

                    if (eq != null)
                    {
                        msg.AppendFormat("!'{0}' equipped {1} @ {2}! ", eq.From, eq.Item.GetType(), (eq.Time - sync.Time).TotalSeconds);
                        w.WriteLine("\t!!'{0}' equipped {1} @ {2}", eq.From, eq.Item.GetType(), (eq.Time - sync.Time).TotalSeconds);
                    }
                    
                    w.WriteLine("\t'{0}'/'{1}' ({2}) @ {3}",
                        sync.From.Name,
                        sync.From.Account == null ? "<null>" : sync.From.Account.Username,
                        sync.Type.FullName,
                        sync.Time);

                    msg.AppendFormat("'{0}' ", sync.From.Name);

                    foreach (TargetInfo info in sync.Matches)
                    {
                        w.WriteLine("\t'{0}'/'{1}' ({2}) @+{3}",
                            info.From.Name,
                            info.From.Account == null ? "<null>" : info.From.Account.Username,
                            info.Type.FullName,
                            (info.Time - sync.Time).TotalSeconds);

                        msg.AppendFormat("'{0}'+{1} ", info.From.Name, (info.Time - sync.Time).TotalSeconds);
                    }

                    w.Flush();
                }

                foreach (NetState ns in NetState.Instances)
                {
                    if (ns.Mobile != null && ns.Mobile.AccessLevel > AccessLevel.Counselor)
                        ns.Mobile.SendMessage(msg.ToString());
                }

                IRCBot.StaffNotice(msg.ToString());
            }
            catch
            {
            }
        }

        public static void EquipReq(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            Item item = from.Holding;

            HandleEquip(new EquipInfo(from, item));

            m_Equip.OnReceive(state, pvSrc);
        }

        public static void EquipReq6017(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            Item item = from.Holding;

            HandleEquip(new EquipInfo(from, item));

            m_Equip6017.OnReceive(state, pvSrc);
        }

        public static void TargetResponse(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            Target t = from.Target;

            if (t == null || IgnoreRegex.IsMatch(t.GetType().FullName))
            {
                m_Target.OnReceive(state, pvSrc);
                return;
            }

            int type = pvSrc.ReadByte();
            int targetID = pvSrc.ReadInt32();
            int flags = pvSrc.ReadByte();
            Serial serial = pvSrc.ReadInt32();

            pvSrc.Seek(1, System.IO.SeekOrigin.Begin);

            if (targetID == unchecked((int)0xDEADBEEF))
                return;

            Mobile target = null;

            if ( serial.IsMobile )
                target = World.FindMobile(serial);

            if (target == null || target == from)
            {
                m_Target.OnReceive(state, pvSrc);
                return;
            }

            HandleTarget(new TargetInfo(from, target, t.GetType()));

            m_Target.OnReceive(state, pvSrc);
        }

        public static void TargetResponse6017(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            Target t = from.Target;

            if ( t == null || IgnoreRegex.IsMatch( t.GetType().FullName ) )
            {
                m_Target6017.OnReceive(state, pvSrc);
                return;
            }

            int type = pvSrc.ReadByte();
            int targetID = pvSrc.ReadInt32();
            int flags = pvSrc.ReadByte();
            Serial serial = pvSrc.ReadInt32();

            pvSrc.Seek(1, System.IO.SeekOrigin.Begin);

            if (targetID == unchecked((int)0xDEADBEEF))
                return;

            Mobile target = null;

            if (serial.IsMobile)
            target = World.FindMobile(serial);

            if (target == null || target == from)
            {
                m_Target6017.OnReceive(state, pvSrc);
                return;
            }

            HandleTarget(new TargetInfo(from, target, t.GetType()));

            m_Target6017.OnReceive(state, pvSrc);
        }
    }
}
