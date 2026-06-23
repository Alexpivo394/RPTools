using System.Windows.Controls;
using ToadTools.UI.Services;

namespace ParamChecker.Views.Pages;

public partial class ExportProfiles : Page

{
    public ExportProfiles()
    {
        InitializeComponent();
        ThemeWatcherService.Watch(this);
    }
}