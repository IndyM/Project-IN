using Model.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
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

        public static void loadModelISA() {

            var path = Environment.CurrentDirectory + "\\..\\..\\..\\Resources\\Model\\isa_BP3D_4.0\\isa_BP3D_4.0_obj_99\\";
            var files = Directory.EnumerateFiles(path);

            foreach (var file in files) {
                var meshObject = ObjReaderBP3D.Read(file);
            }
            System.Diagnostics.Debug.WriteLine("Finished Reading File");
        }
        public static void loadModelPartOf()
        {
            var path = Environment.CurrentDirectory + "\\..\\..\\..\\Resources\\Model\\partof_BP3D_4.0\\partof_BP3D_4.0_obj_99\\";
            var files = Directory.EnumerateFiles(path);

            foreach (var file in files)
            {
                var meshObject = ObjReaderBP3D.Read(file);
            }
            System.Diagnostics.Debug.WriteLine("Finished Reading File");
        }
    }
}
