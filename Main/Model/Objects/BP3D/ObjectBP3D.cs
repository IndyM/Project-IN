using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects.BP3D
{
    public abstract class ObjectBP3D : IObjectBP3D
    {
        public String Name { get; private set; }

        public Double CompatibilityVersion {
            get;
            set;
        } // 4.0

        public String RepresentationID {
            get; set;
        } // BP7409
        //# Build-up logic : FMA 3.0 part_of
        public String ConceptID {
            get; set;
        } // FMA59763

        public ObjectBP3D(String name) {
            Name = name;
        }
        public ObjectBP3D(ObjectBP3D objectBP3D) : this(objectBP3D.Name) {
            CompatibilityVersion = objectBP3D.CompatibilityVersion;
            RepresentationID = objectBP3D.RepresentationID;
            ConceptID = objectBP3D.ConceptID;

        }

        public void Clone(IObjectBP3D clone = null)
        {
            if (clone == null)
                return;

            clone.CompatibilityVersion = CompatibilityVersion;
            clone.ConceptID = ConceptID;
            clone.RepresentationID = RepresentationID;

        }
    }
}
