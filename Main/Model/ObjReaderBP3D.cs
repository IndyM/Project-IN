using DMS.Geometry;
using Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Model
{
    static class ObjReaderBP3D
    {
       
        /// <summary>
        /// Read a PB3D .obj File
        /// </summary>
        /// <param name="path"></param>
        /// <returns> The read MeshObjectBP3D </returns>
        public static MeshObjectBP3D ReadObj(String path)
        {
            MeshObjectBP3D meshObject = null;

            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;

                    string line;

                    List<String> namesBlock = new List<string>();
                    List<String> commentBlock = new List<string>();
                    List<String> vertexBlock = new List<string>();
                    List<String> normalBlock = new List<string>();
                    List<String> faceBlock = new List<string>();


                    // Read until the end of the file is reached.
                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();
                        String key, args;
                        SplitLine(line, out key, out args);
                        //Dataformat described in http://paulbourke.net/dataformats/obj/
                        switch (key)
                        {
                            case "o": namesBlock.Add(args); break;
                            case "#": commentBlock.Add(args); break;
                            case "v": vertexBlock.Add(args); break;
                            case "vn": normalBlock.Add(args); break;
                            case "f": faceBlock.Add(args); break;
                        }
                    }
                    meshObject = getNewMeshObjectByCommentBlock(commentBlock);
                    meshObject.Mesh = createMesh(vertexBlock, normalBlock, faceBlock);
                    var rot = Matrix4x4.CreateRotationX(DMS.Geometry.MathHelper.DegreesToRadians(-90));
                    Transform(meshObject, rot);


                    meshObject.Load();
                    //MeshObjectController.MeshObjects.Add(meshObject);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return meshObject;
        }

        public static void Transform(MeshObjectBP3D m, Matrix4x4 transform)
        {
            for (int i = 0; i < m.Mesh.position.List.Count; i++)
            {
                var newPos = Vector3.Transform(m.Mesh.position.List[i], transform);
                m.Mesh.position.List[i] = newPos;
            }
            for (int i = 0; i < m.Mesh.normal.List.Count; i++)// in m.Mesh.normal.List)
            {
                var newN = Vector3.TransformNormal(m.Mesh.normal.List[i], transform);
                m.Mesh.normal.List[i] = newN;
            }
            /*
            Vector3 vec1 = Vector3.Transform(m.Bounds.Item1, transform);
            Vector3 vec2 = Vector3.Transform(m.Bounds.Item2, transform);
            var bounds = new Tuple<Vector3, Vector3>(vec1, vec2);
            m.Bounds = bounds;
            */
            Mesh tmp = MyCube(m.Bounds.Item1, m.Bounds.Item2);
            tmp = tmp.Transform(transform);

            m.Bounds = getBounds(tmp);
            tmp = null;

        }

        public static Mesh MyCube(System.Numerics.Vector3 q, System.Numerics.Vector3 w)
        {
            var mesh = new Mesh();

            //corners
            var corners = new System.Numerics.Vector3[] {
                new System.Numerics.Vector3(w.X, w.Y, q.Z),  //0
                new System.Numerics.Vector3(w.X, w.Y, w.Z),  //1
                new System.Numerics.Vector3(q.X, w.Y, w.Z),  //2
                new System.Numerics.Vector3(q.X, w.Y, q.Z),  //3
                new System.Numerics.Vector3(w.X, q.Y, q.Z),  //4
                new System.Numerics.Vector3(q.X, q.Y, q.Z),  //5
                new System.Numerics.Vector3(q.X, q.Y, w.Z),  //6
                new System.Numerics.Vector3(w.X, q.Y, w.Z),  //7
            };

            uint id = 0;
            var n = -System.Numerics.Vector3.UnitX;

            Action<int> Add = (int pos) => { mesh.position.List.Add(corners[pos]); mesh.normal.List.Add(n); mesh.IDs.Add(id); mesh.corner.Add(pos); ++id; };

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

        public static Tuple<System.Numerics.Vector3, System.Numerics.Vector3> getBounds(Mesh m)
        {
            System.Numerics.Vector3 max = m.position.List[0];
            System.Numerics.Vector3 min = m.position.List[0];

            for (int i = 1; i < m.position.List.Count; i++)
            {
                if (max.X < m.position.List[i].X)
                {
                    max.X = m.position.List[i].X;
                }
                if (max.Y < m.position.List[i].Y)
                {
                    max.Y = m.position.List[i].Y;
                }
                if (max.Z < m.position.List[i].Z)
                {
                    max.Z = m.position.List[i].Z;
                }

                if (min.X > m.position.List[i].X)
                {
                    min.X = m.position.List[i].X;
                }
                if (min.Y > m.position.List[i].Y)
                {
                    min.Y = m.position.List[i].Y;
                }
                if (min.Z > m.position.List[i].Z)
                {
                    min.Z = m.position.List[i].Z;
                }
            }
            return new Tuple<System.Numerics.Vector3, System.Numerics.Vector3>(min, max); ;
        }

        internal static MeshObjectBP3DGroup ReadRelations(string path)
        {
            var treeBuildList = new ObservableCollection<MeshObjectBP3DGroup>();
            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {
                    
                    stream = null;

                    string line = reader.ReadLine();//skip the first line

                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();
                        var parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        var parent_ConID = parts[0];
                        var parent_name = parts[1];
                        var child_ConID = parts[2];
                        var child_name = parts[3];

                        MeshObjectBP3DGroup child = null;
                        foreach (var meshObject in MeshObjectController.RelationAllGroups)
                        {
                            if (meshObject.ConceptID.Equals(child_ConID)) { 
                                child = meshObject;
                                break;
                            }
                        }

                        MeshObjectBP3DGroup parent = null;
                        foreach (var meshObject in MeshObjectController.RelationAllGroups)
                        {
                            if (meshObject.ConceptID.Equals(parent_ConID))
                            {
                                parent = meshObject;
                                break;
                            }
                        }
                        if (parent == null ) {
                            parent = new MeshObjectBP3DGroup(parent_name) {
                                ConceptID = parent_ConID,
                            };
                            treeBuildList.Add(parent);
                            MeshObjectController.RelationAllGroups.Add(parent);
                        }
                        if (child == null)
                        {
                            child = new MeshObjectBP3DGroup(child_name)
                            {
                                ConceptID = child_ConID,
                            };
                            treeBuildList.Add(child);
                            MeshObjectController.RelationAllGroups.Add(child);
                        }

                        if (parent != null && child != null) {
                            child.Parent = parent;
                            parent.Children.Add(child);
                            treeBuildList.Remove(child);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return treeBuildList.FirstOrDefault();
        }

        public static ObservableCollection<MeshObjectBP3DGroup> ReadElementPartList(String path)
        {
            var ret = new ObservableCollection<MeshObjectBP3DGroup>();
            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {

                    stream = null;

                    MeshObjectBP3DGroup actualGroup = null; 
                    string line = reader.ReadLine();//skip the first line

                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();
                        var parts = line.Split(new char[] { '\t' },StringSplitOptions.RemoveEmptyEntries);
                        var conceptID = parts[0];
                        var name = parts[1];
                        var fileID = parts[2];

                        var found_parent = false;
                        MeshObjectBP3DGroup parent = null;
                        foreach (var meshObject in MeshObjectController.PartAllGroups) {
                            if (meshObject.ConceptID.Equals(conceptID))
                            {
                                found_parent = true;
                                parent = meshObject; 
                                break;
                            }
                        }
                        MeshObjectBP3DGroup child = null;
                        foreach (var meshObject in MeshObjectController.PartAllGroups)
                        {
                            if (meshObject.FileID.Equals(fileID))
                            {
                                child = meshObject; 
                                break;
                            }
                        }
                        if (parent == null) {
                            parent = new MeshObjectBP3DGroup(name) { ConceptID = conceptID };
                        }
                        if (child != null) {
                            if (!found_parent) { 
                                MeshObjectController.PartAllGroups.Add(parent);
                                ret.Add(parent);
                            }
                            parent.Children.Add(child);
                            
                            
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return ret;
        }

        private static Mesh createMesh(List<string> vertexBlock, List<string> normalBlock, List<string> faceBlock)
        {
            var mesh = new Mesh();
            foreach (var vertexLine in vertexBlock) {
                var vertexString = vertexLine.Split(new char[] {' ' },StringSplitOptions.RemoveEmptyEntries);
                var vertex = new Vector3() {
                    X = float.Parse(vertexString[0]),
                    Y = float.Parse(vertexString[1]), //Invert Y Direction
                    Z = float.Parse(vertexString[2]),
                };
                mesh.position.List.Add(vertex);
            }
            foreach (var normalLine in normalBlock)
            {
                var vertexString = normalLine.Split(new char[] {' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vertex = new Vector3()
                {
                    X = float.Parse(vertexString[0]),
                    Y = float.Parse(vertexString[1]),
                    Z = float.Parse(vertexString[2]),
                };
                mesh.normal.List.Add(vertex);
            }
 /*           foreach (var normalLine in normalBlock)
            {
                var vertexString = normalLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vertex = new Vector3()
                {
                    X = float.Parse(vertexString[0]),
                    Y = float.Parse(vertexString[1]),
                    Z = float.Parse(vertexString[2]),
                };
                mesh.normal.List.Add(vertex);
            }*/
            foreach (var faceLine in faceBlock)
            {
                var vertexString = faceLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var ele in vertexString) {
                    var facePart = ele.Split(new char[] { '/' });
                    mesh.IDs.Add(uint.Parse(facePart[0])-1);
                    //no uvs used, just add an empty Vector
                    mesh.uv.List.Add(new Vector2(.0f,.0f));
                }
            }

            return mesh;
        }

        private static MeshObjectBP3D getNewMeshObjectByCommentBlock(List<string> commentBlock)
        {
            if (commentBlock.Count == 0)
                return null;
            var firstline = 5;
            Double compatibilityVersion;

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Double.TryParse( commentBlock[firstline].Split(new char[]{ ':', ' ' }).Last(),out compatibilityVersion);

            String fileID = commentBlock[firstline+1].Split(new char[] { ':', ' ' }).Last();

            String RepresentationID = commentBlock[firstline+2].Split(new char[] { ':', ' ' }).Last();
            //Build UP
            String conceptID = commentBlock[firstline + 4].Split(new char[] { ':', ' ' }).Last();
            String name = commentBlock[firstline + 5].Split(new char[] { ':' }).Last().Substring(1);
            if (name == "")
                name = fileID+" NoName";
            var args = commentBlock[firstline + 6].Split(new char[] { ':' , ' '}).Last();
            var vecs = args.Split(new char[] { ' ','(',',',')' },StringSplitOptions.RemoveEmptyEntries);
            //Debug.WriteLine("vecs0: " + vecs[0]+" vecs1: "+vecs[1]+" vecs2: "+vecs[2] + " vecs3: " + vecs[4] + " vecs4: " + vecs[5] + " vecs5: " + vecs[6]);
            var vec1 = new Vector3() {
                X= float.Parse(vecs[0]),
                Y = float.Parse(vecs[1]),
                Z = float.Parse(vecs[2]),
            };
            var vec2 = new Vector3()
            {
                X = float.Parse(vecs[4]),
                Y = float.Parse(vecs[5]),
                Z = float.Parse(vecs[6]),
            };
            Debug.WriteLine(args);
            Debug.WriteLine("vec1: "+ vec1+ " vec2: "+ vec2);
            var bounds = new Tuple<Vector3, Vector3>(vec1,vec2);

            Double volume;
            Double.TryParse(commentBlock[firstline].Split(new char[] { ':', ' ' }).Last(), out volume);

            var ret = new MeshObjectBP3D(name) {
                  CompatibilityVersion = compatibilityVersion,
                  FileID = fileID,
                  RepresentationID = RepresentationID,
                  ConceptID = conceptID,
                  Bounds = bounds,
                  Volume = volume,
            };


            return ret;
        }

        /// <summary>
        /// Splits a line in keyword and arguments.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="keyword">
        /// The keyword.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        private static void SplitLine(string line, out string keyword, out string arguments)
        {
            int idx = line.IndexOf(' ');
            if (idx < 0)
            {
                keyword = line;
                arguments = null;
                return;
            }

            keyword = line.Substring(0, idx);
            arguments = line.Substring(idx + 1);
        }

    }
}
