
using Open3D.Geometry.Basics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;

namespace Open3D.Geometry.Objects
{
    public class Cuboid : MeshObject , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private Vector3 _instancePosition;
        Vector3 _scale;
        uint _segmentsX;
        uint _segmentsY;
        uint _segmentsZ;



        public Vector3 InstancePosition
        {
            get { return _instancePosition; }
            set { _instancePosition = value; Create(); OnPropertyChanged("InstancePosition"); }
        }

        public Vector3 Scale
        {
            get => _scale;
            set { _scale = value; Create(); OnPropertyChanged("Scale"); }
        } 

        public float ScaleX
        {
            get => _scale.X;
            set { _scale.X = value; Create(); OnPropertyChanged("ScaleX"); }
        }
        public float ScaleY
        {
            get => _scale.Y;
            set { _scale.Y = value; Create(); OnPropertyChanged("ScaleY"); }
}
        public float ScaleZ
        {
            get => _scale.Z; set {_scale.Z = value; Create(); OnPropertyChanged("ScaleZ"); }
}
        public uint SegmentsX
        {
            get { return _segmentsX; }
            set { _segmentsX = value; Create(); OnPropertyChanged("SegmentsX"); }
        }

        public uint SegmentsY
        {
            get { return _segmentsY; }
            set { _segmentsY = value; Create(); OnPropertyChanged("SegmentsY"); }
        }

        public uint SegmentsZ
        {
            get { return _segmentsZ; }
            set { _segmentsZ = value; Create(); OnPropertyChanged("SegmentsZ"); }
        }


        public Sphere Bounding
        {
            get {
 /*               float max = .0;
                Vector3.Max(Scale);*/ 

                return new Sphere();  }
        }

        public Cuboid() : base()
        {
            Scale = new Vector3(10, 10, 10);
            SegmentsX = SegmentsY = SegmentsZ = 1;
        }


        public void Create()
        {
            Vao?.Dispose();
            Mesh = MeshesExtension.CreateCuboid(Scale, SegmentsX, SegmentsY, SegmentsZ);
            
            Load();
        }

        /// <summary>
        /// Get the StartIndex of each Face
        /// 
        /// [0] top
        /// [1] bot
        /// [2] front
        /// [3] back
        /// [4] right
        /// [5] left
        /// 
        /// </summary>
        /// <returns></returns>
        public uint[] getStartIndexFaces() {


            //Top & Bot
            uint top = 0;
            uint bot = top + (SegmentsX + 1) * (SegmentsZ + 1);

            //Front & Back
            uint front = bot + (SegmentsX + 1) * (SegmentsZ + 1);
            uint back =  front+ (SegmentsX + 1) * (SegmentsY + 1);
            //Right & Left
            uint right = back + (SegmentsX + 1) * (SegmentsY + 1);
            uint left = right + (SegmentsZ + 1) * (SegmentsY + 1);


            return new uint[]{
                top, bot, front,back,right,left};
        }
 /*       public List<MeshPoint> getFacePoints()
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
        }*/

        public Basics.Plane[] AsPlanes()
        {
            var ret = new Basics.Plane[6];

            var facesStartIndex = getStartIndexFaces();

            // Just one point on each face needed
            // First Point of each Face with normal used
            for (int i = 0; i < 6; i ++)
                ret[i] =new Basics.Plane(InstancePosition + Mesh.Position[i], Mesh.Normal[i]);

            return ret;
        }

    }
}

