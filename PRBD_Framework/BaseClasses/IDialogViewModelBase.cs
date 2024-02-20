namespace PRBD_Framework; 

public interface IDialogViewModelBase {
    event Action DoClose;
    object DialogResult { get; set; }
}