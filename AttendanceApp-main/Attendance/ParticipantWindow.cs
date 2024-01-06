using Attendance.Class;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Attendance
{
    public partial class ParticipantWindow : Form
    {
        private Koneksi koneksi;
        MySqlConnection conn;

        MySqlCommand cmd = new MySqlCommand();
        private MySqlDataAdapter adapter;
        private DataTable dataTable;

        string loggedInEmail = LoginForm.Session.loggedInEmail;
        string loggedInName = LoginForm.Session.loggedInName;

        public ParticipantWindow()
        {
            InitializeComponent();

            koneksi = new Koneksi();
            conn = koneksi.conn;
        }

        public void updateTable()
        {
            string query = $"SELECT * FROM attendance WHERE nama = '{loggedInName}'";
            cmd = new MySqlCommand(query, conn);

            adapter = new MySqlDataAdapter(cmd);
            dataTable = new DataTable();

            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.HeaderText == "id")
                {
                    column.Visible = false;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        public void updateEventTable()
        {
            string query = $"SELECT * FROM events";
            cmd = new MySqlCommand(query, conn);

            adapter = new MySqlDataAdapter(cmd);
            dataTable = new DataTable();

            adapter.Fill(dataTable);

            dataGridView2.DataSource = dataTable;

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                if (column.HeaderText == "id")
                {
                    column.Visible = false;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void Member_Load(object sender, EventArgs e)
        {
            Email.Text = $"Email : {loggedInEmail}";
            lblNama.Text = $"Nama : {loggedInName}";
            conn.Open();

            updateTable();
            updateEventTable();

            string query = "SELECT * FROM events";
            cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cBoxEvent.Items.Add(reader.GetString(1));
            }
            conn.Close();
        }

        private void btnAbsen_Click(object sender, EventArgs e)
        {
            string status = null;

            if (rbHadir.Checked)
            {
                status = "Hadir";
            }
            else if (rbIzin.Checked)
            {
                status = "Izin";
            }
            else if (rbAbsent.Checked)
            {
                status = "Absent";
            }
            else if (rbTelat.Checked)
            {
                status = "Telat";
            }

            DateTime currentDate = DateTime.Now;
            string hourNOW = currentDate.ToString("HH");

            string event_ = cBoxEvent.Text;

            if (event_ == "")
            {
                MessageBox.Show("select an event first!");
                return;
            }

            if (status == null)
            {
                MessageBox.Show("select status first! (hadir, izin, absent, telat)");
                return;
            }

            conn.Open();
            string getTime = "SELECT time FROM attendance WHERE nama = @nama AND event = @event ORDER BY date DESC, time DESC LIMIT 1";
            MySqlCommand getTimeCmd = new MySqlCommand(getTime, conn);
            getTimeCmd.Parameters.AddWithValue("@nama", loggedInName);
            getTimeCmd.Parameters.AddWithValue("@event", event_);

            using (MySqlDataReader getTimeReader = getTimeCmd.ExecuteReader())
            {
                if (getTimeReader.Read())
                {
                    string timeAbsenStr = getTimeReader["time"].ToString();
                    DateTime timeAbsen = DateTime.ParseExact(timeAbsenStr, "HH:mm:ss", CultureInfo.InvariantCulture);
                    string hourAbsen = timeAbsen.ToString("HH");

                    if (hourAbsen != hourNOW)
                    {
                        string absen = $"INSERT INTO attendance (nama, event, attendance) VALUES ('{loggedInName}', '{event_}', '{status}')";
                        cmd = new MySqlCommand(absen, conn);
                        getTimeReader.Close();
                        cmd.ExecuteNonQuery();
                        updateTable();
                    }
                    else
                    {
                        MessageBox.Show("Oops, you can only record your attendance only once per hour on each event!");
                    }
                }
                else if (!getTimeReader.HasRows)
                {
                    string absen = $"INSERT INTO attendance (nama, event, attendance) VALUES ('{loggedInName}', '{event_}', '{status}')";
                    cmd = new MySqlCommand(absen, conn);
                    getTimeReader.Close();
                    cmd.ExecuteNonQuery();
                    updateTable();
                }
            }
            conn.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm.Session.destroySession();

            LoginForm loginWindowInstance = new LoginForm();
            loginWindowInstance.Show();

            this.Close();
        }
    }
}
