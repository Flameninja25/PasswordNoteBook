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
using System.ComponentModel.DataAnnotations;

namespace PasswordNoteBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Pages
        {
            Lock,
            Main,
            Search,
            Add,
            All,
            Remove
        }
        DataBaseManager dbManager = new DataBaseManager();

        public MainWindow()
        {
            InitializeComponent();
            //forces wait and runs in order
            Task.WaitAll(new Task[] { Task.Run(async () => await dbManager.CreateDB()) });
            OpenLock();
            //dbManager.CreateTables();
        }

        private async void SearchBtnClicked(object sender, RoutedEventArgs e)
        {
            //dbManager.WipeDB(); 
            Passtxt.Text = await dbManager.SearchRecord(tbxServiceName.Text);

        }

        private async void SavePasswordBtnClicked(object sender, RoutedEventArgs e)
        {
            //dbManager.Test();
            await dbManager.AddRecordsAsync(tbxServiceName.Text, tbxPasswordText.Text);


        }
        public void NextBtnClicked(object sender, RoutedEventArgs e)
        {
            HideGlobals();
            tabControl.SelectedIndex += 1;
        }

        public void ResetBtnClicked(object sender, RoutedEventArgs e)
        {
            dbManager.WipeDB();
        }

        public void RemoveBtnClicked(object sender, RoutedEventArgs e)
        {
            dbManager.RemoveRecord(tbxServiceName.Text);
        }

        private void OpenMainPageClicked(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = (int)Pages.Main;

        }

        private void SearcherOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenSearch();
        }


        private void RemoverOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenRemove();
        }

        private void AllbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenAll();
        }

        private void RegisterOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenRegister();
            //SearcherBtn.Visibility = Visibility.Visible;
            //RegisterBtn.Visibility = Visibility.Visible;
        }

        //code to open up the Search Page
        public void OpenSearch()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.Search;
        }


        //code to open up the Register page
        public void OpenRegister()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.Add;

        }

        public void OpenRemove()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.Remove;
        }

        public void OpenAll()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.All;

            DataTable db = new DataTable();

            Task.WaitAll(new Task[] { Task.Run(async () => db = await dbManager.GetTableData()) });

            MyDataSet.ItemsSource = db.DefaultView;
        }

        public void OpenLock()
        {
            HideGlobals();
            tabControl.SelectedIndex = (int)Pages.Lock;
        }

        public void HideGlobals()
        {
            tbxServiceName.Visibility = Visibility.Hidden;
            ServiceTxt.Visibility = Visibility.Hidden;
            NextBtn.Visibility = Visibility.Hidden;
            HomeBtn.Visibility = Visibility.Hidden;
        }

        public void HomeBtnClicked(object sender, RoutedEventArgs e)
        {
            HideGlobals();

            tabControl.SelectedIndex = (int)Pages.Main;
        }
    }


    public class DataBaseManager
    {
        private static readonly String DBMaster = "Server=(localdb)\\MSSQLLocalDB;Integrated security=SSPI;database=master";

        private static readonly String DBLocation = $"{System.AppDomain.CurrentDomain.BaseDirectory}Passwords.mdf";

        private static readonly String DBLogLocation = $"{System.AppDomain.CurrentDomain.BaseDirectory}Passwords_log.ldf";

        private static readonly String DataPath = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={DBLocation};Integrated Security=True";

        /// <summary>
        /// Method for creating the Database
        /// </summary>
        /// <returns></returns>
        public async Task CreateDB()
        {
            if (await CheckConnectDB())return;
            //Task taskResult = Task.Run(async () =>
            //{
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
                        //myCommand.ExecuteNonQuery();
                        await myCommand.ExecuteNonQueryAsync();
                    }

                    string cmdStr2 = "USE [PasswordsDB]; CREATE TABLE PASSWORDS (ID INT PRIMARY KEY IDENTITY(1,1), SERVICENAME VARCHAR(255), PASSWORD VARCHAR(255))";
                    using (var command = new SqlCommand(cmdStr2, connection))
                    {                          
                        await command.ExecuteNonQueryAsync();
                            
                    }
                    await connection.CloseAsync();
                }
            //});

            //Task.WaitAll(new Task[] {taskResult});



            //System.Threading.Thread.Sleep(5000);
            //CreateTables();
        }

        /// <summary>
        /// create the pass word data (not in use)
        /// </summary>
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

        /// <summary>
        /// add record using given data
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task AddRecordsAsync(string serviceName, string password)
        {
            if (!await CheckConnectDB()) return;
            using (var connection = new SqlConnection(DataPath))
            {
                string cmdStr = "INSERT INTO dbo.PASSWORDS (SERVICENAME, PASSWORD) VALUES (@servicename, @password) ";
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    command.Parameters.AddRange(new SqlParameter[] {new SqlParameter("@servicename", serviceName),new SqlParameter("@password", password)});
                    connection.Open();
                    await command.ExecuteNonQueryAsync();                }
            }

            //return true;
        }

        /// <summary>
        /// Searches a record based on a given Service Name
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<string> SearchRecord(string serviceName)
        {
            if (!await CheckConnectDB()) return "Error";

            string cmdStr = "SELECT [PASSWORD] FROM dbo.PASSWORDS WHERE SERVICENAME = @serviceName";

            using( var connection = new SqlConnection(DataPath))
            {
                using(var command = new SqlCommand(cmdStr, connection))
                {
                    command.Parameters.AddRange(new SqlParameter[] { new SqlParameter("@servicename", serviceName)});
                    await connection.OpenAsync();

                    object? result = await command.ExecuteScalarAsync();

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

        /// <summary>
        /// Method to check if the database exists
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckConnectDB()
        {
            using (var connection = new SqlConnection(DataPath))
            {
                try
                {
                    await connection.OpenAsync();
                    
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool CheckExistDB() => (File.Exists(DBLocation) && File.Exists(DBLogLocation));

        /// <summary>
        /// Method to remove a record with the correct service name
        /// </summary>
        /// <param name="serviceName"></param>
        public async void RemoveRecord(string serviceName)
        {
            if(!await CheckConnectDB()) return;
            using (var connection = new SqlConnection(DataPath))
            {
                string cmdStr = "DELETE FROM dbo.PASSWORDS WHERE SERVICENAME = @serviceName";

                using (var command = new SqlCommand(cmdStr, connection))
                {
                    command.Parameters.AddRange(new SqlParameter[] { new SqlParameter("servicename", serviceName) });
                    await connection.OpenAsync();
                    await command.ExecuteScalarAsync();
                }
            }
        }

        public async void Test()
        {
            if (await CheckConnectDB())
            {
                using (var connection = new SqlConnection(DataPath))
                {
                    using (var myCommand = new SqlCommand("SELECT 7", connection))
                    {
                        await connection.OpenAsync();
                        //myCommand.ExecuteNonQuery();
                        int x = (Int32)myCommand.ExecuteScalar();
                        await connection.CloseAsync();
                    }
                }
            }
        }

        /// <summary>
        /// method to wipe the database
        /// </summary>
        public async void WipeDB()
        {
            using (var connection = new SqlConnection(DBMaster))
            {
                using (var command = new SqlCommand("USE Master; ALTER DATABASE PasswordsDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE PasswordsDB", connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    if (File.Exists(DBLocation)) File.Delete(DBLocation);


                    if (File.Exists(DBLogLocation)) File.Delete(DBLogLocation);

                }
            }
        }

        /// <summary>
        /// retrives the full database
        /// </summary>
        public async Task<DataTable> GetTableData()
        {
            string cmdStr = "SELECT * FROM dbo.PASSWORDS";

            using(var connection = new SqlConnection(DataPath))
            {
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable("PASSWORDS");
                    adapter.Fill(dt);
                    adapter.Update(dt);
                    return dt;
                }
            }
        }

    }
}