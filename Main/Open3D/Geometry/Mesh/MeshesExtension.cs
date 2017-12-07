using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;

namespace Zenseless.Geometry
{
    public static partial class MeshesExtension
    {
        public static DefaultMesh CreateCuboid(float sizeX, float sizeY, float sizeZ, uint segmentsX, uint segmentsY, uint segmentsZ)
        {
            float deltaX = (1.0f / segmentsX) * sizeX;
            float deltaY = (1.0f / segmentsY) * sizeY;
            float deltaZ = (1.0f / segmentsZ) * sizeZ;
            DefaultMesh m = new DefaultMesh();
            //add vertices

            // Top Face
            float fix_y = sizeY / 2.0f;
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsZ + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.Position.Add(new Vector3(x, fix_y, z));
                    m.Normal.Add(Vector3.UnitY);
                    m.TexCoord.Add(new Vector2(u, v));
                }
            }

            // Bottom Face
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsZ + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.Position.Add(new Vector3(x, -fix_y, z));
                    m.Normal.Add(-Vector3.UnitY);
                    m.TexCoord.Add(new Vector2(u, v));
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

                    m.Position.Add(new Vector3(x, y, fix_z));
                    m.Normal.Add(Vector3.UnitZ);
                    m.TexCoord.Add(new Vector2(u, v));
                }
            }
            // Back Face
            for (uint u = 0; u < segmentsX + 1; ++u)
            {
                for (uint v = 0; v < segmentsY + 1; ++v)
                {
                    float x = -sizeX / 2.0f + u * deltaX;
                    float y = -sizeY / 2.0f + v * deltaY;

                    m.Position.Add(new Vector3(x, y, -fix_z));
                    m.Normal.Add(-Vector3.UnitZ);
                    m.TexCoord.Add(new Vector2(u, v));
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

                    m.Position.Add(new Vector3(fix_x, y, z));
                    m.Normal.Add(Vector3.UnitX);
                    m.TexCoord.Add(new Vector2(u, v));
                }
            }
            // Left Face
            for (uint u = 0; u < segmentsZ + 1; ++u)
            {
                for (uint v = 0; v < segmentsY + 1; ++v)
                {
                    float y = -sizeY / 2.0f + u * deltaY;
                    float z = -sizeZ / 2.0f + v * deltaZ;

                    m.Position.Add(new Vector3(-fix_x, y, z));
                    m.Normal.Add(-Vector3.UnitX);
                    m.TexCoord.Add(new Vector2(u, v));
                }
            }



            uint verticesZ = segmentsZ + 1;
            uint verticesY = segmentsY + 1;
            //add ids
            uint oneFace_point_count = (uint)(m.Position.Count / 6);
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

        public static DefaultMesh CreateCuboid(Vector3 size, uint segmentsX, uint segmentsY, uint segmentsZ)
        {
            return CreateCuboid(size.X, size.Y, size.Z, segmentsX, segmentsY, segmentsZ);
        }
    }
}
