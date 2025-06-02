using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ParamChecker.ViewModels.PagesViewModels;
using ParamChecker.Views.Dialogs;
using ParamChecker.Views.Pages;
using ParamChecker.Views.Windows;
using Wpf.Ui.Appearance;

namespace ParamChecker.ViewModels.Windows;

    public sealed partial class ParamCheckerViewModel : ObservableObject
    {
        public ObservableCollection<CustomNavItem> CustomNavItems { get; set; } = new();
        
        public Action<Page> NavigateAction { get; set; }
        
        
        [ObservableProperty]
        private string _title;
        
        [ObservableProperty]
        private bool _isChecked;
        
        [RelayCommand]
        private void AddCustomNavItem()
        {
            var vm = new ExportProfilesViewModel();
            vm.ProfileName = $"Профиль {CustomNavItems.Count + 1}";

            var page = new ExportProfiles
            {
                DataContext = vm
            };

            var item = new CustomNavItem(vm.ProfileName, page, vm, RemoveNavItem);
            item.OnNavigate = NavigateAction; // 👈 вот тут магия
            CustomNavItems.Add(item);
        }
        
        [RelayCommand]
        private void RemoveNavItem(CustomNavItem item)
        {
            if (CustomNavItems.Contains(item))
                CustomNavItems.Remove(item);
        }
        [RelayCommand]
        private void RenameNavItem()
        {
            
            var dialog = new Rename(Title);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                Title =  dialog.Title;
            }
        }
        
        
    }
