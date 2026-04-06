using System.Collections.Generic;

namespace coms.COMSK.ui.common
{
    internal sealed class MergeStore
    {
        public readonly Dictionary<CellKey, CellKey> OwnerByCell = new Dictionary<CellKey, CellKey>();
        public readonly Dictionary<CellKey, MergeRegion> RegionByOwner = new Dictionary<CellKey, MergeRegion>();

        public void Clear()
        {
            OwnerByCell.Clear();
            RegionByOwner.Clear();
        }

        public bool TryGetOwner(int row, int col, out CellKey owner)
        {
            return OwnerByCell.TryGetValue(new CellKey(row, col), out owner);
        }

        public bool TryGetRegionByOwner(CellKey owner, out MergeRegion region)
        {
            return RegionByOwner.TryGetValue(owner, out region);
        }

        public bool IsOwnerCell(int row, int col)
        {
            return RegionByOwner.ContainsKey(new CellKey(row, col));
        }
    }
}