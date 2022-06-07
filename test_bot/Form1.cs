using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace test_bot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<User_Co> users = new List<User_Co>();
        List<UserSessionData> sessions = new List<UserSessionData>();
        List<IInstaApi> instas = new List<IInstaApi>();

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbUsers.SelectedText = "User 1";

            for (int i = 1; i <= 100; i++)
            {
                cmbUsers.Items.Add($"User {i.ToString()}");
            }
        }

        int count = 1;

        private void button1_Click(object sender, EventArgs e)
        {
            User_Co user = new User_Co()
            {
                Name = cmbUsers.SelectedItem.ToString(),
                Username = txtUsername.Text,
                Password = txtPassword.Text
            };
            users.Add(user);
            dgvUsers.Rows.Add(count, txtUsername.Text);
            count++;
            cmbUsers.SelectedIndex++;
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            cmbUsers.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            txtMeassage.Enabled = false;
            txtDirect.Enabled = false;
            btnAdd.Enabled = false;
            btnLoginAll.Enabled = false;
            btnSend.Enabled = false;

            for (int i = 0; i < users.Count; i++)
            {
                UserSessionData data = new UserSessionData();
                try
                {
                    data.UserName = users[i].Username;
                    data.Password = users[i].Password;

                }
                catch
                {
                    continue;
                }

                sessions.Add(data);
            }

            int countSucceeded = 0;

            foreach (var item in sessions)
            {
                var result = InstaApiBuilder.CreateBuilder()
               .SetUser(item)
               .UseLogger(new DebugLogger(LogLevel.All))
               .SetRequestDelay(RequestDelay
               .FromSeconds(0, 1)).Build();
                var loginRequest = await result.LoginAsync();
                if (loginRequest.Succeeded)
                {
                    instas.Add(result);
                    dgvUsers.Rows[countSucceeded].Cells["Succeeded"].Value = "True";
                }
                else
                {
                    dgvUsers.Rows[countSucceeded].Cells["Succeeded"].Value = "False";
                }
                countSucceeded++;
                await Patience();
            }

            MessageBox.Show("Logins Succeeded ...");

            cmbUsers.Enabled = true;
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            txtMeassage.Enabled = true;
            txtDirect.Enabled = true;
            btnAdd.Enabled = true;
            btnLoginAll.Enabled = true;
            btnSend.Enabled = true;

        }

        private async void button2_Click(object sender, EventArgs e)
        {

            cmbUsers.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            txtMeassage.Enabled = false;
            txtDirect.Enabled = false;
            btnAdd.Enabled = false;
            btnLoginAll.Enabled = false;
            btnSend.Enabled = false;

            try
            {
                var user = await instas[0].UserProcessor.GetUserAsync(txtDirect.Text);
                foreach (var item in instas)
                {
                    var direct = await item.MessagingProcessor.SendDirectTextAsync(user.Value.Pk.ToString(), null, txtMeassage.Text);
                    await Patience();
                }
                MessageBox.Show("Send All Directs ...");
            }
            catch
            {

                MessageBox.Show("با همه دایرکت نشد");
            }

            cmbUsers.Enabled = true;
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            txtMeassage.Enabled = true;
            txtDirect.Enabled = true;
            btnAdd.Enabled = true;
            btnLoginAll.Enabled = true;
            btnSend.Enabled = true;

        }

        private Task Patience()
        {
            int await = 2000;
            return Task.Run(() =>
            {
                System.Threading.Thread.Sleep(await);
            });
        }
    }

    public class User_Co
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
