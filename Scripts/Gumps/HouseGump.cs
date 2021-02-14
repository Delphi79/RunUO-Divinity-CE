using System;
using System.Reflection;
using System.Collections;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Items;

namespace Server.Gumps
{
    public class HouseListGump : Gump
    {
        private BaseHouse m_House;

        public HouseListGump(int number, ArrayList list, BaseHouse house, bool accountOf)
            : base(100, 50)
        {
            if (house.Deleted)
                return;

            m_House = house;


            AddPage(0);

            AddBackground(100, 100, 400, 300, 0xA28);
            AddBackground(147, 359, 310, 25, 0x13EC);

            AddButton(270, 360, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);

            string listtype = "invalid";

            if (number == 1011275)
                listtype = "Co-owners of the house";
            else if (number == 1011273)
                listtype = "Friends of the house";
            else if (number == 1011271)
                listtype = "Enemies of the house";

            string caption = String.Format("<big><center><basefont color=white>{0}</big></center></basefont>", listtype);
            AddHtml(100, 118, 400, 20, caption, false, false);

            if (list != null)
            {

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 7) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(425, 363, 0x1468, 0x1468, 0, GumpButtonType.Page, (i / 7) + 1);
                        }

                        AddPage((i / 7) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(160, 363, 0x1467, 0x1467, 0, GumpButtonType.Page, i / 7);
                        }
                    }

                    Mobile m;

                    if (list[i] is BaseHouse.BanEntry)
                        m = ((BaseHouse.BanEntry)list[i]).Mobile;
                    else
                        m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    string htmlname;
                    if (accountOf && m.Player && m.Account != null)
                        htmlname = String.Format("<big>Account of {0}</big>", name);
                    else
                        htmlname = String.Format("<big>{0}</big>", name);

                    AddHtml(180, 155 + ((i % 7) * 25), 350, 20, htmlname, false, false);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            from.SendGump(new HouseGump(from, m_House));
        }
    }

    public class HouseRemoveGump : Gump
    {
        private BaseHouse m_House;
        private ArrayList m_List, m_Copy;
        private int m_Number;
        private bool m_AccountOf;

        public HouseRemoveGump(int number, ArrayList list, BaseHouse house, bool accountOf)
            : base(20, 30)
        {
            if (house.Deleted)
                return;

            m_House = house;
            m_List = list;
            m_Number = number;
            m_AccountOf = accountOf;

            AddPage(0);

            AddBackground(100, 100, 400, 300, 0xA28);
            AddBackground(147, 359, 310, 25, 0x13EC);

            AddButton(270, 360, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);

            string listtype = "invalid";

            if (number == 1011274)
                listtype = "Remove a co-owner from the house";
            else if (number == 1011272)
                listtype = "Remove a friend from the house";
            else if (number == 1011269)
                listtype = "Lift a ban from the house";

            string caption = String.Format("<big><center><basefont color=white>{0}</big></center></basefont>", listtype);
            AddHtml(100, 118, 400, 20, caption, false, false);

            AddButton(147, 335, 2714, 2715, 1, GumpButtonType.Reply, 0);
            AddHtml(167, 335, 300, 20, "<big><basefont color=white>Remove now!</basefont>", false, false); // Remove now!


            if (list != null)
            {
                m_Copy = new ArrayList(list);

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 7) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(425, 363, 0x1468, 0x1468, 0, GumpButtonType.Page, (i / 7) + 1);
                        }

                        AddPage((i / 7) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(160, 363, 0x1467, 0x1467, 0, GumpButtonType.Page, i / 7);
                        }
                    }

                    Mobile m;
                    if (list[i] is BaseHouse.BanEntry)
                        m = ((BaseHouse.BanEntry)list[i]).Mobile;
                    else
                        m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    string htmlname;
                    if (accountOf && m.Player && m.Account != null)
                        htmlname = String.Format("<big>Account of {0}</big>", name);
                    else
                        htmlname = String.Format("<big>{0}</big>", name);

                    AddHtml(180, 155 + ((i % 7) * 25), 350, 20, htmlname, false, false);
                    AddCheck(150, 155 + ((i % 7) * 25), 0xD2, 0xD3, false, i);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            if (m_List != null && info.ButtonID == 1) // Remove now
            {
                int[] switches = info.Switches;

                if (switches.Length > 0)
                {
                    for (int i = 0; i < switches.Length; ++i)
                    {
                        int index = switches[i];

                        if (index >= 0 && index < m_Copy.Count)
                        {
                            if (m_Copy[index] is BaseHouse.BanEntry)
                            {
                                m_House.Bans.Remove(((BaseHouse.BanEntry)m_Copy[index]).Mobile);
                            }

                            m_List.Remove(m_Copy[index]);
                        }
                    }

                    if (m_List.Count > 0)
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(m_Number, m_List, m_House, m_AccountOf));
                        return;
                    }
                }
            }

            from.SendGump(new HouseGump(from, m_House));
        }
    }

    public class HouseGump : Gump
    {
        private BaseHouse m_House;

        private ArrayList Wrap(string value)
        {
            if (value == null || (value = value.Trim()).Length <= 0)
                return null;

            string[] values = value.Split(' ');
            ArrayList list = new ArrayList();
            string current = "";

            for (int i = 0; i < values.Length; ++i)
            {
                string val = values[i];

                string v = current.Length == 0 ? val : current + ' ' + val;

                if (v.Length < 10)
                {
                    current = v;
                }
                else if (v.Length == 10)
                {
                    list.Add(v);

                    if (list.Count == 6)
                        return list;

                    current = "";
                }
                else if (val.Length <= 10)
                {
                    list.Add(current);

                    if (list.Count == 6)
                        return list;

                    current = val;
                }
                else
                {
                    while (v.Length >= 10)
                    {
                        list.Add(v.Substring(0, 10));

                        if (list.Count == 6)
                            return list;

                        v = v.Substring(10);
                    }

                    current = v;
                }
            }

            if (current.Length > 0)
                list.Add(current);

            return list;
        }

        public HouseGump(Mobile from, BaseHouse house)
            : base(100, 50)
        {
            if (house.Deleted)
                return;

            bool isTent = house is Tent;

            m_House = house;

            from.CloseGump(typeof(HouseGump));
            from.CloseGump(typeof(HouseListGump));
            from.CloseGump(typeof(HouseRemoveGump));

            bool isCombatRestricted = house.IsCombatRestricted(from);

            bool isOwner = m_House.IsOwner(from);
            bool isCoOwner = isOwner || m_House.IsCoOwner(from);
            bool isFriend = isCoOwner || m_House.IsFriend(from);

            if (isCombatRestricted)
                isFriend = isCoOwner = isOwner = false;

            AddPage(0);

            if (isFriend)
            {
                AddBackground(100, 100, 400, 300, 0xA28);
                AddBackground(116, 165, 369, 25, 0x13EC);
            }

            AddImage(230, 65, 100);

            if (m_House.Sign != null)
            {
                ArrayList lines = Wrap(m_House.Sign.Name);

                if (lines != null) //HOUSE SIGN NAME TO FIX
                {
                    for (int i = 0, y = (101 - (lines.Count * 14)) / 2; i < lines.Count; ++i, y += 14)
                    {
                        string s = (string)lines[i];

                        string housename = String.Format("<big>{0}</big>", s);
                        AddHtml(230 + ((143 - (s.Length * 8)) / 2), y + 65, 100, 20, housename, false, false);
                    }
                }
            }

            if (!isFriend)
                return;

            AddHtml(125, 168, 75, 20, "<big>INFO</big>", false, false); //INFO
            AddButton(170, 169, 0x1459, 0x138B, 0, GumpButtonType.Page, 1);

            AddHtml(230, 168, 75, 20, "<big>FRIENDS</big>", false, false); //FRIENDS
            AddButton(310, 169, 0x1459, 0x138B, 0, GumpButtonType.Page, 2);

            AddHtml(375, 168, 75, 20, "<big>OPTIONS</big>", false, false); //OPTIONS
            AddButton(455, 169, 0x1459, 0x138B, 0, GumpButtonType.Page, 3);

            // Info page
            AddPage(1);

            AddButton(150, 366, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //EXIT;

            AddHtml(245, 365, 300, 20, "<big>Change this house's name!</big>", false, false); //Change this house's name!
            AddButton(425, 365, 0xA9A, 0xA9B, 1, GumpButtonType.Reply, 0);

            string ownedbyname = String.Format("<big><center><basefont color=white>Owned by  <basefont color=black>{0}</big></center>", GetOwnerName());
            AddHtml(100, 205, 400, 20, ownedbyname, false, false);

            string lockdowns = String.Format("<big><basefont color=white>Number of locked down items:<basefont color=black> {0} out of {1}</basefont>", m_House.LockDownCount, m_House.MaxLockDowns);
            AddHtml(140, 240, 350, 20, lockdowns, false, false);

            string secures = String.Format("<big><basefont color=white>Number of secure containers:<basefont color=black> {0} out of {1}</basefont>", m_House.SecureCount, m_House.MaxSecures);
            AddHtml(140, 260, 350, 20, secures, false, false);

            AddHtml(140, 315, 400, 20, "<big><basefont color=white>This house is properly placed.</big></basefont>", false, false);
            AddHtml(140, 333, 400, 20, "<big><basefont color=white>This house is of classic design.</big></basefont>", false, false);

            if (m_House.Public)
            {
                string visits = String.Format("<big><basefont color=white>Number of visits this building has had:  <basefont color=black>{0}</big></basefont>", m_House.Visits);
                AddHtml(140, 295, 400, 20, visits, false, false);
            }

            // Friends page
            AddPage(2);

            AddButton(150, 366, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //EXIT;

            AddHtml(153, 194, 150, 20, "<big><basefont color=white>List of co-owners</basefont>", false, false); // <big><basefont color=white>List of co-owners</basefont>
            AddButton(125, 194, 2714, 2715, 2, GumpButtonType.Reply, 0);

            AddHtml(153, 216, 150, 20, "<big><basefont color=white>Add a co-owner</basefont>", false, false); // <big><basefont color=white>Add a co-owner</basefont>
            AddButton(125, 216, 2714, 2715, 3, GumpButtonType.Reply, 0);

            AddHtml(153, 238, 150, 20, "<big><basefont color=white>Remove a co-owner</basefont>", false, false); // <big><basefont color=white>Remove a co-owner</basefont>
            AddButton(125, 238, 2714, 2715, 4, GumpButtonType.Reply, 0);

            AddHtml(153, 260, 150, 20, "<big><basefont color=white>Clear co-owner list</basefont>", false, false); // <big><basefont color=white>Clear co-owner list</basefont>
            AddButton(125, 260, 2714, 2715, 5, GumpButtonType.Reply, 0);

            AddHtml(340, 194, 155, 20, "<big><basefont color=white>List of Friends</basefont>", false, false); // <big><basefont color=white>List of Friends</basefont>
            AddButton(310, 194, 2714, 2715, 6, GumpButtonType.Reply, 0);

            AddHtml(340, 216, 155, 20, "<big><basefont color=white>Add a Friend</basefont>", false, false); // <big><basefont color=white>Add a Friend</basefont>
            AddButton(310, 216, 2714, 2715, 7, GumpButtonType.Reply, 0);

            AddHtml(340, 238, 155, 20, "<big><basefont color=white>Remove a Friend</basefont>", false, false); // <big><basefont color=white>Remove a Friend</basefont>
            AddButton(310, 238, 2714, 2715, 8, GumpButtonType.Reply, 0);

            AddHtml(340, 260, 155, 20, "<big><basefont color=white>Clear Friends list</basefont>", false, false); // <big><basefont color=white>Clear Friends list</basefont>
            AddButton(310, 260, 2714, 2715, 9, GumpButtonType.Reply, 0);

            if (!isTent)
            {
                AddHtml(225, 281, 280, 20, "<big><basefont color=white>Ban someone from the house</basefont>", false, false); // <big><basefont color=white>Ban someone from the house</basefont>
                AddButton(195, 281, 2714, 2715, 10, GumpButtonType.Reply, 0);

                AddHtml(225, 302, 280, 20, "<big><basefont color=white>Eject someone from the house</basefont>", false, false); // <big><basefont color=white>Eject someone from the house</basefont>
                AddButton(195, 302, 2714, 2715, 11, GumpButtonType.Reply, 0);

                AddHtml(225, 324, 280, 20, "<big><basefont color=white>View a list of banned people</basefont>", false, false); // <big><basefont color=white>View a list of banned people</basefont>
                AddButton(195, 324, 2714, 2715, 12, GumpButtonType.Reply, 0);

                AddHtml(225, 346, 280, 20, "<big><basefont color=white>Lift a ban</basefont>", false, false); // <big><basefont color=white>Lift a ban</basefont>
                AddButton(195, 346, 2714, 2715, 13, GumpButtonType.Reply, 0);
            }

            // Options page
            AddPage(3);

            AddButton(150, 366, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //EXIT;

            AddHtml(185, 195, 355, 30, "<big><basefont color=white>Transfer ownership of the house</basefont>", false, false); // <big><basefont color=white>Transfer ownership of the house</basefont>
            AddButton(155, 195, 2714, 2715, 14, GumpButtonType.Reply, 0);

            AddHtml(185, 225, 355, 30, "<big><basefont color=white>Demolish house and get deed back</basefont>", false, false); // <big><basefont color=white>Demolish house and get deed back</basefont>
            AddButton(155, 225, 2714, 2715, 15, GumpButtonType.Reply, 0);

            if (!m_House.Public)
            {
                AddHtml(185, 255, 355, 30, "<big><basefont color=white>Change the house locks</basefont>", false, false); // <big><basefont color=white>Change the house locks</basefont>
                AddButton(155, 255, 2714, 2715, 16, GumpButtonType.Reply, 0);

                AddHtml(185, 285, 300, 90, "<big><basefont color=white>Declare this building to be public. This will make your front door unlockable.</basefont>", false, false); // <big><basefont color=white>Declare this building to be public. This will make your front door unlockable.</basefont>
                AddButton(155, 285, 2714, 2715, 17, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtml(185, 255, 350, 30, "<big><basefont color=white>Change the sign type</basefont>", false, false); // <big><basefont color=white>Change the sign type</basefont>
                AddButton(155, 255, 2714, 2715, 0, GumpButtonType.Page, 4);

                AddHtml(185, 285, 350, 30, "<big><basefont color=white>Declare this building to be private.</basefont>", false, false); // <big><basefont color=white>Declare this building to be private.</basefont>
                AddButton(155, 285, 2714, 2715, 17, GumpButtonType.Reply, 0);

                // Change the sign type
                AddPage(4);

                for (int i = 0; i < 24; ++i)
                {
                    //AddRadio( 53 + ((i / 4) * 50), 137 + ((i % 4) * 35), 210, 211, false, i + 1 );
                    //AddItem( 60 + ((i / 4) * 50), 130 + ((i % 4) * 35), 2980 + (i * 2) );
                    AddRadio(153 + ((i / 4) * 50), 202 + ((i % 4) * 35), 210, 211, false, i + 1);
                    AddItem(160 + ((i / 4) * 50), 195 + ((i % 4) * 35), 2980 + (i * 2));
                }

                AddHtml(275, 365, 129, 20, "<big><basefont color=white>Guild sign choices</basefont>", false, false); // Guild sign choices
                AddButton(425, 360, 252, 253, 0, GumpButtonType.Page, 5);

                AddButton(150, 366, 0xF7, 0xF8, 18, GumpButtonType.Reply, 0); //OKAY

                AddPage(5);

                for (int i = 0; i < 29; ++i)
                {
                    AddRadio(153 + ((i / 5) * 50), 202 + ((i % 5) * 35), 210, 211, false, i + 25);
                    AddItem(160 + ((i / 5) * 50), 195 + ((i % 5) * 35), 3028 + (i * 2));
                }

                AddHtml(275, 365, 129, 20, "<big><basefont color=white>Shop sign choices</basefont>", false, false); // Shop sign choices
                AddButton(425, 360, 250, 251, 0, GumpButtonType.Page, 4);

                AddButton(150, 366, 0xF7, 0xF8, 18, GumpButtonType.Reply, 0); //OKAY
            }
        }

        private string GetOwnerName()
        {
            Mobile m = m_House.Owner;

            if (m == null)
                return "(unowned)";

            string name;

            if ((name = m.Name) == null || (name = name.Trim()).Length <= 0)
                name = "(no name)";

            return name;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = sender.Mobile;

            bool isCombatRestricted = m_House.IsCombatRestricted(from);
            bool isTent = m_House is Tent;
            bool isOwner = m_House.IsOwner(from);
            bool isCoOwner = isOwner || m_House.IsCoOwner(from);
            bool isFriend = isCoOwner || m_House.IsFriend(from);

            if (isCombatRestricted)
                isFriend = isCoOwner = isOwner = false;

            if (!isFriend || !from.Alive)
                return;

            Item sign = m_House.Sign;

            if (sign == null || from.Map != sign.Map || !from.InRange(sign.GetWorldLocation(), 18))
                return;

            switch (info.ButtonID)
            {
                case 1: // Rename sign
                    {
                        from.Prompt = new RenamePrompt(m_House);
                        from.SendLocalizedMessage(501302); // What dost thou wish the sign to say?

                        break;
                    }
                case 2: // List of co-owners
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011275, m_House.CoOwners, m_House, false));

                        break;
                    }
                case 3: // Add co-owner
                    {
                        if (isOwner)
                        {
                            from.SendLocalizedMessage(501328); // Target the person you wish to name a co-owner of your household.
                            from.Target = new CoOwnerTarget(true, m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501327); // Only the house owner may add Co-owners.
                        }

                        break;
                    }
                case 4: // Remove co-owner
                    {
                        if (isOwner)
                        {
                            from.CloseGump(typeof(HouseGump));
                            from.CloseGump(typeof(HouseListGump));
                            from.CloseGump(typeof(HouseRemoveGump));
                            from.SendGump(new HouseRemoveGump(1011274, m_House.CoOwners, m_House, false));
                        }
                        else
                        {
                            from.SendLocalizedMessage(501329); // Only the house owner may remove co-owners.
                        }

                        break;
                    }
                case 5: // Clear co-owners
                    {
                        if (isOwner)
                        {
                            if (m_House.CoOwners != null)
                                m_House.CoOwners.Clear();

                            from.SendLocalizedMessage(501333); // All co-owners have been removed from this house.
                        }
                        else
                        {
                            from.SendLocalizedMessage(501330); // Only the house owner may remove co-owners.
                        }

                        break;
                    }
                case 6: // List friends
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011273, m_House.Friends, m_House, false));

                        break;
                    }
                case 7: // Add friend
                    {
                        if (isCoOwner)
                        {
                            from.SendLocalizedMessage(501317); // Target the person you wish to name a friend of your household.
                            from.Target = new HouseFriendTarget(true, m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501316); // Only the house owner may add friends.
                        }

                        break;
                    }
                case 8: // Remove friend
                    {
                        if (isCoOwner)
                        {
                            from.CloseGump(typeof(HouseGump));
                            from.CloseGump(typeof(HouseListGump));
                            from.CloseGump(typeof(HouseRemoveGump));
                            from.SendGump(new HouseRemoveGump(1011272, m_House.Friends, m_House, false));
                        }
                        else
                        {
                            from.SendLocalizedMessage(501318); // Only the house owner may remove friends.
                        }

                        break;
                    }
                case 9: // Clear friends
                    {
                        if (isCoOwner)
                        {
                            if (m_House.Friends != null)
                                m_House.Friends.Clear();

                            from.SendLocalizedMessage(501332); // All friends have been removed from this house.
                        }
                        else
                        {
                            from.SendLocalizedMessage(501319); // Only the house owner may remove friends.
                        }

                        break;
                    }
                case 10: // Ban
                    {
                        if (isTent)
                            return;
                        from.SendLocalizedMessage(501325); // Target the individual to ban from this house.
                        from.Target = new HouseBanTarget(true, m_House);

                        break;
                    }
                case 11: // Eject
                    {
                        if (isTent)
                            return;
                        from.SendLocalizedMessage(501326); // Target the individual to eject from this house.
                        from.Target = new HouseKickTarget(m_House);

                        break;
                    }
                case 12: // List bans
                    {
                        if (isTent)
                            return;
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011271, m_House.BanList, m_House, true));

                        break;
                    }
                case 13: // Remove ban
                    {
                        if (isTent)
                            return;
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(1011269, m_House.BanList, m_House, true));

                        break;
                    }
                case 14: // Transfer ownership
                    {
                        if (isOwner)
                        {
                            from.SendLocalizedMessage(501309); // Target the person to whom you wish to give this house.
                            from.Target = new HouseOwnerTarget(m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501310); // Only the house owner may do this.
                        }

                        break;
                    }
                case 15: // Demolish house
                    {
                        if (isOwner)
                        {
                            if (m_House.FindGuildstone() != null)
                            {
                                from.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
                            }
                            else
                            {
                                from.CloseGump(typeof(HouseDemolishGump));
                                from.SendGump(new HouseDemolishGump(from, m_House));
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501320); // Only the house owner may do this.
                        }

                        break;
                    }
                case 16: // Change locks
                    {
                        if (m_House.Public)
                        {
                            from.SendLocalizedMessage(501669);// Public houses are always unlocked.
                        }
                        else
                        {
                            if (isOwner)
                            {
                                m_House.RemoveKeys(from);
                                m_House.ChangeLocks(from);

                                from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                            }
                            else
                            {
                                from.SendLocalizedMessage(501303); // Only the house owner may change the house locks.
                            }
                        }

                        break;
                    }
                case 17: // Declare public/private
                    {
                        if (isOwner)
                        {
                            if (m_House.Public && m_House.PlayerVendors.Count > 0)
                            {
                                from.SendLocalizedMessage(501887); // You have vendors working out of this building. It cannot be declared private until there are no vendors in place.
                                break;
                            }

                            m_House.Public = !m_House.Public;
                            if (!m_House.Public)
                            {
                                m_House.RemoveKeys(from);
                                m_House.ChangeLocks(from);

                                from.SendLocalizedMessage(501888); // This house is now private.
                                from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                            }
                            else
                            {
                                if (!(m_House is Tent))
                                    m_House.RemoveKeys(from);
                                m_House.RemoveLocks();
                                from.SendLocalizedMessage(501886);//This house is now public. Friends of the house my now have vendors working out of this building.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501307); // Only the house owner may do this.
                        }

                        break;
                    }
                case 18: // Change type
                    {
                        if (isOwner)
                        {
                            if (m_House.Public && info.Switches.Length > 0)
                            {
                                int index = info.Switches[0] - 1;

                                if (index >= 0 && index < 53)
                                    m_House.ChangeSignType(2980 + (index * 2));
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501307); // Only the house owner may do this.
                        }

                        break;
                    }
            }
        }
    }
}

namespace Server.Prompts
{
    public class RenamePrompt : Prompt
    {
        private BaseHouse m_House;

        public RenamePrompt(BaseHouse house)
        {
            m_House = house;
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (m_House.IsFriend(from))
            {
                if (m_House.Sign != null)
                    m_House.Sign.Name = text;

                from.SendMessage("Sign changed.");
            }
        }
    }
}
