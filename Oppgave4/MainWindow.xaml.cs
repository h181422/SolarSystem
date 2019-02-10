using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpaceObjects;
using System.Drawing;

namespace Oppgave4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Fil med info om planeter
        const String FILPLASSERING = "Planeter.csv";
        const int TIMER_INTERVAL = 1000000;

        public delegate void deleg(int time, double scale);
        public event deleg moveStuff;

        private System.Windows.Threading.DispatcherTimer t;
        private Star sun;
        private List<Planet> planets = new List<Planet>();
        private List<Moon> moons = new List<Moon>();
        private int time=0;

        double speedOfSimulation = 100;
        double simSpeed;

        double scaleRad = 1;
        double scaledist = 55;

        public MainWindow()
        {
            InitializeComponent();
            loadData();
            simSpeed = 1.0 / speedOfSimulation;
            t = new System.Windows.Threading.DispatcherTimer();
            t.Interval = new TimeSpan((long)(TIMER_INTERVAL*simSpeed));
            t.Tick += t_Tick;
            t.Start();

            sun.Shape = CreateAnEllipse(sun);
            foreach (SpaceObject so in planets)
            {
                moveStuff += so.LogCalculatePosition;
            }
            foreach (SpaceObject so in planets)
            {
                so.Shape = CreateAnEllipse((Planet)so);
            }
            foreach (SpaceObject so in moons)
            {
                moveStuff += so.LogCalculatePosition;
            }
            foreach (SpaceObject so in moons)
            {
                so.Shape = CreateAnEllipse((Moon)so);
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            if(moveStuff != null)
                moveStuff(time++, scaledist);
            Draw();
        }

        void Draw()
        {
            foreach (SpaceObject so in planets)
            {
                double x = so.PositionLog.X + (canvas.Width / 2) - (so.Shape.Width / 2)-1*scaledist;
                double y = so.PositionLog.Y + (canvas.Height / 2) - (so.Shape.Height / 2)-1*scaledist;
                Canvas.SetLeft(so.Shape, x);
                Canvas.SetTop(so.Shape, y);
            }
        }

        private void loadData()
        {
            planets = new List<Planet>();
            moons = new List<Moon>();
            string[] textlinjerFraFil = System.IO.File.ReadAllLines(FILPLASSERING);
            for(int i = 0; i < textlinjerFraFil.Length; i++)
            {
                string[] linje = textlinjerFraFil[i].Split(';');
                if (linje[0] == "")
                    continue;
                if(i == 0)
                    sun = new Star("Sun", 0, 0,Convert.ToInt32(linje[11]), 0,
                        System.Drawing.Color.FromArgb(255, 255, 255, 0));
                else if (i < 10)
                    planets.Add(new Planet(linje[0], Convert.ToInt32(linje[3]),
                        (int)Convert.ToDouble(linje[4]),Convert.ToInt32(linje[11]),
                        0,System.Drawing.Color.Blue,sun));
                else if(i==10)
                    planets.Add(new DwarfPlanet(linje[0], Convert.ToInt32(linje[3]),
                        (int)Convert.ToDouble(linje[4]), Convert.ToInt32(linje[11]),
                        0, System.Drawing.Color.Blue, sun));
                else
                {
                    foreach(Planet p in planets)
                    {
                        if (linje[2] == p.Name)
                            moons.Add(new Moon(linje[2], Convert.ToInt32(linje[3]),
                                (int)Convert.ToDouble(linje[4]), Convert.ToInt32(linje[11]),
                                0, System.Drawing.Color.Gray, p));
                    }
                }
            }
        }

        
        public Ellipse CreateAnEllipse(SpaceObject so)
        {
            int xPos = so.X;
            int yPos = so.Y;
            Ellipse sirkel = new Ellipse();
            //sirkel.Height = logN(so.Radius * 2,2) * scaleRad;
            sirkel.Height = Math.Log(so.Radius*2) * scaleRad;
            sirkel.Width = sirkel.Height;

            SolidColorBrush colBrush = new SolidColorBrush();
            colBrush.Color = Colors.Yellow;
            sirkel.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                so.Color.A, so.Color.R, so.Color.G, so.Color.B));

            double xp = (xPos == 0 ? xPos : Math.Log(xPos) * scaledist - 10 * scaledist);
            double yp = (yPos == 0 ? yPos : Math.Log(yPos) * scaledist - 10 * scaledist);
            Canvas.SetLeft(sirkel, xp + (canvas.Width / 2) - (sirkel.Width / 2));
            Canvas.SetTop(sirkel, yp + (canvas.Height / 2) - (sirkel.Height / 2));
            
            canvas.Children.Add(sirkel);
            return sirkel;
        }

    }


}
