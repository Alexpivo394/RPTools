﻿using System;
using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using Assembly = System.Reflection.Assembly;

const string outputName = "RPTools";
const string projectName = "RPTools";
const string manufacturer = "AlexB";

var project = new Project
{
    OutDir = "output",
    Name = projectName,
    Platform = Platform.x64,
    UI = WUI.WixUI_FeatureTree,
    MajorUpgrade = MajorUpgrade.Default,
    GUID = new Guid("2FAF75B7-963A-4C4B-8F71-E4248014047A"),
    BannerImage = @"install\Resources\Icons\BannerImage.png",
    BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
    Version = Assembly.GetExecutingAssembly().GetName().Version.ClearRevision(),
    ControlPanelInfo =
    {
        Manufacturer = manufacturer,
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(args);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);

BuildSingleUserMsi();
BuildMultiUserUserMsi();

void BuildSingleUserMsi()
{
    project.InstallScope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{project.Version}-SingleUser";
    project.Dirs =
    [
        new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    ];
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.InstallScope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{project.Version}-MultiUser";
    project.Dirs =
    [
        new InstallDir(@"%CommonAppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    ];
    project.BuildMsi();
}