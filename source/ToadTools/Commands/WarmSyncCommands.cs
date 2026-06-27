using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace ToadTools.Commands;

/// <summary>
///     Fills MEP space numbers from the linked architectural rooms.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class RenameSpacesCommand : ExternalCommand
{
    public override void Execute() => new WarmSync.RenameSpaces().Run();
}

/// <summary>
///     Writes each space element id into the "ID элемента" parameter.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class WriteSpaceIdToParamCommand : ExternalCommand
{
    public override void Execute() => new WarmSync.WriteSpaceIdToParam().Run();
}

/// <summary>
///     Exports spaces to an Excel workbook.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExportSpacesToExcelCommand : ExternalCommand
{
    public override void Execute() => new WarmSync.ExportSpacesToExcel().Run();
}

/// <summary>
///     Imports heat-loss values from an Excel workbook into the spaces.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class WriteFromExcelCommand : ExternalCommand
{
    public override void Execute() => new WarmSync.WriteFromExcel().Run();
}
