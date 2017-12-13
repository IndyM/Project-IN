using Model.Controller;
using Open3D.Geometry.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model3D.Controller
{
    public static class Model3DController
    {
        private static List<MeshObjectBP3D> _meshObjects = new List<MeshObjectBP3D>();

        public static List<MeshObjectBP3D> MeshObjects { get => _meshObjects; set => _meshObjects = value; }

        public static void CreateMeshObjects()
        {
            foreach(var polygonObject in ModelController.PolygonObjects)
            {

                MeshObjects.Add(new MeshObjectBP3D(polygonObject));
            }
        }

    }
}
