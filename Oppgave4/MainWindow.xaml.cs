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

        public delegate void deleg(double time, double scale);
        public event deleg moveStuff;

        private System.Windows.Threading.DispatcherTimer t;
        private Star sun;
        private List<Planet> planets = new List<Planet>();
        private List<Moon> moons = new List<Moon>();
        private double time=0;

        double speedOfSimulation = 1000000;
        double simSpeed;
        double speedOfSim2 = 1;

        int hideText = 0;
        double scaleRad = 1;
        double scaledist = 55;

        public MainWindow()
        {
            InitializeComponent();
            loadData();
            simSpeed = 1.0 / speedOfSimulation;
            t = new System.Windows.Threading.DispatcherTimer();
            t.Interval = new TimeSpan((long)(1000000 * simSpeed));
            t.Tick += t_Tick;
            t.Start();

            sun.Shape = CreateAnEllipse(sun);
            moveStuff += sun.LogCalculatePosition;
            foreach (SpaceObject so in planets)
            {
                moveStuff += so.LogCalculatePosition;
            }
            foreach (SpaceObject so in planets)
            {
                so.Shape = CreateAnEllipse((Planet)so);
                so.Shape.MouseLeftButtonDown += viewPlanet;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = so.Name;
                Canvas.SetLeft(textBlock, so.Xlog);
                Canvas.SetTop(textBlock, so.Ylog);
                canvas.Children.Add(textBlock);
                so.txt = textBlock;
            }
            foreach(Moon m in moons)
            {
                m.Shape = CreateAnEllipse((Moon)m);
                m.Hide();
                TextBlock textBlock = new TextBlock();
                textBlock.Text = m.Name;
                Canvas.SetLeft(textBlock, m.Xlog);
                Canvas.SetTop(textBlock, m.Ylog);
                canvas.Children.Add(textBlock);
                m.txt = textBlock;
            }

        }
        void viewSystem(Object sender, EventArgs e)
        {
            foreach (Moon m in moons)
            {
                m.Hide();
                moveStuff -= m.LogCalculatePosition;
            }
            Canvas.SetLeft(sun.Shape, -sun.Shape.Width / 2 + canvas.Width/2);
            foreach (SpaceObject so in planets)
            {
                moveStuff += so.LogCalculatePosition;
            }
            foreach (SpaceObject so in planets)
            {
                so.Shape.MouseLeftButtonDown += viewPlanet;
            }
            planetInfo.Content = "This is the Solar System";
        }
        void viewPlanet(Object sender, EventArgs e)
        {
            canvas.MouseRightButtonDown += viewSystem;
            Canvas.SetLeft(sun.Shape, -1000);
            foreach (SpaceObject so in planets)
            {
                moveStuff -= so.LogCalculatePosition;
                so.Hide();
            }
            Planet planet = null;
            foreach(Planet p in planets)
            {
                if(p.Shape == (Ellipse)sender)
                {
                    planet = p;
                    break;
                }
            }
            planet.Xlog = (int)scaledist;
            planet.Ylog = (int)scaledist;
            int mooncounter = 0;
            foreach (Moon moon in moons)
            {
                if(moon.Planet == planet)
                {
                    moveStuff += moon.LogCalculatePosition;
                    mooncounter++;
                }
            }

            String infoStr = "";
            infoStr += "Name: "+planet.Name + "\n";
            infoStr += "Radius: "+planet.Radius + "\n";
            infoStr += "Orbits: "+planet.Star.Name + "\n";
            infoStr += "Orbital Period: "+planet.OrbitalPeriod + "\n";
            infoStr += "Orbital Radius: "+planet.OrbitalRadius + "\n";
            infoStr += "Number of moons: "+mooncounter + "\n";
            
            planetInfo.Content = infoStr;
        }

        void t_Tick(object sender, EventArgs e)
        {
            if(moveStuff != null)
                moveStuff(time+=(0.1*speedOfSim2), scaledist);
            Draw();
        }

        void Draw()
        {
            if ((bool)showNames.IsChecked)
                hideText = 0;
            else
                hideText = 10000;
            simSpeed = 1.0 / speedOfSimulation;
            t.Interval = new TimeSpan((long)(1000000 * simSpeed));
            foreach (SpaceObject so in planets)
            {
                double x = so.PositionLog.X + (canvas.Width / 2) - (so.Shape.Width / 2)-1*scaledist;
                double y = so.PositionLog.Y + (canvas.Height / 2) - (so.Shape.Height / 2)-1*scaledist;
                Canvas.SetLeft(so.Shape, x);
                Canvas.SetTop(so.Shape, y);
                Canvas.SetLeft((so.txt as TextBlock), x-hideText);
                Canvas.SetTop((so.txt as TextBlock), y-15);
            }
            foreach (SpaceObject so in moons)
            {
                double x = so.PositionLog.X + (canvas.Width / 2) - (so.Shape.Width / 2) - 2 * scaledist;
                double y = so.PositionLog.Y + (canvas.Height / 2) - (so.Shape.Height / 2) - 2 * scaledist;
                Canvas.SetLeft(so.Shape, x);
                Canvas.SetTop(so.Shape, y);
                Canvas.SetLeft((so.txt as TextBlock), x-hideText);
                Canvas.SetTop((so.txt as TextBlock), y - 15);
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
                            moons.Add(new Moon(linje[0], Convert.ToInt32(linje[3]),
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

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speedOfSimulation = speedSlider.Value;
            if(speedSlider2 != null)
                speedOfSim2 = speedSlider2.Value;
            speedLabel.Content = "Speed: " + speedOfSimulation + " & "
                + (speedSlider2 == null ? "*":speedOfSim2.ToString());
        }
    }


}
