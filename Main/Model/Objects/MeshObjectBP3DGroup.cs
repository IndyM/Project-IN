using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Model.Objects
{
    public class MeshObjectBP3DGroup : MeshObjectBP3D
    {
        public MeshObjectBP3DGroup(MeshObjectBP3D meshObject) : base(meshObject.Name)
        {
            Children = new ObservableCollection<MeshObjectBP3DGroup>();

            Bounds = meshObject.Bounds;
            CompatibilityVersion = meshObject.CompatibilityVersion;
            ConceptID = meshObject.ConceptID;
            FileID = meshObject.FileID;
            Mesh = meshObject.Mesh;
            RepresentationID = meshObject.RepresentationID;
            Volume = meshObject.Volume;

            Load();
        }
        public MeshObjectBP3DGroup(String name) : base(name)
        {
            Children = new ObservableCollection<MeshObjectBP3DGroup>();


        }

        public ObservableCollection<MeshObjectBP3DGroup> Children {
            get; set;
        }
        public MeshObjectBP3DGroup Parent {
            get; set;
        }

        public bool HasParent() {
            if (Parent != null)
                return true;
            
            return false;
        }
        public bool HasAMesh() {
            if (Mesh.IDs.Count > 0)
                return true;

            return false;
        }
        public bool HasChildredAMesh() {
            
            foreach (var child in Children) {
                if (child.HasAMesh() || child.HasChildredAMesh())
                    return true;
            }
            
            return false;
        }

        public TreeViewItem getTreeViewChildren() {
            var item = new TreeViewItem();
            item.Header = this.Name;
            item.DataContext = this;

            foreach (var chil in Children) {
                item.Items.Add(chil.getTreeViewChildren());
            }

            return item;
        }
        public TreeViewItem getTreeView()
        {
            var item = new TreeViewItem();
            item.Header = this.Name;
            item.DataContext = this;

            foreach (var chil in Children)
            {
                item.Items.Add(new TreeViewItem() { Header = chil.Name, DataContext = chil });
            }

            return item;
        }
    }
}
