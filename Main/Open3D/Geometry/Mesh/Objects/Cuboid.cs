
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;

namespace Open3D.Geometry.Objects
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
            get { return _scale; }
            set { _scale = value; }
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
            for (int i = 0; i < cuboidOneSegment.Position.Count; i += face_pointCount) {
                ret.Add(new MeshPoint() {
                    position = InstancePosition + cuboidOneSegment.Position[i],
                    normal = cuboidOneSegment.Normal[i],
                });
            }

            return ret;
        }

/*
        private void Create()
        {
            float deltaX = (1.0f / _segmentsX) * _sizeX;
            float deltaY = (1.0f / _segmentsY) * sizeY;
            float deltaZ = (1.0f / _segmentsZ) * sizeZ;
            Mesh m = new Mesh();
            //add vertices

            // Top Face
            float fix_y = sizeY / 2.0f;
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsZ + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.position.List.Add(new Vector3(x, fix_y, z));
                    m.normal.List.Add(Vector3.UnitY);
                    m.uv.List.Add(new Vector2(u, v));
                }
            }

            // Bottom Face
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsZ + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.position.List.Add(new Vector3(x, -fix_y, z));
                    m.normal.List.Add(-Vector3.UnitY);
                    m.uv.List.Add(new Vector2(u, v));
                }
            }

            // Front Face
            float fix_z = sizeZ / 2.0f;
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsY + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float y = -sizeY / 2.0f + v * deltaY;

                    m.position.List.Add(new Vector3(x, y, fix_z));
                    m.normal.List.Add(Vector3.UnitZ);
                    m.uv.List.Add(new Vector2(u, v));
                }
            }
            // Back Face
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsY + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float y = -sizeY / 2.0f + v * deltaY;

                    m.position.List.Add(new Vector3(x, y, -fix_z));
                    m.normal.List.Add(-Vector3.UnitZ);
                    m.uv.List.Add(new Vector2(u, v));
                }
            }
            // Right Face
            float fix_x = sizeX / 2.0f;
            for (uint u = 0; u < segmentsZ + 1; ++u)
            {
                for (uint v = 0; v < segmentsY + 1; ++v)
                {
                    float y = -sizeY / 2.0f + u * deltaY;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.position.List.Add(new Vector3(fix_x, y, z));
                    m.normal.List.Add(Vector3.UnitX);
                    m.uv.List.Add(new Vector2(u, v));
                }
            }
            // Left Face
            for (uint u = 0; u < segmentsZ + 1; ++u)
            {
                for (uint v = 0; v < segmentsY + 1; ++v)
                {
                    float y = -sizeY / 2.0f + u * deltaY;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.position.List.Add(new Vector3(-fix_x, y, z));
                    m.normal.List.Add(-Vector3.UnitX);
                    m.uv.List.Add(new Vector2(u, v));
                }
            }



            uint verticesZ = segmentsZ + 1;
            uint verticesY = segmentsY + 1;
            //add ids
            uint oneFace_point_count = (uint)(m.position.List.Count / 6);
            uint offset = 0;
            //Top & Bot
            for (uint u = 0; u < 2; ++u)
            {
                for (uint v = 0; v < segmentsX; ++v)
                {
                    for (uint w = 0; w < segmentsZ; ++w)
                    {
                        m.IDs.Add(offset + v * verticesZ + w);
                        m.IDs.Add(offset + v * verticesZ + w + 1);
                        m.IDs.Add(offset + (v + 1) * verticesZ + w);

                        m.IDs.Add(offset + (v + 1) * verticesZ + w);
                        m.IDs.Add(offset + v * verticesZ + w + 1);
                        m.IDs.Add(offset + (v + 1) * verticesZ + w + 1);
                    }
                }
                offset += oneFace_point_count;
            }
            //Front& Back
            for (uint u = 0; u < 2; ++u)
            {
                for (uint v = 0; v < segmentsX; ++v)
                {
                    for (uint w = 0; w < segmentsY; ++w)
                    {
                        m.IDs.Add(offset + v * verticesY + w);
                        m.IDs.Add(offset + v * verticesY + w + 1);
                        m.IDs.Add(offset + (v + 1) * verticesY + w);

                        m.IDs.Add(offset + (v + 1) * verticesY + w);
                        m.IDs.Add(offset + v * verticesY + w + 1);
                        m.IDs.Add(offset + (v + 1) * verticesY + w + 1);
                    }
                }
                offset += oneFace_point_count;
            }

            //Right & Left
            for (uint u = 0; u < 2; ++u)
            {
                for (uint v = 0; v < segmentsZ; ++v)
                {
                    for (uint w = 0; w < segmentsY; ++w)
                    {
                        m.IDs.Add(offset + v * verticesY + w);
                        m.IDs.Add(offset + v * verticesY + w + 1);
                        m.IDs.Add(offset + (v + 1) * verticesY + w);

                        m.IDs.Add(offset + (v + 1) * verticesY + w);
                        m.IDs.Add(offset + v * verticesY + w + 1);
                        m.IDs.Add(offset + (v + 1) * verticesY + w + 1);
                    }
                }
                offset += oneFace_point_count;
            }

            return m;

        }
        */
        /*
        public static DMS.Geometry.Mesh CreateCuboid(Vector3 size, uint segmentsX, uint segmentsY, uint segmentsZ)
        {
            return CreateCuboid(size.X, size.Y, size.Z, segmentsX, segmentsY, segmentsZ);
        }*/
    }
}
