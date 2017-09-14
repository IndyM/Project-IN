using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Geometry;
using DMS.OpenGL;
using System.IO;
using DMS.Base;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Model.Objects
{
    public class MeshObject
    {
        private VAO _vao;
        private Shader _shader;

        public String Name { get; private set; }

        public Shader Shader
        {
            get { return _shader; }
            set { _shader = value; }
        }
        public Mesh Mesh { get; set; }

        public MeshObject() {

        }
        public MeshObject(String name) {
            Name = name;
            Mesh = new Mesh();
            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\..\Resources\Shader\";
            _shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_base.glsl");
           // _shader = new Shader();
            
        }

        public MeshObject(String name, Mesh mesh)
        {
            Name = name;
            Mesh = mesh;
        }
        /*
        ~MeshObject()
        {
            if (_vao != null)
                if (_vao.Disposed == false) 
                    _vao.Dispose();
               
            
                if(_shader!=null)
                    if(_shader.Disposed==false)
                        _shader.Dispose();
            }
            */
        public virtual void Load() {
            _vao = VAOLoader.FromMesh(Mesh, _shader);
        }

        public virtual void Render(Matrix4 camera)
        {
            _shader.Activate();
            
            GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref camera);
            _vao.Draw();
            _shader.Deactivate();
        }
    }
}
