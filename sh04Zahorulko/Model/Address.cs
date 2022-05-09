using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sh04Zahorulko
{
    [Serializable]
    public struct Address : IComparable
    {
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                if (regex.IsMatch(value))
                    _path = value;
                else
                    throw new IllegalAddress(value);
            }
        }

        public Address(string path)
        {
            if (regex.IsMatch(path))
                _path = path;
            else
                throw new IllegalAddress(path);
        }

        private readonly static Regex regex = new("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

        public static Task<Address> GetAsync(string path)
        {
            return Task.Run(() =>
            {
                return new Address(path);
            });
        }

        public override string ToString()
        {
            return Path;
        }

        public int CompareTo(object? obj) => Path.CompareTo((obj as Address?)?.Path);
    }

    public class IllegalAddress : Exception
    {
        public string Path { get; private set; }

        public IllegalAddress(string path) : base($"{path} can't be an address") => Path = path;
    }
}
