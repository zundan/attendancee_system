using Attendance.Class;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Attendance
{
    public partial class EventManager : Form
    {
        private Koneksi koneksi;
        MySqlConnection conn;

        MySqlCommand cmd = new MySqlCommand();
        private MySqlDataAdapter adapter;

        private DataTable dataTable;
        public EventManager()
        {
            InitializeComponent();

            koneksi = new Koneksi();
            conn = koneksi.conn;
        }

        public void updateEventTable()
        {
            conn.Open();
            string query = $"SELECT * FROM events";
            cmd = new MySqlCommand(query, conn);

            adapter = new MySqlDataAdapter(cmd);
            dataTable = new DataTable();

            adapter.Fill(dataTable);

            dataGridView2.DataSource = dataTable;

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            conn.Close();
        }

        private void btnEventAdd_Click(object sender, EventArgs e)
        {
            string event_ = eventAddBox.Text.ToString();
            string location = eventLocBox.Text.ToString();
            string date = dateTimePicker1.Value.ToString("yy-MM-dd");

            conn.Open();
            string addEvent = $"INSERT INTO events (event, location, date) VALUES ('{event_}', '{location}', '{date}')";
            cmd = new MySqlCommand(addEvent, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            eventAddBox.Clear();
            eventLocBox.Clear();

            updateEventTable();
        }

        private void btnDelEvent_Click(object sender, EventArgs e)
        {
            string id = eventIDBox.Text.ToString();

            if (id != "")
            {
                conn.Open();
                string delEvent = $"DELETE FROM events WHERE id = {id}";
                cmd = new MySqlCommand(delEvent, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                eventIDBox.Clear();

                updateEventTable();
            }
        }

        private void EventManager_Load(object sender, EventArgs e)
        {
            updateEventTable();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            AdminWindow adminWindowInstance = new AdminWindow();
            adminWindowInstance.Show();

            this.Close();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            string id = idEdit.Text.ToString();

            conn.Open();
            string query = $"SELECT location, date FROM events WHERE id = {id}";
            cmd = new MySqlCommand(query, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string location = reader["location"].ToString();
                    DateTime date = Convert.ToDateTime(reader["date"]);

                    locEdit.Text = location;
                    dateEdit.Value = date;

                    locEdit.Enabled = true;
                    dateEdit.Enabled = true;
                    btnEdit.Enabled = true;
                }
                else
                {
                    MessageBox.Show($"No event found with the id of {id}");
                    idEdit.Clear();
                }
                conn.Close();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string id = idEdit.Text.ToString();
            string location = locEdit.Text.ToString();
            string date = dateEdit.Value.ToString("yy-MM-dd");

            conn.Open();
            string editEvent = $"UPDATE events SET location = '{location}', date = '{date}' WHERE id = {id}";
            cmd = new MySqlCommand(editEvent, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            idEdit.Clear();
            locEdit.Clear();

            locEdit.Enabled = false;
            dateEdit.Enabled = false;
            btnEdit.Enabled = false;

            updateEventTable();
        }
    }
}
