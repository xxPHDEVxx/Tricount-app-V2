using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PRBD_Framework; 

public class DialogWindowBase : WindowBase {
    // see: https://stackoverflow.com/a/958980
    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    
    public DialogWindowBase() {
        Loaded += (o, e) => {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        };
        DataContextChanged += (o, e) => {
            if (e.OldValue != null)
                (e.OldValue as IDialogViewModelBase).DoClose -= DialogWindowBase_DoClose;
            if (e.NewValue != null)
                (e.NewValue as IDialogViewModelBase).DoClose += DialogWindowBase_DoClose;
        };
    }

    private void DialogWindowBase_DoClose() {
        Close();
    }

    public DialogViewModelBase<U, C> GetViewModel<U, C>() where U : EntityBase<C> where C : DbContextBase, new() {
        return (DialogViewModelBase<U, C>)DataContext;
    }
}