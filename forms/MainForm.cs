using System;
using System.Drawing;
using System.Windows.Forms;
using password_manager_project;

public partial class MainForm : Form
{
    private string currentUsername;
    private TextBox searchBox;
    private ListView passwordList;
    private Label usernameLabel;
    private Button addNewButton;
    private Button userMenuButton;
    private ContextMenuStrip userMenu;

    public MainForm(string username)
    {
        currentUsername = username;
        InitializeComponent();
        InitializeMainUI();
        InitializeUserMenu();
    }

    private void InitializeMainUI()
    {
        // Form base settings
        Text = "MyKeyRing";
        Size = new Size(800, 600); // 4:3 aspect ratio
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        // Username label in top left
        usernameLabel = new Label
        {
            Text = currentUsername,
            Font = new Font("SF Pro Display", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(51, 51, 51),
            Location = new Point(20, 20),
            Size = new Size(160, 30)
        };

        // Search box in top middle
        searchBox = new TextBox
        {
            PlaceholderText = "Search",
            Font = new Font("SF Pro Display", 12),
            Size = new Size(300, 30),
            Location = new Point((Width - 300) / 2, 20),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Add New button
        addNewButton = new Button
        {
            Text = "Add New",
            Font = new Font("SF Pro Display", 12),
            Size = new Size(100, 30),
            Location = new Point(Width - 120, 20),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };

        // Password ListView
        passwordList = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Size = new Size(Width - 40, Height - 100),
            Location = new Point(20, 70),
            Font = new Font("SF Pro Display", 11),
            BackColor = Color.FromArgb(245, 245, 245)
        };

        // Add columns to the ListView
        passwordList.Columns.AddRange(new ColumnHeader[]
        {
            new ColumnHeader { Text = "Title", Width = 200 },
            new ColumnHeader { Text = "Username", Width = 200 },
            new ColumnHeader { Text = "Last Modified", Width = 150 },
            new ColumnHeader { Text = "Category", Width = 150 }
        });

        // Add controls to form
        Controls.AddRange(new Control[]
        {
            usernameLabel,
            searchBox,
            addNewButton,
            passwordList
        });

        // Add event handlers
        searchBox.TextChanged += SearchBox_TextChanged;
        addNewButton.Click += AddNewButton_Click;
        passwordList.DoubleClick += PasswordList_DoubleClick;
    }

    private void InitializeUserMenu()
    {
        // Create the dropdown button
        userMenuButton = new Button
        {
            Text = "▼",
            Font = new Font("SF Pro Display", 8),
            Size = new Size(25, 25),
            Location = new Point(180, 22),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(51, 51, 51),
            Cursor = Cursors.Hand
        };

        // Create context menu
        userMenu = new ContextMenuStrip();
        userMenu.Items.Add("Edit Profile", null, (s, e) => { /* TODO: Implement edit profile */ });
        userMenu.Items.Add("Log Out", null, LogOut_Click);

        // Add click handler for the button
        userMenuButton.Click += (s, e) => 
        {
            userMenu.Show(userMenuButton, new Point(0, userMenuButton.Height));
        };

        Controls.Add(userMenuButton);
    }

    private void LogOut_Click(object sender, EventArgs e)
    {
        this.Hide();
        var loginForm = new Login();
        loginForm.Show();
    }

    private void SearchBox_TextChanged(object sender, EventArgs e)
    {
        // TODO: Implement search functionality
        string searchTerm = searchBox.Text.ToLower();
        // Filter the list based on search term
    }

    private void AddNewButton_Click(object sender, EventArgs e)
    {
        // TODO: Implement add new password functionality
        // Open a new form/dialog to add password entry
    }

    private void PasswordList_DoubleClick(object sender, EventArgs e)
    {
        // TODO: Implement password detail view
        if (passwordList.SelectedItems.Count > 0)
        {
            // Open detail view for selected password
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        Application.Exit();
    }
} 