using DMS.Geometry;
using DMS.OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        DMS.Geometry.Mesh Mesh
        {
            get;
            set;
        }

        void Render(Matrix4 camera);
    }
}
