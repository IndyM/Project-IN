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
using Model.Objects.Mesh;

namespace Model.Objects.Cut
{
    public class CuboidCut : Cuboid ,ICutObject
    {

        private float BoundingRadius
        {
            get
            {
                if (Mesh.position.List.Count > 0)
                    return Mesh.position.List[0].Length();

                else
                    return .0f;
            }
        }
        private float BoundingRadiusSquared
        {
            get
            {
                if (Mesh.position.List.Count > 0)
                    return Mesh.position.List[0].LengthSquared();
                else
                    return .0f;
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



        public bool IsPointInBoundingBox(Vector3 point)
        {
            //BoundingBox is a sphere, cause of Rotations
            var center = InstancePosition;
            var radiusSquared = Mesh.position.List[0].LengthSquared();
            var pointDistanceSquared = (-center + point).LengthSquared();

            if (pointDistanceSquared <= radiusSquared)
                return true;
            else
                return false;
        }
        private bool IntersectCuboidSphere() {
            Vector3[] edges = new Vector3[8];


            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray_point"></param>
        /// <param name="ray_dir"></param>
        /// <param name="sphere_center"></param>
        /// <param name="sphere_radius"></param>
        /// <returns></returns>
        private bool IntersectRaySphere(Vector3 ray_point, Vector3 ray_dir, Vector3 sphere_center, double sphere_radius, ref Tuple<double, double> ts ) {

            /// (x+dx*t-Mx)^2+(y+dy-My)^2 +(z+dz*t-Mz)^2 = r^2 // Equation Intesect Sphere/Ray
            /// solve (x+a*t-d)^2+(y+b-e)^2 +(z+c* t-f)^2 = r^2 for t  //Used in Wolfram
            /// 
            /// x,y,z = ray_point
            /// a,b,c = ray_dir
            /// d,e,f = sphere_center
            /// 
            /// Solutions
            /// t1 = (1/2 sqrt((-2 a d + 2 a x - 2 c f + 2 c z)^2 - 4 (a^2 + c^2) (b^2 - 2 b e + 2 b y + d^2 - 2 d x + e^2 - 2 e y + f^2 - 2 f z - r^2 + x^2 + y^2 + z^2)) + a d - a x + c f - c z)/(a^2 + c^2) and a^2 + c^2!=0
            /// t2 = (-1/2 sqrt((-2 a d + 2 a x - 2 c f + 2 c z)^2 - 4 (a^2 + c^2) (b^2 - 2 b e + 2 b y + d^2 - 2 d x + e^2 - 2 e y + f^2 - 2 f z - r^2 + x^2 + y^2 + z^2)) + a d - a x + c f - c z)/(a^2 + c^2) and a^2 + c^2!=0

            /// var radicand = (-2 a d +2 a x -2 c f +2 c z) ^ 2 - 4(a ^ 2 + c ^ 2)(b ^ 2 - 2 b e + 2 b y + d ^ 2 - 2 d x + e ^ 2 - 2 e y + f ^ 2 - 2 f z - r^2 + x ^ 2 + y ^ 2 + z ^ 2)

            var x = ray_point.X; //x
            var y = ray_point.Y; //y
            var z = ray_point.Z; //z

            var dx = ray_dir.X; //a
            var dy = ray_dir.Y; //b
            var dz = ray_dir.Z; //c

            var mx = sphere_center.X; //d
            var my = sphere_center.Y; //e
            var mz = sphere_center.Z; //f

            var r = sphere_radius;

            var radicand = Math.Pow((-2 * dx * mx + 2 * dx * x - 2 * dz * mz + 2 * dz * z), 2)
                - 4 * (dx * dx + dz * dz) 
                * (dy * dy - 2 * dy * my + 2 * dy * y 
                + mx * mx - 2 * mx * x
                + my * my - 2 * my * y 
                + mz * mz - 2 * mz * z
                -r*r + x*x + y*y + z*z);

            double t1 = double.NaN, t2 = double.NaN;

            var compareEpsilon = .00001;

            if (Math.Abs(-dx + dy) < compareEpsilon)
                return false;

 
            ///<todo> Check radicant around .0 </todo>
            
            if (Math.Abs(radicand) < compareEpsilon) // No Intersection
                return false;
            else if (radicand < .0){ // 1 Intersection
                t1 = (1 / 2 * .0 + dx * mx - dx * x + dz * mz - dz * z) / (dx*dx + dz*dz);
            }
            else{ // 2 Intersections (normal Case)
                t1 = ( 1 / 2 * Math.Sqrt(radicand) + dx * mx - dx * x + dz * mz - dz * z) / (dx*dx + dz*dz);
                t1 = (-1 / 2 * Math.Sqrt(radicand) + dx * mx - dx * x + dz * mz - dz * z) / (dx*dx + dz*dz);
            }


            //            t = (1 / 2 sqrt() +a d - a x + c f - c z)/ (a ^ 2 + c ^ 2) and a^ 2 + c ^ 2 != 0
            //            t = (-1 / 2 sqrt() +a d - a x + c f - c z)/ (a ^ 2 + c ^ 2) and a^ 2 + c ^ 2 != 0


                ts = new Tuple<double, double>(t1, t2);

            return true;
        }
    }
}
