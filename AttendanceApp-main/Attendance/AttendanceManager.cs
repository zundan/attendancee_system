using Attendance.Class;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Attendance
{
    public partial class AttendanceManager : Form
    {
        private Koneksi koneksi;
        MySqlConnection conn;

        MySqlCommand cmd = new MySqlCommand();
        private MySqlDataAdapter adapter;
        private DataTable dataTable;

        public AttendanceManager()
        {
            InitializeComponent();

            koneksi = new Koneksi();
            conn = koneksi.conn;
        }

        public void updateTable()
        {
            conn.Open();
            string query = $"SELECT * FROM attendance";
            cmd = new MySqlCommand(query, conn);

            adapter = new MySqlDataAdapter(cmd);
            dataTable = new DataTable();

            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            conn.Close();
        }

        public void updateTableWithEvent(string event_)
        {
            conn.Open();
            string query = $"SELECT * FROM attendance WHERE event = '{event_}'";
            cmd = new MySqlCommand(query, conn);

            adapter = new MySqlDataAdapter(cmd);
            dataTable = new DataTable();

            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            conn.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string status = null;

            if (rbHadirAdd.Checked)
            {
                status = "Hadir";
            }
            else if (rbIzinAdd.Checked)
            {
                status = "Izin";
            }
            else if (rbAbsentAdd.Checked)
            {
                status = "Absent";
            }
            else if (rbTelatAdd.Checked)
            {
                status = "Telat";
            }

            string nama = addAttBox.Text.ToString();

            string event_ = cbAddAtt.Text;

            conn.Open();
            string absen = $"INSERT INTO attendance (nama, event, attendance) VALUES ('{nama}', '{event_}', '{status}')";
            cmd = new MySqlCommand(absen, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            addAttBox.Clear();

            updateTable();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string status = null;

            if (rbHadirEdit.Checked)
            {
                status = "Hadir";
            }
            else if (rbIzinEdit.Checked)
            {
                status = "Izin";
            }
            else if (rbAbsentEdit.Checked)
            {
                status = "Absent";
            }
            else if (rbTelatEdit.Checked)
            {
                status = "Telat";
            }

            string id = IDBoxEdit.Text.ToString();

            conn.Open();
            string editAbsen = $"UPDATE attendance SET attendance = '{status}' WHERE id = {id}";
            cmd = new MySqlCommand(editAbsen, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            IDBoxEdit.Clear();

            updateTable();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string id = IDBoxDel.Text.ToString();

            if (id != "")
            {
                conn.Open();
                string delAbsen = $"DELETE FROM attendance WHERE id = {id}";
                cmd = new MySqlCommand(delAbsen, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                IDBoxDel.Clear();

                updateTable();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataGridView dataGridView = dataGridView1;
            Type excelType = Type.GetTypeFromProgID("Excel.Application");

            if (excelType != null)
            {
                dynamic excel = Activator.CreateInstance(excelType);
                excel.Visible = true;
                dynamic workbook = excel.Workbooks.Add();
                dynamic sheet = workbook.ActiveSheet;

                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    sheet.Cells[1, i + 1] = dataGridView.Columns[i].HeaderText;
                }

                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        dynamic cellValue = dataGridView.Rows[i].Cells[j].Value;
                        sheet.Cells[i + 2, j + 1].Value = cellValue?.ToString();
                    }
                }
                for (int i = 1; i <= dataGridView.Columns.Count; i++)
                {
                    sheet.Columns[i].AutoFit();
                }
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            string event_ = cBoxDisplay.Text;
            updateTableWithEvent(event_);
        }

        private void AttendanceManager_Load(object sender, EventArgs e)
        {
            updateTable();

            conn.Open();
            string query = "SELECT * FROM events";
            cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cbAddAtt.Items.Add(reader.GetString(1));
            }
            conn.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            AdminWindow adminWindowInstance = new AdminWindow();
            adminWindowInstance.Show();

            this.Close();
        }
    }
}
