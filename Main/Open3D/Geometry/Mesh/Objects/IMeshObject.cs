
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace Open3D.Geometry.Objects
{
    public interface IMeshObject
    {


        Shader Shader
        {
            get;
            set;
        }
        VAO Vao
        {
            get;
        }
        DefaultMesh Mesh
        {
            get;
            set;
        }

        void Render(Matrix4 camera);
    }
}
