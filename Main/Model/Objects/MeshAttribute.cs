using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class MeshAttribute<TYPE> : IMeshAttribute<TYPE>
    {
        public MeshAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public List<TYPE> List { get { return list; } }

        private readonly List<TYPE> list = new List<TYPE>();
    }
}
