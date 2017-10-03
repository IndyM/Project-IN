using DMS.Geometry;
using DMS.OpenGL;
using Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class Scene
    {
        public CameraOrbit Camera {
            get;
        }


        public Scene() {
            Camera = new CameraOrbit()
            {
                FarClip = 10000,
                Distance = 1300,
            };
            
        }

        public void Render() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 camera = Camera.CalcMatrix().ToOpenTK();
            foreach (var meshObject in MeshObjectController.MeshObjects)
            {
                meshObject.Render(camera);
            }

            foreach (var cutObject in MeshObjectController.CutObject)
            {
                cutObject.Render(camera);
            }
        }
    }
}
