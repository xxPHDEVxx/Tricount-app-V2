using prbd_2324_a06.Model;
using System.Globalization;

namespace prbd_2324_a06.ViewModel;

public class OperationCardViewModel : ViewModelCommon
{
    public OperationCardViewModel(Operation operation) {
        Operation = operation;
    }

    private Operation _operation;

    public Operation Operation {
        get => _operation;
        init => SetProperty(ref _operation, value);
    }

    public string Title {
        get => Operation.Title;
        set => SetProperty(Operation.Title, value
            , Operation, (o, t) => { });
    }

    public string CreatedBy {
        get => Operation.Initiator.FullName;
        set => SetProperty(Operation.Initiator.FullName
            , value, Operation, (o, i) => { });
    }

    public string Amount {
        get => $"{_operation.Amount:F2} €";
        set => SetProperty($"{_operation.Amount:F2}", value
            , Operation, (o, a) => { });
    }

    public DateTime CreatedAt {
        get => Operation.OperationDate;
        set => SetProperty(Operation.OperationDate, value
            , Operation, (o, d) => { });
    }
}