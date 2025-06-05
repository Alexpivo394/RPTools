using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace RPToolsUI.Services;

public class ThemeWatcherService
{
    private static readonly List<FrameworkElement> ObservedElements = new List<FrameworkElement>();
    public static void Initialize()
    {
        UiApplication.Current.Resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/RPToolsUI;component/Styles/App.Resources.xaml", UriKind.Absolute)
        };
        ApplicationThemeManager.Changed += OnApplicationThemeManagerChanged;
    }

    public static void ApplyTheme(ApplicationTheme theme)
    {
        ApplicationThemeManager.Apply(theme);
        UpdateBackground(theme);
    }

    private static void OnApplicationThemeManagerChanged(ApplicationTheme currentapplicationtheme, System.Windows.Media.Color systemaccent)
    {
        foreach (var frameworkElement in ObservedElements)
        {
            ApplicationThemeManager.Apply(frameworkElement);

            UpdateDictionary(frameworkElement);
        }
    }

    private static void UpdateDictionary(FrameworkElement frameworkElement)
    {
        var themedResources = frameworkElement.Resources.MergedDictionaries
            .Where(dictionary => dictionary.Source.OriginalString.Contains("RPToolsUI;", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        frameworkElement.Resources.MergedDictionaries.Insert(0, UiApplication.Current.Resources.MergedDictionaries[0]);
        frameworkElement.Resources.MergedDictionaries.Insert(1, UiApplication.Current.Resources.MergedDictionaries[1]);

        foreach (var themedResource in themedResources)
        {
            frameworkElement.Resources.MergedDictionaries.Remove(themedResource);
        }
    }
    public static void Watch(FrameworkElement frameworkElement)
    {
        ApplicationThemeManager.Apply(frameworkElement);
        frameworkElement.Loaded += OnWatchedElementLoaded;
        frameworkElement.Unloaded += OnWatchedElementUnloaded;

    }
    private static void OnWatchedElementLoaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        ObservedElements.Add(element);

        if (element.Resources.MergedDictionaries[0].Source.OriginalString != UiApplication.Current.Resources.MergedDictionaries[0].Source.OriginalString)
        {
            ApplicationThemeManager.Apply(element);
            UpdateDictionary(element);
        }
    }
    private static void OnWatchedElementUnloaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        ObservedElements.Remove(element);
    }
    private static void UpdateBackground(ApplicationTheme theme)
    {
        foreach (var window in ObservedElements.Select(Window.GetWindow).Distinct())
        {
            WindowBackgroundManager.UpdateBackground(window, theme, WindowBackdropType.Mica);
        }
    }
}
