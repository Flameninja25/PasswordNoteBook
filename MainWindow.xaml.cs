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
        //enumerator for handling the pages
        enum Pages
        {
            Lock,
            Main,
            Search,
            Add,
            All,
            Remove,
            Pin
        }

        //create a object of the database manager
        DataBaseManager dbManager = new DataBaseManager();

        //sound manager
        SFXManager sFXManager = new SFXManager();

        //variable to store the current page
        Pages currentPage;

        public MainWindow()
        {
            InitializeComponent();
            //forces wait and runs in order
            Task.WaitAll(new Task[] { Task.Run(async () => await dbManager.CreateDB()) });
            OpenLock();
            //dbManager.CreateTables();
        }

        /// <summary>
        /// Triggers when the program loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //sets the grid to a locked size
            ClearValue(SizeToContentProperty);
            RootGrid.ClearValue(WidthProperty);
            RootGrid.ClearValue(HeightProperty);
        }

        /// <summary>
        /// Search Button Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchBtnClicked(object sender, RoutedEventArgs e)
        {
            //dbManager.WipeDB(); 
            //set the password text to the searched record
            Passtxt.Text = await dbManager.SearchRecord(tbxServiceName.Text);

        }

        //the lock button is clicked
        private async void LockedBtnClicked(object sender, RoutedEventArgs e)
        {
            if(Convert.ToBoolean(await dbManager.CheckPassword()) == false)
            {
                OpenHome();
            }
            else
            {
                if(Convert.ToBoolean(await dbManager.PasswordValid(tbxPasswordCheck.Text)) == true)
                {
                    OpenHome();
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// Button for setting the password manager pin to the given text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetPinBtnClicked(object sender, RoutedEventArgs e)
        {
            //remove the set record
            dbManager.RemoveRecord("pin");
            //set the pin to the relevant record
            await dbManager.AddRecordsAsync("pin", tbxPinText.Text);
            ConfirmPinText.Text = "Pin Set";

        }

        /// <summary>
        /// Save button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SavePasswordBtnClicked(object sender, RoutedEventArgs e)
        {
            //dbManager.Test();
            //add the new record to the database
            await dbManager.AddRecordsAsync(tbxServiceName.Text, tbxPasswordText.Text);


        }
        /// <summary>
        /// Next page button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NextBtnClicked(object sender, RoutedEventArgs e)
        {
            sFXManager.ChangePage();
            //HideGlobals();
            //go to the next page based on current page
            switch (currentPage)
            {
                case Pages.Main:
                    OpenSearch();
                    break;
                case Pages.Search:
                    OpenRegister();
                    break;
                case Pages.Add:
                    OpenRemove();
                    break;
                case Pages.Remove:
                    OpenAll();
                    break;
                case Pages.All:
                    OpenHome();
                    break;
                default:
                    OpenHome();
                    break;
            }

        }


        /// <summary>
        /// Reset Button Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResetBtnClicked(object sender, RoutedEventArgs e)
        {
            //wipe the database and all records
            dbManager.WipeDB();
        }

        //button for when the pin page button is clicked
        public void PinbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenPin();
        }

        /// <summary>
        /// Remove button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveBtnClicked(object sender, RoutedEventArgs e)
        {
            //remove the set record
            dbManager.RemoveRecord(tbxServiceName.Text);
        }

        /// <summary>
        /// Open the main page button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMainPageClicked(object sender, RoutedEventArgs e)
        {
            OpenHome();

        }

        /// <summary>
        /// returns the table to the order by ID rather then a-z
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SortBtnClicked(object sender, RoutedEventArgs e)
        {
            //create empty database to be filled
            DataTable db = new DataTable();

            //wait for the task to complete
            Task.WaitAll(new Task[] { Task.Run(async () => db = await dbManager.GetSortedTableData()) });

            //set the data grid to the data in the new datatable
            MyDataSet.ItemsSource = db.DefaultView;
        }


        public void UnSortBtnClicked(object sender, RoutedEventArgs e)
        {
            //create empty database to be filled
            DataTable db = new DataTable();

            //wait for the task to complete
            Task.WaitAll(new Task[] { Task.Run(async () => db = await dbManager.GetTableData()) });

            //set the data grid to the data in the new datatable
            MyDataSet.ItemsSource = db.DefaultView;
        }


        /// <summary>
        /// Button to return to the home page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HomeBtnClicked(object sender, RoutedEventArgs e)
        {
            OpenHome();
        }


        /// <summary>
        /// Button to open teh search page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearcherOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenSearch();
        }

        //button to open the remove page
        private void RemoverOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenRemove();
        }

        /// <summary>
        /// button to open the all page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenAll();
        }

        /// <summary>
        /// button to open the register page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterOpenbtnClicked(object sender, RoutedEventArgs e)
        {
            OpenRegister();
            //SearcherBtn.Visibility = Visibility.Visible;
            //RegisterBtn.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// code to open up the Search Page
        /// </summary>
        public void OpenSearch()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.Search;
            currentPage = Pages.Search;
        }


        /// <summary>
        /// code to open up the Register page
        /// </summary>
        public void OpenRegister()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.Add;
            currentPage = Pages.Add;

        }

        /// <summary>
        /// Opens the remove page
        /// </summary>
        public void OpenRemove()
        {
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            tbxServiceName.Visibility = Visibility.Visible;
            ServiceTxt.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            tabControl.SelectedIndex = (int)Pages.Remove;
            currentPage = Pages.Remove;
        }

        /// <summary>
        /// open the page for showing all data
        /// </summary>
        public void OpenAll()
        {
            //hide the globals
            HideGlobals();
            HomeBtn.Visibility = Visibility.Visible;
            NextBtn.Visibility = Visibility.Visible;

            //do to all page
            tabControl.SelectedIndex = (int)Pages.All;
            currentPage = Pages.All;

            //create empty database to be filled
            DataTable db = new DataTable();

            //wait for the task to complete
            Task.WaitAll(new Task[] { Task.Run(async () => db = await dbManager.GetTableData()) });

            //set the data grid to the data in the new datatable
            MyDataSet.ItemsSource = db.DefaultView;
        }

        /// <summary>
        /// Open the lock page for the start screen
        /// </summary>
        public void OpenLock()
        {
            tbxPasswordCheck.Visibility = Visibility.Hidden;
            bool tempBool = false;
            Task.WaitAll(new Task[] { Task.Run(async () => tempBool = Convert.ToBoolean(await dbManager.CheckPassword())) });
            if(tempBool)
            {
                tbxPasswordCheck.Visibility = Visibility.Visible;
            }
            HideGlobals();
            tabControl.SelectedIndex = (int)Pages.Lock;
            currentPage = Pages.Lock;
        }

        /// <summary>
        /// Hides the form components that are visible on multiple pages
        /// </summary>
        public void HideGlobals()
        {
            tbxServiceName.Visibility = Visibility.Hidden;
            ServiceTxt.Visibility = Visibility.Hidden;
            NextBtn.Visibility = Visibility.Hidden;
            HomeBtn.Visibility = Visibility.Hidden;
            sFXManager.ChangePage();
        }

        /// <summary>
        /// Opens the home page
        /// </summary>
        public void OpenHome()
        {
            HideGlobals();

            tabControl.SelectedIndex = (int)Pages.Main;
            currentPage = Pages.Main;
        }

        /// <summary>
        /// opesn the page to set the pin
        /// </summary>
        private void OpenPin()
        {
            HideGlobals();

            //set text to blank
            ConfirmPinText.Text = "";

            //set the page to the pin page
            tabControl.SelectedIndex = (int)Pages.Pin;
            currentPage = Pages.Pin;

            //set the home btn to visible
            HomeBtn.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Class for handling all database commands
    /// </summary>
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
            //check if the database exists
            if (await CheckConnectDB())return;
            //Task taskResult = Task.Run(async () =>
            //{
                //use the master connection
                using (var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated security=SSPI;database=master"))
                {
                    //Sql used to create the database and the needed files
                    string cmdStr = "CREATE DATABASE PasswordsDB ON PRIMARY " +
                                    "(NAME = PasswordsDB_Data, " +
                                    $"FILENAME = '{DBLocation}', " +
                                    "SIZE = 2MB, MAXSIZE = 20MB, FILEGROWTH = 10%)" +
                                    "LOG ON (NAME = Passwords_log, " +
                                    $"FILENAME = '{System.AppDomain.CurrentDomain.BaseDirectory}Passwords_log.ldf', " +
                                    "SIZE = 1MB, " +
                                    "MAXSIZE = 10MB, " +
                                    "FILEGROWTH = 10%)";
                    
                    //run teh creation command
                    using (var myCommand = new SqlCommand(cmdStr, connection))
                    {
                        connection.Open();
                        //myCommand.ExecuteNonQuery();
                        await myCommand.ExecuteNonQueryAsync();
                    }

                    //create the starting table for the passwords
                    string cmdStr2 = "USE [PasswordsDB]; CREATE TABLE PASSWORDS (ID INT PRIMARY KEY IDENTITY(1,1), SERVICENAME VARCHAR(255), PASSWORD VARCHAR(255))";
                    using (var command = new SqlCommand(cmdStr2, connection))
                    {                          
                        await command.ExecuteNonQueryAsync();
                            
                    }
                    await connection.CloseAsync();
                }
            //});
            //Task.WaitAll(new Task[] {taskResult});
        }

        /// <summary>
        /// add record using given data
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task AddRecordsAsync(string serviceName, string password)
        {
            //check if the database exists
            if (!await CheckConnectDB()) return;
            using (var connection = new SqlConnection(DataPath))
            {
                //SQL to insert the new record using the defined service name and password
                string cmdStr = "INSERT INTO dbo.PASSWORDS (SERVICENAME, PASSWORD) VALUES (@servicename, @password) ";
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    //use parameters to add the service name and password to avoid sql injection
                    command.Parameters.AddRange(new SqlParameter[] {new SqlParameter("@servicename", serviceName),new SqlParameter("@password", password)});
                    connection.Open();
                    await command.ExecuteNonQueryAsync();               
                }
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
            //Check if the database exists
            if (!await CheckConnectDB()) return "Error";

            //SQL code to get the record where the given service name is located
            string cmdStr = "SELECT [PASSWORD] FROM dbo.PASSWORDS WHERE SERVICENAME = @serviceName";

            using( var connection = new SqlConnection(DataPath))
            {
                using(var command = new SqlCommand(cmdStr, connection))
                {
                    //uses parameters to avoid sql injection
                    command.Parameters.AddRange(new SqlParameter[] { new SqlParameter("@servicename", serviceName)});
                    await connection.OpenAsync();

                    //get the result from the command
                    object? result = await command.ExecuteScalarAsync();

                    //get the string result being the password
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

        /// <summary>
        /// method to check if the database exists
        /// </summary>
        /// <returns></returns>
        public bool CheckExistDB() => (File.Exists(DBLocation) && File.Exists(DBLogLocation));

        /// <summary>
        /// Method to remove a record with the correct service name
        /// </summary>
        /// <param name="serviceName"></param>
        public async void RemoveRecord(string serviceName)
        {
            //check if the database exists
            if(!await CheckConnectDB()) return;
            using (var connection = new SqlConnection(DataPath))
            {
                //get the SQL command to delete the record where the given serivce name is
                string cmdStr = "DELETE FROM dbo.PASSWORDS WHERE SERVICENAME = @serviceName";

                using (var command = new SqlCommand(cmdStr, connection))
                {
                    //use parameters to avoid SQL injection
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
            if (!await CheckConnectDB()) return;
            using (var connection = new SqlConnection(DBMaster))
            {
                //Sql code to drop the database
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
            //sql command to get all info from the database
            string cmdStr = "SELECT * FROM dbo.PASSWORDS";

            using(var connection = new SqlConnection(DataPath))
            {
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    //use the SQL adapter to add all the table data to a new data base
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable("PASSWORDS");
                    adapter.Fill(dt);
                    adapter.Update(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Return the records in the table
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> GetSortedTableData()
        {
            //sql command to get all info from the database
            string cmdStr = "SELECT * FROM dbo.PASSWORDS ORDER BY SERVICENAME";

            using (var connection = new SqlConnection(DataPath))
            {
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    //use the SQL adapter to add all the table data to a new data base
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable("PASSWORDS");
                    adapter.Fill(dt);
                    adapter.Update(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Method to check if password exists
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckPassword()
        {
            if (!await CheckConnectDB()) return false;
            //command string to get count of values that are in the pin record
            string cmdStr = "SELECT " +
                "CASE WHEN x.COUNT > 0 THEN 1 " +
                "ELSE 0 " +
                "END AS [EXISTS] " +
                "FROM " +
                "(SELECT COUNT (*) AS [COUNT] FROM dbo.PASSWORDS p WHERE p.[SERVICENAME] = 'pin') x";

            //run query
            using (var connection = new SqlConnection(DataPath))
            {
                using (var command = new SqlCommand(cmdStr, connection))
                {
                    await connection.OpenAsync();
                    object? result = await command.ExecuteScalarAsync();

                    //return the interger converted to a bool
                    return Convert.ToBoolean(result);
                }
            }
        }

        /// <summary>
        /// Method to check if the password is valid
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> PasswordValid(string password)
        {
            //check database connection
            if (!await CheckConnectDB()) return false;
            //make the sql command string
            string cmdStr = "SELECT [PASSWORD] FROM PASSWORDS WHERE SERVICENAME = 'pin'";

            //run query
            using (var connection = new SqlConnection(DataPath))
            {
                using (var command = new SqlCommand(cmdStr,connection))
                {
                    await connection.OpenAsync();
                    object? result = await command.ExecuteScalarAsync();

                    string str = result == null ? "Not Found" : result.ToString();

                    //check if the gotten record data matches the given string
                    if(str == password)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    }
}