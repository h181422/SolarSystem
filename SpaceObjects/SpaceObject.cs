using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Media;



namespace SpaceObjects
{
    public class SpaceObject
    {
        protected String name;
        protected int orbitalRadius;
        protected int orbitalPeriod;
        protected int radius;
        protected int rotationalPeriod;
        protected Color col;
        protected Point position;
        protected Point positionLog;
        public String Name { get { return name; } set { name = value; } }
        public int OrbitalRadius { get { return orbitalRadius; } set { orbitalRadius = value; } }
        public int OrbitalPeriod { get { return orbitalPeriod; } set { orbitalPeriod = value; } }
        public int Radius { get { return radius; } set { radius = value; } }
        public int RotationalPeriod { get { return rotationalPeriod; } set { rotationalPeriod = value; } }
        public Color Color { get { return col; } set { col = value; } }
        public Point Position { get { return position; } set { position = value; } }
        public Point PositionLog { get { return positionLog; } set { positionLog = value; } }
        public void setPosition(int x, int y) { Position = new Point(x, y); }
        public int X { get { return position.X; } set { position.X = value; } }
        public int Y { get { return position.Y; } set { position.Y = value; } }
        public int Xlog { get { return positionLog.X; } set { positionLog.X = value; } }
        public int Ylog { get { return positionLog.Y; } set { positionLog.Y = value; } }
        public Object txt { get; set; }
        public Ellipse Shape { get; set; }
        public void Hide()
        {
            PositionLog = new Point(-1000, 0);
        }
        public SpaceObject(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col)
        {
            this.name = name;
            this.orbitalRadius = orbitalRadius;
            this.orbitalPeriod = orbitalPeriod;
            this.radius = radius;
            this.rotationalPeriod = rotationalPeriod;
            this.col = col;
        }

        public virtual void CalculatePosition(double time)
        {
            int orbitX = 0;
            int orbitY = 0;
            this.position = new Point(orbitX + (int)(OrbitalRadius * 
                Math.Cos(Math.PI * time / (OrbitalPeriod/2.0))),
                orbitY + (int)(OrbitalRadius * 
                Math.Cos(Math.PI * time / (OrbitalPeriod/2.0) + (Math.PI/2.0))));
        }
        public virtual void LogCalculatePosition(double time, double scaledist)
        {
            int orbitX = 0;
            int orbitY = 0;
            this.PositionLog = new Point(orbitX + (int)(LogOrbitRadius(scaledist) *
                Math.Cos(Math.PI * time / (OrbitalPeriod / 2.0)))+(int)scaledist,
                orbitY + (int)(LogOrbitRadius(scaledist) *
                Math.Cos(Math.PI * time / (OrbitalPeriod / 2.0) + (Math.PI / 2.0))) + (int)scaledist);
        }
        public virtual double LogOrbitRadius(double scaledist)
        {
            return Math.Log(Math.Abs(this.OrbitalRadius)) * scaledist-10*scaledist;
        }
        
        protected void CalculatePosition(double time, Point p)
        {
            int orbitX = p.X;
            int orbitY = p.Y;
            this.position = new Point(orbitX + (int)(OrbitalRadius *
                Math.Cos(Math.PI * time / (OrbitalPeriod / 2.0))),
                orbitY + (int)(OrbitalRadius *
                Math.Cos(Math.PI * time / (OrbitalPeriod / 2.0) + (Math.PI / 2.0))));
        }

    }
    public class Star : SpaceObject
    {
        public Star(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col) { }


        public override void CalculatePosition(double time)
        {
            this.position = new Point(0,0);
        }
    }
    public class Planet : SpaceObject
    {
        Star star;
        public Planet(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col, Star star)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col)
        {
            this.star = star;
        }
        public Star Star { get { return star; } set { star = value; } }

        public override void CalculatePosition(double time)
        {
            base.CalculatePosition(time, star.Position);
        }
    }
    public class Moon : SpaceObject
    {
        Planet planet;
        public Moon(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col, Planet planet)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col)
        {
            this.planet = planet;
        }
        public Planet Planet { get { return planet; } set { planet = value; } }

        public override double LogOrbitRadius(double scaledist)
        {
            return Math.Log(Math.Abs(this.OrbitalRadius)) * scaledist - 7 * scaledist;
        }
        public override void CalculatePosition(double time)
        {
            base.CalculatePosition(time, planet.Position);
        }
        public override void LogCalculatePosition(double time, double scaledist)
        {
            int orbitX = Planet.PositionLog.X;
            int orbitY = Planet.PositionLog.Y;
            this.PositionLog = new Point(orbitX + (int)(LogOrbitRadius(scaledist) *
                Math.Cos(Math.PI * time / (OrbitalPeriod / 2.0))) + (int)scaledist,
                orbitY + (int)(LogOrbitRadius(scaledist) *
                Math.Cos(Math.PI * time / (OrbitalPeriod / 2.0) + (Math.PI / 2.0))) + (int)scaledist);
            CalculatePosition(time);
        }
    }
    public class Comet : SpaceObject
    {
        public Comet(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col) { }
        
    }
    public class Asteroid : SpaceObject
    {
        public Asteroid(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col) { }
        
    }
    public class AsteroidBelt : SpaceObject
    {
        public AsteroidBelt(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col) { }
        
    }
    public class DwarfPlanet : Planet
    {
        public DwarfPlanet(String name, int orbitalRadius, int orbitalPeriod, int radius,
            int rotationalPeriod, Color col, Star star)
            : base(name, orbitalRadius, orbitalPeriod, radius, rotationalPeriod, col, star) { }
    }
}
