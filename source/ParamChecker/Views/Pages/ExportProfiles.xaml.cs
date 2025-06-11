using System.Windows.Controls;
using RPToolsUI.Services;

namespace ParamChecker.Views.Pages;

public partial class ExportProfiles : Page

{
    public ExportProfiles()
    {
        InitializeComponent();
        ThemeWatcherService.Watch(this);
    }
}