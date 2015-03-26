using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;


namespace SQLToNoSQLConverter
{
    public partial class SQLToNoSQLConverterForm : Form
    {
        SqlConnection conn;
        String connectionString;
        SqlCommand cmd;

        public SQLToNoSQLConverterForm()
        {
            InitializeComponent();
        }

        private void SQLToNoSQLConverterForm_Load(object sender, EventArgs e)
        {

        }

        private void convertItems(String table, String outputfile)
        {
            tbProgressLog.Text += "Started processing table " + table + "\r\n";
            cmd.CommandText = String.Format("SELECT * FROM {0}", table);
            SqlDataReader reader = cmd.ExecuteReader();
            StreamWriter sw = new StreamWriter(outputfile);
            sw.WriteLine("{");
            while (reader.Read())
            {
                String[] data = new String[reader.FieldCount];
                sw.WriteLine("\t{");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    String field = "\t\"" + reader.GetName(i) + "\":\"" + reader[i] + "\"";
                    if (i + 1 < reader.FieldCount) field += ",";
                    sw.WriteLine(field);
                }
                sw.WriteLine("\t}");
                sw.WriteLine();
            }
            sw.WriteLine("}");
            sw.Close();
            reader.Close();
            tbProgressLog.Text += "Table converted and written to " + outputfile + "\r\n";
        }

        private void btnConvertAllTables_Click(object sender, EventArgs e)
        {
            try
            {
                connectionString = tbConnectionString.Text;
                conn = new SqlConnection(connectionString);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                conn.Open();
                DataTable tables = conn.GetSchema("Tables");
                foreach (DataRow row in tables.Rows)
                {
                    String tablename = row[2].ToString();
                    convertItems(tablename, tablename + ".txt");
                }
                conn.Close();
                tbProgressLog.Text += "Database converted\r\n";
            }

            catch
            {
                MessageBox.Show("An error has occurred. Please verify that the connection string is correct.");
            }
        }

        private void btnConvertTable_Click(object sender, EventArgs e)
        {
            try
            {
                connectionString = tbConnectionString.Text;
                conn = new SqlConnection(connectionString);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                conn.Open();
                convertItems(tbTable.Text, tbTable.Text + ".txt");
                conn.Close();
            }
            catch
            {
                MessageBox.Show("An error has occurred. Please verify that the connection string and table name are correct.");
            }
        }
    }
}
