using DMS.Base;
using DMS.OpenGL;
using Model.Objects;
using Model.Objects.BP3D;
using Model.Objects.Cut;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Model
{
    

    public static class MeshObjectController
    {
        public static event EventHandler TreeStartChanged;
        private static void OnTreeStartChanged(EventArgs e) {
            TreeStartChanged?.Invoke(RelationTreeStart, e);
        }

        private static ObservableCollection<MeshObjectBP3D> _meshObjects;

        private static IObjectBP3DGroup _relationtreeStart;
        private static ObservableCollection<IObjectBP3DGroup> _elementParts;

        private static CuboidCut _cutObject;
        private static List<MeshObjectBP3D> _meshObjectsCut;

        /// <summary>
        /// All MeshObjects 
        /// </summary>
        public static ObservableCollection<MeshObjectBP3D> MeshObjects {
            get { return _meshObjects; }
            set { _meshObjects = value; }
        }

       public static IObjectBP3DGroup RelationTreeStart
        {
            get { return _relationtreeStart; }
            set { _relationtreeStart = value;
                OnTreeStartChanged(new EventArgs());
            }
        }
        public static ObservableCollection<IObjectBP3DGroup> ElementParts
        {
            get { return _elementParts; }
            set { _elementParts = value; }
        }

        /// <summary>
        /// Cut Object Cube
        /// </summary>
        public static CuboidCut CutObject {
            get { return _cutObject; }
            set { _cutObject = value; }
        }
        public static List<MeshObjectBP3D> MeshObjectsCut
        {
            get { return _meshObjectsCut; }
            set { _meshObjectsCut = value; }
        }

        static MeshObjectController() {
            _meshObjects = new ObservableCollection<MeshObjectBP3D>();
            _elementParts  = new ObservableCollection<IObjectBP3DGroup>();
            _relationtreeStart = null;

            MeshObjectsCut = new List<MeshObjectBP3D>();
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
            reset();
            var base_files = Directory.EnumerateFiles(path_base);
            var obj_files = Directory.EnumerateFiles(path_obj);

            // Load Mesh Files
            foreach (var file in obj_files)
            {
                var meshObjectBP3D = ObjReaderBP3D.ReadObj(file);
                if (meshObjectBP3D != null) { 
                    MeshObjects.Add(meshObjectBP3D);


                    MeshObjectsCut.Add(meshObjectBP3D);
                }
            }


            var file_element_parts = base_files.Where(x => x.Contains("element_parts")).ToList().FirstOrDefault();
            var file_inclusion_relation_list = base_files.Where(x => x.Contains("inclusion_relation_list")).ToList().FirstOrDefault();

            RelationTreeStart = ObjReaderBP3D.ReadRelations(file_inclusion_relation_list);
            ElementParts = ObjReaderBP3D.ReadElementPartList(file_element_parts);

            System.Diagnostics.Debug.WriteLine("Finished Reading File");

            _cutObject = new CuboidCut();
        }

        private static void reset()
        {
            _meshObjects.Clear();
            _elementParts.Clear();
            _relationtreeStart = null;

            _meshObjectsCut.Clear();
            _cutObject = null;
        }

        private static List<MeshObjectBP3D> getAndRemoveObjectsinCutObject()
        {
            var ret = new List<MeshObjectBP3D>();
            for (int i = 0; i < MeshObjectsCut.Count; i++)
            {
                var ids = getIDsOfMeshObjectInCutObject(MeshObjectsCut[i]);
                if (ids.Count>0){
                    ret.Add(MeshObjectsCut[i]);
                    MeshObjectsCut.RemoveAt(i);
                    i--;
                }
            }

            return ret;
        }
        public static List<int> getIDsOfMeshObjectInCutObject(MeshObjectBP3D meshObject) {

            var cutFaceCenters = CutObject.getFacePoints();
            List<Vector3> positions = meshObject.MeshObject.Mesh.position.List;
            List<int> idsInCut = new List<int>();
            for (int i=0;i< positions.Count;i++) {
                bool inCut = true;
                foreach(var face in cutFaceCenters)
                {
                    var vecToBoundPoint = -face.position + positions[i]; //Vector from Face-Center to vertex
                    float dot = Vector3.Dot( vecToBoundPoint, face.normal);

                    if (dot > 0)
                    {
                        inCut = false;
                        break;
                    }
                }
                if (inCut)// Vertex is inside the Quad (all Faces)
                { 
                    idsInCut.Add(i);
                    meshObject.MeshObject.baseColor.List[i] = new Vector4(.0f,1.0f,1.0f,.3f);
                }
            }
            return idsInCut;
        }
        public static void cutWithCutObject()
        {
            var objectsToCut = getAndRemoveObjectsinCutObject();
        }
    }
}
