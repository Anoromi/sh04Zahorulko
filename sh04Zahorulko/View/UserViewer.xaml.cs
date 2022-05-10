using sh04Zahorulko.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sh04Zahorulko.View
{
    /// <summary>
    /// Interaction logic for UserViewer.xaml
    /// </summary>
    public partial class UserViewer : UserControl
    {

        public UserViewer()
        {
            InitializeComponent();
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var vm = (DataContext as UserViewerViewModel)!;
            Func<Person, IComparable?> action = e.Column.Header switch
            {
                "Id" => (Person p) => p.Id,
                "FirstName" => (Person p) => p.FirstName,
                "LastName" => (Person p) => p.LastName,
                "Address" => (Person p) => p.Address,
                "Birthday" => (Person p) => p.Birthday,
                "IsAdult" => (Person p) => p.IsAdult,
                "Age" => (Person p) => p.Age,
                "SunSign" => (Person p) => p.SunSign,
                "ChineseSign" => (Person p) => p.ChineseSign,
                "IsBirthday" => (Person p) => p.IsBirthday,
                _ => throw new ArgumentException(),
            };
            var direction = e.Column.SortDirection switch
            {
                null => ListSortDirection.Ascending,
                ListSortDirection.Ascending => ListSortDirection.Descending,
                ListSortDirection.Descending => ListSortDirection.Ascending,
                _ => throw new NotImplementedException(),
            };
            e.Column.SortDirection = direction;
            var ascending = direction switch
            {
                ListSortDirection.Ascending => true,
                ListSortDirection.Descending => false,
                _ => throw new NotImplementedException()
            };

            e.Handled = true;

            vm.sort.Invoke(new SortOptions(action, ascending));
        }
    }
}
