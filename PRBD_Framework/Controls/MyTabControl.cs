using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PRBD_Framework; 

public class MyTabControl : TabControl, IDisposable {
    public TabItem Add(ContentControl content, string header, object tag = null) {

        // refuser l'ajout si le tab courant est dirty
        if (IsTabDirty(SelectedItem as TabItem)) {
            DisplayChangeTabError();
            return null;
        }

        // crée le tab
        var tab = new TabItem() {
            Content = content,
            Header = header,
            Tag = tag ?? header
        };

        if (HasCloseButton) {
            // crée le header du tab avec le bouton de fermeture
            var headerPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            headerPanel.Children.Add(new TextBlock() { Text = header, VerticalAlignment = VerticalAlignment.Center });
            var closeButton = new Button() {
                Content = "X",
                FontWeight = FontWeights.Bold,
                Background = Brushes.Transparent,
                Foreground = Brushes.Red,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "Close"
            };
            headerPanel.Children.Add(closeButton);
            tab.Header = headerPanel;

            closeButton.Click += (_, _) => CloseTab(tab);
        }

        // ajoute cet onglet à la liste des onglets existant du TabControl
        Items.Add(tab);
        // exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
        SetFocus(tab);
        return tab;
    }

    private void CloseTab(TabItem tab) {
        if (tab == null) return;
            
        if (IsTabDirty(tab))
            DisplayChangeTabError();
        else
            Items.Remove(tab);

        //if (tab.Content != null) {
        //    var vm = (tab.Content as dynamic)?.DataContext;
        //    if (vm != null && (vm.HasErrors || vm.HasChanges || !vm.MayLeave))
        //        MessageBox.Show(Application.Current.MainWindow,
        //            "You have unsaved changes and/or errors", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    else
        //        Items.Remove(tab);
        //} else
        //    Items.Remove(tab);
    }

    public static void RenameTab(TabItem tab, string newName) {
        if (tab.Header is StackPanel stackPanel)
            ((TextBlock)stackPanel.Children[0]).Text = newName;
        else
            tab.Header = newName;
        tab.Tag = newName;
    }

    private static void Dispose(IEnumerable<TabItem> tabs) {
        var tabsToDispose = new List<TabItem>(tabs);
        foreach (TabItem tab in tabsToDispose) {
            if (tab.Content == null) continue;
            var content = (tab.Content as ContentControl)?.Content as FrameworkElement;
            if (content?.DataContext is IDisposable ctx) {
                ctx.Dispose();
                content.DataContext = null;
            }
            // dispose du UserControlBase lui-même
            if (tab.Content is UserControlBase uc)
                uc.Dispose();
        }
    }

    public void Dispose() {
        Dispose(Items.OfType<TabItem>());
    }

    public void CloseByTag(string tag) {
        var tab = FindByTag(tag);
        if (tab != null)
            CloseTab(tab);
    }

    public TabItem FindByTag(string tag) {
        return (from TabItem t in Items where tag == t.Tag?.ToString() select t).FirstOrDefault();
    }

    public TabItem FindByHeader(string header) {
        return (from TabItem t in Items where header == t.Header?.ToString() select t).FirstOrDefault();
    }

    public void SetFocus(TabItem tab) {
        Dispatcher.InvokeAsync(() => {
            tab.Focus();
            SelectedItem = tab;
        });
    }

    private bool InheritsFrom(Type source, Type toCheck) {
        //TODO: improve this check
        if (source.Name == toCheck.Name)
            return true;
        if (source.BaseType != typeof(object))
            return InheritsFrom(source.BaseType, toCheck);
        return false;
    }

    private bool IsTabDirty(HeaderedContentControl tab) {
        if (tab?.Content is not UserControlBase uc ||
            !InheritsFrom(uc.DataContext.GetType(), typeof(ViewModelBase<,>))) return false;
        dynamic vm = uc.DataContext;
        if (vm.HasErrors)
            Console.WriteLine($"Tab dirty (errors): {tab.Name} - {tab.Header} - {tab.Tag}");
        if (vm.HasChanges)
            Console.WriteLine($"Tab dirty (changes): {tab.Name} - {tab.Header} - {tab.Tag}");
        if (!vm.MayLeave)
            Console.WriteLine($"Tab dirty (may not leave): {tab.Name} - {tab.Header} - {tab.Tag}");
        return vm.HasErrors || vm.HasChanges || !vm.MayLeave;
    }

    private static void DisplayChangeTabError() {
        //TODO: parfois ce message apparaît alors qu'il ne devrait pas, mais pas encore identifié pourquoi/quand ?
        // return;

        if (Application.Current.MainWindow != null)
            MessageBox.Show(Application.Current.MainWindow,
                "You have unsaved changes and/or errors", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private bool selectedTabChanging;

    protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
        // vérifie si le VM associé au UC contenu dans le TabItem n'a pas de changements en cours
        if (selectedTabChanging) return;
        var oldItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as TabItem : null;
        if (IsTabDirty(oldItem)) {
            selectedTabChanging = true;
            SelectedItem = oldItem;
            DisplayChangeTabError();
            selectedTabChanging = false;
        }

        // vérifie si au moins un des tabs sélectionnés est visible
        bool visible = e.AddedItems.Cast<TabItem>().Any(tab => tab.Visibility == Visibility.Visible);

        if (!visible) {
            // cherche un tab visible
            foreach (TabItem tab in Items)
                if (tab.Visibility == Visibility.Visible) {
                    SelectedItem = tab;
                    e.Handled = false;
                    break;
                }
        }

        base.OnSelectionChanged(e);
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
        base.OnItemsChanged(e);
        switch (e.Action) {
            case NotifyCollectionChangedAction.Remove: {
                if (e.OldItems != null) Dispose(e.OldItems.OfType<TabItem>());
                break;
            }
            case NotifyCollectionChangedAction.Add: {
                if (e.NewItems != null)
                    foreach (TabItem tab in e.NewItems) {
                        tab.MouseDown += (_, ee) => {
                            if (ee.ChangedButton == MouseButton.Middle &&
                                ee.ButtonState == MouseButtonState.Pressed) {
                                CloseTab(tab);
                            }
                        };
                        tab.PreviewKeyDown += (_, ee) => {
                            if (ee.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl)) {
                                CloseTab(tab);
                            }
                        };
                    }

                break;
            }
        }
    }

    public bool HasCloseButton {
        get => (bool)GetValue(HasCloseButtonProperty);
        set => SetValue(HasCloseButtonProperty, value);
    }

    public static readonly DependencyProperty HasCloseButtonProperty = DependencyProperty.Register(
        nameof(HasCloseButton), typeof(bool), typeof(MyTabControl), new PropertyMetadata(false));
}