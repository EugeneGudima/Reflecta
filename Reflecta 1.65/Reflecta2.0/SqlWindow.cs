using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reflecta2._0
{
    public partial class SqlWindow : Form
    {
        ADatabase db;

        public SqlWindow()
        {
            InitializeComponent();
            db = new ADatabase("database");
            if (!File.Exists("data\\database.fdb"))
            {
                db.CreateDB();
                db.Connect();
                db.CreateDBStructure();
                db.WriteParameters();
            }
            else
            {
                db.Connect();
            }
        }

        private void Run_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = Query_textBox.Text;
                FbCommand command = new FbCommand(sql, db.Connection);
                command.ExecuteNonQuery();
                Message_textBox.Text = "ОК!";
            }
            catch (Exception ex)
            {
                Message_textBox.Text = ex.Message;
            }
        }
    }
}
