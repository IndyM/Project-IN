using Model;
using Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI.ViewModel
{
    public class ObjectBP3DVM
    {

        public static ObservableCollection<TreeViewItem> RelationTreeItems
        {
            get; 
            set; 
        }
        public static ObservableCollection<TreeViewItem> ElementPartsTreeItems
        {
            get;
            set;
        }

        public ObjectBP3DVM() {
            RelationTreeItems = new ObservableCollection<TreeViewItem>();
            ElementPartsTreeItems = new ObservableCollection<TreeViewItem>();

            MeshObjectController.TreeStartChanged += MeshObjectController_TreeStartChanged;
            MeshObjectController.ElementParts.CollectionChanged += MeshObjectController_ElementParts_CollectionChanged;
        }

        private void MeshObjectController_TreeStartChanged(object sender, EventArgs e)
        {
            RelationTreeItems.Add(getTreeItemsOfObject(sender as IObjectBP3DGroup));
        }

        private void MeshObjectController_ElementParts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in sender as ObservableCollection<IObjectBP3DGroup>)
                ElementPartsTreeItems.Add(getTreeItemsOfObject(item));
        }


        
        private TreeViewItem getTreeItemsOfObject(IObjectBP3DGroup objectBP3D) {
                    if (objectBP3D == null)
                        return new TreeViewItem();
            var item = new TreeViewItem();
            item.Header = objectBP3D.IsMeshObject ? "m " +objectBP3D.Name : objectBP3D.Name;
            item.DataContext = objectBP3D;

            foreach (var chil in objectBP3D.Children) {
                item.Items.Add(getTreeItemsOfObject(chil));
            }

            return item;
}
        /*
public TreeViewItem getTreeView()
{
    var item = new TreeViewItem();
    item.Header = this.Name;
    item.DataContext = this;

    foreach (var chil in Children)
    {
        var treeItem = new TreeViewItem();
        treeItem.Header = chil.Name;
        treeItem.DataContext = chil;
        item.Items.Add(treeItem);
    }

    return item;
}*/
    }
}
