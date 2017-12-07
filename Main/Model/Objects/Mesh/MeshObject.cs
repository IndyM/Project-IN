using DMS.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Geometry;
using System.IO;
using DMS.Base;
using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace Model.Objects.Mesh
{
    public class MeshObject : IMeshObject
    {
        public struct MeshPoint
        {
            public Vector3 position;
            public Vector3 normal;
        };

        public IMeshAttribute<Vector4> baseColor = new MeshAttribute<Vector4>(nameof(baseColor));
        protected DMS.Geometry.Mesh _mesh;
        protected VAO _vao;
        protected Shader _shader;


        public Shader Shader
        {
            get { return _shader; }
            set
            {
                _shader = value;
                if (Mesh != null)
                    Load();
            }
        }
        public VAO Vao
        {
            get { return _vao; }
            private set { _vao = value; }
        }
        public DMS.Geometry.Mesh Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value;
                foreach (var normal in Mesh.normal.List)
                    baseColor.List.Add(new Vector4(normal, 1.0f));
                if (Shader != null)
                    Load();
            }
        }


        public MeshObject()
        {

            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\..\..\Resources\Shader\";
            Shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_base.glsl");
        }

        public MeshObject(DMS.Geometry.Mesh mesh) : this()
        {
            Mesh = mesh;
        }

        protected virtual void Load()
        {
            Vao = VAOLoader.FromMesh(Mesh, _shader);
        }

        public virtual void Render(OpenTK.Matrix4 camera)
        {
            if (Vao == null)
            {
                System.Diagnostics.Debug.WriteLine(nameof(MeshObject) + " Render, Vao null");
                return;
            }
            //            GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.FrontAndBack);

            _shader.Activate();
            _vao.SetAttribute(_shader.GetAttributeLocation(baseColor.Name), baseColor.List.ToArray(), VertexAttribPointerType.Float, 4);
            GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref camera);
            _vao.Draw();
            _shader.Deactivate();
        }
    }
}
