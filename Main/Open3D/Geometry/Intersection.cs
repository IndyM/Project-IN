using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Open3D.Geometry.Basics;

namespace Open3D.Geometry
{
    public static class Intersection
    {
        public static bool RaySphere(Vector3 ray_point, Vector3 ray_dir, Vector3 sphere_center, double sphere_radius, ref Tuple<Vector3, Vector3> intersectionPoints)
        {

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
                - r * r + x * x + y * y + z * z);

            double t1 = double.NaN, t2 = double.NaN;

            var compareEpsilon = .00001;

            if (Math.Abs(-dx + dy) < compareEpsilon)
                return false;

            if (Math.Abs(radicand) < compareEpsilon) // No Intersection
                return false;
            else if (radicand < .0)
            { // 1 Intersection
                t1 = (1 / 2 * .0 + dx * mx - dx * x + dz * mz - dz * z) / (dx * dx + dz * dz);
            }
            else
            { // 2 Intersections (normal Case)
                t1 = (1 / 2 * Math.Sqrt(radicand) + dx * mx - dx * x + dz * mz - dz * z) / (dx * dx + dz * dz);
                t1 = (-1 / 2 * Math.Sqrt(radicand) + dx * mx - dx * x + dz * mz - dz * z) / (dx * dx + dz * dz);
            }
            
            intersectionPoints = new Tuple<Vector3, Vector3>((float)(t1)*ray_dir, (float)(t2) * ray_dir);

            return true;
        }

        public static bool PlaneSphere(Basics.Plane plane, Sphere sphere)
        {
            // d = (q - p.p[0])*p.n;
            //
            // Except d, all variables are 3d vectors(I use a simple class I made with operator overload).
            // d: the distance(scalar).
            // q: the point, in your case is the center of the sphere.
            // p.p[0]: a point belonging to the plane.Note that any point belonging to the plane will work.
            // p.n: normal to the plane.
            // The * is a dot product.



            var distToSphere = Vector3.Dot(sphere.center - plane.position, plane.normal);
            var compareEpsilon = .00001;
            if (Math.Abs(distToSphere) >= sphere.radius - compareEpsilon)
                return true;
            else
                return false;
        }

    }
}
