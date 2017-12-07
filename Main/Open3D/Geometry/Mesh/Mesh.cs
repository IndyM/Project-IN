
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Open3D.Geometry
{
    public class Mesh
    {
        /// <summary>
        /// The position name
        /// </summary>
        public static readonly string PositionName = "position";
        /// <summary>
        /// The normal name
        /// </summary>
        public static readonly string NormalName = "normal";
        /// <summary>
        /// The tex coord name
        /// </summary>
        public static readonly string TexCoordName = "uv";

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public List<Vector3> Position => position = new List<Vector3>();
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>
        /// The normal.
        /// </value>
        public List<Vector3> Normal => normal = new List<Vector3>();
        /// <summary>
        /// Gets the tex coord.
        /// </summary>
        /// <value>
        /// The tex coord.
        /// </value>
        public List<Vector2> TexCoord => texCoord = new List<Vector2>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        public Mesh()
        {

        }

        /// <summary>
        /// The position
        /// </summary>
        private List<Vector3> position;
        /// <summary>
        /// The normal
        /// </summary>
        private List<Vector3> normal;
        /// <summary>
        /// The tex coord
        /// </summary>
        private List<Vector2> texCoord;
    }
}
