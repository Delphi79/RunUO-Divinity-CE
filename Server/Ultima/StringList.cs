using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Ultima
{
	public class StringList
	{
		private Hashtable m_Table;
		private StringEntry[] m_Entries;
		private string m_Language;

        private Hashtable m_EntTable;

		public StringEntry[] Entries{ get{ return m_Entries; } }
		public Hashtable Table{ get{ return m_Table; } }
		public string Language{ get{ return m_Language; } }

		private static byte[] m_Buffer = new byte[1024];

		public string Format( int num, params object[] args )
		{
            StringEntry ent = m_EntTable[num] as StringEntry;
			
            if ( ent != null )
                return ent.Format( args );
            else
			    return String.Format( "CliLoc string {0}/{1} not found!", num, m_EntTable.Count );
		}

		public string SplitFormat( int num, string argstr )
		{
            StringEntry ent = m_EntTable[num] as StringEntry;

            if (ent != null)
                return ent.SplitFormat(argstr);
            else
                return String.Format("CliLoc string {0}/{1} not found!", num, m_EntTable.Count);
		}

		public StringList( string language )
		{
			m_Language = language;
			m_Table = new Hashtable();
            m_EntTable = new Hashtable();

			string path = Client.GetFilePath( String.Format( "cliloc.{0}", language ) );

			if ( path == null || !File.Exists( path ) )
			{
                Console.WriteLine("Error! Unable to open {0}!", path);
				m_Entries = new StringEntry[0];
				return;
			}

			ArrayList list = new ArrayList();

			using ( BinaryReader bin = new BinaryReader( new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read ), Encoding.UTF8 ) )
			{
				bin.ReadInt32();
				bin.ReadInt16();

				try
				{
					while ( true )
					{
						int number = bin.ReadInt32();
						bin.ReadByte();
						int length = bin.ReadInt16();

						if ( length > m_Buffer.Length )
							m_Buffer = new byte[(length + 1023) & ~1023];

						bin.Read( m_Buffer, 0, length );

						try
						{
                            StringEntry ent;
							string text = Encoding.UTF8.GetString( m_Buffer, 0, length );

                            ent = new StringEntry(number, text);
							list.Add( ent );
                            m_EntTable[number] = ent;
							m_Table[number] = text;
						}
						catch
						{
						}
					}
				}
				catch ( System.IO.EndOfStreamException )
				{
					// end of file.  stupid C#.
				}
			}

			m_Entries = (StringEntry[])list.ToArray( typeof( StringEntry ) );
		}
	}
}
