using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK.Graphics.OpenGL;

using System.Numerics;
using Zenseless.OpenGL;
using Zenseless.Geometry;
using Zenseless.Base;
using Zenseless.HLGL;

namespace Open3D.Geometry.Objects
{
    public class MeshObject : IMeshObject
    {
        public struct MeshPoint
        {
            public Vector3 position;
            public Vector3 normal;
        };

        protected DefaultMesh _mesh;
        protected VAO _vao;
        protected Shader _shader;
        private OpenTK.Vector4 _baseColor;

        public OpenTK.Vector4 BaseColor
        {
            get => _baseColor; set => _baseColor = value;
        }
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
        public DefaultMesh Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value;
                if (Shader != null)
                    Load();
            }
        }


        public MeshObject()
        {
            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Shader\";
            Shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_base.glsl");
        }

        public MeshObject(DefaultMesh mesh) : this()
        {
            Mesh = mesh;
        }

        protected virtual void Load()
        {
            Vao = VAOLoader.FromMesh(_mesh, _shader);
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

            //_vao.SetAttribute(_shader.GetAttributeLocation(baseColor.Name), baseColor.List.ToArray(), VertexAttribPointerType.Float, 4);
            GL.Uniform4(_shader.GetResourceLocation(ShaderResourceType.Uniform, "baseColor"), ref _baseColor);

            GL.UniformMatrix4(_shader.GetResourceLocation(ShaderResourceType.Uniform, "camera"), true, ref camera);
            _vao.Draw();
            _shader.Deactivate();
        }

        public IMeshObject Clone()
        {
            var clone = new MeshObject();
            clone.Mesh.Add(Mesh);
            clone.Shader = Shader;
            clone._baseColor = new OpenTK.Vector4(_baseColor.X,_baseColor.Y,_baseColor.Z,_baseColor.W);
            clone.Load();

            return clone;
        }
            
    }
}
