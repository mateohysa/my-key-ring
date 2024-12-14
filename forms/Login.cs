namespace password_manager_project;

public partial class Login : Form
{
    public Login()
    {
        InitializeComponent();
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Form settings
        this.Text = "Password Manager";
        this.Size = new Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create buttons
        Button loginButton = new Button
        {
            Text = "Log In",
            Size = new Size(120, 40),
            Location = new Point(140, 100),
            Font = new Font("Arial", 10)
        };

        Button createProfileButton = new Button
        {
            Text = "Create Profile",
            Size = new Size(120, 40),
            Location = new Point(140, 160),
            Font = new Font("Arial", 10)
        };

        // Add buttons to form
        this.Controls.Add(loginButton);
        this.Controls.Add(createProfileButton);

        // Add event handlers (empty for now)
        loginButton.Click += LoginButton_Click;
        createProfileButton.Click += CreateProfileButton_Click;
    }

    private void LoginButton_Click(object sender, EventArgs e)
    {
        // Will be implemented later
    }

    private void CreateProfileButton_Click(object sender, EventArgs e)
    {
        // Will be implemented later
    }
}
