using Model.Objects.BP3D;
using Model3D.Objects.Cut;
using Open3D.Geometry;
using Open3D.Geometry.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model3D.Controller
{
    public static class CutController
    {


        public static event EventHandler SelectedCutObjectChanged;
        private static void OnSelectedCutObjectChanged(EventArgs e)
        {
            SelectedCutObjectChanged?.Invoke(CutObject, e);
        }
        private static CuboidCut _cutObject;
        public static List<MeshObjectBP3D> MeshObjectsToCut { get; set; }
        public static CuboidCut CutObject {
            get { return _cutObject; }
            set { _cutObject = value; OnSelectedCutObjectChanged(new EventArgs()); }
        }
        /// <summary>
        /// 
        /// </summary>

        public static List<CuboidCut> CutObjects { get; set; }


        
        private static void MeshObjectController_SelectedCutObjectChanged(object sender, EventArgs e)
        {
            //Fill MeshObjectsToCut
            foreach(var meshObject in Model3DController.MeshObjects)
            {
  //              foreach(var cutObject in CutObjects)
                foreach (var plane in meshObject.Bounding.AsPlanes())
                    if (Intersection.IsPlaneInSphere(plane, CutObject.Bounding))
                        MeshObjectsToCut.Add(meshObject);
            }
        }

        public static void Init()
        {
            CutObjects = new List<CuboidCut>();
            MeshObjectsToCut = new List<MeshObjectBP3D>();
            CutObject = new CuboidCut();
            //            Mode3DlController.SelectedCutObjectChanged += MeshObjectController_SelectedCutObjectChanged;
        }
        public static void DoCut()
        {
            var objectsToCut = getAndRemoveObjectsinCutObject();
        }

        private static List<MeshObjectBP3D> getAndRemoveObjectsinCutObject()
        {
            var ret = new List<MeshObjectBP3D>();
            for (int i = 0; i < Model3DController.MeshObjects.Count; i++)
            {
                var ids = getIDsOfMeshObjectInCutObject(Model3DController.MeshObjects[i]);
                if (ids.Count>0){
                    ret.Add(Model3DController.MeshObjects[i]);
                    Model3DController.MeshObjects.RemoveAt(i);
                    i--;
                }
            }

            return ret;
        }

        public static List<int> getIDsOfMeshObjectInCutObject(MeshObjectBP3D meshObject) {

            var cutFaceCenters = CutObject.AsPlanes();//.getFacePoints();
            List<Vector3> positions = meshObject.Mesh.Position;
            List<int> idsInCut = new List<int>();
            for (int i=0;i< positions.Count;i++) {
                bool inCut = true;
                foreach(var face in cutFaceCenters)
                {
                    var vecToBoundPoint = -face.position + positions[i]; //Vector from Face-Center to vertex
                    float dot = Vector3.Dot( vecToBoundPoint, face.normal);

                    if (dot > 0)
                    {
                        inCut = false;
                        break;
                    }
                }
                if (inCut)// Vertex is inside the Quad (all Faces)
                { 
                    idsInCut.Add(i);
                    //meshObject.BaseColor = new OpenTK.Vector4(.0f,1.0f,1.0f,.3f);
                }
            }
            return idsInCut;
        }

    }
}
