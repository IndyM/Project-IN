using DMS.Base;
using DMS.Geometry;
using DMS.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
//using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Model.Objects.Cut
{
    public class CuboidCut : MeshObject
    {
        private Vector3 _instancePosition;
        Vector3 _scale;
        uint _segmentsX;
        uint _segmentsY;
        uint _segmentsZ;

        public struct FaceCenter
        {
            public System.Numerics.Vector3 position;
            public System.Numerics.Vector3 normal;
        };



        public Vector3 InstancePosition
        {
            get { return _instancePosition; }
            set { _instancePosition = value; }
        }

        public Vector3 Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                _scale = value;
            }
        }

        public uint SegmentsX
        {
            get
            {
                return _segmentsX;
            }

            set
            {
                _segmentsX = value;
            }
        }

        public uint SegmentsY
        {
            get
            {
                return _segmentsY;
            }

            set
            {
                _segmentsY = value;
            }
        }

        public uint SegmentsZ
        {
            get
            {
                return _segmentsZ;
            }

            set
            {
                _segmentsZ = value;
            }
        }

        public CuboidCut() :base()
        {
            Scale = new Vector3(10,20,30);

            SegmentsX = SegmentsY = SegmentsZ = 10;
            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\..\..\Resources\Shader\";
            Shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_cutCube.glsl");

            Mesh = Meshes.CreateCuboid(Scale,SegmentsX,SegmentsY,SegmentsZ);
        }

        protected override void Load()
        {
            base.Load();
            var tmpMesh = Meshes.CreateCube();
            var edgePosition = new List<Vector3>();
            foreach (var position in Mesh.position.List)
                edgePosition.Add(new Vector3() {
                    X = position.X / ( 0.5f * Scale.X),
                    Y = position.Y / (0.5f * Scale.Y),
                    Z = position.Z / (0.5f * Scale.Z),
                });
            Vao.SetAttribute(_shader.GetAttributeLocation("edgePosition"), edgePosition.ToArray(), VertexAttribPointerType.Float, 3);
        }


        public override void Render(OpenTK.Matrix4 camera)
        {
        //    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.FrontAndBack);
        //    GL.Enable(EnableCap.Blend);
            
            GL.DepthMask(false);
            
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            
            Shader.Activate();
             //vao.SetAttribute(shader.GetAttributeLocation(mesh.position.Name), mesh.position.List.ToArray(), VertexAttribPointerType.Float, 3);
            
            GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref camera);
            GL.Uniform3(_shader.GetUniformLocation("instancePosition"), InstancePosition.ToOpenTK());
            _vao.Draw();
            _shader.Deactivate();
            

            base.Render(camera);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.Disable(EnableCap.Blend);
            //GL.CullFace(CullFaceMode.FrontAndBack);
            //GL.Enable(EnableCap.CullFace);
            GL.DepthMask(true);
        }

        public List<FaceCenter> getFaceCenters() {
            var ret = new List<FaceCenter>();
            //position[i] and [i+1] are opposite
            for (int i = 0; i < Mesh.position.List.Count; i += 6) {
                ret.Add(new FaceCenter() {
                    position = InstancePosition + Mesh.position.List[i]+0.5f*(-Mesh.position.List[i]+ Mesh.position.List[i+1]),
                    normal = Mesh.normal.List[i],
                    }
                );
            }

            return ret;
        }

    }
}
