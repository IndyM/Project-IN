﻿using Model;
using Model.Controller;
using Model3D.Controller;
using Model3D.Objects.Cut;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;
using Zenseless.OpenGL;

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
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            Matrix4 camera = Camera.CalcMatrix().ToOpenTK();
            foreach (var meshObject in Model3DController.MeshObjects)
            {
                meshObject.Render(camera);
            }
            foreach(var cutObject in CutController.CutObjects)
                cutObject.Render(camera);
            CutController.CutObject?.Render(camera);

        }
    }
}
