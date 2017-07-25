using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class MeshObject
    {
        public String Name { get; private set; }
        public Mesh Mesh { get; private set; }
        public MeshObject(String name) {
            Name = name;
            Mesh = new Mesh();
        }
        public MeshObject(String name, Mesh mesh)
        {
            Name = name;
            Mesh = mesh;
        }
    }
}
