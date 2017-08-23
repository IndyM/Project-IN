using DMS.Base;
using DMS.OpenGL;
using Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class MeshObjectController
    {
        private static ObservableCollection<MeshObject> _meshObjects;
        private static ObservableCollection<MeshObjectBP3DGroup> _allGroups;

        private static MeshObjectBP3DGroup _treeStart;
        //private static Shader baseShader;

        public static ObservableCollection<MeshObject> MeshObjects {
            get { return _meshObjects; }
            set { _meshObjects = value; }
        }

        public static ObservableCollection<MeshObjectBP3DGroup> AllGroups
        {
            get { return _allGroups; }
            set { _allGroups = value; }
        }
        public static MeshObjectBP3DGroup TreeStart {
            get { return _treeStart; }
            set { _treeStart = value; }
        }       

        static MeshObjectController() {
            _meshObjects = new ObservableCollection<MeshObject>();
            AllGroups = new ObservableCollection<MeshObjectBP3DGroup>();
        }

        public static void loadModelISA() {
            var path_dir_base = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\Resources\\Model\\isa_BP3D_4.0\\";
            var path_dir_obj = path_dir_base + "isa_BP3D_4.0_obj_99\\";

            loadModel(path_dir_base, path_dir_obj);
        }
        public static void loadModelPartOf()
        {
            var path_dir_base = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\Resources\\Model\\partof_BP3D_4.0\\";
            var path_dir_obj = path_dir_base + "partof_BP3D_4.0_obj_99\\";

            loadModel(path_dir_base, path_dir_obj);
        }

        private static void loadModel(string path_base, string path_obj)
        {

            var base_files = Directory.EnumerateFiles(path_base);
            var obj_files = Directory.EnumerateFiles(path_obj);

            // Load Mesh Files
            foreach (var file in obj_files)
            {
                var meshObject = ObjReaderBP3D.ReadObj(file);

                MeshObjects.Add(meshObject);
                AllGroups.Add(new MeshObjectBP3DGroup(meshObject));
            }

            var file_element_parts = base_files.Where(x => x.Contains("element_parts")).ToList().FirstOrDefault();
            var file_inclusion_relation_list = base_files.Where(x => x.Contains("inclusion_relation_list")).ToList().FirstOrDefault();

            TreeStart = ObjReaderBP3D.ReadRelations(file_inclusion_relation_list);

            System.Diagnostics.Debug.WriteLine("Finished Reading File");
        }
    }
}
