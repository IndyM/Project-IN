using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public static class ObjReader
    {
        public static List<MeshObject> read(String path) {
           
            var meshObjects = new List<MeshObject>();

            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;
   
                    MeshObject meshObject = null;
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();


                    }
                }
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return meshObjects;
        }
    }
}
