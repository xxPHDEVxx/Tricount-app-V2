using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PRBD_Framework; 

// see: https://forums.xamarin.com/discussion/29925/observablecollection-addrange
public class ObservableCollectionFast<T> : ObservableCollection<T> {
    public ObservableCollectionFast() { }

    public ObservableCollectionFast(IEnumerable<T> collection) : base(collection) { }

    public ObservableCollectionFast(List<T> list) : base(list) { }

    public void RefreshFromModel(IEnumerable<T> range) {
        Items.Clear();

        if (range != null)
            foreach (var item in range) {
                Items.Add(item);
            }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}