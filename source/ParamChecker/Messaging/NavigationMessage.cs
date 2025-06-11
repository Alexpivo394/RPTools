using System.Windows.Controls;

namespace ParamChecker.Messaging;

public class NavigationMessage
{
    public NavigationMessage(Page page)
    {
        Page = page;
    }

    public Page Page { get; }
}