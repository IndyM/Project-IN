﻿

using Model.Controller;
using Model.Objects.BP3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Reader
{
    public static class ObjReader
    {
        // http://paulbourke.net/dataformats/obj/
        public static List<PolygonObjectBP3D> Read(String path) {
           
            var meshObjects = new List<PolygonObjectBP3D>();

            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;
   
                    PolygonObjectBP3D meshObject = null;
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = reader.ReadLine()) != null)
                    {
                        line.Trim();
                        String key, args;
                        SplitLine(line,out key,out args);

                        switch (key) {
                            case "o": meshObject = new PolygonObjectBP3D(args); break;
                            case "v":
                                if (meshObject == null)
                                    meshObject = new PolygonObjectBP3D("Object_"+ ModelController.PolygonObjects.Count);


                                break;
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            return meshObjects;
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
