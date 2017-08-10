using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model.Objects
{
    public class MeshObjectBP3D : MeshObject
    {
        public Double CompatibilityVersion {
            get;
            set;
        } // 4.0
        public String FileID {
            get; set;
        } // FJ1252
        public String RepresentationID {
            get; set;
        } // BP7409
        //# Build-up logic : FMA 3.0 part_of
        public String ConceptID {
            get; set;
        } // FMA59763

        public Tuple<Vector3,Vector3> Bounds{
            get; set;
          }  //(mm): (-35.461100,-181.682000,1461.347400)-(34.135000,-127.714200,1479.842700)
        public Double Volume {
            get; set;
        }//(cm3): 21.758500





        public MeshObjectBP3D(String name) : base(name) {
            
        }
        /// <summary>
        /// Get the Center-Point of the Mesh
        /// </summary>
        /// <returns> Center-Point as Vector3 </returns>
        public Vector3 GetCenter() {
            var ret = Bounds.Item1 + 0.5f * (-Bounds.Item1 + Bounds.Item2);
            return ret;
        }


    }
}
