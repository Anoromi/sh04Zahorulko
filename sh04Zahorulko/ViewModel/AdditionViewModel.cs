using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Navigation;
using sh01Zahorulko;
using sh04Zahorulko;

namespace sh04Zahorulko.ViewModel
{

    public class AdditionViewModel : INotifyPropertyChanged, INavigatable<MainNavigationTypes>
    {
        private readonly Action<string> notify;

        private Dependency<bool> active;
        public bool Active { get => active.Value; set => active.Value = value; }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string AddressPath { get; set; } = "";
        public DateTime? Birthday { get; set; } = null;

        private Dependency<Repository?> repository;

        private Dependency<string?> error;
        public string? Error
        {
            get => error.Value;
            set => error.Value = value;
        }


        private Dependency<Person?> person;
        public Person? Person
        {
            get => person.Value;
            set => person.Value = value;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public Command<object> Click
        {
            get => new(async _ =>
            {
                active.Value = false;
                await Task.Delay(1000);
                await ApplyValues();
                active.Value = true;
            }, canExecute: CheckAvailability);
        }

        public MainNavigationTypes ViewType => MainNavigationTypes.Addition;

        public async Task ApplyValues()
        {
            Address address;
            try
            {
                address = new Address(AddressPath);
            }
            catch (IllegalAddress e)
            {
                Error = e.Message;
                return;
            }
            BirthdayDate birthdayDate;
            try
            {
                birthdayDate = BirthdayDate.Parse(Birthday!.Value);
            }
            catch (TooOldException e)
            {
                Error = e.Message;
                return;
            }
            catch (NotYetBornException e)
            {
                Error = e.Message;
                return;
            }

            var p = new Person(repository.Value!.IdProvider.NextId, FirstName, LastName, address, birthdayDate);
            Person = p;
            await repository.Value!.PeopleProvider.Add(p);
        }

        public bool CheckAvailability(object? _) => repository.Value is not null && !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName) && !string.IsNullOrWhiteSpace(AddressPath) && Birthday != null;

        public AdditionViewModel(Dependency<Repository?> repository)
        {
            notify = (s) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(s));
            active = new(true, (nameof(Active), notify));
            person = new(null, (nameof(Person), notify));
            error = new(null, postAction: (s) => { if (s is not null) MessageBox.Show(s); });
            this.repository = repository.Copy();
        }
    }
}
