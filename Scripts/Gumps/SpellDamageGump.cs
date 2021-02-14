using System;
using Server.Network;
using Server.Spells;
using Server.Commands;

namespace Server.Gumps
{
    public class SpellAdminSetupGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpellAdmin", AccessLevel.Administrator, new CommandEventHandler(OnSpellAdmin_Command));
        }

        public static void OnSpellAdmin_Command(CommandEventArgs e)
        {
            e.Mobile.SendGump(new SpellAdminSetupGump());
        }

        public SpellAdminSetupGump()
            : base(20, 20)
        {
            AddBackground(0, 0, 300, 320, 0x13BE);

            AddLabel(95, 10, 0, "Spell Speed Setup");

            AddLabel(10, 35, 0, "Spell speed:");
            AddTextField(95, 35, 40, 18, 100, MagerySpell.SpeedTable[0].ToString());
            AddLabel(140, 35, 0, "* circle +");
            AddTextField(215, 35, 40, 18, 101, MagerySpell.SpeedTable[1].ToString());

            AddLabel(95, 60, 0, "Spell Damage Setup");

            for (int row = 0; row < 8; row++)
            {
                AddLabel(10, row * 25 + 85, 0, String.Format("Circle {0}:", row + 1));
                AddLabel(97, row * 25 + 85, 0, "d");
                AddLabel(137, row * 25 + 85, 0, "+");

                int x = MagerySpell.DamageTable[row * 3], y = MagerySpell.DamageTable[row * 3 + 1], z = MagerySpell.DamageTable[row * 3 + 2];

                int minDamage, maxDamage;
                minDamage = x + z;
                maxDamage = x * y + z;

                AddLabel(180, row * 25 + 85, 0, String.Format("({0}-{1})", minDamage, maxDamage));
                AddLabel(240, row * 25 + 85, 0, String.Format("Avg: {0}", (minDamage + maxDamage) / 2));

                for (int column = 0; column < 3; column++)
                    AddTextField(column * 40 + 70, row * 25 + 85, 25, 18, row * 3 + column, MagerySpell.DamageTable[row * 3 + column].ToString());

            }

            AddButton(100, 290, 1153, 1154, 1, GumpButtonType.Reply, 0);
            AddButton(185, 290, 1150, 1151, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from != null && info.ButtonID == 1)
            {
                TextRelay relay;

                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        relay = info.GetTextEntry(100 + i);
                        double number = Double.Parse(relay.Text);
                        MagerySpell.SpeedTable[i] = number;
                    }

                }
                catch
                {
                    from.SendMessage("Couldn't parse speed entries.");
                }

                for (int row = 0; row < 8; row++)
                    for (int column = 0; column < 3; column++)
                    {
                        relay = info.GetTextEntry(row * 3 + column);
                        if (relay != null)
                        {
                            string text = relay.Text.Trim();
                            try
                            {
                                int number = Int32.Parse(text);
                                if (number < 0)
                                    number = 0;

                                if (number > 99)
                                    number = 99;

                                MagerySpell.DamageTable[row * 3 + column] = number;
                            }
                            catch
                            {
                                from.SendMessage("Invalid textentry at column {0} on circle {1}.", column + 1, row + 1);
                            }
                        }
                    }
                from.SendMessage("Saved spell damages. Saving to file...");
                try
                {
                    MagerySpell.SaveTables();
                    from.SendMessage("done.");
                }
                catch (Exception e)
                {
                    from.SendMessage("Couldn't save damage table: {0}", e.Message);
                }

                from.SendGump(new SpellAdminSetupGump());
            }
        }

        public void AddTextField(int x, int y, int width, int height, int index, string text)
        {
            AddBackground(x - 2, y - 2, width + 4, height + 6, 0x2486);
            AddTextEntry(x + 2, y, width - 4, height - 2, 0, index, text);
        }
    }
}
