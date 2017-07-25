using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public static class MeshObjectController
    {
        private static List<MeshObject> _meshObjects;


        public static List<MeshObject> MeshObjects {
            get { return _meshObjects; }
            set { _meshObjects = value; }
        }

        static MeshObjectController() {
            _meshObjects = new List<MeshObject>();
        }

    }
}
