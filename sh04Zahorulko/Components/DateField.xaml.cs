using System;
using System.Collections.Generic;
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

namespace sh04Zahorulko.Component
{
    /// <summary>
    /// Interaction logic for DateField.xaml
    /// </summary>
    public partial class DateField : UserControl
    {
        public string Label
        {
            get => labelText.Text;
            set => labelText.Text = value;
        }

        public DateTime? SelectedDate
        {
            get => (DateTime)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public double LabelFontSize
        {
            get => labelText.FontSize;
            set => labelText.FontSize = value;
        }

        public double DateFontSize
        {
            get => datePicker.FontSize;
            set => datePicker.FontSize = value;
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(DateField));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(DateField));

        public DateField()
        {
            InitializeComponent();
            LabelFontSize = 20;
            DateFontSize = 20;
        }


    }
}
