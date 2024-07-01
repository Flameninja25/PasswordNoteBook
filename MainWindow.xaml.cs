using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordNoteBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBaseManager dbManager = new DataBaseManager();

        public MainWindow()
        {
            InitializeComponent();
            dbManager.CreateDB();
        }
       
        private void SearchBtnClicked(object sender, RoutedEventArgs e)
        {
            Passtxt.Text = dbManager.SearchRecord(tbxServiceSearch.Text);
        }

        private void SavePasswordBtnClicked(object sender, RoutedEventArgs e)
        {
            dbManager.AddRecords(ServiceTxt.Text, PasswordTxt.Text);
        }


        private void SearcherOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenSearch();
        }

        private void RegisterOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenRegister();
        }

        //code to open up the Search Page
        public void OpenSearch()
        {
            Passtxt.Visibility = Visibility.Visible;
            SearchBtn.Visibility = Visibility.Visible;
            tbxServiceSearch.Visibility = Visibility.Visible;   
            WantedServiceTxt.Visibility = Visibility.Visible;


            tbxServiceName.Visibility = Visibility.Hidden;
            ServiceTxt.Visibility = Visibility.Hidden;
            tbxPasswordText.Visibility = Visibility.Hidden;
            PasswordTxt.Visibility = Visibility.Hidden;
            SaveBtn.Visibility = Visibility.Hidden;

        }


        //code to open up the Register page
        public void OpenRegister()
        {
            Passtxt.Visibility = Visibility.Hidden;
            SearchBtn.Visibility = Visibility.Hidden;
            tbxServiceSearch.Visibility = Visibility.Hidden;
            WantedServiceTxt.Visibility = Visibility.Hidden;

            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible; 
            tbxPasswordText.Visibility = Visibility.Visible;
            PasswordTxt.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Visible; 

        }
    }

    public class DataBaseManager
    {
        public bool CreateDB()
        {
            if(CheckDB())
            {   

            }
            return true;
        }

        public bool AddRecords(string serviceName, string password)
        {
            return true;
        }

        public string SearchRecord(string serviceName)
        {
            return "";
        }

        public bool CheckDB()
        {
            return true;
        }

        public void RemoveRecord()
        {

        }
    }
}