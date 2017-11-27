using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects.BP3D
{
    public class MeshObjectBP3DGroup : MeshObjectBP3D , IObjectBP3DGroup
    {
        public ObservableCollection<IObjectBP3DGroup> Children
        {
            get;
            set;
        }

        public bool IsMeshObject
        {
            get { return true; }
        }

        public MeshObjectBP3DGroup(MeshObjectBP3D meshObject) : base(meshObject)
        {
            Children = new ObservableCollection<IObjectBP3DGroup>();
        }
    }
}
