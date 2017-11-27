using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Model.Objects.BP3D
{
    public class ObjectBP3DGroup : ObjectBP3D, IObjectBP3DGroup
    {

        public ObservableCollection<IObjectBP3DGroup> Children
        {
            get; set;
        }
        public bool IsMeshObject {
            get { return false; }
        }

        public ObjectBP3DGroup(String name) : base(name)
        {
            init();
        }

        public ObjectBP3DGroup(ObjectBP3D objectBP3D) : base(objectBP3D)
        {
            init();

        }
        private void init() {
            Children = new ObservableCollection<IObjectBP3DGroup>();
        }
    }
}
