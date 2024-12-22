using System;
using System.Drawing;
using System.Windows.Forms;
using password_manager_project;
using System.IO;

public partial class MainForm : Form
{
    private const int FORM_WIDTH = 1280;  // 16:9 ratio
    private const int FORM_HEIGHT = 720;
    private string currentUsername;
    private Panel sidebarPanel;
    private Panel mainContentPanel;
    private Panel detailPanel;
    private Panel bottomPanel;
    private Button addNewButton;
    private Button exitButton;
    private ListView passwordList;
    private TextBox searchBox;
    private Label usernameLabel;
    private Button userMenuButton;
    private ContextMenuStrip userMenu;
    private TreeView categoryTree;

    public MainForm(string username)
    {
        currentUsername = username;
        InitializeComponent();
        Size = new Size(FORM_WIDTH, FORM_HEIGHT);

        // Create bottom panel and buttons
        bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            BackColor = Color.FromArgb(245, 245, 245)
        };

        addNewButton = new Button
        {
            Text = "Add New",
            Size = new Size(100, 35),
            Location = new Point(FORM_WIDTH - 230, 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White,
            Font = new Font("SF Pro Display", 10)
        };
        addNewButton.Click += AddNewButton_Click;

        exitButton = new Button
        {
            Text = "Exit",
            Size = new Size(100, 35),
            Location = new Point(FORM_WIDTH - 120, 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(0, 122, 255),
            Font = new Font("SF Pro Display", 10)
        };
        exitButton.Click += (s, e) => Application.Exit();

        bottomPanel.Controls.AddRange(new Control[] { addNewButton, exitButton });
        Controls.Add(bottomPanel);

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
            System.Diagnostics.Debug.WriteLine($"Icon load error: {ex.Message}");
        }

        // Initialize all UI components
        InitializeMainUI();
        InitializeUserMenu();
        InitializeSidebar();
        InitializeMainContent();
        InitializeDetailPanel();
    }

    private void InitializeMainUI()
    {
        // Form base settings
        Text = "MyKeyRing";
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        // Initialize top bar
        InitializeTopBar();
    }

    private void InitializeTopBar()
    {
        // Username and dropdown
        usernameLabel = new Label
        {
            Text = currentUsername,
            Font = new Font("SF Pro Display", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(51, 51, 51),
            Location = new Point(20, 20),
            Size = new Size(160, 30)
        };

        // Search box in top right
        searchBox = new TextBox
        {
            PlaceholderText = "Search",
            Font = new Font("SF Pro Display", 12),
            Size = new Size(200, 30),
            Location = new Point(FORM_WIDTH - 220, 20),
            BorderStyle = BorderStyle.FixedSingle
        };
        searchBox.TextChanged += SearchBox_TextChanged;

        Controls.AddRange(new Control[] { usernameLabel, searchBox });
    }

    private void InitializeSidebar()
    {
        sidebarPanel = new Panel
        {
            Location = new Point(0, 60),
            Size = new Size(200, FORM_HEIGHT - 60),
            BackColor = Color.FromArgb(245, 245, 245)
        };

        categoryTree = new TreeView
        {
            Location = new Point(0, 0),
            Size = new Size(200, FORM_HEIGHT - 60),
            Font = new Font("SF Pro Display", 12),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(245, 245, 245)
        };

        // Add categories
        categoryTree.Nodes.Add("All Items");
        categoryTree.Nodes.Add("Suggested");
        
        categoryTree.AfterSelect += CategoryTree_AfterSelect;
        sidebarPanel.Controls.Add(categoryTree);
        Controls.Add(sidebarPanel);
    }

    private void InitializeMainContent()
    {
        mainContentPanel = new Panel
        {
            Location = new Point(200, 60),
            Size = new Size(FORM_WIDTH - 200, FORM_HEIGHT - 60),
            BackColor = Color.White
        };

        passwordList = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Location = new Point(0, 0),
            Size = new Size(FORM_WIDTH - 200, FORM_HEIGHT - 60),
            Font = new Font("SF Pro Display", 11)
        };

        // Add columns with sorting capability
        passwordList.Columns.AddRange(new ColumnHeader[]
        {
            new ColumnHeader { Text = "Service Name", Width = 200 },
            new ColumnHeader { Text = "Password", Width = 150 },
            new ColumnHeader { Text = "Created At", Width = 150 }
        });

        // Enable sorting
        passwordList.ListViewItemSorter = new ListViewItemComparer();
        passwordList.ColumnClick += PasswordList_ColumnClick;
        passwordList.SelectedIndexChanged += PasswordList_SelectedIndexChanged;

        mainContentPanel.Controls.Add(passwordList);
        Controls.Add(mainContentPanel);
    }

    private void InitializeDetailPanel()
    {
        detailPanel = new Panel
        {
            Location = new Point(200, 60),
            Size = new Size(FORM_WIDTH - 200, 150),
            BackColor = Color.FromArgb(250, 250, 250),
            Visible = false
        };

        // Will be populated when a password is selected
        Controls.Add(detailPanel);
    }

    private async void CategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (e.Node?.Text == "All Items")
        {
            await LoadAllPasswords();
        }
        else if (e.Node?.Text == "Suggested")
        {
            await LoadSuggestedPasswords();
        }
    }

    private async Task LoadAllPasswords()
    {
        try
        {
            var repository = new GrainRepository();
            var user = new UserRepository().GetUser(currentUsername);
            if (user != null)
            {
                var passwords = await repository.GetGrainsByUserId(user.Id);
                UpdatePasswordList(passwords);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading passwords: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);    
        }
    }

    private async Task LoadSuggestedPasswords()
    {
        var repository = new GrainRepository();
        var user = new UserRepository().GetUser(currentUsername);
        if (user != null)
        {
            var allPasswords = await repository.GetGrainsByUserId(user.Id);
            var suggestedPasswords = allPasswords.Where(p => 
                (DateTime.Now - p.UpdatedAt).TotalDays > 90).ToList();
            UpdatePasswordList(suggestedPasswords);
        }
    }

    private void UpdatePasswordList(List<Grain> passwords)
    {
        passwordList.Items.Clear();
        foreach (var pwd in passwords)
        {
            var item = new ListViewItem(pwd.ServiceName);
            item.SubItems.Add("••••••••");
            item.SubItems.Add(pwd.CreatedAt.ToShortDateString());
            passwordList.Items.Add(item);
        }
    }

    private void PasswordList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (passwordList.SelectedItems.Count > 0)
        {
            var selectedItem = passwordList.SelectedItems[0];
            var repository = new GrainRepository();
            var user = new UserRepository().GetUser(currentUsername);
            if (user != null)
            {
                var grains = repository.GetGrainsByUserId(user.Id).Result;
                var selectedGrain = grains.FirstOrDefault(g => g.ServiceName == selectedItem.Text);
                if (selectedGrain != null)
                {
                    ShowPasswordDetail(selectedGrain);
                }
            }
        }
    }

    private void ShowPasswordDetail(Grain password)
    {
        try
        {
            detailPanel.Visible = true;
            detailPanel.Controls.Clear();

            // Service name with larger font
            var serviceLabel = new Label
            {
                Text = password.ServiceName,
                Font = new Font("SF Pro Display", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };

            // Password field with visibility toggle and copy button
            var passwordBox = new TextBox
            {
                Text = password.Password,
                UseSystemPasswordChar = true,
                Location = new Point(20, 60),
                Size = new Size(220, 30),
                ReadOnly = true
            };

            var toggleButton = new Button
            {
                Text = "👁",
                Location = new Point(250, 60),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat
            };
            toggleButton.Click += (s, e) => 
                passwordBox.UseSystemPasswordChar = !passwordBox.UseSystemPasswordChar;

            var copyButton = new Button
            {
                Text = "📋",
                Location = new Point(290, 60),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat
            };
            copyButton.Click += (s, e) =>
            {
                try
                {
                    Clipboard.SetText(password.Password);
                    MessageBox.Show("Password copied to clipboard!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying password: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            detailPanel.Controls.AddRange(new Control[] 
            { 
                serviceLabel, 
                passwordBox, 
                toggleButton,
                copyButton
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error showing password details: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SearchBox_TextChanged(object sender, EventArgs e)
    {
        string searchTerm = searchBox.Text.ToLower();
        // Implement search functionality
    }

    private void InitializeUserMenu()
    {
        userMenuButton = new Button
        {
            Text = "▼",
            Font = new Font("SF Pro Display", 8),
            Size = new Size(25, 25),
            Location = new Point(180, 22),
            FlatStyle = FlatStyle.Flat
        };

        userMenu = new ContextMenuStrip();
        userMenu.Items.Add("Edit Profile", null, (s, e) => { /* TODO: Implement edit profile */ });
        userMenu.Items.Add("Log Out", null, (s, e) => 
        {
            this.Hide();
            var loginForm = new Login();
            loginForm.Show();
        });

        userMenuButton.Click += (s, e) => userMenu.Show(userMenuButton, new Point(0, userMenuButton.Height));
        Controls.Add(userMenuButton);
    }

    // Add this class for sorting
    private class ListViewItemComparer : System.Collections.IComparer
    {
        private int col;
        private bool ascending;

        public ListViewItemComparer()
        {
            col = 0;
            ascending = true;
        }

        public void ToggleSort(int column)
        {
            if (col == column)
                ascending = !ascending;
            else
            {
                col = column;
                ascending = true;
            }
        }

        public int Compare(object? x, object? y)
        {
            var itemX = (ListViewItem)x!;
            var itemY = (ListViewItem)y!;

            if (col == 2) // Date column
            {
                DateTime dateX = DateTime.Parse(itemX.SubItems[col].Text);
                DateTime dateY = DateTime.Parse(itemY.SubItems[col].Text);
                return ascending ? DateTime.Compare(dateX, dateY) : DateTime.Compare(dateY, dateX);
            }
            else // Text columns
            {
                return ascending ? 
                    String.Compare(itemX.SubItems[col].Text, itemY.SubItems[col].Text) :
                    String.Compare(itemY.SubItems[col].Text, itemX.SubItems[col].Text);
            }
        }
    }

    private void PasswordList_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        var sorter = (ListViewItemComparer)passwordList.ListViewItemSorter;
        sorter.ToggleSort(e.Column);
        passwordList.Sort();
    }

    private void AddNewButton_Click(object sender, EventArgs e)
    {
        var addForm = new AddPasswordForm(currentUsername);
        if (addForm.ShowDialog() == DialogResult.OK)
        {
            // Refresh the password list
            if (categoryTree.SelectedNode?.Text == "Suggested")
                LoadSuggestedPasswords().Wait();
            else
                LoadAllPasswords().Wait();
        }
    }
} 