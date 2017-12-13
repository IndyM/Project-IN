using Model.Objects.BP3D;
using Model3D.Controller;
using Open3D.Geometry.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Base;
using Zenseless.Geometry;
using Zenseless.OpenGL;

namespace Model3D
{
    public class MeshObjectBP3D : MeshObject
    {
        PolygonObjectBP3D PolygonObject { get; set; }

        public Cuboid Bounding
        {
            get; set;
        }

        public MeshObjectBP3D(PolygonObjectBP3D polygonObject) :base()
        {
            BaseColor = ColorController.getRandomColor();
            PolygonObject = polygonObject;

            CreateBaseMesh();
            //Create Bounding

        }

        public void CreateBaseMesh()
        {
            var mesh = new DefaultMesh();
            foreach (var vertex in PolygonObject.VertexList) {
                mesh.Position.Add(new Vector3(vertex.X,vertex.Y,vertex.Z));
                mesh.TexCoord.Add(new Vector2(.0f, .0f));
            }
            foreach (var normal in PolygonObject.NormalList)
                mesh.Normal.Add(new Vector3(normal.X, normal.Y, normal.Z));
            foreach (var id in PolygonObject.IdList)
                mesh.IDs.Add( id );
            Mesh = mesh;
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
/*
        public virtual void Render(OpenTK.Matrix4 camera)
        {

            //            GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.FrontAndBack);

            MeshObject.Render(camera);//.Activate();

            //           Bounding.Render(camera);


            //_vao.SetAttribute(_shader.GetAttributeLocation(baseColor.Name), baseColor.List.ToArray(), VertexAttribPointerType.Float, 4);
            // GL.UniformMatrix4(_shader.GetUniformLocation("camera"), true, ref camera);
            //  _vao.Draw();
            //  _shader.Deactivate();
        }*/
    }
}
