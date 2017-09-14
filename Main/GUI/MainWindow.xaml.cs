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
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Diagnostics;
using Model;
using DMS.Geometry;
using Model.Objects;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private  GLControl _glc;

        private bool _rotating;
        private System.Drawing.Point _rotatingStart;
        public GLControl Glc {
            get { return _glc; }
        }
        public Scene Scene {
            get;
            set;
        }
        
        private System.Timers.Timer timer;
        public MainWindow()
        {
            InitializeComponent();

            _rotating = false;
            _glc = new GLControl();
            Scene = new Scene();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenTK.Toolkit.Init();

            //_glc.BackColor = System.Drawing.Color.DarkGray;
            _glc.Load += new EventHandler(glc_Load);
            _glc.Paint += new PaintEventHandler(glc_Paint);

            // Assign the GLControl as the host control's child.
            host.Child = _glc;

            //_glc.Height = (int)host.ActualHeight;
            //_glc.Width = (int)host.ActualWidth;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            sw.Stop();
            Debug.WriteLine("Time needed to load Model: " + sw.Elapsed.ToString());

            // Timer for 60 fps Render.
            timer = new System.Timers.Timer() {
                Interval = 1000 / 60,
                AutoReset = true,
            };

            timer.Elapsed += OnTimedEvent;
            // Start the timer
            timer.Enabled = true;

        }
        


        private void glc_Load(object sender, EventArgs e)
        {

            // Make background "chocolate"
            GL.ClearColor(System.Drawing.Color.DarkGray);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);


            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();

            int w = Glc.Width;
            int h = Glc.Height;

            float orthoW = w;
            float orthoH = h;

            //GL.Ortho(0, orthoW, 0, orthoH, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

            _glc.Resize += _glc_Resize;

            _glc.MouseMove += _glc_MouseMove;

            _glc.MouseWheel += _glc_MouseWheel;
            
        }

        private void _glc_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down)
            {
                Scene.Camera.FovY *= (float)Math.Pow(1.05, e.Delta);
            }
            else
            {
                Debug.WriteLine("Delta : " + e.Delta);
                var sign = 1;
                if (e.Delta < 0)
                    sign = -1;
                //Scene.Camera.Distance = sign*0.2f*(float)Math.Pow(1.05, e.Delta);
                Scene.Camera.Distance += e.Delta/5;
                if (Scene.Camera.Distance < 0)
                    Scene.Camera.Distance = 0;
            }
        }

        private void _glc_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            //       if (ButtonState.Pushed == e.Mouse.LeftButton)
            {
                if (!_rotating)
                {
                    _rotating = true;
                    _rotatingStart = e.Location;
                }
                var deltaLocX = -_rotatingStart.X + e.X;
                var deltaLocY = -1 * _rotatingStart.Y + e.Y;

                Scene.Camera.Azimuth += 300*(deltaLocX / (float)_glc.Width);
                Scene.Camera.Elevation += 300 * (deltaLocY / (float)_glc.Height);

                _rotatingStart = e.Location;
            }
            else {
                _rotating = false;
            }
        }

        private void _glc_Resize(object sender, EventArgs e)
        {
            Scene.Camera.Aspect = (float)Glc.Width / Glc.Height;
            int w = Glc.Width;
            int h = Glc.Height;

            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        void glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            
            Scene.Render();
 
            _glc.SwapBuffers();
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            _glc.Invalidate();
           // Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;


            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ObjReader.Read(openFileDialog1.FileName);
            }
        }

        private void loadFull_Click(object sender, RoutedEventArgs e)
        {
            MeshObjectController.loadModelISA();

        }

        private void loadPart_Click(object sender, RoutedEventArgs e)
        {
            MeshObjectController.loadModelPartOf();

        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null || e.NewValue==null)
                return;
            var mod = ((MeshObjectBP3DGroup)((TreeViewItem)e.NewValue).DataContext);
            if(mod.HasAMesh())
                Scene.Camera.Target = mod.GetCenter();

        }
    }
}
