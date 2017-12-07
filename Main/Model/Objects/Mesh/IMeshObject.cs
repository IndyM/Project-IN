using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace Model.Objects.Mesh
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

        void Render(OpenTK.Matrix4 camera);
    }
}
