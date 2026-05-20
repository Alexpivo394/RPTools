using System.Collections.ObjectModel;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using WriteDash.Models;
using WriteDash.Services;

namespace WriteDash.ViewModels;

public sealed partial class WriteDashViewModel : ObservableObject
{
    private readonly ParameterProcessorService _processorService;

    [ObservableProperty]
    private bool _darkTheme = true;

    [ObservableProperty]
    private string? _parametersFilter;

    public ObservableCollection<ParameterDescriptor> Parameters { get; } = new();

    public ObservableCollection<ParameterDescriptor> FilteredParameters { get; } = new();

    public WriteDashViewModel(
        Document doc,
        RevitParameterService parameterService,
        ParameterProcessorService processorService)
    {
        _processorService = processorService;

        var parameters = parameterService.GetProjectParameters(doc);

        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter);
        }

        ApplyFilter();
    }

    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;

        ThemeWatcherService.ApplyTheme(newTheme);
    }

    partial void OnParametersFilterChanged(string? value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        FilteredParameters.Clear();

        IEnumerable<ParameterDescriptor> filtered = Parameters;

        // "*" -> только выбранные
        if (ParametersFilter == "*")
        {
            filtered = filtered.Where(x => x.IsSelected);
        }
        else if (!string.IsNullOrWhiteSpace(ParametersFilter))
        {
            filtered = filtered.Where(x =>
                x.Name != null &&
#if REVIT2025_OR_GREATER
                x.Name.Contains(
                    ParametersFilter,
                    StringComparison.OrdinalIgnoreCase));
#else
                x.Name.IndexOf(
                    ParametersFilter,
                    StringComparison.OrdinalIgnoreCase)>= 0);
#endif
        }

        foreach (var parameter in filtered.OrderBy(x => x.Name))
        {
            FilteredParameters.Add(parameter);
        }
    }

    [RelayCommand]
    private void Start()
    {
        var selectedParameters = Parameters
            .Where(x => x.IsSelected)
            .ToList();

        _processorService.Process(selectedParameters);
    }
}