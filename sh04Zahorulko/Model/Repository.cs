using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace sh04Zahorulko;

public interface IIdProvider
{
    public int NextId { get; }
}
public class IdProvider : IIdProvider
{
    const string FILEPATH = "ids";
    private int id;
    public int NextId
    {
        get { return ++id; }
    }

    private IdProvider(int id)
    {
        this.id = id;
    }

    public static async Task<IdProvider> OpenOrCreate(string dir)
    {
        var filePath = Path.Combine(dir, FILEPATH);
        if (File.Exists(filePath))
        {
            var id = int.Parse(await File.ReadAllTextAsync(filePath));
            return new IdProvider(id);
        }
        else
        {
            return new IdProvider(0);
        }
    }

    public async Task Save(string dir)
    {
        var filePath = Path.Combine(dir, FILEPATH);
        await File.WriteAllTextAsync(filePath, id.ToString());
    }
}

public class PeopleProvider
{
    private string file;
    private readonly ObservableCollection<Person> people;
    public ObservableCollection<Person> People { get => people; }

    private PeopleProvider(string file, IEnumerable<Person> people)
    {
        this.file = file;
        this.people = new ObservableCollection<Person>(people);
    }

    private async Task AddOrUpdate(Person person)
    {
        var path = Path.Combine(file, person.Id.ToString());
        using var stream = new BufferedStream(File.Create(path));
        await JsonSerializer.SerializeAsync(stream, person.ToSerializable());
    }

    private Task Delete(int id)
    {
        var path = Path.Combine(file, id.ToString());
        File.Delete(path);
        return null!;
    }

    public async Task Add(Person p)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            people.Add(p);
        });
        await AddOrUpdate(p);
    }

    public async Task Edit(int ind, Person p)
    {
        if (ind > people.Count)
            throw new ArgumentOutOfRangeException();
        App.Current.Dispatcher.Invoke(() =>
        {
            people[ind] = p;
        });
        await AddOrUpdate(p);
    }

    public void Remove(int ind)
    {
        if (ind < people.Count)
        {
            var id = people[ind].Id;
            App.Current.Dispatcher.Invoke(() =>
            {
                Debug.WriteLine(people.Count);
                people.RemoveAt(ind);
            });
            Delete(id);
        }
    }

    public static PeopleProvider OpenOrCreate(string dir)
    {
        var path = Path.Combine(dir, "people");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        var list = new List<Person>();
        var files = Directory.GetFiles(path);
        foreach (var v in files)
        {
            var file = File.OpenRead(v);
            var person = JsonSerializer.Deserialize<SerializablePerson>(file) ?? throw new Exception();

            list.Add(person.ToPerson());
        }

        return new PeopleProvider(path, list);
    }



}

public class Repository
{

    public static readonly string BaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sh04Zahorulko");
    public IdProvider IdProvider { get; }
    public PeopleProvider PeopleProvider { get; }


    private Repository(IdProvider idProvider, PeopleProvider peopleProvider)
    {
        IdProvider = idProvider;
        PeopleProvider = peopleProvider;
    }

    private static Repository? instance;
    private static readonly SemaphoreSlim instanceMutex = new(1, 1);

    public static async Task<Repository> Get()
    {
        await instanceMutex.WaitAsync();
        try
        {
            if (instance is not null)
            {
                return instance;
            }
            instance = new Repository(await IdProvider.OpenOrCreate(BaseFolder), PeopleProvider.OpenOrCreate(BaseFolder));
            return instance;
        }
        finally
        {
            instanceMutex.Release(1);
        }

    }

    public async Task Save()
    {
        await IdProvider.Save(BaseFolder);
    }
}
