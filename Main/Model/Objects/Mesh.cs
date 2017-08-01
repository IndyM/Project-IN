
using System.Collections.Generic;
using System.Numerics;


namespace Geometry.Mesh
{
    public class Mesh2
    {
        public IMeshAttribute<Vector3> position = new MeshAttribute<Vector3>(nameof(position));
        public IMeshAttribute<Vector3> normal = new MeshAttribute<Vector3>(nameof(normal));
        public IMeshAttribute<Vector2> uv = new MeshAttribute<Vector2>(nameof(uv));
        public List<uint> IDs = new List<uint>();
    }
}
