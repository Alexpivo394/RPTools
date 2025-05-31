using System.Windows.Controls;

namespace ParamChecker.Messaging;


public class NavigationMessage
{
    public Page Page { get; }

    public NavigationMessage(Page page)
    {
        Page = page;
    }
}