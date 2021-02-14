using System;
using Server.Network;
using Server.Gumps;

namespace Server.Misc
{
	public class LoginStats
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs args )
		{
			int userCount = Server.Admin.AdminNetwork.GetNumOnline();
			int itemCount = World.Items.Count;
			int mobileCount = World.Mobiles.Count;

			Mobile m = args.Mobile;

            //m.SendGump( new ChestOpening( m ) );

            if (m.AccessLevel == AccessLevel.Player)
            {
                m.SendMessage("Welcome, {0}! There {1} currently {2} user{3} online.",
                    args.Mobile.Name,
                    userCount == 1 ? "is" : "are",
                    userCount, userCount == 1 ? "" : "s");
            }
            else
            {
                m.SendMessage("Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.",
                    args.Mobile.Name,
                    userCount == 1 ? "is" : "are",
                    userCount, userCount == 1 ? "" : "s",
                    itemCount, itemCount == 1 ? "" : "s",
                    mobileCount, mobileCount == 1 ? "" : "s");
            }
		}
	}

    public class ChestOpening : Gump
    {
        private Mobile _Mob;
        private int _Stage;
        Timer _Timer;

        public ChestOpening(Mobile m)
            : this(m, 0)
        {
        }

        public ChestOpening(Mobile m, int stage)
            : base(0, 0)
        {
            _Mob = m;
            _Stage = stage;

            Closable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            //AddImageTiled( 0, 0, 1600, 1200, 0xA40 );
            AddImage(0, 0, 1415);
            AddImage(122, 103, 1140);
            AddLabel(215, 200, 0, "Welcome to Divinity CE!");

            AddPage(1);
            AddImage(0, 0, 1400 + stage);

            TimeSpan delay;
            if (_Stage > 0 && _Stage < 14)
                delay = TimeSpan.FromSeconds(0.25);
            else
                delay = TimeSpan.FromSeconds(0.5);

            _Timer = Timer.DelayCall(delay, new TimerCallback(NextStage));
            _Timer.Start();
        }

        public void NextStage()
        {
            try
            {
                _Mob.CloseGump(this.GetType());

                if (_Stage < 14)
                    _Mob.SendGump(new ChestOpening(_Mob, _Stage + 1));
            }
            catch
            {
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                _Timer.Stop();

            base.OnResponse(sender, info);
        }
    }
}
