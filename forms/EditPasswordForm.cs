using System.Security.Cryptography;
using System.Text;
using System.IO;

public class EditPasswordForm : Form
{
    private TextBox serviceNameBox = null!;
    private TextBox emailBox = null!;
    private TextBox passwordBox = null!;
    private Button generatePasswordButton = null!;
    private Button togglePasswordButton = null!;
    private Button saveButton = null!;
    private readonly string currentUsername;
    private readonly Grain currentGrain;

    public EditPasswordForm(string username, Grain grain)
    {
        currentUsername = username;
        currentGrain = grain;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(400, 500);
        this.Text = "Edit Password";
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var serviceLabel = new Label
        {
            Text = "Service Name",
            Location = new Point(20, 20),
            Size = new Size(360, 20),
            Font = new Font("SF Pro Display", 10)
        };

        serviceNameBox = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(360, 30),
            Font = new Font("SF Pro Display", 12),
            Text = currentGrain.ServiceName
        };

        var emailLabel = new Label
        {
            Text = "Email",
            Location = new Point(20, 85),
            Size = new Size(360, 20),
            Font = new Font("SF Pro Display", 10)
        };

        emailBox = new TextBox
        {
            Location = new Point(20, 110),
            Size = new Size(360, 30),
            Font = new Font("SF Pro Display", 12),
            Text = currentGrain.Email
        };

        var passwordLabel = new Label
        {
            Text = "Password",
            Location = new Point(20, 150),
            Size = new Size(360, 20),
            Font = new Font("SF Pro Display", 10)
        };

        passwordBox = new TextBox
        {
            Location = new Point(20, 175),
            Size = new Size(300, 30),
            UseSystemPasswordChar = true,
            Font = new Font("SF Pro Display", 12),
            Text = currentGrain.Password
        };

        generatePasswordButton = new Button
        {
            Text = "Generate",
            Location = new Point(20, 215),
            Size = new Size(100, 30),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White
        };
        generatePasswordButton.Click += (s, e) => 
        {
            passwordBox.Text = GeneratePassword();
        };

        togglePasswordButton = new Button
        {
            Text = "👁",
            Location = new Point(330, 175),
            Size = new Size(50, 30),
            FlatStyle = FlatStyle.Flat
        };
        togglePasswordButton.Click += (s, e) => 
        {
            passwordBox.UseSystemPasswordChar = !passwordBox.UseSystemPasswordChar;
        };

        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Color.White
        };

        saveButton = new Button
        {
            Text = "Save",
            Size = new Size(100, 35),
            Location = new Point(this.ClientSize.Width - 230, 12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White
        };
        saveButton.Click += SaveButton_Click;

        var deleteButton = new Button
        {
            Text = "Delete",
            Size = new Size(100, 35),
            Location = new Point(this.ClientSize.Width - 120, 12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Red,
            ForeColor = Color.White
        };
        deleteButton.Click += DeleteButton_Click;

        bottomPanel.Controls.AddRange(new Control[] { saveButton, deleteButton });

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

    private async void SaveButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(serviceNameBox.Text) || 
            string.IsNullOrWhiteSpace(emailBox.Text) || 
            string.IsNullOrWhiteSpace(passwordBox.Text))
        {
            MessageBox.Show("Please fill in all fields", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var repository = new GrainRepository();
            currentGrain.ServiceName = serviceNameBox.Text;
            currentGrain.Email = emailBox.Text;
            currentGrain.Password = PasswordEncryption.Encrypt(passwordBox.Text);
            currentGrain.UpdatedAt = DateTime.Now;

            await repository.UpdateGrain(currentGrain);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving password: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void DeleteButton_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            $"Are you sure you want to delete the password for {currentGrain.ServiceName}?", 
            "Confirm Delete", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            try
            {
                var repository = new GrainRepository();
                await repository.DeleteGrain(currentGrain.Id);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting password: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private string GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
} 