namespace SSPlan.ViewModels;

public class FamilyItem
{
    public FamilySymbol Symbol { get; }
    public string DisplayName { get; }

    public FamilyItem(FamilySymbol symbol)
    {
        Symbol = symbol;
        DisplayName = $"{symbol.Family.Name} - {symbol.Name}";
    }

    public override string ToString() => DisplayName;
}