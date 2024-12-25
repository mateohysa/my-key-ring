using System;
using System.Windows.Forms;
using password_manager_project;

namespace password_manager_project
{
    public partial class AddPasswordForm : Form
    {
        private readonly string username;
        private TextBox serviceNameBox = null!;
        private TextBox emailBox = null!;
        private TextBox passwordBox = null!;
        private Button generatePasswordButton = null!;
        private Button togglePasswordButton = null!;

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

            // Email input
            var emailLabel = new Label
            {
                Text = "Email:",
                Location = new Point(20, 80),
                Size = new Size(100, 20)
            };

            emailBox = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(340, 25)
            };

            // Password input (moved down)
            var passwordLabel = new Label
            {
                Text = "Password:",
                Location = new Point(20, 140),
                Size = new Size(100, 20)
            };

            passwordBox = new TextBox
            {
                Location = new Point(20, 165),
                Size = new Size(240, 25),
                UseSystemPasswordChar = true
            };

            generatePasswordButton = new Button
            {
                Text = "Generate",
                Location = new Point(270, 165),
                Size = new Size(70, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White
            };
            generatePasswordButton.Click += GeneratePassword_Click;

            togglePasswordButton = new Button
            {
                Text = "👁",
                Location = new Point(340, 165),
                Size = new Size(25, 25),
                FlatStyle = FlatStyle.Flat
            };
            togglePasswordButton.Click += (s, e) => 
                passwordBox.UseSystemPasswordChar = !passwordBox.UseSystemPasswordChar;

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
            this.Controls.AddRange(new Control[] 
            { 
                serviceLabel, 
                serviceNameBox,
                emailLabel,
                emailBox,
                passwordLabel, 
                passwordBox,
                generatePasswordButton,
                togglePasswordButton,
                bottomPanel
            });
        }

        private void GeneratePassword_Click(object sender, EventArgs e)
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string special = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            
            var random = new Random();
            var password = new char[16];

            // Ensure at least one of each type
            password[0] = upperCase[random.Next(upperCase.Length)];
            password[1] = lowerCase[random.Next(lowerCase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = special[random.Next(special.Length)];

            // Fill the rest randomly
            string allChars = upperCase + lowerCase + numbers + special;
            for (int i = 4; i < password.Length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the password
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }

            passwordBox.Text = new string(password);
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serviceNameBox.Text) || 
                string.IsNullOrWhiteSpace(emailBox.Text) ||
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
                    string encryptedPassword = PasswordEncryption.Encrypt(passwordBox.Text);
                    await repository.AddGrain(
                        serviceNameBox.Text,
                        emailBox.Text,
                        encryptedPassword,
                        user.Id);
                    DialogResult = DialogResult.OK;
                    Close();
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