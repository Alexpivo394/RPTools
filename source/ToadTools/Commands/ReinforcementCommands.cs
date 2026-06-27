using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ReinforcementByColor;

namespace ToadTools.Commands;

/// <summary>
///     Replaces filled regions with R-SHP reinforcement aligned along Y.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class RSHPLeftRightCommand : ExternalCommand
{
    public override void Execute() => new RSHPLeftRight().Run();
}

/// <summary>
///     Replaces filled regions with R-SHP reinforcement aligned along X.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class RSHPUpDownCommand : ExternalCommand
{
    public override void Execute() => new RSHPUpDown().Run();
}

/// <summary>
///     Replaces filled regions with R-SUM reinforcement aligned along Y.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class RSUMLeftRightCommand : ExternalCommand
{
    public override void Execute() => new RSUMLeftRight().Run();
}

/// <summary>
///     Replaces filled regions with R-SUM reinforcement aligned along X.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class RSUMUpDownCommand : ExternalCommand
{
    public override void Execute() => new RSUMUpDown().Run();
}
