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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Attendance
{
    public partial class AdminWindow : Form
    {
        string loggedInEmail = LoginForm.Session.loggedInEmail;
        string loggedInName = LoginForm.Session.loggedInName;

        public AdminWindow()
        {
            InitializeComponent();
        }

        private void AdminWindow_Load(object sender, EventArgs e)
        {
            Email.Text = $"Email : {loggedInEmail}";
            lblNama.Text = $"Nama : {loggedInName}";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm.Session.destroySession();

            LoginForm loginWindowInstance = new LoginForm();
            loginWindowInstance.Show();

            this.Close();
        }

        private void btnManageAcc_Click(object sender, EventArgs e)
        {
            this.Hide();
            var mngAcc = new ManageAccounts();
            mngAcc.Show();
        }

        private void btnManageAttendance_Click(object sender, EventArgs e)
        {
            this.Hide();
            var mngAtt = new AttendanceManager();
            mngAtt.Show();  
        }

        private void btnEvents_Click(object sender, EventArgs e)
        {
            this.Hide();
            var mngEvents = new EventManager();
            mngEvents.Show();
        }
    }
}
