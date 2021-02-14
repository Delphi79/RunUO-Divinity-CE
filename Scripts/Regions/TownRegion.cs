using System;
using System.Xml;
using Server;

namespace Server.Regions
{
	public class TownRegion : GuardedRegion
	{
		public TownRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

        public TownRegion(string name, Map map, int priority, params Rectangle2D[] area)
            : base(name, map, priority, area)
        {
        }

        public TownRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }
	}
}
