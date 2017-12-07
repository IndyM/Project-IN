using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;

namespace Model.Objects.Mesh
{
    public class Cuboid : MeshObject
    {

        private Vector3 _instancePosition;
        Vector3 _scale;
        uint _segmentsX;
        uint _segmentsY;
        uint _segmentsZ;

        public Vector3 InstancePosition
        {
            get { return _instancePosition; }
            set { _instancePosition = value; }
        }

        public Vector3 Scale
        {
            get => _scale; set => _scale = value;
        }

        public float ScaleX
        {
            get => _scale.X; set => _scale.X = value;
        }
        public float ScaleY
        {
            get => _scale.Y; set => _scale.Y = value;
        }
        public float ScaleZ
        {
            get => _scale.Z; set => _scale.Z = value;
        }
        public uint SegmentsX
        {
            get { return _segmentsX; }
            set { _segmentsX = value; }
        }

        public uint SegmentsY
        {
            get { return _segmentsY; }
            set { _segmentsY = value; }
        }

        public uint SegmentsZ
        {
            get { return _segmentsZ; }
            set { _segmentsZ = value; }
        }

        public List<MeshPoint> getFacePoints()
        {
            var ret = new List<MeshPoint>();

            ///<todo> Take care of different Segments
            var cuboidOneSegment = MeshesExtension.CreateCuboid(Scale, 1, 1, 1);

            var face_pointCount = cuboidOneSegment.Position.Count / 6; // 6 Faces
            // Just one point on each face needed
            // First Point of each Face with normal used
            for (int i = 0; i < cuboidOneSegment.Position.Count; i += face_pointCount)
            {
                ret.Add(new MeshPoint()
                {
                    position = InstancePosition + cuboidOneSegment.Position[i],
                    normal = cuboidOneSegment.Normal[i],
                });
            }

            return ret;
        }
    }
}
