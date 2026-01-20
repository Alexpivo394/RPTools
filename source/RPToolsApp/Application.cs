using Nice3point.Revit.Toolkit.External;
using System.Windows.Media;
using Autodesk.Revit.UI;

namespace RPToolsApp
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            Host.Start();
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            //Добавляем панели
            var panelBim = Application.CreatePanel("BIM", "RPTools");
            var panelBackgroundBrushDustyBlue =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(173, 216, 230));
            panelBim.SetTitleBarBackground(panelBackgroundBrushDustyBlue);


            var panelTray = Application.CreatePanel("Лотки", "RPTools");
            var panelBackgroundBrushMutedOlive =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(152, 168, 124));
            panelTray.SetTitleBarBackground(panelBackgroundBrushMutedOlive);

            var panelGeneral = Application.CreatePanel("Общие", "RPTools");
            var panelBackgroundBrushSoftTerracotta =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 153, 102));
            panelGeneral.SetTitleBarBackground(panelBackgroundBrushSoftTerracotta);
            
            var panelSs = Application.CreatePanel("Сети связи", "RPTools");
            var panelBackgroundBrushPlum =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 242, 222));
            panelSs.SetTitleBarBackground(panelBackgroundBrushPlum);
            
            var panelOv = Application.CreatePanel("ОВ", "RPTools");
            var panelBackgroundBrushRose = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 209, 220));
            panelOv.SetTitleBarBackground(panelBackgroundBrushRose);
            
            var panelAr = Application.CreatePanel("АР", "RPTools");
            var panelBackgroundBrushBezheviy = new SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 245, 220));
            panelAr.SetTitleBarBackground(panelBackgroundBrushBezheviy);

            //Добавляем кнопки на панели
            //BIM
            panelBim.AddPushButton<ParamChecker.Commands.StartupCommand>("ParamChecker")
                .SetImage("/RPToolsApp;component/Resources/Icons/Export16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Export32.png")
                .SetToolTip("Выгружает из моделей на сервере\nотчет по заполнению параметров.")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/paramchecker-movFIkYSwy"));

            panelBim.AddPushButton<WorkingSet.Commands.StartupCommand>("Создание Рабочих\nнаборов")
                .SetImage("/RPToolsApp;component/Resources/Icons/Create16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Create32.png")
                .SetToolTip("Создает в модели рабочие наборы\nдля выбранного раздела.")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/sozdanie-rabochih-naborov-plaginom-rptools-j3Ycy39HVW"));
            
            panelBim.AddPushButton<QuantityCheck.Commands.StartupCommand>("Записать количество")
                .SetImage("/RPToolsApp;component/Resources/Icons/QuantityCheck16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/QuantityCheck32.png")
                .SetToolTip("Заполняет параметр количества\nв элементах модели.")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/zapolnenie-parametra-kolichestvo-beinN88LwP"));

            panelBim.AddPushButton<ModelTransplanter.Commands.StartupCommand>("Копирование элементов")
                .SetImage("/RPToolsApp;component/Resources/Icons/Transplanter16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Transplanter32.png")
                .SetToolTip("Позволяет копировать элементы\nиз модели в модель.")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/kopirovanie-soderzhimogo-modeli-7q8Wf296FB"));
            
            panelBim.AddPushButton<WorksetCheck.Commands.StartupCommand>("Проверка рабочих\nнаборов")
                .SetImage("/RPToolsApp;component/Resources/Icons/WorksetsCheck16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/WorksetsCheck32.png")
                .SetToolTip("Позволяет проверить заданные\nмодели на соответствие элементов\nрабочим наборам.")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/worksetcheck-rfycq6j0Bd"));

            //Лотки
            var pullButtonTray = panelTray.AddPullDownButton("Кабельные лотки", "Кабельные лотки");

            pullButtonTray.SetImage("/RPToolsApp;component/Resources/Icons/1Tray16.png");
            pullButtonTray.SetLargeImage("/RPToolsApp;component/Resources/Icons/1Tray32.png");
            pullButtonTray.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url,
                "https://bim-baza.yonote.ru/doc/kabelnye-lotki-hSP8BFIW8g"));

            pullButtonTray.AddPushButton<ArticulLotok.StartupCommand>("Артикулы и наименования");
            pullButtonTray.AddPushButton<LotkiColor.StartupCommand>("Покрасить по перфорации");
            pullButtonTray.AddPushButton<LotkiColorIsp.StartupCommand>("Покрасить по исполнению");
            // pullButton.AddPushButton<LotkiColorKrshka.StartupCommand>("Покрасить крышки");
            
            panelTray.AddPushButton<CreateCover.Commands.StartupCommand>("Разместить\nкрышки")
                .SetImage("/RPToolsApp;component/Resources/Icons/CreateCover16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/CreateCover32.png")
                .SetToolTip("Разместить в модели крышки на кабельные лотки")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/razmestit-kryshki-pJSWjFkyzL"));
            
            //Общие
            panelGeneral.AddPushButton<Leght.StartupCommand>("Длина элементов\nмодели")
                .SetImage("/RPToolsApp;component/Resources/Icons/Ruler16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Ruler32.png")
                .SetToolTip("Посчитать суммарную длину выбранных элементов.");
            
            //Сети связи
            panelSs.AddPushButton<SSPlan.Commands.StartupCommand>("Структурная\nсхема")
                .SetImage("/RPToolsApp;component/Resources/Icons/SSPlan16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/SSPlan32.png")
                .SetToolTip("Создать структурную схему.")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/strukturnaya-shema-yO3TQrgqVS"));
            
            //Панель ОВ
            var pullButtonWarm = panelOv.AddPullDownButton("Теплопотери", "Теплопотери");
            
            pullButtonWarm.SetImage("/RPToolsApp;component/Resources/Icons/WarmSync16.png");
            pullButtonWarm.SetLargeImage("/RPToolsApp;component/Resources/Icons/WarmSync32.png");
            pullButtonWarm.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url,
                "https://bim-baza.yonote.ru/doc/sinhronizaciya-teplopoter-RWMUuxZBdq"));

            pullButtonWarm.AddPushButton<WarmSync.CreateSpaces>("Создание пространств");
            pullButtonWarm.AddPushButton<WarmSync.RenameSpaces>("Заполнение имен пространств");
            pullButtonWarm.AddPushButton<WarmSync.WriteSpaceIdToParam>("Запись ID пространств");
            pullButtonWarm.AddPushButton<WarmSync.ExportSpacesToExcel>("Экспорт пространств в Excel");
            pullButtonWarm.AddPushButton<WarmSync.WriteFromExcel>("Импорт значений из Excel");
            
            //Панель АР
            
            panelAr.AddPushButton<DoorSide.DoorSideCommand>("Открывание\nдверей")
                .SetImage("/RPToolsApp;component/Resources/Icons/DoorSide16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/DoorSide32.png")
                .SetToolTip("Записать сторону открывания двери в параметр Открывание")
                .SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://bim-baza.yonote.ru/doc/otkryvanie-dveri-JJyZ1390oD"));
        }        

    }
}