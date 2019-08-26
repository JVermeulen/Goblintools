using Goblintools.Types;
using Microsoft.Win32;
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

namespace Goblintools.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SavedVariableProcessor SavedVariable { get; set; }
        private GTGoblinDBManager Manager { get; set; }
        private ImageCreator Creator { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Manager = new GTGoblinDBManager();
            Creator = new ImageCreator();

            SavedVariable = new SavedVariableProcessor();
            SavedVariable.OnGoblinDBChanged.OnReceive.Subscribe(Work);
            SetSavedVariable();
            SetAutoStart(true);
        }

        private void Work(GTGoblinDB goblinDB)
        {
            AddGoblinDB(goblinDB);
        }

        private void AddGoblinDB(GTGoblinDB goblinDB)
        {
            Manager.Add(goblinDB);

            //UpdateComboboxes();
        }

        private void SetSavedVariable()
        {
            if (MnuWatchCharacters.IsChecked)
                SavedVariable.Start();
            else
                SavedVariable.Stop();
        }

        private void SetAutoStart(bool initialize = false)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (initialize)
                {
                    MnuAutoStart.IsChecked = (key.GetValue(Title) != null);
                }
                else
                {
                    if (MnuAutoStart.IsChecked)
                        key.SetValue(Title, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    else if (key.GetValue(Title) != null)
                        key.DeleteValue(Title);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MnuWatchCharacters_Click(object sender, RoutedEventArgs e)
        {
            MnuWatchCharacters.IsChecked = !MnuWatchCharacters.IsChecked;

            SetSavedVariable();
        }

        private void MnuAutoStart_Click(object sender, RoutedEventArgs e)
        {
            MnuAutoStart.IsChecked = !MnuAutoStart.IsChecked;

            SetAutoStart();
        }
    }
}
