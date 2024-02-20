using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PRBD_Framework;

public class MyDataGrid : DataGrid {
    public MyDataGrid() {
        SelectionChanged += CustomDataGrid_SelectionChanged;
    }

    private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        SelectedItemsList = this.SelectedItems;
    }

    public IList SelectedItemsList {
        get => (IList)GetValue(SelectedItemsListProperty);
        set => SetValue(SelectedItemsListProperty, value);
    }

    public static readonly DependencyProperty SelectedItemsListProperty =
        DependencyProperty.Register(nameof(SelectedItemsList), typeof(IList), typeof(MyDataGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
}