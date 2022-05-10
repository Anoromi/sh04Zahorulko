using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Navigation;
using sh01Zahorulko;
using sh04Zahorulko.ViewModel;

namespace sh04Zahorulko.ViewModel
{
    public struct SortOptions
    {
        public readonly Func<Person, IComparable?> selector;
        public readonly bool ascending;

        public SortOptions(Func<Person, IComparable?> selector, bool ascending)
        {
            this.selector = selector;
            this.ascending = ascending;
        }
    }
    public class UserViewerViewModel : INotifyPropertyChanged, INavigatable<DisplayNavigationTypes>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Action<string> notify;

        private readonly Dependency<PeopleProvider?> peopleProvider;
        public PeopleProvider? Provider
        {
            get => peopleProvider.Value;
            private set => peopleProvider.Value = value;
        }

        private Dependency<ObservableCollection<Person>> people;
        public ObservableCollection<Person> People { get => people; private set => people.Value = value; }

        private readonly Action toEdit;
        private readonly Dependency<bool> tableActivity;
        public bool TableActivity { get => tableActivity; set => tableActivity.Value = value; }
        public Command<object> Edit
        {
            get => new(_ =>
            {
                if (SelectedIndex >= 0)
                {
                    toEdit();
                }
            }, _ => SelectedIndex >= 0 && tableActivity.Value);
        }
        public Command<object> Delete
        {
            get
            {
                return new(_ =>
                {
                    if (SelectedIndex >= 0)
                    {
                        tableActivity.Value = false;
                        Provider!.Remove(SelectedIndex);
                        tableActivity.Value = true;
                    }

                }, _ => SelectedIndex >= 0 && tableActivity.Value);
            }
        }

        public readonly Command<SortOptions> sort;

        private Dependency<int> selectedIndex;
        private Person? selectedValue;

        public int SelectedIndex
        {
            get => selectedIndex;
            set => selectedIndex.Value = value;
        }

        public Person? SelectedValue
        {
            get => selectedValue;
            set
            {
                selectedValue = value;
                SelectedIndex = selectedValue is not null ? People.IndexOf(selectedValue) : -1;
            }
        }

        public void SortValues(SortOptions options)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                tableActivity.Value = false;

                var sorted = new ObservableCollection<Person>(options.ascending ?
                    People.OrderBy(options.selector) : People.OrderByDescending(options.selector));
                People.Clear();
                foreach (var v in sorted)
                    People.Add(v);
                tableActivity.Value = true;
            });
        }


        public DisplayNavigationTypes ViewType => DisplayNavigationTypes.Viewer;

        public UserViewerViewModel(Action toEdit, Dependency<PeopleProvider?> peopleProvider, Dependency<int> selectedIndex, Dependency<bool> tableActivity)
        {
            notify =
                (s) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(s));

            this.toEdit = toEdit;
            this.selectedIndex = selectedIndex.Copy((nameof(SelectedIndex), OnPropertyChanged));
            this.tableActivity = tableActivity.Copy((nameof(TableActivity), OnPropertyChanged));

            people = new(new(), (nameof(People), OnPropertyChanged));
            this.peopleProvider = peopleProvider.Copy((nameof(Provider), OnPropertyChanged), postAction: v =>
            {
                if (v is not null)
                    People = v.People;
            });
            sort = new(SortValues, _ => tableActivity.Value);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
