using Nice3point.Revit.Toolkit.External;
using System.Windows.Media;

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
            ;

            //Добавляем кнопки на панели
            //BIM
            panelBim.AddPushButton<CheckLOI.Command.StartupCommand>("Экспорт отчета\nLOI")
                .SetImage("/RPToolsApp;component/Resources/Icons/Export16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Export32.png")
                .SetToolTip("Выгружает из моделей на сервере\nотчет по заполнению параметров.");

            panelBim.AddPushButton<WorkingSet.Commands.StartupCommand>("Создание Рабочих\nнаборов")
                .SetImage("/RPToolsApp;component/Resources/Icons/Create16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Create32.png")
                .SetToolTip("Создает в модели рабочие наборы\nдля выбранного раздела.");

            panelBim.AddPushButton<ModelTransplanter.Commands.StartupCommand>("Копирование элементов")
                .SetImage("/RPToolsApp;component/Resources/Icons/Transplanter16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Transplanter32.png")
                .SetToolTip("Позволяет копировать элементы\nиз модели в модель.");

            //Лотки
            panelTray.AddPushButton<ArticulLotok.StartupCommand>("Артикулы и \nнаименования")
                .SetImage("/RPToolsApp;component/Resources/Icons/1Tray16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/1Tray32.png")
                .SetToolTip("Заполнение артикулов и наименования у выбранных лотков.");

            panelTray.AddPushButton<LotkiColor.StartupCommand>("Покрасить\nпо перфорации")
                .SetImage("/RPToolsApp;component/Resources/Icons/2Tray16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/2Tray32.png")
                .SetToolTip("Покрасит лотки, \nобычные - в фиолетовый, \nперфорированные - в зеленый.");

            panelTray.AddPushButton<LotkiColorIsp.StartupCommand>("Покрасить\nпо исполнению")
                .SetImage("/RPToolsApp;component/Resources/Icons/3Tray16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/3Tray32.png")
                .SetToolTip("Покрасит лотки, \n1 - по методу Сендзимира - в розовый\n2 - горячего цинкования - в синий \n3 - из нержавеющей стали - в зеленый");

            panelTray.AddPushButton<LotkiColorKrshka.StartupCommand>("Покрасить\nкрышки")
                .SetImage("/RPToolsApp;component/Resources/Icons/4Tray16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/4Tray32.png")
                .SetToolTip("Покрасит лотки, \nпо наличию крышки");

            //Общие
            panelGeneral.AddPushButton<Leght.StartupCommand>("Длина элементов\nмодели")
                .SetImage("/RPToolsApp;component/Resources/Icons/Ruler16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Ruler32.png")
                .SetToolTip("Посчитать суммарную длину выбранных элементов.");
        }        

    }
}