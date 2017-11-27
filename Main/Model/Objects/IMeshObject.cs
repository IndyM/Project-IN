using DMS.Geometry;
using DMS.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects
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
        Mesh Mesh
        {
            get;
            set;
        }

        void Render(OpenTK.Matrix4 camera);
    }
}
