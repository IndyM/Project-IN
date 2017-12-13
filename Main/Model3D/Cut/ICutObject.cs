using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model3D.Objects.Cut
{
    interface ICutObject
    {
        bool IsPointInBoundingBox(Vector3 point);
    }
}
