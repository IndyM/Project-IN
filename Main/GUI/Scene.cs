using DMS.Geometry;
using DMS.OpenGL;
using Model;
using OpenTK;

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
                FarClip = 5000,
                Distance = 30,
            };

        }

        public void Render() {
            //Camera.Distance += 1;
            foreach (var meshObject in MeshObjectController.MeshObjects)
            {
                Matrix4 camera = Camera.CalcMatrix().ToOpenTK();
                
                meshObject.Render(camera);
            }
        }
    }
}
