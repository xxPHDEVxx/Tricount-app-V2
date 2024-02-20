using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace PRBD_Framework; 

public static class ObservableCollectionExtensions {
    [Obsolete("Cette méthode est obsolète car très lente à cause du fait que les notifications sont déclenchées " +
              "pour chaque élément inséré. Il est recommandé d'utiliser plutôt la classe ObservableCollectionFast qui permet " +
              "de faire un reset du contenu en une seule opération et de ne déclencher les notifications qu'à la fin.")]
    public static void RefreshFromModel<T>(this ObservableCollection<T> list, IEnumerable<T> model) {
        if (model == null) return;
        list.Clear();
        foreach (var o in model)
            list.Add(o);
    }

    public static ICollectionView GetCollectionView<T>(this ObservableCollection<T> list,
        params SortDescription[] sortDescriptions) {
        var view = CollectionViewSource.GetDefaultView(list);
        if (view != null && view.SortDescriptions.Count == 0)
            foreach (var sd in sortDescriptions)
                view.SortDescriptions.Add(sd);
        return view;
    }

    public static ICollectionView GetCollectionView<T>(this ObservableCollection<T> list,
        string propName, ListSortDirection direction = ListSortDirection.Ascending) {
        return GetCollectionView(list, new SortDescription(propName, direction));
    }

    public static void RemoveRange<T>(this ObservableCollection<T> list, params T[] itemsToDelete) {
        foreach (T item in itemsToDelete)
            list.Remove(item);
    }
}