using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects.BP3D
{
    public class PolygonObjectBP3DGroup : PolygonObjectBP3D , IObjectBP3DGroup
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

        public PolygonObjectBP3DGroup(PolygonObjectBP3D meshObject) : base(meshObject)
        {
            Children = new ObservableCollection<IObjectBP3DGroup>();
        }
    }
}
