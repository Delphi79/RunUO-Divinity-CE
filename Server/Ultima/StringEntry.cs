using System;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Ultima
{
	public class StringEntry
	{
		private static Regex m_RegEx = new Regex( @"~(\d+)[_\w]+~", RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.CultureInvariant );
		private int m_Number;
		private string m_Text;
		private string m_FmtTxt;

		public int Number{ get{ return m_Number; } }
		public string Text{ get{ return m_Text; } }

		public StringEntry( int number, string text )
		{
			m_Number = number;
			m_Text = text;
			m_FmtTxt = null;
		}

		private static object[] m_Args = new object[]{ "", "", "", "", "", "", "", "", "", "", "" };

		public string Format( params object[] args )
		{
			if ( m_FmtTxt == null )
				m_FmtTxt = m_RegEx.Replace( m_Text, @"{$1}" );
			for(int i=0;i<args.Length && i<10;i++)
				m_Args[i+1] = args[i];
			return String.Format( m_FmtTxt, m_Args );
		}

		public string SplitFormat( string argstr )
		{
			if ( m_FmtTxt == null )
				m_FmtTxt = m_RegEx.Replace( m_Text, @"{$1}" );
			string[] args = argstr.Split( '\t' );// adds an extra on to the args array
			for(int i=0;i<args.Length && i<10;i++)
				m_Args[i+1] = args[i];
			return String.Format( m_FmtTxt, m_Args );
			/*
			{
				StringBuilder sb = new StringBuilder();
				sb.Append( m_FmtTxt );
				for(int i=0;i<args.Length;i++)
				{
					sb.Append( "|" );
					sb.Append( args[i] == null ? "-null-" : args[i] );
				}
				throw new Exception( sb.ToString() );
			}*/
		}
	}
}
