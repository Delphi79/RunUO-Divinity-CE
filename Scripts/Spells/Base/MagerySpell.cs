using System;
using Server;
using Server.Items;
using System.IO;

namespace Server.Spells
{
	public abstract class MagerySpell : Spell
	{
        public static double[] SpeedTable = new double[]
			{
			//	x * circle + y
			//	x,		y,
				0.25,	0.5,
			};

		public static int[] DamageTable = new int[]
			{
			// dice rolls xdy + z
			//	x,	y,	z,
				2,	2,	1, //2,	2,	2, old (4-6) new (3-5) ma
				1,	8,	3, //1,	8,	4  old (5-12) new (4-11) harm 7.5
				3,	4,	5, //3,	5,	1, old (5-17) new (8-17) fb 13
				5,	2,	10, //3,	6,	4, old (7-22) new (15-20) lb 16
				5,	5,	5, //5,	5,	5,
				3,	5,	16, //6,	6,	4, old (10-40) new (19-31) exp/eb
				4,	5,	25, //8,	5,	5, old (13-45) new (29-45) FS
				8,	5,	5, //8,	5,	5,
			};

		public static void LoadTables()
		{
			StreamReader sr = new StreamReader(new FileStream("Data/spelltables.cfg", FileMode.Open, FileAccess.Read, FileShare.Read));

			double[] speedTable = new double[2];

			string line = sr.ReadLine();
			string[] parts = line.Split(' ');

			speedTable[0] = Double.Parse( parts[0] );
			speedTable[1] = Double.Parse( parts[1] );

			int[] damageTable = new int[8 * 3];

			for ( int row = 0; row < 8; row++ )
			{
				line = sr.ReadLine();
				parts = line.Split(' ');
                
				damageTable[row * 3] = Int32.Parse( parts[0] );
				damageTable[row * 3 + 1] = Int32.Parse( parts[1] );
				damageTable[row * 3 + 2] = Int32.Parse( parts[2] );
			}

			sr.Close();

			SpeedTable[0] = speedTable[0];
			SpeedTable[1] = speedTable[1];

			for ( int i = 0; i < 8 * 3; i++ )
				DamageTable[i] = damageTable[i];
		}

		public static void SaveTables()
		{
			StreamWriter sw = new StreamWriter(new FileStream("Data/spelltables.cfg", FileMode.Create, FileAccess.Write, FileShare.Write));

			sw.WriteLine( String.Format( "{0} {1}", SpeedTable[0], SpeedTable[1] ) );

			for ( int row = 0; row < 8; row++ )
				sw.WriteLine( String.Format( "{0} {1} {2}", DamageTable[row * 3], DamageTable[row * 3 + 1], DamageTable[row * 3 + 2] ) );

			sw.Close();
		}

		public static void Configure()
		{
			try
			{
				LoadTables();
			}
			catch (Exception e)
			{
				Console.WriteLine( "Couldn't load spell tables: {0} Using default tables.", e.Message );
                SaveTables();
			}
		}

		public MagerySpell( Mobile caster, Item scroll, SpellInfo info )
			: base( caster, scroll, info )
		{
		}

		public abstract SpellCircle Circle { get; }

        public static int GetPreUORDamage(SpellCircle Circle)
        {
            int circle = (int)Circle;

            if (circle < 0 || circle > 7)
                return 1;
            else
            {
                int row = circle;
                int dmg = Utility.Dice(DamageTable[row * 3], DamageTable[row * 3 + 1], DamageTable[row * 3 + 2]);
                return dmg;
            }

            /*switch ( Circle )
            {
                case SpellCircle.First:		return Utility.Dice( 1, 3, 3 );
                case SpellCircle.Second:	return Utility.Dice( 1, 8, 4 );
                case SpellCircle.Third:		return Utility.Dice( 4, 4, 4 );
                case SpellCircle.Fourth:	return Utility.Dice( 3, 8, 5 );
                case SpellCircle.Fifth:		return Utility.Dice( 5, 8, 6 );
                case SpellCircle.Sixth:		return Utility.Dice( 6, 8, 8 );
                case SpellCircle.Seventh:	return Utility.Dice( 7, 8, 10 );
                case SpellCircle.Eighth:	return Utility.Dice( 7, 8, 10 );
                default:					return 1;
            }*/
        }

        public int GetPreUORDamage()
        {
            return GetPreUORDamage(Circle);
        }

		public override bool ConsumeReagents()
		{
			if( base.ConsumeReagents() )
				return true;

			if( ArcaneGem.ConsumeCharges( Caster, (Core.SE ? 1 : 1 + (int)Circle) ) )
				return true;

			return false;
		}

		private const double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.0;

		public override void GetCastSkills( out double min, out double max )
		{
			int circle = (int)Circle;

			if( Scroll != null )
				circle -= 2;

			double avg = ChanceLength * circle;

			min = avg - ChanceOffset;
			max = avg + ChanceOffset;
		}

		public static readonly int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };

		public override int GetMana()
		{
			if( Scroll is BaseWand )
				return 0;

			return m_ManaTable[(int)Circle];
		}

		public override double GetResistSkill( Mobile m )
		{
			int maxSkill = (1 + (int)Circle) * 10;
			maxSkill += (1 + ((int)Circle / 6)) * 25;

			if( m.Skills[SkillName.MagicResist].Value < maxSkill )
				m.CheckSkill( SkillName.MagicResist, maxSkill - 40, maxSkill );

			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual bool CheckResisted( Mobile target )
		{
            return CheckResistedStatic( target, Caster, (int)Circle );
		}

        public static bool CheckResistedStatic(Mobile target, Mobile Caster, int Circle)
        {
            double n = GetResistPercentForCircleStatic(target, Caster, Circle);

            n /= 100.0;

            n /= 2.0; // Seems should be about half of what stratics says.
            //n /= 2.0; // for good measure? because jakob says so?

            if (n <= 0.0)
                return false;

            if (n >= 1.0)
                return true;

            int maxSkill = (1 + Circle) * 15; // 7th = 75 to 105, 6th = 60 to 90, 4th = 30 to 60
            int minSkill = maxSkill - 30;
            if (target.Skills[SkillName.MagicResist].Value < maxSkill)
                target.CheckSkill(SkillName.MagicResist, minSkill, maxSkill);

            /*int maxSkill = (1 + Circle) * 10;
            maxSkill += (1 + (Circle / 6)) * 25;

            if (target.Skills[SkillName.MagicResist].Value < maxSkill)
                target.CheckSkill(SkillName.MagicResist, 0.0, 120.0);*/

            return (n >= Utility.RandomDouble());
        }

        public static bool CheckResistedEasyStatic( Mobile target, int Circle )
        {
            int sk = (1 + Circle) * 10; // easy resist for mana vamp, poison, etc
            if (!target.Player)
                sk *= 2; // make it easy to use these spells on monsters
            return target.CheckSkill(SkillName.MagicResist, sk - 20, sk + 20);
        }

        public virtual bool CheckResistedEasy( Mobile target )
        {
            return CheckResistedEasyStatic( target, (int)Circle );
        }

        public static double GetResistPercentForCircleStatic( Mobile target, Mobile Caster, int circle )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[SkillName.Magery].Value - 20.0) / 5.0) + (1 + circle) * 5.0);

            return Math.Max( firstPercent, secondPercent );
		}

		public virtual double GetResistPercentForCircle( Mobile target, SpellCircle circle )
		{
            return GetResistPercentForCircleStatic( target, Caster, (int)circle );
        }

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target, Circle );
		}

		public override TimeSpan GetCastDelay()
		{
			if( Scroll is BaseWand )
				return TimeSpan.Zero;

			//if( !Core.AOS )
			//	return TimeSpan.FromSeconds( 0.5 + (0.25 * (int)Circle) );

			//return base.GetCastDelay();
            return TimeSpan.FromSeconds( SpeedTable[0] * (int)Circle + SpeedTable[1] );
		}

		public override TimeSpan CastDelayBase
		{
			get
			{
				return TimeSpan.FromSeconds( (3 + (int)Circle) * CastDelaySecondsPerTick );
			}
		}

        public override TimeSpan GetCastRecovery()
        {
            double scalar = 1.00;
            switch ( Circle )
            {
                case SpellCircle.First:
                    scalar = 2.00;
                    break;
                case SpellCircle.Second:
                    scalar = 1.50;
                    break;
                case SpellCircle.Third:
                    scalar = 1.00;
                    break;
                case SpellCircle.Fourth:
                case SpellCircle.Fifth:
                    scalar = 0.75;
                    break;
                case SpellCircle.Sixth:
                case SpellCircle.Seventh:
                case SpellCircle.Eighth:
                    scalar = 0.50;
                    break;
            }
            return TimeSpan.FromSeconds( base.GetCastRecovery().TotalSeconds * scalar );
        }
	}
}
