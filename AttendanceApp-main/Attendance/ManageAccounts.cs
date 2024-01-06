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
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Attendance
{
    public partial class ManageAccounts : Form
    {
        private Koneksi koneksi;
        MySqlConnection conn;

        MySqlCommand checkUname = new MySqlCommand();
        MySqlCommand cmd = new MySqlCommand();
        private MySqlDataAdapter adapter;
        private DataTable dataTable;

        string adminName = LoginForm.Session.loggedInName;

        public ManageAccounts()
        {
            InitializeComponent();

            koneksi = new Koneksi();
            conn = koneksi.conn;
        }

        public string encryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashB= sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashB).Replace("-", "").ToLower();
            }
        }

        public void updateTable()
        {
            conn.Open();
            string query = $"SELECT * FROM users WHERE nama != '{adminName}'";
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string email = EmailBox.Text.ToString();
            string nama = NamaBox.Text.ToString();
            string pangkat = PangkatBox.Text.ToString();
            string event_ = comboBox1.Text;
            string RAWpassword = PasswordBox.Text.ToString();
            string password = encryptPassword(RAWpassword);

            if (email != "" && nama != "" && pangkat != "" && password != "")
            {
                conn.Open();
                string check = $"SELECT COUNT(*) FROM users WHERE email = '{email}'";
                checkUname = new MySqlCommand(check, conn);
                int userCount = Convert.ToInt32(checkUname.ExecuteScalar());

                if (userCount > 0)
                {
                    warning.Text = "Email ini sudah terdaftar!";
                }
                else
                {
                    string register = $"INSERT INTO users (email, nama, pangkat, assigned_event, password) VALUES ('{email}', '{nama}', '{pangkat}', '{event_}', '{password}')";
                    cmd = new MySqlCommand(register, conn);
                    cmd.ExecuteNonQuery();
                    warning.Text = "User ditambahkan!";

                    EmailBox.Clear();
                    NamaBox.Clear();
                    PangkatBox.Clear();
                    PasswordBox.Clear();
                }
                conn.Close();

                updateTable();
            }
            else
            {
                warning.Text = "Data wajib diisi!";
            }
        }

        private void updateComboBox()
        {
            comboBox1.Items.Clear();
            eventEdit.Items.Clear();

            conn.Open();

            string query = "SELECT * FROM events";
            cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetString(1));
                eventEdit.Items.Add(reader.GetString(1));
            }
            conn.Close();

        }

        private void Register_Load(object sender, EventArgs e)
        {
            updateTable();

            updateComboBox();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            AdminWindow adminWindowInstance = new AdminWindow();
            adminWindowInstance.Show();

            this.Close();
        }

        private void btnDelAcc_Click(object sender, EventArgs e)
        {
            string id = IDBoxDel.Text.ToString();

            conn.Open();
            string delAcc = $"DELETE FROM users WHERE id = {id}";
            cmd = new MySqlCommand(delAcc, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            IDBoxDel.Clear();

            updateTable();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            string id = idEdit.Text.ToString();

            conn.Open();
            string query = $"SELECT email, nama, pangkat, assigned_event FROM users WHERE id = {id}";
            cmd = new MySqlCommand(query, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string email = reader["email"].ToString();
                    string nama = reader["nama"].ToString();
                    string pangkat = reader["pangkat"].ToString();
                    string assigned_event = reader["assigned_event"].ToString();

                    emailEdit.Text = email;
                    namaEdit.Text = nama;
                    pangkatEdit.Text = pangkat;
                    eventEdit.Text = assigned_event;

                    emailEdit.Enabled = true;
                    namaEdit.Enabled = true;
                    pangkatEdit.Enabled = true;
                    eventEdit.Enabled = true;
                    passEdit.Enabled = true;
                    btnEdit.Enabled = true;
                }
                else
                {
                    MessageBox.Show($"No user found with the id of {id}");
                    idEdit.Clear();
                }
                conn.Close();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string id = idEdit.Text.ToString();
            string email = emailEdit.Text.ToString();
            string nama = namaEdit.Text.ToString();
            string pangkat = pangkatEdit.Text.ToString();
            string assignaed_event = eventEdit.Text.ToString();
            string RAWpassword = passEdit.Text.ToString();
            string password = encryptPassword(RAWpassword);

            conn.Open();
            if (RAWpassword != "" )
            {
                string editEvent = $"UPDATE users SET email = '{email}', nama = '{nama}', pangkat = '{pangkat}', assigned_event = '{assignaed_event}', password = '{password}' WHERE id = {id}";
                cmd = new MySqlCommand(editEvent, conn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string editEvent = $"UPDATE users SET email = '{email}', nama = '{nama}', pangkat = '{pangkat}', assigned_event = '{assignaed_event}' WHERE id = {id}";
                cmd = new MySqlCommand(editEvent, conn);
                cmd.ExecuteNonQuery();
            }
            conn.Close();

            idEdit.Clear();
            emailEdit.Clear();
            namaEdit.Clear();
            passEdit.Clear();
            pangkatEdit.Clear();

            emailEdit.Enabled = false;
            namaEdit.Enabled = false;
            pangkatEdit.Enabled = false;
            eventEdit.Enabled = false;
            passEdit.Enabled = false;
            btnEdit.Enabled = false;

            updateTable();
        }
    }
}
