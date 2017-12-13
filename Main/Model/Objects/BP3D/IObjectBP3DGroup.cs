using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects.BP3D
{
    public interface IObjectBP3DGroup : IObjectBP3D
    {
        ObservableCollection<IObjectBP3DGroup> Children
        {
            get; set;
        }

        bool IsMeshObject
        {
            get;
        }
    }
}
