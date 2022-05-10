using Navigation;
using sh01Zahorulko;
using sh04Zahorulko.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh04Zahorulko.ViewModel
{
    public class MainWindowViewModel : BaseNavigatableViewModel<MainNavigationTypes>
    {

        public Command<object> ToAddition
        {
            get => new(_ =>
            {
                Navigate(MainNavigationTypes.Addition);
            }, _ => tableActivity.Value);
        }

        public Command<object> ToView
        {
            get => new(_ =>
            {
                Navigate(MainNavigationTypes.Display);
            }, _ => tableActivity.Value);
        }


        private Dependency<bool> tableActivity;
        public bool TableActivity { get => tableActivity.Value; set => tableActivity.Value = value; }

        private Dependency<Repository?> repository;
        public Repository? Repository { get => repository; private set => repository.Value = value; }

        public MainWindowViewModel()
        {
            tableActivity = new(true, (nameof(TableActivity), OnPropertyChanged));
            repository = new(null);
            Task.Run(async () => await LoadRepository());
            Navigate(MainNavigationTypes.Addition);
        }
        protected override INavigatable<MainNavigationTypes> CreateViewModel(MainNavigationTypes type) => type switch
        {
            MainNavigationTypes.Display => new DisplayViewModel(tableActivity, repository),
            MainNavigationTypes.Addition => new AdditionViewModel(repository),
            _ => throw new Exception(),
        };

        private async Task LoadRepository()
        {
            tableActivity.Value = false;
            await Task.Delay(1000);
            var r = await Repository.Get();
            repository.Value = r;
            tableActivity.Value = true;
        }
    }
}
