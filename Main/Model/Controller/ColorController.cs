using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Controller
{
    public static class ColorController
    {
        private static Random r = new Random();
        
        public static OpenTK.Vector4 getColor()
        {
            return new OpenTK.Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1.0f);
        }
    }
}
