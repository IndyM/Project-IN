using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Open3D.Geometry.Basics
{
    public struct Plane
    {
        public Vector3 position;
        public Vector3 normal;

        public Plane(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }
    }
}
