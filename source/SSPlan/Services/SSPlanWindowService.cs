using System.Windows;

namespace SSPlan.Services;

public class SSPlanWindowService : IWindowService
{
    private Window? _window;
    private bool _isDialogOpen;

    public SSPlanWindowService(Window window)
    {
        _window = window;
        _window.Closed += (s, e) => _window = null;
    }

    public void Show()
    {
        if (_window == null) return;
        
        if (!_window.IsVisible)
        {
            _isDialogOpen = true;
            _window.ShowDialog();
            _isDialogOpen = false;
        }
        else
        {
            _window.Activate();
        }
    }

    public void Hide()
    {
        if (_window != null && _window.IsVisible)
        {
            _window.Hide();
        }
    }

    public void Activate()
    {
        _window?.Activate();
    }

    public void Close()
    {
        if (_window != null)
        {
            if (_isDialogOpen)
            {
                _window.DialogResult = false;
            }
            _window.Close();
            _window = null;
        }
    }
}