using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBViewer
{
    public class ChunkPos
    {
        private int x;
        private int z;
        private int dimension;
        public ChunkPos(int x, int z, int dimension)
        {
            this.x = x;
            this.z = z;
            this.dimension = dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ChunkPos)) return false;
            var other = obj as ChunkPos;
            return other.x == x && other.z == z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, z, dimension);
        }
    }
}
