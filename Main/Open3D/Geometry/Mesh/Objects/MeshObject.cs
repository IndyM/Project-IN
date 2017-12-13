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
using OpenTK;

namespace Open3D.Geometry.Objects
{
    public class MeshObject : IMeshObject
    {

        protected DefaultMesh _mesh;
        protected VAO _vao;
        protected Shader _shader;
        private OpenTK.Vector4 _baseColor;

        private Matrix4 _transform;

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

        public Matrix4 Transform { get => _transform; set => _transform = value; }

        public MeshObject()
        {
            Mesh = new DefaultMesh();
            Transform = Matrix4.Identity;

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

            GL.Uniform4(_shader.GetResourceLocation(ShaderResourceType.Uniform, "baseColor"), ref _baseColor);

            GL.UniformMatrix4(_shader.GetResourceLocation(ShaderResourceType.Uniform, "camera"), true, ref camera);
            _vao.Draw();
            _shader.Deactivate();
        }

        public IMeshObject Clone()
        {
            var clone = new MeshObject();

            var mesh = new DefaultMesh();
            foreach (var pos in Mesh.Position)
                mesh.Position.Add(new System.Numerics.Vector3(pos.X, pos.Y, pos.Z));
            foreach (var nor in Mesh.Normal)
                mesh.Normal.Add(new System.Numerics.Vector3(nor.X, nor.Y, nor.Z));
            foreach (var tex in Mesh.TexCoord)
                mesh.TexCoord.Add(new System.Numerics.Vector2(tex.X, tex.Y));
            foreach (var id in Mesh.IDs)
                mesh.IDs.Add(id);

            clone.Mesh = mesh;
            clone.Shader = Shader;
            clone._baseColor = new OpenTK.Vector4(_baseColor.X,_baseColor.Y,_baseColor.Z,_baseColor.W);
            clone.Load();

            return clone;
        }
            
    }
}
