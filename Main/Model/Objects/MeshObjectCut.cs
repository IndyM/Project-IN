using DMS.Base;
using DMS.Geometry;
using DMS.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Model.Objects
{
    public class MeshObjectCut : MeshObject
    {
        public MeshObjectCut()
        {
            Mesh = Meshes.CreateCubeSpecial(100);

            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\..\Resources\Shader\";
            Shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_cutCube.glsl");

            Load();
        }


        public override void Render(Matrix4 camera)
        {
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            GL.Disable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Enable(EnableCap.Blend);
            
            GL.DepthMask(false);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);


            base.Render(camera);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.Blend);
            GL.CullFace(CullFaceMode.Front);
            //GL.Enable(EnableCap.CullFace);
            GL.DepthMask(true);
        }
    }
}
