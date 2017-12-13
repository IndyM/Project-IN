
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects.BP3D
{
    public interface IObjectBP3D
    {
        String Name { get; }

        Double CompatibilityVersion
        {
            get;
            set;
        } // 4.0

        String RepresentationID
        {
            get; set;
        } // BP7409
        //# Build-up logic : FMA 3.0 part_of
        String ConceptID
        {
            get; set;
        } // FMA59763
    }
}
