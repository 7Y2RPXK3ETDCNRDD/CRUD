using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace CRUD
{
    public partial class DBForm : Form
    {
        public DBForm()
        {
            InitializeComponent();
        }

        private List<string> tableList = new List<string>();
        private DataSet tables = new DataSet();

        private void DBForm_Shown(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    var selectTablesQuery = new SqlCommand($@"SELECT TABLE_NAME
                                                            FROM INFORMATION_SCHEMA.TABLES
                                                            WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '{connection.Database}'", connection);
                    SqlDataReader tablesReader = selectTablesQuery.ExecuteReader();
                    while (tablesReader.Read())
                    {
                        tableList.Add(tablesReader.GetString(0));
                    }
                    tablesReader.Close();

                    foreach (string tableName in tableList)
                    {
                        var selectAllQuery = new SqlCommand($"SELECT * FROM {tableName}", connection);
                        SqlDataReader allReader = selectAllQuery.ExecuteReader();
                        var table = new DataTable(tableName);
                        table.Load(allReader);
                        tables.Tables.Add(table);
                    }
                }
                catch (Exception except)
                {
                    MessageBox.Show(except.Message);
                    return;
                }   
            }

            //Fill combobox with tables
            foreach (string elem in tableList)
            {
                tableBox.Items.Add(elem);
            }
            tableBox.SelectedIndex = 0;
        }

        private void tableBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView.DataSource = tables.Tables[tableBox.SelectedItem.ToString()];
        }

        private void commitBtn_Click(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db"].ConnectionString))
            {
                try
                {
                    var adapter = new SqlDataAdapter();
                    var cmdb = new SqlCommandBuilder(adapter);
                    foreach (string tableName in tableList)
                    {
                        adapter.SelectCommand = new SqlCommand($"Select * From {tableName}", connection);
                        cmdb.GetDeleteCommand();
                        cmdb.GetInsertCommand();
                        cmdb.GetUpdateCommand();
                        adapter.Update(tables, tableName);
                    }
                }
                catch (Exception except)
                {
                    MessageBox.Show(except.Message);
                    return;
                }
            }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            tables.Tables[tableBox.SelectedItem.ToString()].Rows[(dataGridView.CurrentCell.RowIndex)].Delete();
        }

        private void DBForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
