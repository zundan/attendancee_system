using Attendance.Class;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance
{
    public partial class OperatorWindow : Form
    {
        private Koneksi koneksi;
        MySqlConnection conn;

        MySqlCommand cmd = new MySqlCommand();
        private MySqlDataAdapter adapter;
        private DataTable dataTable;

        string loggedInEmail = LoginForm.Session.loggedInEmail;
        string loggedInName = LoginForm.Session.loggedInName;
        string loggedInEvent = LoginForm.Session.loggedInEvent;

        public OperatorWindow()
        {
            InitializeComponent();

            koneksi = new Koneksi();
            conn = koneksi.conn;
        }

        public void updateTable()
        {
            conn.Open();
            string query = $"SELECT * FROM attendance WHERE event = '{loggedInEvent}'";
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

            string event_ = loggedInEvent;

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

        private void OperatorWindow_Load(object sender, EventArgs e)
        {
            Email.Text = $"Email : {loggedInEmail}";
            Nama.Text = $"Nama : {loggedInName}";
            Event.Text = $"Assigned Event : {loggedInEvent}";

            updateTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm.Session.destroySession();

            LoginForm loginWindowInstance = new LoginForm();
            loginWindowInstance.Show();

            this.Close();
        }
    }
}
