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
            
            var panelSS = Application.CreatePanel("Сети связи", "RPTools");
            var panelBackgroundBrushPlum =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 242, 222)); // Plum Purple
            panelSS.SetTitleBarBackground(panelBackgroundBrushPlum);
            ;

            //Добавляем кнопки на панели
            //BIM
            panelBim.AddPushButton<ParamChecker.Commands.StartupCommand>("ParamChecker")
                .SetImage("/RPToolsApp;component/Resources/Icons/Export16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Export32.png")
                .SetToolTip("Выгружает из моделей на сервере\nотчет по заполнению параметров.");

            panelBim.AddPushButton<WorkingSet.Commands.StartupCommand>("Создание Рабочих\nнаборов")
                .SetImage("/RPToolsApp;component/Resources/Icons/Create16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Create32.png")
                .SetToolTip("Создает в модели рабочие наборы\nдля выбранного раздела.");
            
            panelBim.AddPushButton<QuantityCheck.Commands.StartupCommand>("Записать количество")
                .SetImage("/RPToolsApp;component/Resources/Icons/QuantityCheck16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/QuantityCheck32.png")
                .SetToolTip("Заполняет параметр количества\nв элементах модели.");

            panelBim.AddPushButton<ModelTransplanter.Commands.StartupCommand>("Копирование элементов")
                .SetImage("/RPToolsApp;component/Resources/Icons/Transplanter16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Transplanter32.png")
                .SetToolTip("Позволяет копировать элементы\nиз модели в модель.");

            //Лотки
            var pullButton = panelTray.AddPullDownButton("Кабельные лотки", "Кабельные лотки");

            pullButton.SetImage("/RPToolsApp;component/Resources/Icons/1Tray16.png");
            pullButton.SetLargeImage("/RPToolsApp;component/Resources/Icons/1Tray32.png");

            pullButton.AddPushButton<ArticulLotok.StartupCommand>("Артикулы и наименования");
            pullButton.AddPushButton<LotkiColor.StartupCommand>("Покрасить по перфорации");
            pullButton.AddPushButton<LotkiColorIsp.StartupCommand>("Покрасить по исполнению");
            // pullButton.AddPushButton<LotkiColorKrshka.StartupCommand>("Покрасить крышки");
            pullButton.AddPushButton<CreateCover.StartupCommand>("Разместить крышки");
            
            //Общие
            panelGeneral.AddPushButton<Leght.StartupCommand>("Длина элементов\nмодели")
                .SetImage("/RPToolsApp;component/Resources/Icons/Ruler16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/Ruler32.png")
                .SetToolTip("Посчитать суммарную длину выбранных элементов.");
            
            //Сети связи
            panelSS.AddPushButton<SSPlan.Commands.StartupCommand>("Структурная\nсхема")
                .SetImage("/RPToolsApp;component/Resources/Icons/SSPlan16.png")
                .SetLargeImage("/RPToolsApp;component/Resources/Icons/SSPlan32.png")
                .SetToolTip("Создать структурную схему.");
        }        

    }
}