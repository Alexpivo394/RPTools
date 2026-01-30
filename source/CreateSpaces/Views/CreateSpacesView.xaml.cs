using CreateSpaces.ViewModels;

namespace CreateSpaces.Views;

public sealed partial class CreateSpacesView
{
    public CreateSpacesView(CreateSpacesViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}