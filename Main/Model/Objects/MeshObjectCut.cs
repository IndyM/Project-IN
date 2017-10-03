using DMS.Base;
using DMS.Geometry;
using DMS.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
//using OpenTK.Graphics.OpenGL;
using System.Diagnostics;


namespace Model.Objects
{
    public class MeshObjectCut : MeshObject
    {
        public float rotX_deg { get; set; }
        public float rotY_deg { get; set; }
        public float rotZ_deg { get; set; }
        public float tx { get; set; }
        public float ty { get; set; }
        public float tz { get; set; }
        public float scaleXl { get; set; }
        public float scaleXr { get; set; }
        public float scaleYf { get; set; }
        public float scaleYb { get; set; }
        public float scaleZt { get; set; }
        public float scaleZb { get; set; }
        public float edge_ltx { get; set; }
        public float edge_lty { get; set; }
        public float edge_lbx { get; set; }
        public float edge_lby { get; set; }
        public float edge_rtx { get; set; }
        public float edge_rty { get; set; }
        public float edge_rbx { get; set; }
        public float edge_rby { get; set; }

        public float edge_ftz { get; set; }
        public float edge_fty { get; set; }
        public float edge_fbz { get; set; }
        public float edge_fby { get; set; }
        public float edge_btz { get; set; }
        public float edge_bty { get; set; }
        public float edge_bbz { get; set; }
        public float edge_bby { get; set; }

        public System.Numerics.Vector3 normalX { get; set; }
        public System.Numerics.Vector3 normalY { get; set; }
        public System.Numerics.Vector3 normalZ { get; set; }

        public static int TYPE_ACHSE_X { get { return 0; } }
        public static int TYPE_ACHSE_Y { get { return 1; } }
        public static int TYPE_ACHSE_Z { get { return 2; } }



        private int[] left_face = { 2, 5, 6, 3 };
        private int[] right_face = { 1, 4, 0, 7 };
        private int[] top_face = { 0, 2, 1, 3 };
        private int[] bot_face = { 4, 6, 5, 7 };
        private int[] front_face = { 1, 6, 7, 2 };
        private int[] back_face = { 0, 5, 3, 4 };

        private int[] left_edge_top = { 2,3 };
        private int[] left_edge_bot = { 6, 5 };
        private int[] right_edge_top = { 1, 0 };
        private int[] right_edge_bot = { 7, 4 };
        private int[] front_edge_top = { 1, 2 };
        private int[] front_edge_bot = { 6, 7 };
        private int[] back_edge_top = { 3, 0 };
        private int[] back_edge_bot = { 5, 4 };

        private System.Numerics.Vector3 boundsMin;
        private System.Numerics.Vector3 boundsMax;

        public List<MeshObjectCut> childs = new List<MeshObjectCut>();
        public System.Windows.Controls.TreeViewItem parent = new System.Windows.Controls.TreeViewItem();
        private int childCount = 1;
        public string name;

        public MeshObjectCut(System.Numerics.Vector3 bounds_min, System.Numerics.Vector3 bounds_max, string name, int freirum=0)
        {
            boundsMin = bounds_min;
            boundsMax = bounds_max;
            boundsMin.X -= freirum;
            boundsMin.Y -= freirum;
            boundsMin.Z -= freirum;
            boundsMax.X += freirum;
            boundsMax.Y += freirum;
            boundsMax.Z += freirum;
            this.name = name;
            tx = 0;
            ty = 0;
            tz = 0;
            rotX_deg = 0;
            rotY_deg = 0;
            rotZ_deg = 0;

            scaleXl = bounds_max.X + freirum;
            scaleXr = bounds_min.X - freirum;
            scaleYf = bounds_max.Y + freirum;
            scaleYb = bounds_min.Y - freirum;
            scaleZt = bounds_max.Z + freirum;
            scaleZb = bounds_min.Z - freirum;

            edge_ltx = 0;
            edge_lty = 0;
            edge_lbx = 0;
            edge_lby = 0;

            edge_rtx = 0;
            edge_rty = 0;
            edge_rbx = 0;
            edge_rby = 0;

            edge_ftz = 0;
            edge_fty = 0;
            edge_fbz = 0;
            edge_fby = 0;

            edge_btz = 0;
            edge_bty = 0;
            edge_bbz = 0;
            edge_bby = 0;

            normalX = new System.Numerics.Vector3(1, 0, 0);
            normalY = new System.Numerics.Vector3(0, 1, 0);
            normalZ = new System.Numerics.Vector3(0, 0, 1);

            Mesh = MyCube(new System.Numerics.Vector3(0, 0, 0), 0, 0, 0);

            var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\..\Resources\Shader\";
            Shader = ShaderLoader.FromFiles(dir + "vertex_base.glsl", dir + "frag_cutCube.glsl");

            update();

            Debug.WriteLine("bounds_min: " + bounds_min + " bounds_max: " + bounds_max + " int min: " + getBounds().Item1 + " int max: " + getBounds().Item2);
        }

        public void update()
        {
            TransformCube();
            Load();
        }


        public MeshObjectCut addChildren(System.Windows.Controls.TreeViewItem parent)
        {
            this.parent = parent;
            Tuple<System.Numerics.Vector3, System.Numerics.Vector3> bounds = getBounds();
            Debug.WriteLine("addChildren: bounds: " + bounds);
            MeshObjectCut obj = new MeshObjectCut(bounds.Item1, bounds.Item2, "Child " + childCount);
            childs.Add(obj);
            childCount++;
            return obj;
        }

        public void deleteChilds()
        {
            for(int i = 0; i < childs.Count; i++)
            {
                childs[i].deleteChilds();
            }
            childs.Clear();
        }

        public void scale(int type_achse, int side, float value)
        {
            Debug.WriteLine("scaleX: " + scaleXr + " l: " + scaleXl+ " boundsMax.X: "+ boundsMax.X+ "boundsMin.X: "+ boundsMin.X);
            if (type_achse == TYPE_ACHSE_X)
            {
                if (side == 0)
                {
                    if (value < scaleXl)
                        if (boundsMin.X < value)
                            scaleXr = value;
                }
                else if (side == 1)
                {
                    if (value > scaleXr)
                        if (boundsMax.X > value)
                            scaleXl = value;
                }
            }
            else if (type_achse == TYPE_ACHSE_Y)
            {
                if (side == 0)
                {
                    if (value < scaleYf)
                        if (boundsMin.Y < value)
                            scaleYb = value;
                }
                else if (side == 1)
                {
                    if (value > scaleYb)
                        if (boundsMax.Y > value)
                            scaleYf = value;
                }
             }
            else if (type_achse == TYPE_ACHSE_Z)
            {
                if (side == 0)
                {
                    if (value < scaleZt)
                        if (boundsMin.Z < value)
                            scaleZb = value;
                }
                else if (side == 1)
                {
                    if (value > scaleZb)
                        if (boundsMax.Z > value)
                            scaleZt = value;
                }
            }
        }
        public float getMaxScaleXl()
        {
            /*
            left_face;
            boundsMin;*/

            return 0;
        }

        private void TransformCube()
        {
            
            //Position ------------------------------------
            for (int i = 0; i < Mesh.position.List.Count; i++)
            {
                var newPos = Mesh.position.List[i];
                if (left_face.Contains(Mesh.corner[i]))
                {
                    newPos.X = scaleXl;
                }
                if (right_face.Contains(Mesh.corner[i]))
                {
                    newPos.X = scaleXr;
                }
                if (top_face.Contains(Mesh.corner[i]))
                {
                    newPos.Y = scaleYf;
                }
                if (bot_face.Contains(Mesh.corner[i]))
                {
                    newPos.Y = scaleYb;
                }
                if (front_face.Contains(Mesh.corner[i]))
                {
                    newPos.Z = scaleZt;
                }
                if (back_face.Contains(Mesh.corner[i]))
                {
                    newPos.Z = scaleZb;
                }

                

                //Hier dürfen nur absolut-Werte übergeben werden!
                if (left_edge_top.Contains(Mesh.corner[i]))
                {
                    newPos.X += edge_ltx;
                    newPos.Y -= edge_lty;
                }
                if (left_edge_bot.Contains(Mesh.corner[i]))
                {
                    newPos.X += edge_lbx;
                    newPos.Y += edge_lby;
                }
                if (right_edge_top.Contains(Mesh.corner[i]))
                {
                    newPos.X -= edge_rtx;
                    newPos.Y -= edge_rty;
                }
                if (right_edge_bot.Contains(Mesh.corner[i]))
                {
                    newPos.X -= edge_rbx;
                    newPos.Y += edge_rby;
                }
                if (front_edge_top.Contains(Mesh.corner[i]))
                {
                    newPos.Z -= edge_ftz;
                    newPos.Y -= edge_fty;
                }
                if (front_edge_bot.Contains(Mesh.corner[i]))
                {
                    newPos.Z -= edge_fbz;
                    newPos.Y += edge_fby;
                }
                if (back_edge_top.Contains(Mesh.corner[i]))
                {
                    newPos.Z += edge_btz;
                    newPos.Y -= edge_bty;
                }
                if (back_edge_bot.Contains(Mesh.corner[i]))
                {
                    newPos.Z += edge_bbz;
                    newPos.Y += edge_bby;
                }

                Mesh.position.List[i] = newPos;
            }

            //Translation und Rotation
            for (int i = 0; i < Mesh.position.List.Count; i++)
            {
                var newPos = System.Numerics.Vector3.Transform(Mesh.position.List[i], CalcMatrix());
                Mesh.position.List[i] = newPos;
            }
            for (int i = 0; i < Mesh.normal.List.Count; i++)
            {
                var newN = System.Numerics.Vector3.TransformNormal(Mesh.normal.List[i], CalcMatrix());
                Mesh.normal.List[i] = newN;
            }

/*
            normalX = System.Numerics.Vector3.TransformNormal(normalX, CalcMatrix());
            normalY = System.Numerics.Vector3.TransformNormal(normalY, CalcMatrix());
            normalZ = System.Numerics.Vector3.TransformNormal(normalZ, CalcMatrix());*/
        }



        public Matrix4x4 CalcMatrix()
        {
            var tran = Matrix4x4.Transpose(Matrix4x4.CreateTranslation(tx, ty, tz));
            var rotX = Matrix4x4.Transpose(Matrix4x4.CreateRotationX(DMS.Geometry.MathHelper.DegreesToRadians(rotX_deg), getRotCenter()));
            var rotY = Matrix4x4.Transpose(Matrix4x4.CreateRotationY(DMS.Geometry.MathHelper.DegreesToRadians(rotY_deg), getRotCenter()));
            var rotZ = Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(DMS.Geometry.MathHelper.DegreesToRadians(rotZ_deg), getRotCenter()));
            //var scal = Matrix4x4.Transpose(Matrix4x4.CreateScale(scaleX, scaleY, scaleZ));
            return Matrix4x4.Transpose(tran * rotX * rotY * rotZ);
        }

        public static Mesh MyCube(System.Numerics.Vector3 center, float a, float b, float c)
        {
            a = a * 0.5f;
            b = b * 0.5f;
            c = c * 0.5f;
            var mesh = new Mesh();

            //corners
            var corners = new System.Numerics.Vector3[] {
                new System.Numerics.Vector3(center.X + a, center.Y + b, center.Z - c),  //0
                new System.Numerics.Vector3(center.X + a, center.Y + b, center.Z + c),  //1
                new System.Numerics.Vector3(center.X - a, center.Y + b, center.Z + c),  //2
                new System.Numerics.Vector3(center.X - a, center.Y + b, center.Z - c),  //3
                new System.Numerics.Vector3(center.X + a, center.Y - b, center.Z - c),  //4
                new System.Numerics.Vector3(center.X - a, center.Y - b, center.Z - c),  //5
                new System.Numerics.Vector3(center.X - a, center.Y - b, center.Z + c),  //6
                new System.Numerics.Vector3(center.X + a, center.Y - b, center.Z + c),  //7
            };

            uint id = 0;
            var n = -System.Numerics.Vector3.UnitX;

            Action<int> Add = (int pos) => { mesh.position.List.Add(corners[pos]); mesh.normal.List.Add(n); mesh.IDs.Add(id); mesh.corner.Add(pos);  ++id; };

            //Left face
            Add(2);
            Add(5);
            Add(6);
            Add(2);
            Add(3);
            Add(5);
            //Right face
            n = System.Numerics.Vector3.UnitX;
            Add(1);
            Add(4);
            Add(0);
            Add(1);
            Add(7);
            Add(4);
            //Top Face
            n = System.Numerics.Vector3.UnitY;
            Add(0);
            Add(2);
            Add(1);
            Add(0);
            Add(3);
            Add(2);
            //Bottom Face
            n = -System.Numerics.Vector3.UnitY;
            Add(4);
            Add(6);
            Add(5);
            Add(4);
            Add(7);
            Add(6);
            //Front Face
            n = System.Numerics.Vector3.UnitZ;
            Add(1);
            Add(6);
            Add(7);
            Add(1);
            Add(2);
            Add(6);
            //Back Face
            n = -System.Numerics.Vector3.UnitZ;
            Add(0);
            Add(5);
            Add(3);
            Add(0);
            Add(4);
            Add(5);

            return mesh;
        }


        public override void Render(Matrix4 camera)
        {
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            GL.Disable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Enable(EnableCap.Blend);
            
            GL.DepthMask(false);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            base.Render(camera);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.Blend);
            GL.CullFace(CullFaceMode.Front);
            //GL.Enable(EnableCap.CullFace);
            GL.DepthMask(true);
            
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].Render(camera);
            }
        }

        public Tuple<System.Numerics.Vector3, System.Numerics.Vector3> getBounds()
        {
            System.Numerics.Vector3 max = Mesh.position.List[0];
            System.Numerics.Vector3 min = Mesh.position.List[0];

            for (int i = 1; i < Mesh.position.List.Count; i++)
            {
                if(max.X < Mesh.position.List[i].X)
                {
                    max.X = Mesh.position.List[i].X;
                }
                if (max.Y < Mesh.position.List[i].Y)
                {
                    max.Y = Mesh.position.List[i].Y;
                }
                if (max.Z < Mesh.position.List[i].Z)
                {
                    max.Z = Mesh.position.List[i].Z;
                }

                if (min.X > Mesh.position.List[i].X)
                {
                    min.X = Mesh.position.List[i].X;
                }
                if (min.Y > Mesh.position.List[i].Y)
                {
                    min.Y = Mesh.position.List[i].Y;
                }
                if (min.Z > Mesh.position.List[i].Z)
                {
                    min.Z = Mesh.position.List[i].Z;
                }
            }
                return new Tuple<System.Numerics.Vector3, System.Numerics.Vector3>(min, max); ;
        }

        public System.Numerics.Vector3 getCenter()
        {
            Tuple<System.Numerics.Vector3, System.Numerics.Vector3> bounds = getBounds();
            System.Numerics.Vector3 ret = bounds.Item1 + 0.5f * (-bounds.Item1 + bounds.Item2);
            return ret;
        }

        private System.Numerics.Vector3 getRotCenter()
        {
            Tuple<System.Numerics.Vector3, System.Numerics.Vector3> bounds = getBounds();
            System.Numerics.Vector3 ret = boundsMin + 0.5f * (-boundsMin + boundsMax);
            return ret;
        }
    }
}













//System.Diagnostics.Debug.WriteLine("XXX: " + v.X.ToString());
//Mesh.position.List[3] = new System.Numerics.Vector3(-100, 50, -50);
/*
Mesh.position.List[2] = new System.Numerics.Vector3(-50, 50, 50);
Mesh.position.List[3] = new System.Numerics.Vector3(-80, 50, -50);
Mesh.position.List[5] = new System.Numerics.Vector3(-50, -50, -50);
*/

/*
new Vector3(s2, s2, -s2),   //0
new Vector3(s2, s2, s2),    //1
new Vector3(-s2, s2, s2),   //2
new Vector3(-s2, s2, -s2),  //3
new Vector3(s2, -s2, -s2),  //4
new Vector3(-s2, -s2, -s2), //5
new Vector3(-s2, -s2, s2),  //6
new Vector3(s2, -s2, s2),   //7

mesh.position.List.Add(new Vector3(s2, s2, -s2)); //0
mesh.position.List.Add(new Vector3(s2, s2, s2)); //1
mesh.position.List.Add(new Vector3(-s2, s2, s2)); //2
mesh.position.List.Add(new Vector3(-s2, s2, -s2)); //3
mesh.position.List.Add(new Vector3(s2, -s2, -s2)); //4
mesh.position.List.Add(new Vector3(-s2, -s2, -s2)); //5
mesh.position.List.Add(new Vector3(-s2, -s2, s2)); //6
mesh.position.List.Add(new Vector3(s2, -s2, s2)); //7

//Left face
mesh.IDs.Add(2);
mesh.IDs.Add(5);
mesh.IDs.Add(6);
mesh.IDs.Add(2);
mesh.IDs.Add(3);
mesh.IDs.Add(5);
*/
