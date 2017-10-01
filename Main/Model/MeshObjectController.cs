using DMS.Base;
using DMS.OpenGL;
using Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;

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

        public static Vector3 bounds_min = new Vector3(-300,-300,-300);
        public static Vector3 bounds_max = new Vector3(300, 300, 300);

        private static ObservableCollection<TreeViewItem> _treeItems;
        private static ObservableCollection<TreeViewItem> _elementPartsTreeItems;
        private static ObservableCollection<TreeViewItem> _treeCutItems;

        private static int cutItemsCount = 1;


        private static ObservableCollection<MeshObjectCut> _cutObject;
        //private static MeshObjectCut _cutObject;

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
        public static ObservableCollection<TreeViewItem> TreeCutItems
        {
            get { return _treeCutItems; }
            set { _treeCutItems = value; }
        }
        public static ObservableCollection<TreeViewItem> ElementPartsTreeItems
        {
            get { return _elementPartsTreeItems; }
            set { _elementPartsTreeItems = value; }
        }

        public static ObservableCollection<MeshObjectCut> CutObject {
            get { return _cutObject; }
            set { _cutObject = value; }
        }

        static MeshObjectController() {

            _meshObjects = new ObservableCollection<MeshObjectBP3D>();
            _relationAllGroups = new ObservableCollection<MeshObjectBP3DGroup>();
            _partAllGroups = new ObservableCollection<MeshObjectBP3DGroup>();

            _elementParts  = new ObservableCollection<MeshObjectBP3DGroup>();
            _treeItems = new ObservableCollection<TreeViewItem>();
            _treeCutItems = new ObservableCollection<TreeViewItem>();
            _elementPartsTreeItems = new ObservableCollection<TreeViewItem>();

            _cutObject = new ObservableCollection<MeshObjectCut>();

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
                    
                    //berechnet die Gesamtgröße des Objektes
                    Vector3 min = meshObject.Bounds.Item1;
                    Vector3 max = meshObject.Bounds.Item2;
                    if (min.X < bounds_min.X) bounds_min.X = min.X;
                    if (min.Y < bounds_min.Y) bounds_min.Y = min.Y;
                    if (min.Z < bounds_min.Z) bounds_min.Z = min.Z;
                    if (max.X > bounds_max.X) bounds_max.X = max.X;
                    if (max.Y > bounds_max.Y) bounds_max.Y = max.Y;
                    if (max.Z > bounds_max.Z) bounds_max.Z = max.Z;
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


            //CutObject.Add(new MeshObjectCut(bounds_min, bounds_max,100));
        }

        public static MeshObjectCut addNewCuttingRoom()
        {
            string name = "Room " + cutItemsCount;
            var item = new TreeViewItem();

            MeshObjectCut obj = new MeshObjectCut(bounds_min, bounds_max, name, 100);
            CutObject.Add(obj);

            item.Header = name;
            item.DataContext = obj;
            TreeCutItems.Add(item);
            item.IsSelected = true;

            cutItemsCount++;
            return obj;
        }

        public static MeshObjectCut addNewCuttingChild(TreeViewItem item)
        {
            var obj = ((MeshObjectCut)(item).DataContext);
            MeshObjectCut child = obj.addChildren(item);
            string name = child.name;

            TreeViewItem itemc = new TreeViewItem();
            itemc.Header = name;
            itemc.DataContext = child;
            item.Items.Add(itemc);

            item.IsExpanded = true;
            itemc.IsSelected = true;

            return child;
        }

        private static bool deleteCutItems(TreeViewItem item, ItemCollection compare)
        {
            bool b;
            foreach (TreeViewItem it in compare)
            {
                b = it.Items.Contains(item);
                if (b == true)
                {
                    it.Items.Remove(item);
                    
                    return true;
                }
                else
                {
                    b = deleteCutItems(item, it.Items);
                    if (b == true)
                        return true;
                }
            }
            return false;
        }

        public static void deleteCuttingElement(TreeViewItem item)
        {
            bool b=false;

            MeshObjectCut obj = ((MeshObjectCut)(item).DataContext);
            b = TreeCutItems.Remove(item);

            obj.deleteChilds();
            foreach(MeshObjectCut del in CutObject)
            {
                del.childs.Remove(obj);
            }
            CutObject.Remove(obj);
            obj = null;

            if (b == false)
            {
                foreach (TreeViewItem it in TreeCutItems)
                {
                    b = it.Items.Contains(item);
                    if (b == true)
                    {
                        it.Items.Remove(item);
                        break;
                    }
                    b = deleteCutItems(item, it.Items);
                    if (b == true)
                        break;
                }
            }

        }


        private static void reset()
        {
            _meshObjects.Clear();
            _cutObject.Clear();
            _relationAllGroups.Clear();
            _partAllGroups.Clear();

            _elementParts.Clear();
            _treeItems.Clear();
            _treeCutItems.Clear();
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

                //var rotX = Matrix4x4.CreateRotationX(DMS.Geometry.MathHelper.DegreesToRadians(90));
                //meshObject = Transform(meshObject, rotX);
            }
            System.Diagnostics.Debug.WriteLine("Finished Reading File");

            addNewCuttingRoom();
        }

       
    }
}
