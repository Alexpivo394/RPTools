using System.Collections.ObjectModel;
using Autodesk.Revit.DB.Electrical;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using Settings = SSPlan.Configuration.Settings;

namespace SSPlan.ViewModels;

public partial class SSPlanViewModel(Document doc) : ObservableObject
{
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private int _axisX = 0;
    [ObservableProperty] private int _axisY = 0;
    [ObservableProperty] private PanelItem _selectedPanel = null!;
    
    public ObservableCollection<PanelItem> Panels { get; } = new();

    public void LoadPanels()
    {
        var panels = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
            .WhereElementIsNotElementType()
            .OfType<FamilyInstance>()
            .Where(fi =>
            {
                var systems = fi.MEPModel?.GetElectricalSystems();
                return systems != null && systems.Any();

            });

        foreach (var fi in panels)
            Panels.Add(new PanelItem(fi));
    }

    
    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }
    
    public void LoadFromSettings(Settings settings)
    {
        DarkTheme = settings.DarkTheme;
        AxisX = settings.AxisX;
        AxisY = settings.AxisY;
    }

    public Settings ToSettings()
    {
        return new Settings
        {
            DarkTheme = DarkTheme,
            AxisX = AxisX,
            AxisY = AxisY
        };
    }
}