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
    /// Interaction logic for LableField.xaml
    /// </summary>
    public partial class LabelField : UserControl
    {
        public string Label
        {
            get => labelText.Text;
            set => labelText.Text = value;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double LabelFontSize
        {
            get => labelText.FontSize;
            set => labelText.FontSize = value;
        }

        public double InputFontSize
        {
            get => textInput.FontSize;
            set => textInput.FontSize = value;
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(LabelField), new PropertyMetadata(null));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(LabelField));

        public LabelField()
        {
            InitializeComponent();
            LabelFontSize = 20;
            InputFontSize = 20;
        }


    }
}
