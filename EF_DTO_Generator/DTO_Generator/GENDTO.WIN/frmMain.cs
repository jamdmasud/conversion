using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace GENDTO.WIN
{
    public partial class frmMain : Form
    {
        private string SYSTEM_TABLE_NAME = "sysdiagrams";

        public frmMain()
        {
            InitializeComponent();
            txtConnectionString.Text = @"Data Source=(local)\nybsys;Initial Catalog=BGFIDBBD;User ID=sa;Password=141";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtOutputLocation.Text = dlg.SelectedPath;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                txtConnectionString.Enabled = false;
                //string fileName = txtOutputLocation.Text+@"\MyName.txt";
                //System.IO.File.AppendAllText(fileName, "dfhskjdhfksjhfdksj");
                CreateDTO();
            }
        }

        private void btnFields_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                txtConnectionString.Enabled = false;
                CreateFields();
            }
        }

        private bool ValidateInput()
        {
            if (String.IsNullOrEmpty(txtConnectionString.Text.Trim()))
            {
                ShowWarning("Please enter connection string.");
                txtConnectionString.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtOutputLocation.Text.Trim()))
            {
                ShowWarning("Please select output path.");
                btnBrowse.Focus();
                return false;
            }

            if (!IsConnectionStringOK(txtConnectionString.Text.Trim()))
            {
                return false;
            }

            return true;
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, ":: GEN_DTO ::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, ":: GEN_DTO ::", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CreateDTO()
        {

            List<string> tableNames = ReadAllTable();

            if (tableNames.Count > 0)
            {
                foreach (string tableName in tableNames)
                {
                    string dto = GetDTOContent(tableName);
                    string filePath = string.Format(@"{0}\{1}.cs", txtOutputLocation.Text, tableName);
                    System.IO.File.WriteAllText(filePath, dto);
                }

                MessageBox.Show("Complete !!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CreateFields()
        {
            List<string> tableNames = ReadAllTable();
            List<string> lstColumnName = new List<string>();

            StringBuilder sb = new StringBuilder();
            string namespaceString = "namespace  CIB.Common.DTO";

            if (tableNames.Count > 0)
            {
                foreach (string tableName in tableNames)
                {
                    if (tableName != this.SYSTEM_TABLE_NAME)
                    {
                        GetColumnNames(tableName, lstColumnName);
                    }
                }

                sb.AppendLine("using System;");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(namespaceString);
                sb.AppendLine("{");
                sb.AppendLine("public class Fields");
                sb.AppendLine("{");

                foreach (string columnName in lstColumnName)
                {
                    sb.AppendLine(string.Format("public string {0} = \"{0}\";", columnName));
                }

                sb.AppendLine("}");
                sb.AppendLine("}");

                string filePath = string.Format(@"{0}\Fields.cs", txtOutputLocation.Text);
                System.IO.File.WriteAllText(filePath, sb.ToString());

                MessageBox.Show("Complete !!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private List<string> ReadAllTable()
        {
            List<string> tableNames = new List<string>();
            DataSet dsTables = ExecuteQuery("SELECT name FROM sys.tables");

            if (dsTables != null && dsTables.Tables.Count > 0)
            {
                foreach (DataRow dr in dsTables.Tables[0].Rows)
                {
                    tableNames.Add(dr["name"].ToString());
                }
            }

            return tableNames;
        }

        private bool IsConnectionStringOK(string connectionString)
        {
            bool result = false;

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                connection.Close();
                result = true;
            }
            catch (Exception ex)
            {
                ShowError("Unable to open connection to database due to following reason:" + Environment.NewLine + ex.ToString());
            }

            return result;
        }

        private DataSet ExecuteQuery(string query)
        {
            DataSet ds = null;
            SqlConnection connection = null;
            string connectionString = txtConnectionString.Text.Trim();

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                ds = new DataSet();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                ShowError("Unable to open read from database due to following reason:" + Environment.NewLine + ex.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return ds;
        }

        private string GetDTOContent(string tableName)
        {
            string namespaceString = "namespace CIB.Common.DTO";
            StringBuilder sb = new StringBuilder();
            string query = String.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'", tableName);

            DataSet ds = ExecuteQuery(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                sb.AppendLine("using System;");
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                sb.AppendLine("");
                sb.AppendLine(namespaceString);
                sb.AppendLine("{");
                sb.AppendLine("public class " + tableName);
                sb.AppendLine("{");
                int indexer = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string columnName = dr["COLUMN_NAME"].ToString();
                    string dataType = dr["DATA_TYPE"].ToString();
                    string variableName = "_" + columnName.Substring(0, 1).ToLower() + columnName.Substring(1);
                    if (indexer == 0)
                    {
                        if (dataType.Contains("tinyint"))
                        {
                            sb.AppendLine(string.Format(@"private Int16 {0} = 0;
                                        [Key]
                                        public Int16 {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));


                        }
                        else if (dataType.Contains("bigint"))
                        {
                            sb.AppendLine(string.Format(@"private long {0} = 0;
 [Key]
                                        public long {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("int"))
                        {
                            sb.AppendLine(string.Format(@"private int {0} = 0;
                                          [Key]
                                        public int {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("decimal"))
                        {
                            sb.AppendLine(string.Format(@"private decimal {0} = 0;
                                         [Key]
                                        public decimal {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("date"))
                        {
                            sb.AppendLine(string.Format(@"private DateTime {0} = DateTime.Now;
                         
                                        public DateTime {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("image"))
                        {
                            sb.AppendLine(string.Format(@"private byte[] {0} = null;
 
                                        public byte[] {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("bit"))
                        {
                            sb.AppendLine(string.Format(@"private bool {0} = null;
 
                                        public bool {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else
                        {
                            sb.AppendLine(string.Format(@"private string {0} = """";
 [Key]
                                        public string {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        indexer++;
                    }
                    else
                    {
                        if (dataType.Contains("tinyint"))
                        {
                            sb.AppendLine(string.Format(@"private Int16 {0} = 0;

                                        public Int16 {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));


                        }
                        else if (dataType.Contains("bit"))
                        {
                            sb.AppendLine(string.Format(@"private bool {0} = null;
 
                                        public bool {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("bigint"))
                        {
                            sb.AppendLine(string.Format(@"private long {0} = 0;

                                        public long {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("int"))
                        {
                            sb.AppendLine(string.Format(@"private int {0} = 0;
                                         
                                        public int {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("decimal"))
                        {
                            sb.AppendLine(string.Format(@"private decimal {0} = 0;
                                        
                                        public decimal {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("date"))
                        {
                            sb.AppendLine(string.Format(@"private DateTime {0} = DateTime.Now;

                                        public DateTime {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else if (dataType.Contains("image"))
                        {
                            sb.AppendLine(string.Format(@"private byte[] {0} = null;

                                        public byte[] {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                        else
                        {
                            sb.AppendLine(string.Format(@"private string {0} = """";

                                        public string {1}
                                        {{
                                            get {{ return {0}; }}
                                            set {{ {0} = value; }}
                                        }}", variableName, columnName, dataType));
                        }
                    }

                    sb.AppendLine("");
                    sb.AppendLine("");
                }

                sb.AppendLine("}");
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private void GetColumnNames(string tableName, List<string> lstColumnName)
        {
            StringBuilder sb = new StringBuilder();
            string query = String.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'", tableName);

            DataSet ds = ExecuteQuery(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    string columnName = dr["COLUMN_NAME"].ToString();
                    if (!lstColumnName.Contains(columnName))
                    {
                        lstColumnName.Add(columnName);
                    }

                }
            }
        }
    }
}
