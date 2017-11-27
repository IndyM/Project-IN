using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Geometry;
using DMS.OpenGL;
using System.IO;
using DMS.Base;
//using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Numerics;

namespace Model.Objects.BP3D
{
    public class MeshObjectBP3D : ObjectBP3D
    {

        public String FileID
        {
            get; set;
        } // FJ1252
        public Tuple<Vector3, Vector3> Bounds
        {
            get; set;
        }  //(mm): (-35.461100,-181.682000,1461.347400)-(34.135000,-127.714200,1479.842700)
        public Double Volume
        {
            get; set;
        }//(cm3): 21.758500


        public MeshObject MeshObject {
            get; set;
        }
        public MeshObjectBP3D(String name) : base(name) {
            MeshObject = new MeshObject();


        }


        public MeshObjectBP3D(String name, Mesh mesh) : this(name)
        {
            MeshObject.Mesh = mesh;
        }
        public MeshObjectBP3D(MeshObjectBP3D meshObject) : base(meshObject)
        {
            FileID = meshObject.FileID;
            Bounds = meshObject.Bounds;
            Volume = meshObject.Volume;
        }

        private void init()
        {
            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\..\..\Resources\Shader\";
            MeshObject.Shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_base.glsl");
        }
        /*
        ~MeshObject()
        {
            if (_vao != null)
                if (_vao.Disposed == false) 
                    _vao.Dispose();
               
            
                if(_shader!=null)
                    if(_shader.Disposed==false)
                        _shader.Dispose();
            }
            */

        public virtual void Render(OpenTK.Matrix4 camera)
        {

            //            GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.FrontAndBack);

            MeshObject.Render(camera);//.Activate();
            //_vao.SetAttribute(_shader.GetAttributeLocation(baseColor.Name), baseColor.List.ToArray(), VertexAttribPointerType.Float, 4);
           // GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref camera);
          //  _vao.Draw();
          //  _shader.Deactivate();
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
