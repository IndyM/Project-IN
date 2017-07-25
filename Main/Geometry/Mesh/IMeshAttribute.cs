using System.Collections.Generic;

namespace Geometry
{
    public interface IMeshAttribute<TYPE>
    {
        string Name { get; }
        List<TYPE> List { get; }
    }
}