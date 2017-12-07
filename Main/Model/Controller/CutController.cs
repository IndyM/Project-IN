using Model.Objects.Cut;
using Model.Objects.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Controller
{
    public static class CutController
    {
        public static List<IMeshObject> MeshObjectsToCut { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public static List<CuboidCut> CutObjects { get; set; }


    }
}
