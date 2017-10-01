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
        private GLControl _glc;

        float MOUSE_SPEED = 0.1f;
        float ROTATION_SPEED = 2.0f;
        private double mousePosX;
        private double mousePosY;
        private double mouseOldX;
        private double mouseOldY;
        private double angelCamera;

        private bool dragStarted;
        float scaleXl;

        public TreeViewItem selectedItem = null;
        private bool cb_auto_center = true;


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
            _glc.MouseDown += _glc_MouseDown;
            _glc.MouseUp += _glc_MouseUp;

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
                Scene.Camera.Distance += e.Delta / 5;
                if (Scene.Camera.Distance < 0)
                    Scene.Camera.Distance = 0;
            }
        }

        private void _glc_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mousePosX = e.X;
            mousePosY = e.Y;
            mouseOldX = e.X;
            mouseOldY = e.Y;
            angelCamera = Scene.Camera.Elevation;

            if (selectedItem != null)
            {
                MeshObjectCut obj = (MeshObjectCut)selectedItem.DataContext;
                scaleXl = obj.scaleXl;
            }
        }

        private void _glc_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)

        {
            Debug.WriteLine("Mouse up!");
            if (e.Button == MouseButtons.Left)
            {
                if (selectedItem != null)
                {
                    if (cb_auto_center == true)
                    {
                        MeshObjectCut obj = (MeshObjectCut)selectedItem.DataContext;
                        Scene.Camera.Target = obj.getCenter();
                    }
                }
            }
        }

        private void _glc_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Kamera-Modus -----------------------------------------------------
            if (e.Button == MouseButtons.Middle)
            {
                mouseOldX = mousePosX;
                mouseOldY = mousePosY;
                mousePosX = e.X;
                mousePosY = e.Y;
                double mouseDeltaX = (mousePosX - mouseOldX);
                double mouseDeltaY = (mousePosY - mouseOldY);

                double modifier = 1.0;


                double anglex, angley;

                if ((angelCamera > 90 && angelCamera < 270))
                    angley = Scene.Camera.Azimuth - mouseDeltaX * MOUSE_SPEED * modifier * ROTATION_SPEED;
                else
                    angley = Scene.Camera.Azimuth + mouseDeltaX * MOUSE_SPEED * modifier * ROTATION_SPEED;
                if (angley < 0)
                    angley = 360 - angley;
                else if (angley > 360)
                    angley = angley - 360;

                anglex = Scene.Camera.Elevation + mouseDeltaY * MOUSE_SPEED * modifier * ROTATION_SPEED;
                if (anglex < 0)
                    anglex = 360 - anglex;
                else if (anglex > 360)
                    anglex = anglex - 360;

                if (Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.LeftAlt) == false)
                {
                    Scene.Camera.Azimuth = (float)angley;
                    Scene.Camera.Elevation = (float)anglex;
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) == true && Keyboard.IsKeyDown(Key.LeftAlt) == false)
                {
                    Scene.Camera.Azimuth = (float)angley;
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.LeftAlt) == true)
                {
                    Scene.Camera.Elevation = (float)anglex;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                mouseOldX = mousePosX;
                mouseOldY = mousePosY;
                mousePosX = e.X;
                mousePosY = e.Y;
                double mouseDeltaX = (mousePosX - mouseOldX);
                double mouseDeltaY = (mousePosY - mouseOldY);

                if (selectedItem != null)
                {
                    MeshObjectCut obj = (MeshObjectCut)selectedItem.DataContext;
                    obj.scaleXl = scaleXl + (float)mouseDeltaX;
                    scaleXl = obj.scaleXl;
                    obj.update();
                }
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
            var ret = MeshObjectController.bounds_min + 0.5f * (-MeshObjectController.bounds_min + MeshObjectController.bounds_max);
            Scene.Camera.Target = ret;
        }

        private void loadPart_Click(object sender, RoutedEventArgs e)
        {
            MeshObjectController.loadModelPartOf();
            var ret = MeshObjectController.bounds_min + 0.5f * (-MeshObjectController.bounds_min + MeshObjectController.bounds_max);
            Scene.Camera.Target = ret;
        }

        private void loadCube_Click(object sender, RoutedEventArgs e)
        {
            MeshObjectController.loadCube();
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null || e.NewValue==null)
                return;
            var mod = ((MeshObjectBP3DGroup)((TreeViewItem)e.NewValue).DataContext);
            if (mod.HasAMesh())
            {
                Scene.Camera.Target = mod.GetCenter();
            }
        }

        private void TreeView_SelectedCutItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null || e.NewValue == null)
                return;
            selectedItem = (TreeViewItem)e.NewValue;
            var obj = ((MeshObjectCut)((TreeViewItem)e.NewValue).DataContext);
            Scene.Camera.Target = obj.getCenter();
            Debug.WriteLine("Item selected: "+ obj.name);
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Buttons pressed ...");
            System.Windows.Controls.Button Quelle = (System.Windows.Controls.Button)sender;

            if (Quelle.Name == "new_room")
            {
                MeshObjectController.addNewCuttingRoom();
            }
            else if(Quelle.Name == "new_child")
            {
                MeshObjectController.addNewCuttingChild(selectedItem);
            }
            else if(Quelle.Name == "delete")
            {
                MeshObjectController.deleteCuttingElement(selectedItem);
            }
            else if(Quelle.Name == "center")
            {
                MeshObjectCut obj = (MeshObjectCut)selectedItem.DataContext;
                Scene.Camera.Target = obj.getCenter();
            }
            
        }

        private void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Buttons pressed ...");
            System.Windows.Controls.CheckBox Quelle = (System.Windows.Controls.CheckBox) sender;

            if(Quelle.Name == "auto_center")
            {
                cb_auto_center = (bool)Quelle.IsChecked;
            }
        }



            private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //Debug.WriteLine("comlete: "+((Slider)sender).Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //Debug.WriteLine("start: " + ((Slider)sender).Value);
            this.dragStarted = true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (dragStarted==true)
                Debug.WriteLine("Changed: "+((Slider)sender).Value);
        }


        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void host_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

    }
}
