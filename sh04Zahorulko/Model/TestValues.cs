using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh04Zahorulko
{
    public class TestValuesProvider
    {

        static readonly (string, string, string)[] values = new[] {
                ("Galahad", "Rodney",  "bjornk@optonline.net"),
                ("Aristaeus", "André",  "animats@sbcglobal.net"),
                ("'Azazyahu", "Bryson",  "ovprit@yahoo.ca"),
                ("Boitumelo", "Van Amersvoort",  "liedra@gmail.com"),
                ("Pierrick", "Mag Raith",  "bdthomas@hotmail.com"),
                ("Antelmo", "Abbasi",  "mosses@outlook.com"),
                ("Kshitij", "Agua",  "sblack@sbcglobal.net"),
                ("Arkadios", "Grayson",  "naoya@sbcglobal.net"),
                ("Abimbola", "Arena",  "mavilar@optonline.net"),
                ("Vera", "Andreev",  "symbolic@att.net"),
                ("Włodek", "Finch",  "facet@yahoo.com"),
                ("Tamara", "Voronin",  "jeffcovey@sbcglobal.net"),
                ("Priscus", "Hermans",  "parasite@aol.com"),
                ("Awilix", "Randal",  "wetter@yahoo.ca"),
                ("Ulrik", "Arriola",  "dkasak@att.net"),
                ("Belenos", "Muller",  "mastinfo@icloud.com"),
                ("Belphoebe", "Meyrick",  "dprice@yahoo.com"),
                ("Tobias", "Holgersen",  "tristan@att.net"),
                ("Emmerson", "MacIomhair",  "papathan@hotmail.com"),
                ("Tullio", "Jansink",  "bowmanbs@aol.com"),
                ("Maria", "Dumitrescu",  "johnbob@gmail.com"),
                ("Richa", "Grünberg",  "thrymm@gmail.com"),
                ("Lisbet", "Rundström",  "retoh@gmail.com"),
                ("Hodia", "Bancroft",  "caidaperl@msn.com"),
                ("Timeus", "Haisam",  "jyoliver@icloud.com"),
                ("Edvin", "Filipek",  "punkis@hotmail.com"),
                ("Bada", "Moralez",  "bigmauler@outlook.com"),
                ("Jean-Luc", "Grenville",  "grolschie@msn.com"),
                ("Samuel", "Böhmer",  "jtorkbob@icloud.com"),
                ("Ceylan", "Mathieson",  "engelen@icloud.com"),
                ("Isaac", "Bukoski",  "leakin@icloud.com"),
                ("Margit", "Van Agthoven",  "ntegrity@yahoo.com"),
                ("Zenobios", "Queen",  "ninenine@mac.com"),
                ("Feodosiy", "Hayden",  "tattooman@me.com"),
                ("Mwenya", "Dam",  "miyop@comcast.net"),
                ("Ares", "Adamson",  "bmcmahon@optonline.net"),
                ("Vijaya", "Hampton", "wenzlaff@aol.com"),
                ("Živa", "Engberg", "akoblin@me.com"),
                ("Filip", "Moriarty", "drjlaw@aol.com"),
                ("Fanny", "Vastagh", "nanop@msn.com"),
                ("Mahsa", "Simms", "jrkorson@icloud.com"),
                ("Klara", "Oberto", "webteam@comcast.net"),
                ("Katleho", "Madigan", "noneme@yahoo.com"),
                ("Èibhlin", "Guldbrandsen", "rafasgj@comcast.net"),
                ("Eloise", "Gabrielson ", "adhere@icloud.com"),
                ("Warcisław", "Madeira", "syncnine@icloud.com"),
                ("Isapo-Muxika", "Eldridge", "graham@aol.com"),
                ("Fabricius", "Monroe", "miturria@mac.com"),
                ("Waldemar", "Tillens", "xnormal@msn.com"),
                ("Buğra", "Ezra", "policies@yahoo.com"),
                ("Vasco", "Wuopio", "mailarc@icloud.com"),
                ("Olga", "Clinton", "wetter@optonline.net"),
                ("Ángela", "Pugh", "lukka@aol.com"),
                ("Heidi", "Stoddard", "ovprit@yahoo.ca"),
                ("Arend", "Laurito", "magusnet@optonline.net"),
                ("Nicolae", "Xie", "citadel@live.com"),
                ("Frantziska", "Dunai", "heine@live.com"),
                ("Pamila", "Arthur", "reziac@verizon.net"),
                ("Udane", "Spear", "preneel@msn.com"),
                ("Filipina", "Street", "cmdrgravy@hotmail.com"),
            };
        const string FILENAME = "test";
        private static async Task WriteValues(Repository r)
        {
            Random random = new();
            var minDate = new DateTime(1980, 1, 1, 0, 0, 0).Ticks;
            var maxDate = new DateTime(2003, 12, 12, 0, 0, 0).Ticks;
            Person create(string firstName, string secondName, string address) =>
            new(r.IdProvider.NextId,
                firstName,
                secondName,
                new(address),
                BirthdayDate.Parse(new DateTime(random.NextInt64(minDate, maxDate))));
            foreach (var v in values)
            {
                await r.PeopleProvider.Add(create(v.Item1, v.Item2, v.Item3));
            }
        }


        public static async Task Fill(Repository r)
        {
            var path = Path.Combine(Repository.BaseFolder, FILENAME);
            if (!File.Exists(path))
            {
                File.Create(path);
                await WriteValues(r);
            }

        }
    }
}
