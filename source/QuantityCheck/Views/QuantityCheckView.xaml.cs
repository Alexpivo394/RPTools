using QuantityCheck.ViewModels;

namespace QuantityCheck.Views;

public sealed partial class QuantityCheckView
{
    public QuantityCheckView(QuantityCheckViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}