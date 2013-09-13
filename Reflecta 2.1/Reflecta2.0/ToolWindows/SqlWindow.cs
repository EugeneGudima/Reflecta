using Npgsql;
using NpgsqlTypes;
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
            if (db.Connect())
            {
                if (!ADatabase.MainDatabaseStructureIsOK(db.Connection))
                {
                    db.CreateDBStructure();
                    db.WriteParameters();
                }
            }
            else
            {
                MessageBox.Show("Не удалось соединиться с базой данных");
                this.Close();
            }
        }

        private void Run_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = Query_textBox.Text;
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
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
