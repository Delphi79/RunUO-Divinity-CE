using System;

namespace Server
{
	[AttributeUsage( AttributeTargets.Class )]
	public class CorpseNameAttribute : Attribute
	{
		private string m_Name;

		public string Name
		{
			get{ return m_Name; }
            set{ m_Name = value; }
		}

		public CorpseNameAttribute( string name )
		{
			m_Name = name;
		}
	}
}