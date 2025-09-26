using System.Diagnostics;
using Autodesk.Revit.UI;

namespace WorksetCheck.Services;

public class OpenModelService
{
    public Document? OpenDocumentAsDetach(ExternalCommandData commandData, string filePath)
    {
        var app = commandData.Application;
        var modelPathServ = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
        var controlledApp = app.Application;

        var openOptions = new OpenOptions();

        openOptions.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
        
        FailureProcessorOpenDocument? failureProcessor;
        failureProcessor = null;
        Document? openDoc;

        try
        {
            failureProcessor = new FailureProcessorOpenDocument();

            controlledApp.FailuresProcessing += failureProcessor.ApplicationOnFailuresProcessing;
            app.DialogBoxShowing += failureProcessor.UIApplicationOnDialogBoxShowing;

            using (var transGroup = new TransactionGroup(app.ActiveUIDocument.Document, "Open Document Group"))
            {
                transGroup.Start();

                using (var transaction = new Transaction(app.ActiveUIDocument.Document, "Open Document"))
                {
                    transaction.Start();
                    openDoc = controlledApp.OpenDocumentFile(modelPathServ, openOptions);
                    transaction.Commit();
                }

                transGroup.Assimilate();
            }

            if (openDoc != null && openDoc.IsValidObject)
            {
                return openDoc;
            }
            
            return null;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
            throw;
        }
        finally
        {
            if (failureProcessor != null)
            {
                controlledApp.FailuresProcessing -= failureProcessor.ApplicationOnFailuresProcessing;
                app.DialogBoxShowing -= failureProcessor.UIApplicationOnDialogBoxShowing;
            }
        }
    }
}