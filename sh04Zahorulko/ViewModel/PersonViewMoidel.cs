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
using sh04Zahorulko.ViewModel;

namespace sh04Zahorulko.ViewModel;

public class PersonViewModel : INotifyPropertyChanged, INavigatable<DisplayNavigationTypes>
{
    private readonly Action<string> notify;

    private Dependency<bool> active;
    public bool Active { get => active.Value; set => active.Value = value; }

    private readonly Dependency<Person?> person;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string AddressPath { get; set; } = "";
    public DateTime? Birthday { get; set; } = null;

    private readonly Action onChange;

    private readonly Action goBack;
    public Command<object> Back { get; }

    private Dependency<string?> error;
    public string? Error
    {
        get => error.Value;
        set => error.Value = value;
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    public Command<object> Click
    {
        get => new(async _ =>
        {
            active.Value = false;
            await Task.Delay(1000);
            ApplyValues();
            active.Value = true;
        }, canExecute: CheckAvailability);
    }

    public DisplayNavigationTypes ViewType => DisplayNavigationTypes.Editor;

    public void ApplyValues()
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

        //var p = new Person(repository.IdProvider.NextId, FirstName, LastName, address, birthdayDate);
        //person = p;
        person.Value!.FirstName = FirstName;
        person.Value!.LastName = LastName;
        person.Value!.Birthday = birthdayDate;
        person.Value!.Address = address;

        onChange();
        //await repository.PeopleProvider.Add(p);
    }

    public bool CheckAvailability(object? _) => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName) && !string.IsNullOrWhiteSpace(AddressPath) && Birthday != null;

    public PersonViewModel(Dependency<Person?> person, Action onChange, Action goBack)
    {
        notify = (s) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(s));
        active = new(true, (nameof(Active), notify));
        error = new(null, postAction: (s) => { if (s is not null) MessageBox.Show(s); });
        this.person = person.Copy(postAction: p =>
        {
            FirstName = p?.FirstName ?? "";
            LastName = p?.LastName ?? "";
            AddressPath = p?.Address?.Path ?? "";
            Birthday = p?.Birthday?.Date;
        });
        this.goBack = goBack;
        Back = new Command<object>(_ => goBack());
        this.onChange = onChange;
    }
}

