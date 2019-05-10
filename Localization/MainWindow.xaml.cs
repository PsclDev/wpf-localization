using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Localization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var langCode = Properties.Settings.Default.lang;
            cmbbx.ItemsSource = Properties.LocUtil.LanguageList;
            Properties.LocUtil.SwitchLanguage(this, langCode);
            cmbbx.SelectedItem = cmbbx.Items.Cast<ComboBoxItem>().Where(e => e.Tag.ToString() == langCode).FirstOrDefault();
            MainGrid.Children.Add(test);
        }

        private UserControl_Test test = new UserControl_Test();

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newLanguage = ((ComboBoxItem)cmbbx.SelectedItem).Tag.ToString();
            Properties.LocUtil.SwitchLanguage(this, newLanguage);
            Properties.LocUtil.SwitchLanguage(test, newLanguage);
        }
    }
}
