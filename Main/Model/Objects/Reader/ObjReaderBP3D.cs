using Model.Controller;
using Model.Objects.BP3D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenseless.Geometry;

namespace Model.Reader
{
    static class ObjReaderBP3D
    {
        //Translate to Model-Center using Verteces min/max
        /*      public static readonly Vector3 vec_center_translation = new Vector3() {
                  X = .0f,
                  Y = 859.735591888428f,
                  Z = 146.015302658081f,
              };*/
        //Translate to Model-Center using Verteces sum
        public static readonly Vector3 vec_center_translation = new Vector3()
        {
            X = .0f,
            Y = 1096.42678852839f,
            Z = -97.8305799292432f,
        };


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
                            case "#": commentBlock.Add(args); break;
                            case "v": vertexBlock.Add(args); break;
                            case "vn": normalBlock.Add(args); break;
                            case "f": faceBlock.Add(args); break;
                        }
                    }
                    meshObject = getNewMeshObjectByCommentBlock(commentBlock);
                    meshObject.MeshObject.Mesh = createMesh(vertexBlock, normalBlock, faceBlock);
                    //meshObject.Load();
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

        internal static IObjectBP3DGroup ReadRelations(string path)
        {
            var treeBuildList = new ObservableCollection<IObjectBP3DGroup>();
            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;

                    var relationAllGroups = new List<IObjectBP3DGroup>();
                    foreach(var meshObjectBP3D in MeshObjectController.MeshObjects)
                        relationAllGroups.Add(new MeshObjectBP3DGroup(meshObjectBP3D) );

                    string line = reader.ReadLine();//skip the first line

                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();
                        var parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        var parent_ConID = parts[0];
                        var parent_name = parts[1];
                        var child_ConID = parts[2];
                        var child_name = parts[3];

                        IObjectBP3DGroup child = null;
                        IObjectBP3DGroup parent = null;

                        foreach(var objectBP3D in relationAllGroups)
                        {
                            if (child==null && objectBP3D.ConceptID.Equals(child_ConID)) 
                                child = objectBP3D;
                            else if(parent==null && objectBP3D.ConceptID.Equals(parent_ConID))
                                parent = objectBP3D;
                            
                            if (child != null && parent != null)
                                break;
                        }

                        if (parent == null ) {
                            parent = new ObjectBP3DGroup(parent_name) {
                                ConceptID = parent_ConID,
                            };
                            treeBuildList.Add(parent);
                            relationAllGroups.Add(parent);
                        }
                        if (child == null)
                        {
                            child = new ObjectBP3DGroup(child_name) {
                                ConceptID = child_ConID,
                            };
                            treeBuildList.Add(child);
                            relationAllGroups.Add(child);
                        }

                        parent.Children.Add(child);
                        treeBuildList.Remove(child);
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

        public static ObservableCollection<IObjectBP3DGroup> ReadElementPartList(String path)
        {
            var ret = new ObservableCollection<IObjectBP3DGroup>();
            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {

                    stream = null;

                    //MeshObjectBP3DGroup actualGroup = null; 
                    string line = reader.ReadLine();//skip the first line

                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();
                        var parts = line.Split(new char[] { '\t' },StringSplitOptions.RemoveEmptyEntries);
                        var conceptID = parts[0];
                        var name = parts[1];
                        var fileID = parts[2];



                        IObjectBP3DGroup child = null;
                        IObjectBP3DGroup parent = null;
                        
                        foreach (var meshObject in ret) {
                            if (meshObject.ConceptID.Equals(conceptID)) {
                                parent = meshObject; 
                                break;
                            }
                        }

                        if (parent == null) // Parent can be MeshObject
                        {
                            foreach (MeshObjectBP3D meshObjectBP3D in MeshObjectController.MeshObjects)
                            {
                                if (meshObjectBP3D.ConceptID.Equals(conceptID)) // Parent is MeshObject ... create MeshObjectGroup
                                {
                                    parent = new MeshObjectBP3DGroup(meshObjectBP3D);
                                    ret.Add(parent);

                                    break;
                                }
                            }
                        }


                        foreach (MeshObjectBP3D meshObjectBP3D in MeshObjectController.MeshObjects) {
                            if (meshObjectBP3D.FileID.Equals(fileID)) {
                                child = new MeshObjectBP3DGroup (meshObjectBP3D); 
                                break;
                            }
                        }

                        if (parent == null) { // No PArent found ... create new ObjectGroup
                            parent = new ObjectBP3DGroup(name) { ConceptID = conceptID };
                            ret.Add(parent);
                        }

                        if (child != null) {
                            parent.Children.Add(child);
                        }
                        else
                            System.Diagnostics.Debug.WriteLine(nameof(ObjReaderBP3D) + "ReadElementPartList, couldn't find child MeshObject");
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
        private static Vector3 convertOrientation(Vector3 bp3dVector) {

            Vector3 normalSystem = new Vector3()
            {
                X = bp3dVector.X,
                Y = bp3dVector.Z ,
                Z = bp3dVector.Y ,
            };
            return normalSystem ;
        }

        private static DefaultMesh createMesh(List<string> vertexBlock, List<string> normalBlock, List<string> faceBlock)
        {

            var mesh = new DefaultMesh();
            foreach (var vertexLine in vertexBlock) {
                var vertexString = vertexLine.Split(new char[] {' ' },StringSplitOptions.RemoveEmptyEntries);


                var vertex = new Vector3() {
                    X = float.Parse(vertexString[0]),
                    Y = float.Parse(vertexString[1]),
                    Z = float.Parse(vertexString[2]),
            };
                mesh.Position.Add(convertOrientation(vertex) - vec_center_translation);
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
                mesh.Normal.Add(convertOrientation(vertex));
            }
            foreach (var faceLine in faceBlock)
            {
                var vertexString = faceLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var ele in vertexString) {
                    var facePart = ele.Split(new char[] { '/' });
                    mesh.IDs.Add(uint.Parse(facePart[0])-1);
                    //no uvs used, just add an empty Vector
                    mesh.TexCoord.Add(new Vector2(.0f,.0f));
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
            var vecs = args.Split(new string[] { ")-(" },StringSplitOptions.None);
           
            var first = vecs[0].Split(new char[] { ' ', '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            var sec =  vecs[1].Split(new char[] { ' ', '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            var vec1 = new Vector3() {
                X= float.Parse(first[0]),
                Y = float.Parse(first[1]),
                Z = float.Parse(first[2]),
            };
            var vec2 = new Vector3()
            {
                X = float.Parse(sec[0]),
                Y = float.Parse(sec[1]),
                Z = float.Parse(sec[2]),
            };
            var bounds = new Tuple<Vector3, Vector3>(convertOrientation(vec1)-vec_center_translation, convertOrientation(vec2)- vec_center_translation);

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
