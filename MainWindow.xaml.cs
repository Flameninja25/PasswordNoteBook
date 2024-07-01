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
using System.Data.SqlClient;
using System.Data;
using System;
using System.Reflection.Metadata.Ecma335;
using System.IO;
using System.Data.Common;

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
            //dbManager.CreateTables();
        }
       
        private void SearchBtnClicked(object sender, RoutedEventArgs e)
        {
            //dbManager.WipeDB(); 

            Passtxt.Text = dbManager.SearchRecord(tbxServiceSearch.Text);
        }

        private void SavePasswordBtnClicked(object sender, RoutedEventArgs e)
        {
            //dbManager.Test();

            dbManager.AddRecords(tbxServiceName.Text, tbxPasswordText.Text);
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
        private static readonly String DBMaster = "Server=(localdb)\\MSSQLLocalDB;Integrated security=SSPI;database=master";

        private static readonly String DBLocation = $"{System.AppDomain.CurrentDomain.BaseDirectory}Passwords.mdf";

        private static readonly String DBLogLocation = $"{System.AppDomain.CurrentDomain.BaseDirectory}Passwords_log.ldf";

        private static readonly String DataPath = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={DBLocation};Integrated Security=True";

        //Method for creating the Database
        public void CreateDB()
        {
            if (CheckConnectDB())return;

            using (var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated security=SSPI;database=master"))
            {
                string cmdStr = "CREATE DATABASE PasswordsDB ON PRIMARY " +
                                "(NAME = PasswordsDB_Data, " +
                                $"FILENAME = '{DBLocation}', " +
                                "SIZE = 2MB, MAXSIZE = 20MB, FILEGROWTH = 10%)" +
                                "LOG ON (NAME = Passwords_log, " +
                                $"FILENAME = '{System.AppDomain.CurrentDomain.BaseDirectory}Passwords_log.ldf', " +
                                "SIZE = 1MB, " +
                                "MAXSIZE = 10MB, " +
                                "FILEGROWTH = 10%)";

                using (var myCommand = new SqlCommand(cmdStr, connection))
                {
                    connection.Open();
                    myCommand.ExecuteNonQuery();
                    connection.Close(); 
                }
            }

            System.Threading.Thread.Sleep(3000);
            CreateTables();
        }

        //create the pass word data
        public void CreateTables()
        {
            //if (!CheckConnectDB()) return;
            using (var connection = new SqlConnection(DataPath))
            {

                string cmdStr = "CREATE TABLE PASSWORDS (ID INT PRIMARY KEY IDENTITY(1,1), SERVICENAME VARCHAR(255), PASSWORD VARCHAR(255))";
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //add record using given data
        public bool AddRecords(string serviceName, string password)
        {
            if (!CheckConnectDB()) return false;
            using (var connection = new SqlConnection(DataPath))
            {
                string cmdStr = "INSERT INTO dbo.PASSWORDS (SERVICENAME, PASSWORD) VALUES (@servicename, @password) ";
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    command.Parameters.AddRange(new SqlParameter[] {new SqlParameter("@servicename", serviceName),new SqlParameter("@password", password)});
                    connection.Open();
                    command.ExecuteNonQuery();                }
            }

            return true;
        }

        public string SearchRecord(string serviceName)
        {
            if (!CheckConnectDB()) return "Error";

            string cmdStr = "SELECT [PASSWORD] FROM dbo.PASSWORDS WHERE SERVICENAME = @serviceName";

            using( var connection = new SqlConnection(DataPath))
            {
                using(var command = new SqlCommand(cmdStr, connection))
                {
                    command.Parameters.AddRange(new SqlParameter[] { new SqlParameter("@servicename", serviceName)});
                    connection.Open();

                    object? result = command.ExecuteScalar();

                    string str = result == null ? "Not Found" : result.ToString();

                    return str;
                    //var data = new DataTable();
                    //using (SqlDataReader reader = command.ExecuteReader())
                    //{
                    //    data.Load(reader);
                    //    return data.Rows.Count > 0 ? (data.Rows[0][0].ToString()) : "Not Found";
                    //}

                }
            }

            return "";
        }

        public bool CheckConnectDB()
        {
            using (var connection = new SqlConnection(DataPath))
            {
                try
                {
                    connection.Open();
                    
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool CheckExistDB() => (File.Exists(DBLocation) && File.Exists(DBLogLocation));

        public void RemoveRecord()
        {

        }

        public void Test()
        {
            if (CheckConnectDB())
            {
                using (var connection = new SqlConnection(DataPath))
                {
                    using (var myCommand = new SqlCommand("SELECT 7", connection))
                    {
                        connection.Open();
                        //myCommand.ExecuteNonQuery();
                        int x = (Int32)myCommand.ExecuteScalar();
                        connection.Close();
                    }
                }
            }
        }

        public void WipeDB()
        {
            using (var connection = new SqlConnection(DBMaster))
            {
                using (var command = new SqlCommand("USE Master; ALTER DATABASE PasswordsDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE PasswordsDB", connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    if (File.Exists(DBLocation)) File.Delete(DBLocation);


                    if (File.Exists(DBLogLocation)) File.Delete(DBLogLocation);

                }
            }
        }

    }
}