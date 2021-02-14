using System;
using System.IO;
using System.Text;
using System.Collections;
using Server;
using Server.Network;
using Server.Accounting;
using Server.Gumps;

namespace Server.Misc
{
	public class ClientValidity
	{
		private static Hashtable m_Table = new Hashtable();
		private static ArrayList m_Exes = new ArrayList();

        private static Version CryptDLLVersion = new Version( 1, 0, 11 );
        private static byte[] CryptDLLChecksum = new byte[16]{
                   0x89, 0x2D, 0x39, 0xC3, 0xD, 0xB6, 0xBC, 0x32, 0x46, 0x84, 0x41, 0x3A, 0x89, 0x7D, 0x2, 0x2E };

		public static void Initialize()
		{
			Server.EventSink.Login += new LoginEventHandler( EventSink_Login );
			Server.EventSink.Logout += new LogoutEventHandler( EventSink_Logout );
			Server.Network.PacketHandlers.Register( 0x03, 0, true, new OnPacketReceive(	OnAsciiPacket ) );
            Server.Network.PacketHandlers.Register6017(0x03, 0, true, new OnPacketReceive(OnAsciiPacket));
            
            ProtocolExtensions.Register( 0xFF, true, new OnPacketReceive(OnHandshakeResponse) );

			Server.Network.PacketHandlers.ThirdPartyAuthCallback = new Server.Network.PacketHandlers.PlayCharCallback( OnThirdPartyAuth ); 
			Server.Network.PacketHandlers.ThirdPartyHackedCallback = new Server.Network.PacketHandlers.PlayCharCallback( OnThirdPartyHacked );

            Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.FilterWeather );
			Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.FilterLight );
			Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.ProtectHeals );
            Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.OverheadHealth );
            Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.ClosestTargets );
            Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.AutoOpenDoors );
            Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.AutoPotionEquip );
			Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.SmartTarget );
			Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.RangedTarget );
            Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.ProtectHeals );
			//Server.Network.FeatureProtection.Disable( Server.Network.ThirdPartyFeature.DequipOnCast  );

		
			try
			{
				using ( StreamReader sr = new StreamReader( "ExeNames.log" ) )
				{
					string line;

					while ( (line=sr.ReadLine()) != null )
						m_Exes.Add( line );
				}
			}	
			catch
			{
			}
		}
		
		private static byte[] m_Key = new byte[16]{ 0x98, 0x5B, 0x51, 0x7E, 0x11, 0x0C, 0x3D, 0x77, 0x2D, 0x28, 0x41, 0x22, 0x74, 0xAD, 0x5B, 0x39 };



		private static void OnAsciiPacket( NetState state, PacketReader pvSrc )
		{
			byte type = pvSrc.ReadByte();

			if ( type == 0x20 )
			{
				pvSrc.ReadInt16(); // hue
				pvSrc.ReadInt16(); // font

                StringBuilder cheat = new StringBuilder(), version = new StringBuilder();

                int pos = 0;

                /*
                'C'(43) 'H'(48) 'E'(45) 'A'(41) 'T'(54) ' '
                (20) 'U'(55) 'O'(4F) '.'(2E) 'e'(65) 'x'(78) 'e'(65) ' '(20) 
                '1'(31) ' '(20) 
                '0'(30) '-'(2D) '-'(2D) ''(0) 
                '&'(26) '^'(5E) 'O'(D8) ''(1) ''(1F) 'c'(A9) ''(1A) ''(F9) 'a'(61) ''(7F) 'B'(42) '?'(99) 'E'(CA) ''(D0) '?'(8B) 'B'(42)
                '1'(31) '.'(2E) '0'(30) '.'(2E) '1'(31) '0'(30) ''(0) 
                 */

				for (pos=0; pos < 128; )
				{
					byte c = pvSrc.ReadByte();

					c ^= m_Key[pos++ & 0xF];

                    if ( c == 0 )
                        break;

                    //Console.Write(String.Format("'{0}'({0:X}) ", c));

					cheat.Append( (char)c ); // todo: better way to convert byte to char?
				}
                
                bool allZero = true;
                bool cryptValid = true;
                StringBuilder sum = new StringBuilder();
                for ( int i = 0; i < 16; i++ )
                {
                    byte c = pvSrc.ReadByte();

                    if ( c != 0 )
                        allZero = false;

					c ^= m_Key[pos++ & 0xF];

                    sum.AppendFormat("{0:X} ", c);

                    cryptValid = cryptValid && c == CryptDLLChecksum[i];
                }

                if (!cryptValid && !allZero)
                {
                    try
                    {
                        sum.AppendFormat("\t{0}\t{1}", state.Mobile.Account, DateTime.Now);

                        using (StreamWriter sw = new StreamWriter("CryptSums.log", true))
                            sw.WriteLine(sum.ToString());
                    }
                    catch
                    {
                    }
                }

                for ( ; pos < 128; )
                {
                    byte c = pvSrc.ReadByte();

                    c ^= m_Key[pos++ & 0xF];

                    if (c == 0)
                        break;
                    version.Append( (char)c );
                }

                Version ver;
                
                try {
                    ver = new Version( version.ToString() );
                } catch {
                    ver = new Version( 0, 0, 0 );
                }

                /*Console.WriteLine( "Auth... String=\"{0}\"", cheat.ToString() );
                Console.WriteLine( "Valid={0} / allZero={1}", cryptValid, allZero );
                Console.WriteLine( "Version={0}", version.ToString() );*/

				/*int now = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
				int timeStamp = pvSrc.ReadByte() | (pvSrc.ReadByte()<<8) | (pvSrc.ReadByte()<<16) | (pvSrc.ReadByte()<<24);
				timeStamp = ~timeStamp;
				timeStamp ^= 0x54494D45;

				int serverIP = pvSrc.ReadByte() | (pvSrc.ReadByte()<<8) | (pvSrc.ReadByte()<<16) | (pvSrc.ReadByte()<<24);
				serverIP = ~serverIP;
				serverIP ^= timeStamp;*/

                string val = cheat.ToString();

				string[] split = val.Split( ' ' );

				if ( split.Length >= 2 && split[0] == "CHEAT" )
				{
					split[1] = split[1].ToUpper();

					// this could be changed to only allow certain programs (EXPLORER.EXE, UO.EXE, UOG.EXE, etc) and block all others.

					ClientValidity vd = m_Table[state] as ClientValidity;

					if ( vd == null )
						return;

					vd.m_GotResp = true;

					if ( split[1] == "RAZOR.EXE" )
					{
						vd.m_ClientType = ClientType.OldRazor;
					}
					else if ( split[1] == "INJECTION.EXE" || split[1] == "ILAUNCH.EXE" ) 
					{
						vd.m_ClientType = ClientType.Injection;
					}
					else if ( split[1] == "UO.EXE" ) // only new razor should report this.
					{
						//if ( Math.Abs( now - timeStamp ) > 60*60*15 ) // the packet is good for 15 hours only
						//	vd.m_ClientType = ClientType.FakeRazor;
						//else
                        if ( ver < CryptDLLVersion )
                            vd.m_ClientType = ClientType.OldRazor;
                        else
					        vd.m_ClientType = ClientType.NewRazor;
                        
                        if ( !allZero && !cryptValid )
                        {
                            vd.m_TPHackedDLL = true;
                        }
					}
					else if ( split[1] == "EASYUO.EXE" || split[1] == "EUOX.EXE" || split[1] == "EUO.EXE" )
					{
						vd.m_ClientType = ClientType.EasyUO;
					}
                    else if ( split[1] == "UOG.EXE" )
                    {
                        // UOG.EXE is an old razor user or a UOG only user
                    }
					else if ( split[1] == "EXPLORER.EXE" || split[1] == "IEXPLORE.EXE" || split[1] == "CONNEC" || split[1] == "CONNECTUO.EXE" || split[1] == "CUODESKTOP.EXE" )
					{
						// these are launched from ConnectUO or by double clicking client.exe
					}
					else
					{
						try 
						{
                            string str = string.Format("{0} {1}", split[1], state.Account);
							bool found = false;

							for (int i = 0; i<m_Exes.Count;i++)
							{
                                if (((string)m_Exes[i]) == str)
								{
									found = true;
									break;
								}
							}

							if ( !found )
							{
								m_Exes.Add( str );
								using ( StreamWriter sw = new StreamWriter( "ExeNames.log", true ) )
									sw.WriteLine( "{0} {1}", str, DateTime.Now );
							}
						}
						catch
						{
						}
					}
				}
			}
			else
			{
				// seek back to the begining and call the default handler
				pvSrc.Seek( 3, SeekOrigin.Begin );
				PacketHandlers.AsciiSpeech( state, pvSrc );
			}
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			ClientValidity cl;
			NetState ns;

			Mobile mob = e.Mobile;
			if ( mob == null )
				return;

			ns = mob.NetState;
			if ( ns == null || !ns.Running )
				return;

			if ( !m_Table.ContainsKey( ns ) )
				m_Table[ns] = cl = new ClientValidity( ns );
			else
				cl = (ClientValidity)m_Table[ns];

			cl.Begin();
		}

		private static void EventSink_Logout( LogoutEventArgs e )
		{
			if ( e.Mobile != null && e.Mobile.NetState != null )
			{
				if ( m_Table.ContainsKey( e.Mobile.NetState ) )
					((ClientValidity)m_Table[e.Mobile.NetState]).Detach();
			}
		}

		private static void OnThirdPartyAuth( NetState ns, bool authOK )
		{
			ClientValidity cl;
			if ( !m_Table.ContainsKey( ns ) )
				m_Table[ns] = cl = new ClientValidity( ns );
			else
				cl = (ClientValidity)m_Table[ns];

			cl.m_TPAuthed = authOK;
		}

		private static void OnThirdPartyHacked( NetState ns, bool hacked )
		{
			ClientValidity cl;
			if ( !m_Table.ContainsKey( ns ) )
				m_Table[ns] = cl = new ClientValidity( ns );
			else
				cl = (ClientValidity)m_Table[ns];

			cl.m_TPHacked = hacked;
		}

        private static void OnHandshakeResponse( NetState ns, PacketReader pvSrc )
		{
            ClientValidity cl;
            if (m_Table.ContainsKey(ns))
            {
                cl = (ClientValidity)m_Table[ns];

                cl.m_HandshakeOK = true;
            }
		}

        private sealed class BeginRazorHandshake : ProtocolExtension
		{
			public BeginRazorHandshake() : base( 0xFE, 8 )
			{
				m_Stream.Write( (uint)((ulong)Server.Network.FeatureProtection.DisabledFeatures >> 32) );
				m_Stream.Write( (uint)((ulong)Server.Network.FeatureProtection.DisabledFeatures & 0xFFFFFFFF) );
			}
		}

		private static Server.Gumps.WarningGumpCallback m_WarningCallback = new WarningGumpCallback( OnWarningOK );

		private static void OnWarningOK( Mobile from, bool OK, object state )
		{
		}
		
		private enum ClientType
		{
			Unknown,
			OldRazor,
			NewRazor,
			FakeRazor,
			Injection,
			EasyUO
		}

		private ClientType m_ClientType = ClientType.Unknown;

		private NetState m_Client;

		private int m_Count;
		private Timer m_Timer;
		private TimerCallback m_Callback;

		private bool m_GotResp = false;
		private bool m_TPAuthed = false;
		private bool m_TPHacked = false;
        private bool m_TPHackedDLL = false;
        private bool m_HandshakeOK = false;

		public ClientValidity( NetState state )
		{
			m_Client = state;
		}

		public void Begin()
		{
			Delay( 15, 30, new TimerCallback( Query ) );

            if ( FeatureProtection.DisabledFeatures != 0 )
                m_Client.Send( new BeginRazorHandshake() );
		}

		public void Detach()
		{
			if ( m_Table[m_Client] == this )
				m_Table.Remove( m_Client );
		}

		public void Query()
		{
			if ( m_Client != null && m_Client.Running )
			{
				m_Client.Send( new AsciiMessage( Serial.Zero, 0, MessageType.Regular, -1, -1, "SYSTEM\x00gqSetHelpMessage\x00sethel", "" ) );
				Delay( 10, 20, new TimerCallback( Verify ) );
			}
			else
			{
				Detach();
			}
		}

		public void Verify()
		{
			if ( m_Client != null && m_Client.Running )
			{
				bool valid = true;
				string msg = "Invalid client detected.";

				switch ( m_ClientType )
				{
					case ClientType.Injection:
						msg = "Invalid client detected.<br><b>Injection</b> is not allowed on this server. Disable <b>Injection</b> and log in again.<br><i>This violation has been noted on your account.</i>";
						if ( m_Client.Account is Accounting.Account )
							((Account)m_Client.Account).Comments.Add( new AccountComment( "Cheat Detection", String.Format( "Using Injection on {0}", DateTime.Now ) ) );
						valid = false;
						break;

					case ClientType.EasyUO:
						msg = "Invalid client detected.<br><b>EasyUO</b> is not allowed on this server. Disable <b>EasyUO</b> and log in again. <br><i>This violation has been noted on your account.</i>";
						if ( m_Client.Account is Accounting.Account )
							((Account)m_Client.Account).Comments.Add( new AccountComment( "Cheat Detection", String.Format( "Using EasyUO on {0}", DateTime.Now ) ) );
						valid = false;
						break;

					case ClientType.OldRazor:
						msg = "Invalid client detected.<br>You are using an old version of Razor.<br>You must upgrade to the newest version in order to play on this server.  Visit <a href=\"https://github.com/msturgill/razor\">Razor's Webpage</a> to download the latest version or run 'Updater.exe' in your Razor directory.<br><br>Only Razor <b>"+CryptDLLVersion+"</b> or higher is allowed on this server.";
						valid = false;
						break;

					case ClientType.NewRazor:
						if ( FeatureProtection.DisabledFeatures != 0 && !( m_TPAuthed || m_HandshakeOK ) )
						{
							msg = "Unable to verify Razor features.<br>Please check the <u>Negotiate features with server</u> box on the <u>Options</u> tab and then log in again.<br><br>Only Razor <b>v1.0.9</b> or higher is allowed on this server, make sure you have the latest version.";
							valid = false;
						}
						else
						{
							valid = true;
						}
						break;
						
					case ClientType.Unknown:
					default:
						if ( !m_GotResp )
						{
							msg = "Invalid client detected.<br>PlayUO/KUOC and old OSI clients are not allowed on this server.<br><br>You must use an OSI client version 4.0.0 or later.  To patch your OSI client automatically, run the 'UOPatch.exe' program which can be found in your UO install folder.<br><br>Once you have patched your client, log in again.";
							valid = false;
						}
						else
						{
							valid = true;
						}
						break;
				}

                if (m_Client.Mobile.AccessLevel >= AccessLevel.Administrator)
                    m_Client.Mobile.SendMessage("Client Verification: Valid={5} Type={0} Resp={4} Authed={1} Hacked={2}/{6} Handshaken={3}", m_ClientType, m_TPAuthed, m_TPHacked, m_HandshakeOK, m_GotResp, valid, m_TPHackedDLL);

				if ( m_TPHacked ) // hacked "idoc" razors
				{
					// innocent sounding message.
					msg = "Invalid client detected.<br>This Razor version is invalid, you need to reinstall Razor.";

					// comment their account
					if ( m_Client != null && m_Client.Account is Accounting.Account )
						((Account)m_Client.Account).Comments.Add( new AccountComment( "Cheat Detection", String.Format( "Using hacked Razor (Ultima) on {0}", DateTime.Now ) ) );

					//valid = false;
				}

                /*if ( m_TPHackedDLL ) // hacked "features" razors
				{
					// innocent sounding message.
					msg = "Invalid client detected.<br>This Razor version is invalid, you need to reinstall Razor.";

					// comment their account
					if ( m_Client != null && m_Client.Account is Accounting.Account )
						((Account)m_Client.Account).Comments.Add( new AccountComment( "Cheat Detection", String.Format( "Using hacked Razor (Crypt) on {0}", DateTime.Now ) ) );

					//valid = false;
				}*/

				if ( !valid )
				{
					string warning = "";

					//m_Client.Send( new AsciiMessage( Serial.MinusOne, -1, MessageType.Regular, 0x22, 3, "System", msg ) );
					
					if ( m_Client.Mobile != null && m_Client.Mobile.AccessLevel >= AccessLevel.Counselor )
					{
						warning = String.Format( "{0}<br><br>(You are staff, so you will not be disconnected.)", msg );
					}
					else
					{
						warning = String.Format( "{0}<br><br><u>You will be disconnected in 5 seconds.</u>", msg );
						Delay( 5, 5, new TimerCallback( TheBoot ) );
					}

					if ( m_Client.Mobile != null )
						m_Client.Mobile.SendGump( new WarningGump( 1060635, 30720, warning, 0xFFC000, 420, 250, m_WarningCallback, null ) );
				}
			}

			Detach();
		}

		public void TheBoot()
		{
			if ( m_Client != null && m_Client.Running )
			{
				m_Client.Send( new AsciiMessage( Serial.MinusOne, -1, MessageType.Regular, 0x22, 3, "System", "Disconnecting invalid client." ) );
				m_Client.Dispose();
			}
		}

		private void Delay( int min, int max, TimerCallback callback )
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Count = Utility.RandomMinMax( min, max );
			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Delay_Callback ) );
			m_Callback = callback;
		}

		private void Delay_Callback()
		{
			if ( --m_Count == 0 )
			{
				m_Timer.Stop();
				m_Timer = null;

				m_Callback();
			}
		}
	}
}

