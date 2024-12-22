using System;
using System.Windows.Forms;
using password_manager_project;

namespace password_manager_project
{
    public partial class AddPasswordForm : Form
    {
        private readonly string username;
        private TextBox serviceNameBox;
        private TextBox passwordBox;

        public AddPasswordForm(string username)
        {
            this.username = username;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Add New Password";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Service Name input
            var serviceLabel = new Label
            {
                Text = "Service Name:",
                Location = new Point(20, 20),
                Size = new Size(100, 20)
            };

            serviceNameBox = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(340, 25)
            };

            // Password input
            var passwordLabel = new Label
            {
                Text = "Password:",
                Location = new Point(20, 80),
                Size = new Size(100, 20)
            };

            passwordBox = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(340, 25),
                UseSystemPasswordChar = true
            };

            // Add bottom panel with buttons
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            var addButton = new Button
            {
                Text = "Add",
                Size = new Size(100, 35),
                Location = new Point(this.ClientSize.Width - 230, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                Font = new Font("SF Pro Display", 10)
            };
            addButton.Click += SaveButton_Click;

            var exitButton = new Button
            {
                Text = "Exit",
                Size = new Size(100, 35),
                Location = new Point(this.ClientSize.Width - 120, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 122, 255),
                Font = new Font("SF Pro Display", 10)
            };
            exitButton.Click += (s, e) => this.Close();

            bottomPanel.Controls.AddRange(new Control[] { addButton, exitButton });
            this.Controls.Add(bottomPanel);
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serviceNameBox.Text) || 
                string.IsNullOrWhiteSpace(passwordBox.Text))
            {
                MessageBox.Show("Please fill in all fields", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var repository = new GrainRepository();
                var user = new UserRepository().GetUser(username);
                if (user != null)
                {
                    await repository.AddGrain(
                        serviceNameBox.Text,
                        passwordBox.Text,
                        user.Id);
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving password: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 