using System.Windows.Forms;

namespace coms.COMSK.ui.common
{
    public interface IVerticalMergeProvider<T>
    {
        bool MergeWithNextRow(DataGridView grid, T row, T nextRow, string columnName, int rowIndex);
    }
}