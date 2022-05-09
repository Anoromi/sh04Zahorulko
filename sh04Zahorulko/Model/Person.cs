using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sh01Zahorulko;

namespace sh04Zahorulko
{
    [Serializable]
    public record SerializablePerson(int Id, string FirstName, string LastName, string? Address, DateTime? Birthday)
    {
        public Person ToPerson() => new(Id,
                                        FirstName,
                                        LastName,
                                        Address is not null ? new sh04Zahorulko.Address(Address) : null,
                                        Birthday is not null ? BirthdayDate.Parse(Birthday.Value) : null);
    }
    [Serializable]
    public class Person
    {
        public int Id { get; init; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address? Address { get; set; }
        public BirthdayDate? Birthday
        {
            get => birthday; set
            {
                birthday = value;
                _isAdult = birthday is not null ? birthday.Years >= 18 : null;
                _age = birthday?.Years;
                _sunSign = birthday?.WesternZodiac;
                _chineseSign = birthday?.ChineseZodiac;
                _isBirthday = birthday?.IsBirthDay(DateTime.Now);
            }
        }

        public Person(int id, string firstName, string lastName, Address? address = null, BirthdayDate? birthday = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Birthday = birthday;
        }

        private bool? _isAdult;
        public bool? IsAdult => _isAdult;

        private int? _age;
        public int? Age => _age;

        private WesternZodiac? _sunSign;
        public WesternZodiac? SunSign => _sunSign;

        private ChineseZodiac? _chineseSign;
        public ChineseZodiac? ChineseSign => _chineseSign;

        private bool? _isBirthday;
        private BirthdayDate? birthday;

        public bool? IsBirthday => _isBirthday;

        public SerializablePerson ToSerializable() => new(Id, FirstName, LastName, Address?.Path, Birthday?.Date);
    }

}
