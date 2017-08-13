using DMS.Geometry;
using Model.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
                    meshObject.Mesh = createMesh(vertexBlock, normalBlock, faceBlock);
                    meshObject.Load();
                    MeshObjectController.MeshObjects.Add(meshObject);
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

        private static Mesh createMesh(List<string> vertexBlock, List<string> normalBlock, List<string> faceBlock)
        {
            var mesh = new Mesh();
            foreach (var vertexLine in vertexBlock) {
                var vertexString = vertexLine.Split(new char[] {' ' },StringSplitOptions.RemoveEmptyEntries);
                var vertex = new Vector3() {
                    X = float.Parse(vertexString[0]),
                    Y = float.Parse(vertexString[1]),
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
            foreach (var normalLine in normalBlock)
            {
                var vertexString = normalLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vertex = new Vector3()
                {
                    X = float.Parse(vertexString[0]),
                    Y = float.Parse(vertexString[1]),
                    Z = float.Parse(vertexString[2]),
                };
                mesh.normal.List.Add(vertex);
            }
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
            var vecs = args.Split(new char[] { '-', ' ','(',',',')' },StringSplitOptions.RemoveEmptyEntries);
            var vec1 = new Vector3() {
                X= float.Parse(vecs[0]),
                Y = float.Parse(vecs[1]),
                Z = float.Parse(vecs[2]),
            };
            var vec2 = new Vector3()
            {
                X = float.Parse(vecs[3]),
                Y = float.Parse(vecs[4]),
                Z = float.Parse(vecs[5]),
            };
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
