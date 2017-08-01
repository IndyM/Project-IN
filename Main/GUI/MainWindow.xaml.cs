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

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private  GLControl _glc;
        public GLControl Glc {
            get { return _glc; }
        }
        public MainWindow()
        {
            InitializeComponent();

            _glc = new GLControl();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            OpenTK.Toolkit.Init();

            _glc.Load += new EventHandler(glc_Load);
            _glc.Paint += new PaintEventHandler(glc_Paint);

            // Assign the GLControl as the host control's child.
            host.Child = _glc;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //MeshObjectController.loadModelISA();
            MeshObjectController.loadModelPartOf();
            sw.Stop();
            Debug.WriteLine("Time needed to load Model: " + sw.Elapsed.ToString());
        }
        


        private void glc_Load(object sender, EventArgs e)
        {
            // Make background "chocolate"
            GL.ClearColor(System.Drawing.Color.DarkGray);

            int w = _glc.Width;
            int h = _glc.Height;

            // Set up initial modes
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1);
            GL.Viewport(0, 0, w, h);
        }
        void glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Draw a little yellow triangle
            GL.Color3(System.Drawing.Color.Yellow);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(200, 50);
            GL.Vertex2(200, 200);
            GL.Vertex2(100, 50);
            GL.End();

            _glc.SwapBuffers();
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
    }
}
