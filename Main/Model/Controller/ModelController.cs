using Model.Objects.BP3D;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Model.Reader;
using Zenseless.Base;
using System.ComponentModel;

namespace Model.Controller
{
    

    public static class ModelController 
    {
        public static event EventHandler TreeStartChanged;
        private static void OnTreeStartChanged(EventArgs e) {
            TreeStartChanged?.Invoke(RelationTreeStart, e);
        }




        private static ObservableCollection<PolygonObjectBP3D> _polygonObjects;
//        private static List<PolygonObjectBP3D> _workingMeshObjects;

        private static IObjectBP3DGroup _relationtreeStart;
        private static ObservableCollection<IObjectBP3DGroup> _elementParts;

//        private static CuboidCut _cutObject; 


        /// <summary>
        /// All MeshObjects 
        /// </summary>
        public static ObservableCollection<PolygonObjectBP3D> PolygonObjects {
            get { return _polygonObjects; }
            set { _polygonObjects = value; }
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
 /*       public static CuboidCut CutObject {
            get { return _cutObject; }
            set { _cutObject = value; OnSelectedCutObjectChanged(new EventArgs()); }
        }
        public static List<PolygonObjectBP3D> WorkingMeshObjects
        {
            get { return _workingMeshObjects; }
            set { _workingMeshObjects = value; }
        }
        */

        public static int VertexCount {
            get;set;
        }

        static ModelController() {
            _polygonObjects = new ObservableCollection<PolygonObjectBP3D>();
     //       WorkingMeshObjects = new List<PolygonObjectBP3D>();
            _elementParts  = new ObservableCollection<IObjectBP3DGroup>();
            _relationtreeStart = null;

        }

        public static void LoadModelISA() {
            var path_dir_base = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\..\\Resources\\Model\\isa_BP3D_4.0\\";
            var path_dir_obj = path_dir_base + "isa_BP3D_4.0_obj_99\\";

            LoadModel(path_dir_base, path_dir_obj);
        }
        public static void LoadModelPartOf()
        {
            var path_dir_base = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\..\\Resources\\Model\\partof_BP3D_4.0\\";
            var path_dir_obj = path_dir_base + "partof_BP3D_4.0_obj_99\\";

            LoadModel(path_dir_base, path_dir_obj);
        }

        private static void LoadModel(string path_base, string path_obj)
        {
            reset();
            var base_files = Directory.EnumerateFiles(path_base);
            var obj_files = Directory.EnumerateFiles(path_obj);

 /*           var highestY = .0;
            var lowestY = 1000.0;

            var highestZ = .0;
            var lowestZ = 1000.0;

            var counter = 0;
            var sumY = .0;
            var sumZ = .0;*/

            // Load Mesh Files
            foreach (var file in obj_files)
            {
                var meshObjectBP3D = ObjReaderBP3D.ReadObj(file);
 //               meshObjectBP3D.MeshObject.BaseColor = ColorController.getRandomColor();
                if (meshObjectBP3D != null) { 
                    PolygonObjects.Add(meshObjectBP3D);
                    VertexCount += meshObjectBP3D.VertexList.Count;
                    /*
                    foreach (var point in meshObjectBP3D.MeshObject.Mesh.Position)
                    {
                        counter++;
                        sumY += point.Y;
                        sumZ += point.Z;
                        if (point.Y > highestY)
                            highestY = point.Y;
                        else if (point.Y < lowestY)
                            lowestY = point.Y;
                        if (point.Z > highestZ)
                            highestZ = point.Z;
                        else if (point.Z < lowestZ)
                            lowestZ = point.Z;
                    }
                    */

 //                   WorkingMeshObjects.Add(meshObjectBP3D.Clone());
                }
            }
  /*          System.Diagnostics.Debug.WriteLine("sum Y : "+sumY/counter);
            System.Diagnostics.Debug.WriteLine("sum z : " + sumZ/counter);
            System.Diagnostics.Debug.WriteLine("Center Y : " + 0.5* (-lowestY +highestY));
            System.Diagnostics.Debug.WriteLine("CenterZ : " + 0.5 * (-lowestZ+ highestZ));*/
            var file_element_parts = base_files.Where(x => x.Contains("element_parts")).ToList().FirstOrDefault();
            var file_inclusion_relation_list = base_files.Where(x => x.Contains("inclusion_relation_list")).ToList().FirstOrDefault();

            RelationTreeStart = ObjReaderBP3D.ReadRelations(file_inclusion_relation_list);
            ElementParts = ObjReaderBP3D.ReadElementPartList(file_element_parts);

            System.Diagnostics.Debug.WriteLine("Finished Reading File");

//            CutObject = new CuboidCut();
        }

        private static void reset()
        {
            _polygonObjects.Clear();
//            _workingMeshObjects.Clear();
            _elementParts.Clear();
            _relationtreeStart = null;

//            _workingMeshObjects.Clear();
//            _cutObject = null;
        }



    }
}
