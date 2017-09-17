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
using System.Windows.Controls;

namespace Model
{
    public static class MeshObjectController
    {
        private static ObservableCollection<MeshObjectBP3D> _meshObjects;
        private static ObservableCollection<MeshObjectBP3DGroup> _relationAllGroups;
        private static ObservableCollection<MeshObjectBP3DGroup> _partAllGroups;
        private static MeshObjectBP3DGroup _treeStart;
        private static ObservableCollection<MeshObjectBP3DGroup> _elementParts;
        //private static Shader baseShader;



        private static ObservableCollection<TreeViewItem> _treeItems;
        private static ObservableCollection<TreeViewItem> _elementPartsTreeItems;



        private static MeshObjectCut _cutObject;

        /// <summary>
        /// All MeshObjects 
        /// </summary>
        public static ObservableCollection<MeshObjectBP3D> MeshObjects {
            get { return _meshObjects; }
            set { _meshObjects = value; }
        }

        /// <summary>
        /// All Groups 
        /// </summary>
        public static ObservableCollection<MeshObjectBP3DGroup> RelationAllGroups
        {
            get { return _relationAllGroups; }
            set { _relationAllGroups = value; }
        }

        public static MeshObjectBP3DGroup TreeStart {
            get { return _treeStart; }
            set { _treeStart = value; }
        }
        public static ObservableCollection<MeshObjectBP3DGroup> ElementParts
        {
            get { return _elementParts; }
            set { _elementParts = value; }
        }

        public static ObservableCollection<MeshObjectBP3DGroup> PartAllGroups
        {
            get { return _partAllGroups; }
            set { _partAllGroups = value; }
        }
        public static ObservableCollection<TreeViewItem> TreeItems {
            get { return _treeItems; }
            set { _treeItems = value; }
        }
        public static ObservableCollection<TreeViewItem> ElementPartsTreeItems
        {
            get { return _elementPartsTreeItems; }
            set { _elementPartsTreeItems = value; }
        }

        public static MeshObjectCut CutObject {
            get { return _cutObject; }
            set { _cutObject = value; }
        }

        static MeshObjectController() {

            _meshObjects = new ObservableCollection<MeshObjectBP3D>();
            _relationAllGroups = new ObservableCollection<MeshObjectBP3DGroup>();
            _partAllGroups = new ObservableCollection<MeshObjectBP3DGroup>();

            _elementParts  = new ObservableCollection<MeshObjectBP3DGroup>();
            _treeItems = new ObservableCollection<TreeViewItem>();
            _elementPartsTreeItems = new ObservableCollection<TreeViewItem>();

            _treeStart = null;


            

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
                var meshObject = ObjReaderBP3D.ReadObj(file);
                if (meshObject != null) { 
                    MeshObjects.Add(meshObject);
                    RelationAllGroups.Add(new MeshObjectBP3DGroup(meshObject));
                    PartAllGroups.Add(new MeshObjectBP3DGroup(meshObject));
                }
            }

            var file_element_parts = base_files.Where(x => x.Contains("element_parts")).ToList().FirstOrDefault();
            var file_inclusion_relation_list = base_files.Where(x => x.Contains("inclusion_relation_list")).ToList().FirstOrDefault();

            TreeStart = ObjReaderBP3D.ReadRelations(file_inclusion_relation_list);
            ElementParts = ObjReaderBP3D.ReadElementPartList(file_element_parts);
            System.Diagnostics.Debug.WriteLine("Finished Reading File");
            System.Diagnostics.Debug.WriteLine("Setting up Trees ...");
            setUpTree();
            setUpTreeParts();
 

            _cutObject = new MeshObjectCut();
        }

        private static void reset()
        {
            _meshObjects.Clear();
            _relationAllGroups.Clear();
            _partAllGroups.Clear();

            _elementParts.Clear();
            _treeItems.Clear();
            _elementPartsTreeItems.Clear();

            _treeStart = null;

    }

        private static void setUpTree()
        {

            TreeItems.Add(MeshObjectController.TreeStart.getTreeViewChildren());
            /*
            foreach (var meshGroup in MeshObjectController.TreeStart.Children) {
                var treeitem = meshGroup.getTreeViewChildren();
                GroupView.Items.Add(treeitem);

            }*/
        }
        private static void setUpTreeParts()
        {
            foreach (var element in MeshObjectController.ElementParts)
                ElementPartsTreeItems.Add(element.getTreeView());
        }

        public static void loadCube()
        {
            // var path = Environment.CurrentDirectory + "\\..\\..\\..\\Resources\\Model\\partof_BP3D_4.0\\partof_BP3D_4.0_obj_99\\";
            var path = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + "\\Resources\\Model\\Cube\\";
            var files = Directory.EnumerateFiles(path);

            foreach (var file in files)
            {
                var meshObject = ObjReaderBP3D.ReadObj(file);

            }
            System.Diagnostics.Debug.WriteLine("Finished Reading File");
        }
    }
}
