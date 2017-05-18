using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace CRUD
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            var setting = new ConnectionStringSettings
            {
                Name = "Db",
                ConnectionString = $@"User id = {loginEdit.Text}; Password = {passEdit.Text}; 
                                    Server = {serverEdit.Text};
                                    Integrated Security = false;
                                    Database = {dbEdit.Text};
                                    Connection Timeout = 5;
                                    Pooling = true;"
            };

            Configuration config;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Clear();
            config.ConnectionStrings.ConnectionStrings.Add(setting);
            config.Save();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db"].ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception except)
                {
                    MessageBox.Show(except.Message);
                    return;
                }
            }

            var dbForm = new DBForm();
            dbForm.Show();
            this.Hide();
        }
    }
}
