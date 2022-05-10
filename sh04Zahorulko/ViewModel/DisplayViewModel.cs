using Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh04Zahorulko.ViewModel
{
    public enum DisplayNavigationTypes
    {
        Viewer,
        Editor
    }
    class DisplayViewModel : BaseNavigatableViewModel<DisplayNavigationTypes>, INavigatable<MainNavigationTypes>
    {
        private readonly Dependency<int> selectedIndex;
        private readonly Dependency<Person?> selectedPerson = new(null);
        private readonly Dependency<bool> tableActivity;
        private readonly Dependency<PeopleProvider?> peopleProvider;
        private readonly Dependency<Repository?> repository = new(null);

        public DisplayViewModel(Dependency<bool> tableActivity, Dependency<Repository?> repository)
        {
            this.tableActivity = tableActivity.Copy();
            this.selectedIndex = new(-1, postAction: v =>
            {
                selectedPerson.Value = v >= 0 ? peopleProvider!.Value!.People[v] : null;
            });
            peopleProvider = new(null);
            this.repository = repository.Copy(postAction: v =>
            {
                if (v is not null)
                    peopleProvider.Value = v.PeopleProvider;
            });

            Navigate(DisplayNavigationTypes.Viewer);

        }

        public MainNavigationTypes ViewType => MainNavigationTypes.Display;

        protected override INavigatable<DisplayNavigationTypes> CreateViewModel(DisplayNavigationTypes type)
        {
            return type switch
            {
                DisplayNavigationTypes.Viewer => new UserViewerViewModel(() => Navigate(DisplayNavigationTypes.Editor), peopleProvider, selectedIndex, tableActivity),
                DisplayNavigationTypes.Editor => new PersonViewModel(selectedPerson, () =>
                {
                    Navigate(DisplayNavigationTypes.Viewer);
                    Task.Run(async () => await peopleProvider.Value!.Edit(selectedIndex, selectedPerson!));

                }, 
                () => Navigate(DisplayNavigationTypes.Viewer)),
                _ => throw new NotImplementedException(),
            };
        }


    }
}
