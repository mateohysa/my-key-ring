using password_manager_project.encryption;
namespace password_manager_project;

public partial class Login : Form
{
    private readonly Button loginButton;
    private readonly Button createProfileButton;
    private Panel welcomePanel = null!;
    private Panel loginPanel = null!;
    private Panel createAccountPanel = null!;
    
    // Login panel controls
    private TextBox? usernameTextBox;
    private TextBox? passwordTextBox;
    private Button? loginSubmitButton;
    private Button? loginCancelButton;

    // Create account panel controls
    private TextBox? newUsernameTextBox;
    private TextBox? newPasswordTextBox;
    private TextBox? confirmPasswordTextBox;

    public Login()
    {
        try
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "key.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
        }
        catch (Exception ex)
        {
            // Silently continue if icon can't be loaded
            System.Diagnostics.Debug.WriteLine($"Icon load error: {ex.Message}");
        }

        // Remove or comment out this line
        // this.Icon = new Icon("resources/key.ico");
        
        // Initialize readonly buttons in constructor
        loginButton = new Button
        {
            Text = "Log In",
            Size = new Size(280, 45),
            Location = new Point(85, 180),
            Font = new Font("SF Pro Display", 12, FontStyle.Regular),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };

        createProfileButton = new Button
        {
            Text = "Create User",
            Size = new Size(280, 45),
            Location = new Point(85, 240),
            Font = new Font("SF Pro Display", 12, FontStyle.Regular),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(0, 122, 255),
            Cursor = Cursors.Hand
        };

        InitializeComponent();
        InitializePanels();
        InitializeWelcomeUI();
        InitializeLoginUI();
        InitializeCreateAccountUI();
        
        // Show welcome panel initially
        ShowWelcomePanel();
    }

    private void InitializePanels()
    {
        // Form base settings
        Text = "MyKeyRing";
        Size = new Size(450, 500);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // Initialize panels
        welcomePanel = new Panel
        {
            Size = new Size(450, 500),
            Location = new Point(0, 0),
            BackColor = Color.White
        };

        loginPanel = new Panel
        {
            Size = new Size(450, 500),
            Location = new Point(0, 0),
            BackColor = Color.White,
            Visible = false
        };

        createAccountPanel = new Panel
        {
            Size = new Size(450, 500),
            Location = new Point(0, 0),
            BackColor = Color.White,
            Visible = false
        };

        Controls.Add(welcomePanel);
        Controls.Add(loginPanel);
        Controls.Add(createAccountPanel);
    }

    private void InitializeWelcomeUI()
    {
        // Welcome panel controls
        Label welcomeLabel = new Label
        {
            Text = "MyKeyRing",
            Font = new Font("SF Pro Display", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 51, 51),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(350, 40),
            Location = new Point(50, 40)
        };

        Label subtitleLabel = new Label
        {
            Text = "Store all your passwords safely, in one place",
            Font = new Font("SF Pro Display", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(128, 128, 128),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(350, 30),
            Location = new Point(50, 80)
        };

        welcomePanel.Controls.AddRange(new Control[] { 
            welcomeLabel, 
            subtitleLabel, 
            loginButton, 
            createProfileButton 
        });

        loginButton.Click += (s, e) => ShowLoginPanel();
        createProfileButton.Click += (s, e) => ShowCreateAccountPanel();
    }

    private void InitializeLoginUI()
    {
        // Title label centered at top
        Label titleLabel = new Label
        {
            Text = "Log In",
            Font = new Font("SF Pro Display", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 51, 51),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(350, 40),
            Location = new Point((loginPanel.Width - 350) / 2, 80)
        };

        // Center the textboxes
        int textBoxWidth = 280;
        int centerX = (loginPanel.Width - textBoxWidth) / 2;

        usernameTextBox = new TextBox
        {
            Location = new Point(centerX, 160),
            Size = new Size(textBoxWidth, 30),
            PlaceholderText = "Username",
            Font = new Font("SF Pro Display", 12)
        };

        passwordTextBox = new TextBox
        {
            Location = new Point(centerX, 210),
            Size = new Size(textBoxWidth, 30),
            PlaceholderText = "Mater Password",
            PasswordChar = '*',
            Font = new Font("SF Pro Display", 12)
        };

        // Center the buttons
        int buttonWidth = 130;
        int buttonSpacing = 20;
        int buttonsStartX = (loginPanel.Width - ((buttonWidth * 2) + buttonSpacing)) / 2;

        loginSubmitButton = new Button
        {
            Location = new Point(buttonsStartX, 270),
            Size = new Size(buttonWidth, 45),
            Text = "Login",
            Font = new Font("SF Pro Display", 12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White
        };

        loginCancelButton = new Button
        {
            Location = new Point(buttonsStartX + buttonWidth + buttonSpacing, 270),
            Size = new Size(buttonWidth, 45),
            Text = "Back",
            Font = new Font("SF Pro Display", 12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Gray,
            ForeColor = Color.White
        };

        // Add controls to login panel
        loginPanel.Controls.AddRange(new Control[] 
        { 
            titleLabel,
            usernameTextBox, 
            passwordTextBox, 
            loginSubmitButton, 
            loginCancelButton 
        });

        // Add click handlers
        loginSubmitButton.Click += LoginSubmitButton_Click;
        loginCancelButton.Click += LoginCancelButton_Click;
    }

    private void LoginSubmitButton_Click(object? sender, EventArgs e)
    {
        try
        {
            string username = usernameTextBox?.Text ?? string.Empty;
            string password = passwordTextBox?.Text ?? string.Empty;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userRepository = new UserRepository();
            var user = userRepository.GetUser(username);

            if (user != null && PasswordHasher.VerifyPassword(password, user.MasterPassword))
            {
                this.Hide();
                var mainForm = new MainForm(username);
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Login error: {ex.Message}\n\nStack trace: {ex.StackTrace}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoginCancelButton_Click(object? sender, EventArgs e)
    {
        // Add login logic here
        ShowWelcomePanel();
    }

    private void InitializeCreateAccountUI()
    {
        // Title label centered at top
        Label titleLabel = new Label
        {
            Text = "Create Account",
            Font = new Font("SF Pro Display", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 51, 51),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(350, 40),
            Location = new Point((createAccountPanel.Width - 350) / 2, 80)
        };

        // Center the textboxes
        int textBoxWidth = 280;
        int centerX = (createAccountPanel.Width - textBoxWidth) / 2;

        newUsernameTextBox = new TextBox
        {
            Location = new Point(centerX, 160),
            Size = new Size(textBoxWidth, 30),
            PlaceholderText = "Username",
            Font = new Font("SF Pro Display", 12)
        };

        newPasswordTextBox = new TextBox
        {
            Location = new Point(centerX, 210),
            Size = new Size(textBoxWidth, 30),
            PlaceholderText = "Master Password",
            PasswordChar = '*',
            Font = new Font("SF Pro Display", 12)
        };

        confirmPasswordTextBox = new TextBox
        {
            Location = new Point(centerX, 260),
            Size = new Size(textBoxWidth, 30),
            PlaceholderText = "Confirm Master Password",
            PasswordChar = '*',
            Font = new Font("SF Pro Display", 12)
        };

        // Center the buttons
        int buttonWidth = 130;
        int buttonSpacing = 20;
        int buttonsStartX = (createAccountPanel.Width - ((buttonWidth * 2) + buttonSpacing)) / 2;

        Button createButton = new Button
        {
            Location = new Point(buttonsStartX, 320),
            Size = new Size(buttonWidth, 45),
            Text = "Create",
            Font = new Font("SF Pro Display", 12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White
        };

        Button cancelButton = new Button
        {
            Location = new Point(buttonsStartX + buttonWidth + buttonSpacing, 320),
            Size = new Size(buttonWidth, 45),
            Text = "Back",
            Font = new Font("SF Pro Display", 12),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Gray,
            ForeColor = Color.White
        };

        // Add controls to create account panel
        createAccountPanel.Controls.AddRange(new Control[] 
        { 
            titleLabel,
            newUsernameTextBox,
            newPasswordTextBox,
            confirmPasswordTextBox,
            createButton,
            cancelButton
        });

        // Add click handlers
        createButton.Click += CreateAccount_Click;
        cancelButton.Click += (s, e) => ShowWelcomePanel();
    }

    private void CreateAccount_Click(object? sender, EventArgs e)
    {
        if (newUsernameTextBox?.Text == null || 
            newPasswordTextBox?.Text == null || 
            confirmPasswordTextBox?.Text == null)
        {
            MessageBox.Show("Please fill in all fields.", "Registration Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string username = newUsernameTextBox.Text;
        string password = newPasswordTextBox.Text;
        string confirmPassword = confirmPasswordTextBox.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Please fill in all fields.", "Registration Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (password != confirmPassword)
        {
            MessageBox.Show("Passwords do not match!", "Registration Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var userRepository = new UserRepository();
        var newUser = new User
        {
            Username = username,
            MasterPassword = PasswordHasher.HashPassword(password)
        };

        userRepository.CreateUser(newUser).Wait();
        MessageBox.Show("Account created successfully!", "Success", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);

        this.Hide();
        var mainForm = new MainForm(username);
        mainForm.Show();
    }

    private void ShowWelcomePanel()
    {
        welcomePanel.Visible = true;
        loginPanel.Visible = false;
        createAccountPanel.Visible = false;
    }

    private void ShowLoginPanel()
    {
        welcomePanel.Visible = false;
        loginPanel.Visible = true;
        createAccountPanel.Visible = false;
    }

    private void ShowCreateAccountPanel()
    {
        welcomePanel.Visible = false;
        loginPanel.Visible = false;
        createAccountPanel.Visible = true;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        Application.Exit();
    }
}