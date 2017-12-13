using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Numerics;


namespace Model.Objects.BP3D
{
    public class PolygonObjectBP3D : ObjectBP3D
    {
        protected Tuple<Vector3, Vector3> _bounds;

        public String FileID
        {
            get; set;
        } // FJ1252
        public Tuple<Vector3, Vector3> Bounds
        {
            get {
                return _bounds;
            }
            set {
                _bounds = value;
            }
        }  //(mm): (-35.461100,-181.682000,1461.347400)-(34.135000,-127.714200,1479.842700)

        public Double Volume
        {
            get; set;
        }//(cm3): 21.758500

        public List<Vector3> VertexList { get; set; }
        public List<Vector3> NormalList { get; set; }
        public List<uint> IdList { get; set; }



        public PolygonObjectBP3D(String name) : base(name) {
        }

        public PolygonObjectBP3D(PolygonObjectBP3D meshObject) : base(meshObject)
        {
            FileID = meshObject.FileID;
            Bounds = meshObject.Bounds;
            Volume = meshObject.Volume;
        }

        /// <summary>
        /// Get the Center-Point of the Mesh
        /// </summary>
        /// <returns> Center-Point as Vector3 </returns>
        public Vector3 GetCenter()
        {
            var ret = Bounds.Item1 + 0.5f * (-Bounds.Item1 + Bounds.Item2);
            return ret;
        }
        public List<System.Numerics.Vector3> getBoundPoints()
        {
            var ret = new List<System.Numerics.Vector3>();

            ret.Add(new Vector3(Bounds.Item1.X, Bounds.Item1.Y, Bounds.Item1.Z));
            ret.Add(new Vector3(Bounds.Item1.X, Bounds.Item1.Y, Bounds.Item2.Z));
            ret.Add(new Vector3(Bounds.Item1.X, Bounds.Item2.Y, Bounds.Item1.Z));
            ret.Add(new Vector3(Bounds.Item1.X, Bounds.Item2.Y, Bounds.Item2.Z));

            ret.Add(new Vector3(Bounds.Item2.X, Bounds.Item1.Y, Bounds.Item1.Z));
            ret.Add(new Vector3(Bounds.Item2.X, Bounds.Item1.Y, Bounds.Item2.Z));
            ret.Add(new Vector3(Bounds.Item2.X, Bounds.Item2.Y, Bounds.Item1.Z));
            ret.Add(new Vector3(Bounds.Item2.X, Bounds.Item2.Y, Bounds.Item2.Z));

            return ret;
        }
    }
}
