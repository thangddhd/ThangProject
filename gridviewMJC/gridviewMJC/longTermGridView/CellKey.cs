using System;

namespace coms.COMSK.ui.common
{
    internal struct CellKey : IEquatable<CellKey>
    {
        public readonly int Row;
        public readonly int Col;

        public CellKey(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool Equals(CellKey other)
        {
            return Row == other.Row && Col == other.Col;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CellKey)) return false;
            return Equals((CellKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Col;
            }
        }

        public override string ToString()
        {
            return "R" + Row + "C" + Col;
        }
    }
}