using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum HitType
    {
        Red,
        Blue
    }
    public struct HitMark
    {
        public HitType Type;
        public float Time;
    }
}
