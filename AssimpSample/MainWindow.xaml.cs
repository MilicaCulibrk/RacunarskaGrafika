using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

  

        float slider = -1000;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

      

      
        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Person"), "stickman.OBJ", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cb1.ItemsSource = Enum.GetValues(typeof(VerticalScaling));
            cb1.SelectedIndex = 0;
            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                 case Key.E:
                    if (World.startAnimation == false)
                    {
                        if (m_world.RotationX > -85.0f)
                        {
                            m_world.RotationX -= 5f;
                        }

                    }
                        break;
                case Key.D:
                    if (World.startAnimation == false)
                    {
                        if (m_world.RotationX < 85.0f)
                        {
                            m_world.RotationX += 5f;
                        }

                    }
                        break;
                case Key.S:
                    if (World.startAnimation == false)
                    {
                        m_world.RotationY -= 5f;

                    }
                    break;
                case Key.F:
                    if (World.startAnimation == false)
                    {
                        m_world.RotationY += 5f;

                    }
                    break;
                case Key.Subtract:
                    if (World.startAnimation == false)
                    {
                        m_world.SceneDistance += 100.0f;

                    }
                    break;

                case Key.Add:
                    if (World.startAnimation == false)
                    {
                        m_world.SceneDistance -= 100.0f;

                    }
                    break;
                case Key.V:
                    if (World.startAnimation)
                    {
                        World.timer.Stop();
                        World.timer = null;                      
                        World.startAnimation = false;
                        cb1.IsEnabled = true;
                        
                       
                    }
                    else
                    {
                        World.startAnimation = true;
                        World.timer = new System.Windows.Threading.DispatcherTimer();
                        World.timer.Tick += new EventHandler(World.Animacija);
                        World.timer.Interval = TimeSpan.FromMilliseconds(-slider);
                        World.x = -180f;
                        World.y = -205f;
                        World.z = -100f;
                        cb1.IsEnabled = false;
                        World.timer.Start();
                    }
                    break;
            }


        }

     

     
        private void rMinus(object sender, RoutedEventArgs e)
        {
            if (m_world.rAmbient > 0.0f)
            {
                m_world.rAmbient -= 0.1f;

            }
        }
        private void gMinus(object sender, RoutedEventArgs e)
        {

            if (m_world.gAmbient > 0.0f)
            {
                m_world.gAmbient -= 0.1f;
            }
        }
        private void bMinus(object sender, RoutedEventArgs e)
        {
            if (m_world.bAmbient > 0.0f)
            {
                m_world.bAmbient -= 0.1f;
            }
        }
        private void rPlus(object sender, RoutedEventArgs e)
        {
            if (m_world.rAmbient < 1.0f)
            {
                m_world.rAmbient += 0.1f;
            }
        }
        private void gPlus(object sender, RoutedEventArgs e)
        {
            if (m_world.gAmbient < 1.0f)
            {
                m_world.gAmbient += 0.1f;
            }
        }
        private void bPlus(object sender, RoutedEventArgs e)
        {
            if (m_world.bAmbient < 1.0f)
            {
                m_world.bAmbient += 0.1f;
            }
        }

        private void sliderBrzina_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider = (float)e.NewValue;

            if (m_world != null && World.startAnimation == true)
                World.ubrzajAnimaciju(-slider);
        }

    

        private void changeVerticalScale(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.PersonVerticalScaling = (VerticalScaling)cb1.SelectedIndex;
        }


    }
}

