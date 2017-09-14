﻿using DMS.Geometry;
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
                FarClip = 5000,
                Distance = 500,
            };
            
        }

        public void Render() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);




            Matrix4 camera = Camera.CalcMatrix().ToOpenTK();
            foreach (var meshObject in MeshObjectController.MeshObjects)
            {
                

                meshObject.Render(camera);

            }
        
            MeshObjectController.CutObject?.Render(camera);

        }
    }
}
