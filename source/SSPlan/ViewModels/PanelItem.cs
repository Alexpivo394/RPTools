namespace SSPlan.ViewModels;

public class PanelItem
{
    public ElementId Id { get; }
    public string Name { get; }

    public PanelItem(FamilyInstance fi)
    {
        Id = fi.Id;
        Name = fi.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME)?.AsString() ?? $"[Unnamed {fi.Id}]";
    }

    public override string ToString() => Name; // чтобы ComboBox показывал имя
}