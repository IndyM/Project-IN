﻿using Model.Objects.BP3D;
using Model.Objects.Cut;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using Model.Reader;
using Zenseless.Base;
using System.ComponentModel;

namespace Model.Controller
{
    

    public static class MeshObjectController 
    {
        public static event EventHandler TreeStartChanged;
        private static void OnTreeStartChanged(EventArgs e) {
            TreeStartChanged?.Invoke(RelationTreeStart, e);
        }

        public static event EventHandler SelectedCutObjectChanged;
        private static void OnSelectedCutObjectChanged(EventArgs e)
        {
            SelectedCutObjectChanged?.Invoke(CutObject, e);
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
            set { _cutObject = value; OnSelectedCutObjectChanged(new EventArgs()); }
        }
        public static List<MeshObjectBP3D> MeshObjectsCut
        {
            get { return _meshObjectsCut; }
            set { _meshObjectsCut = value; }
        }


        public static int VertexCount {
            get;set;
        }

        static MeshObjectController() {
            _meshObjects = new ObservableCollection<MeshObjectBP3D>();
            _elementParts  = new ObservableCollection<IObjectBP3DGroup>();
            _relationtreeStart = null;

            MeshObjectsCut = new List<MeshObjectBP3D>();

        }

        public static void loadModelISA() {
            var path_dir_base = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\..\\Resources\\Model\\isa_BP3D_4.0\\";
            var path_dir_obj = path_dir_base + "isa_BP3D_4.0_obj_99\\";

            loadModel(path_dir_base, path_dir_obj);
        }
        public static void loadModelPartOf()
        {
            var path_dir_base = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\..\\Resources\\Model\\partof_BP3D_4.0\\";
            var path_dir_obj = path_dir_base + "partof_BP3D_4.0_obj_99\\";

            loadModel(path_dir_base, path_dir_obj);
        }

        private static void loadModel(string path_base, string path_obj)
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
                meshObjectBP3D.MeshObject.BaseColor = ColorController.getColor();
                if (meshObjectBP3D != null) { 
                    MeshObjects.Add(meshObjectBP3D);
                    VertexCount += meshObjectBP3D.MeshObject.Mesh.Position.Count;
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
                    MeshObjectsCut.Add(meshObjectBP3D);
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

            CutObject = new CuboidCut();
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
            List<Vector3> positions = meshObject.MeshObject.Mesh.Position;
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
  //                  meshObject.MeshObject.baseColor.List[i] = new Vector4(.0f,1.0f,1.0f,.3f);
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
