//#define WORLD_IMPORT
#if WORLD_IMPORT
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;
using Server.Multis;
using Server.Multis.Deeds;
using System.Collections.Generic;

namespace Server
{
    public class WorldImport
    {
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(ImportWorld));
        }

        public static void InsertError(string fmt, params object[] args)
        {
            string err = String.Format(fmt,args);
            if (!_Errors.ContainsKey(err))
            {
                Console.WriteLine(err);
                _Errors.Add(err, 1);
            }
            else
                _Errors[err]++;
        }

        private static Hashtable _Mobiles = new Hashtable();
        private static Hashtable _Items = new Hashtable();

        private static Dictionary<string, uint> _Errors = new Dictionary<string,uint>();

        public static Mobile FindMobile(Serial s)
        {
            if ( !_Mobiles.ContainsKey( s ) )
                return null;
            return _Mobiles[s] as Mobile;
        }

        public static Item FindItem(Serial s)
        {
            if ( !_Items.ContainsKey( s ) )
                return null;
            return _Items[s] as Item;
        }

        public static void ImportWorld()
        {
            ArrayList retry = new ArrayList(), houses = new ArrayList();
            int count = 0;

            foreach ( Account a in Accounts.List.Values )
            {
                for (int i = 0; i < 6; i++)
                {
                    if ( a[i] != null )
                        a[i].Player = false;

                    a[i] = null;
                }
            }
            
            using (StreamReader r = new StreamReader("world.txt", System.Text.Encoding.ASCII))
            {
                string line;
                IEntity curObj = null;
                Mobile prevMob = null;
                Type curType = null;

                while ((line = r.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] parts = line.Split(new char[] { '=' }, 3, StringSplitOptions.None);

                    if ( parts.Length != 3 )
                    {
                        Console.WriteLine( "Warning! Wrong number of fields: {0} -- {1}", parts.Length, line );
                        continue;
                    }

                    /*if (parts[0] == "Profile")
                    {
                        long prevPos = r.BaseStream.Position;
                        while ((line = r.ReadLine()) != null)
                        {
                            string[] split = line.Split(new char[] { '=' }, 3, StringSplitOptions.None);

                            if (split.Length != 3)
                            {
                                parts[2] += "\n" + line;
                                prevPos = r.BaseStream.Position;
                            }
                            else
                            {
                                r.BaseStream.Seek(prevPos, SeekOrigin.Begin);
                                break;
                            }
                        }
                    }*/

                    try
                    {
                        if (parts[0] == "Object")
                        {
                            PostProcess( curObj );
                            parts[1] = TranslateType( parts[1] );
                            curObj = null;
                            curType = ScriptCompiler.FindTypeByFullName(parts[1], true);

                            if (curType == null)
                            {
                                InsertError( "{0}: Type missing", parts[1] );
                                continue;
                            }

                            Serial oldSer = Utility.ToInt32(parts[2]);
                            /*if (IsType(curType, typeof(Item)))
                                Serial.NewItem = Utility.ToInt32(parts[2]) - 1;
                            else if (IsType(curType, typeof(Mobile) ))
                                Serial.NewMobile = Utility.ToInt32(parts[2]) - 1;
                            */
                            try { curObj = Activator.CreateInstance(curType, new object[0]) as IEntity; } catch { }

                            if (curObj == null)
                            {
                                try
                                {
                                    if (curType == typeof(BankBox) && prevMob != null)
                                        curObj = new BankBox(prevMob);
                                    //curObj = Activator.CreateInstance( curType, new object[1]{prevMob} ) as IEntity;
                                }
                                catch { }

                                if (curObj == null)
                                {
                                    InsertError("{0}: Unable to construct", parts[1]);
                                    continue;
                                }
                            }

                            if (curObj is Mobile)
                            {
                                _Mobiles[oldSer] = curObj;
                                prevMob = (Mobile)curObj;
                            }
                            else if ( curObj is Item )
                            {
                                _Items[oldSer] = curObj;
                            }
                            
                            count++;
                            if ((count % 10000) == 0)
                                Console.WriteLine("Completed {0} objects so far", count - 1);
                        }
                        else if (parts[0] == "House")
                        {
                            string type, loc, map, owner;

                            type = r.ReadLine();
                            loc = r.ReadLine();
                            map = r.ReadLine();
                            owner = r.ReadLine();
                            houses.Add(new string[] { type, loc, map, owner });
                        }
                        else if (parts[0] == "Skills")
                        {
                            Mobile m = FindMobile(Utility.ToInt32(parts[2]));

                            while ((line = r.ReadLine()) != null)
                            {
                                if (line == "EndSkills" || line == "")
                                    break;

                                parts = line.Split(new char[] { '=' }, 3, StringSplitOptions.None);

                                if (parts.Length != 3)
                                    continue;

                                SkillName name = (SkillName)Enum.Parse(typeof(SkillName), parts[0]);

                                m.Skills[name].BaseFixedPoint = Utility.ToInt32(parts[2]);
                            }
                        }
                        else if (parts[0] == "RuneBookEntry")
                        {
                            if (curObj is Runebook)
                            {
                                Point3D pt = Point3D.Parse(parts[1]);

                                bool okay = true;
                                if (Spells.SpellHelper.IsAnyT2A(Map.Felucca, pt))
                                    okay = false;
                                else if (Spells.SpellHelper.IsFeluccaDungeon(Map.Felucca, pt))
                                {
                                    Regions.DungeonRegion reg = Region.Find(pt, Map.Felucca) as Regions.DungeonRegion;

                                    if (reg != null)
                                    {
                                        if (reg.Name == "Fire" || reg.Name == "Ice" || reg.Name == "Orc Cave" || reg.Name == "Terathan Keep")
                                            okay = false;
                                    }
                                }

                                if (okay)
                                    ((Runebook)curObj).Entries.Add(new RunebookEntry(pt, Map.Felucca, parts[2], null));
                            }
                        }
                        else if (curObj != null)
                        {
                            DoProperty(curObj, curType, parts, retry);
                        }
                    }
                    catch ( Exception e )
                    {
                        InsertError("Exception '{0}' for line '{1}'", e.Message, line);
                    }
                }

                PostProcess( curObj );
            }

            foreach (object[] list in retry)
                DoProperty( list[0], list[1] as Type, list[2] as string[], null );

            foreach (string[] list in houses)
            {
                list[0] = TranslateType(list[0]);
                Type deedType = ScriptCompiler.FindTypeByFullName(list[0], true);
                Point3D loc = Point3D.Parse(list[1]);
                Map map = Map.Parse(list[2]);
                Mobile owner = FindMobile(Utility.ToInt32(list[3]));

                if (deedType == null || !deedType.IsSubclassOf(typeof(HouseDeed)))
                {
                    InsertError("{0}: Type missing\n", list[0]);
                    continue;
                }

                if (owner == null || map == null || map == Map.Internal || loc == Point3D.Zero)
                {
                    InsertError("House properties messed up (owner probably missing)\n");
                    continue;
                }

                HouseDeed deed = null ;
                try { deed = Activator.CreateInstance(deedType, new object[0]) as HouseDeed; }
                catch { }

                if (deed == null)
                {
                    InsertError("Failed to create deed {0}\n", list[0]);
                    continue;
                }

                BaseHouse house = deed.GetHouse(owner);
                house.MoveToWorld(loc, map);
                deed.Delete();
            }
            
            Console.WriteLine("Import complete, imported {0} objects.", count);
            Console.WriteLine( "\nErrors:" );
            foreach ( string err in _Errors.Keys )
                Console.WriteLine( "{0}  (Count={1})", err, _Errors[err] );

            Console.WriteLine("Done.  Saving...");
            World.Save();

            Console.WriteLine( "Press enter to quit.  REMOVE THIS SCRIPT BEFORE RUNNING THE SERVER AGAIN" );
            Console.ReadLine();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private static void PostProcess(object obj)
        {
            if (obj is RecallRune)
            {
                RecallRune rune = (RecallRune)obj;

                if ( Spells.SpellHelper.IsAnyT2A( rune.TargetMap, rune.Target ) )
                    rune.Delete();
                else if (Spells.SpellHelper.IsFeluccaDungeon(rune.TargetMap, rune.Target))
                {
                    Regions.DungeonRegion reg = Region.Find(rune.Target, rune.TargetMap) as Regions.DungeonRegion;

                    if (reg != null)
                    {
                        if (reg.Name == "Fire" || reg.Name == "Ice" || reg.Name == "Orc Cave" || reg.Name == "Terathan Keep")
                            rune.Delete();
                    }
                }
            }
        }

        private static void DoProperty(object curObj, Type curType, string[] parts, ArrayList retry)
        {
            TranslateProperty(curObj, parts);

            if (parts[0] == "")
                return;

            Type thisType = ScriptCompiler.FindTypeByFullName(parts[1], true);
            PropertyInfo prop = null;

            if (thisType != null)
                prop = curType.GetProperty(parts[0], thisType);
            if (prop == null)
                prop = curType.GetProperty(parts[0]);

            if (prop == null)
            {
                InsertError("{0}.{1}: Prop missing", curObj.GetType().FullName, parts[0]);
                return;
            }

            if (thisType == null || !prop.PropertyType.IsAssignableFrom(thisType))
                thisType = prop.PropertyType;

            object val = GetObjectFromString(thisType, parts[2]);

            if (val == null)
            {
                if (retry != null)
                {
                    retry.Add(new object[] { curObj, curType, parts });
                }
                else
                {
                    InsertError( "{0}.{1}: Value ({2}) is null", curObj.GetType().FullName, parts[0], parts[2]);
                }
                return;
            }
            else
            {
                if (val is Account && curObj is Mobile)
                {
                    Account a = (Account)val;
                    int free = -1;
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == null || a[i] == curObj)
                        {
                            free = i;
                            break;
                        }
                    }

                    if (free != -1)
                        a[free] = (Mobile)curObj;
                }

                if (parts[0] == "Parent" && curObj is Item)
                {
                    if (val is Mobile)
                        ((Mobile)val).AddItem((Item)curObj);
                    else if (val is Item)
                        ((Item)val).AddItem((Item)curObj);
                    else
                        Console.WriteLine("wtf?");
                }
                else
                    prop.SetValue(curObj, val, null);
            }
        }

        private static string TranslateType(string type)
        {
            if (type == "Server.Misc.GiftBag")
                return "Server.Items.Bag";
            else if ( type == "Server.BaseItem" )
                return "Server.Item";
            else if ( type == "Server.Multis.Deeds.SmallTrainingHouseDeed" )
                return "Server.Multis.Deeds.SmallBrickHouseDeed";
            else if ( type == "Server.Multis.Deeds.LargeForgeHouseDeed" )
                return "Server.Multis.Deeds.LargePatioDeed";
            else
                return type;
        }

        private static void TranslateProperty( object obj, string[] parts )
        {
            if ( parts[0] == "Newbied" )
            {
                if (Convert.ToBoolean(parts[2]))
                {
                    parts[0] = "LootType";
                    parts[1] = "Server.LootType";
                    parts[2] = "Newbied";
                }
                else
                {
                    parts[0] = "";
                }
            }
            else if ( parts[0] == "MaxHits" && obj is BaseWeapon )
            {
                parts[0] = "MaxHitPoints";
            }
            else if ( parts[0] == "Hits" && obj is BaseWeapon )
            {
                parts[0] = "HitPoints";
            }
            else if (
                parts[0] == "TotalGold" || parts[0] == "Attributes" || parts[0] == "ArmorAttributes" ||
                parts[0] == "ArmorAttributes" || parts[0] == "SkillBonuse" || parts[0] == "ContainerData" ||
                parts[0] == "WeaponAttributes" || parts[0] == "TotalItems" || parts[0] == "AosWeaponAttributes" ||
                parts[0] == "SkillBonuses" || parts[0] == "TotalWeight" || parts[0] == "Trapped" || parts[0] == "Bounds" ||
                parts[0] == "Skills" || parts[0] == "Stabled" || parts[0] == "Virtues"
               )
            {
                parts[0] = "";
            }
        }

        public static object GetObjectFromString(Type t, string s)
        {
            if (t == typeof(string))
            {
                return s.Replace( "\\r", "\r" ).Replace( "\\n", "\n" );
            }
            else if (t == typeof(bool) || t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong))
            {
                if (s.StartsWith("0x"))
                {
                    if (t == typeof(ulong) || t == typeof(uint) || t == typeof(ushort) || t == typeof(byte))
                    {
                        return Convert.ChangeType(Convert.ToUInt64(s.Substring(2), 16), t);
                    }
                    else
                    {
                        return Convert.ChangeType(Convert.ToInt64(s.Substring(2), 16), t);
                    }
                }
                else
                {
                    return Convert.ChangeType(s, t);
                }
            }
            else if (t == typeof(double) || t == typeof(float))
            {
                return Convert.ChangeType(s, t);
            }
            else if (IsType(t, typeof(Mobile)))
            {
                return FindMobile((Serial)Convert.ToInt32(s));
            }
            else if (IsType(t, typeof(Item)))
            {
                return FindItem((Serial)Convert.ToInt32(s));
            }
            else if (IsType(t, typeof(Account)))
            {
                return Accounts.GetAccount( s );
            }
            else if (IsType(t, typeof(Enum)))
            {
                return Enum.Parse(t, s);
            }
            else if (IsType(t, typeof(Server.Body)))
            {
                return new Server.Body(Convert.ToInt32(s.Substring(2), 16));
            }
            else
            {
                MethodInfo parseMethod = t.GetMethod("Parse", new Type[] { typeof(string) });

                if (parseMethod != null)
                    return parseMethod.Invoke(null, new object[] { s });
            }

            return null;
        }


        private static bool IsType(Type type, Type check)
        {
            return type == check || type.IsSubclassOf(check);
        }

        private static bool IsType(Type type, Type[] check)
        {
            for (int i = 0; i < check.Length; ++i)
                if (IsType(type, check[i]))
                    return true;

            return false;
        }
    }
}
#endif
