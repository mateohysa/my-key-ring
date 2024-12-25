using System;
using System.Drawing;
using System.Windows.Forms;
using password_manager_project;
using System.IO;
using Timer = System.Windows.Forms.Timer;

public partial class MainForm : Form
{
    private readonly string currentUsername;
    private Panel sidebarPanel = null!;
    private Panel mainContentPanel = null!;
    private Panel bottomPanel = null!;
    private Button addNewButton = null!;
    private Button editButton = null!;
    private Button exitButton = null!;
    private ListView passwordList = null!;
    private TextBox searchBox = null!;
    private Label usernameLabel = null!;
    private ContextMenuStrip userMenu = null!;
    private TreeView categoryTree = null!;
    private Panel selectedPasswordPanel = null!;
    private Label selectedServiceLabel = null!;
    private Label selectedEmailLabel = null!;
    private TextBox selectedPasswordBox = null!;
    private Button showPasswordButton = null!;
    private Button copyPasswordButton = null!;
    private ListView suggestedList = null!;
    private ListView keysListInSuggested = null!;
    private List<Grain> currentPasswords = new List<Grain>();
    private Timer searchTimer = null!;
    private ContextMenuStrip addNewMenu = null!;
    private ContextMenuStrip editMenu = null!;

    private const int FORM_WIDTH = 800;
    private const int FORM_HEIGHT = 720;
    private const int SIDEBAR_WIDTH = 150;

    public MainForm(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));
            
        currentUsername = username;
        InitializeComponent();
        Size = new Size(FORM_WIDTH, FORM_HEIGHT);

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

        InitializeMainUI();
        LoadAllPasswords().Wait();
        categoryTree.SelectedNode = categoryTree.Nodes[0];
    }

    private void InitializeMainUI()
    {
        InitializeTopBar();
        InitializeSidebar();
        InitializeMainContent();
        InitializeButtons();
        InitializeSelectedPasswordPanel();
    }

    private void InitializeTopBar()
    {
        usernameLabel = new Label
        {
            Text = currentUsername,
            Font = new Font("SF Pro Display", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(51, 51, 51),
            Location = new Point(20, 20),
            Size = new Size(160, 30),
            Cursor = Cursors.Hand
        };

        userMenu = new ContextMenuStrip();
        var editProfileItem = userMenu.Items.Add("Edit Profile");
        editProfileItem.Enabled = false;
        
        userMenu.Items.Add("-");
        
        userMenu.Items.Add("Log Out", null, (s, e) => 
        {
            this.Hide();
            var loginForm = new Login();
            loginForm.Show();
        });

        usernameLabel.Click += (s, e) => userMenu.Show(usernameLabel, new Point(0, usernameLabel.Height));

        searchBox = new TextBox
        {
            Location = new Point(FORM_WIDTH - 270, 20),
            Size = new Size(250, 25),
            Font = new Font("SF Pro Display", 10),
            PlaceholderText = "Search passwords..."
        };
        searchBox.TextChanged += SearchBox_TextChanged;

        searchTimer = new Timer
        {
            Interval = 300
        };
        searchTimer.Tick += SearchTimer_Tick;

        Controls.AddRange(new Control[] { usernameLabel, searchBox });
    }

    private void InitializeSidebar()
    {
        sidebarPanel = new Panel
        {
            Location = new Point(0, 60),
            Size = new Size(SIDEBAR_WIDTH, FORM_HEIGHT - 60),
            BackColor = Color.FromArgb(245, 245, 245)
        };

        categoryTree = new TreeView
        {
            Location = new Point(0, 0),
            Size = new Size(SIDEBAR_WIDTH, FORM_HEIGHT - 60),
            Font = new Font("SF Pro Display", 11),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(245, 245, 245),
            ItemHeight = 30,
            Indent = 10
        };

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
            Location = new Point(SIDEBAR_WIDTH, 60),
            Size = new Size(FORM_WIDTH - SIDEBAR_WIDTH, FORM_HEIGHT - 60),
            BackColor = Color.White
        };

        InitializePasswordListView();
        InitializeSuggestedView();

        mainContentPanel.Controls.Add(passwordList);
        mainContentPanel.Controls.Add(suggestedList);
        mainContentPanel.Controls.Add(keysListInSuggested);

        suggestedList.Visible = false;
        keysListInSuggested.Visible = false;

        Controls.Add(mainContentPanel);
    }

    private void InitializePasswordListView()
    {
        passwordList = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Location = new Point(0, 100),
            Size = new Size(FORM_WIDTH - SIDEBAR_WIDTH, FORM_HEIGHT - 160),
            Font = new Font("SF Pro Display", 11)
        };

        passwordList.Columns.AddRange(new ColumnHeader[]
        {
            new ColumnHeader { Text = "Service Name", Width = 150 },
            new ColumnHeader { Text = "Email", Width = 200 },
            new ColumnHeader { Text = "Password", Width = 150 },
            new ColumnHeader { Text = "Created At", Width = 150 }
        });

        passwordList.SelectedIndexChanged += PasswordList_SelectedIndexChanged;
    }

    private void InitializeSuggestedView()
    {
        suggestedList = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Location = new Point(0, 100),
            Size = new Size(FORM_WIDTH - SIDEBAR_WIDTH, (FORM_HEIGHT - 160) / 2 - 5),
            Font = new Font("SF Pro Display", 11),
            BackColor = Color.FromArgb(250, 250, 250)
        };

        suggestedList.Columns.AddRange(new ColumnHeader[]
        {
            new ColumnHeader { Text = "Service Name", Width = 150 },
            new ColumnHeader { Text = "Email", Width = 200 },
            new ColumnHeader { Text = "Password", Width = 150 },
            new ColumnHeader { Text = "Last Updated", Width = 150 }
        });

        suggestedList.SelectedIndexChanged += PasswordList_SelectedIndexChanged;

        keysListInSuggested = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Location = new Point(0, 100 + (FORM_HEIGHT - 160) / 2 + 5),
            Size = new Size(FORM_WIDTH - SIDEBAR_WIDTH, (FORM_HEIGHT - 160) / 2 - 5),
            Font = new Font("SF Pro Display", 11)
        };

        keysListInSuggested.Columns.AddRange(new ColumnHeader[]
        {
            new ColumnHeader { Text = "Service Name", Width = 150 },
            new ColumnHeader { Text = "Email", Width = 200 },
            new ColumnHeader { Text = "Password", Width = 150 },
            new ColumnHeader { Text = "Created At", Width = 150 }
        });

        keysListInSuggested.SelectedIndexChanged += PasswordList_SelectedIndexChanged;
    }

    private void InitializeButtons()
    {
        bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            BackColor = Color.White
        };

        addNewButton = new Button
        {
            Text = "Add New",
            Size = new Size(100, 35),
            Location = new Point(FORM_WIDTH - 340, 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White,
            Font = new Font("SF Pro Display", 10)
        };
        addNewButton.Click += AddNewButton_Click;

        editButton = new Button
        {
            Text = "Edit",
            Size = new Size(100, 35),
            Location = new Point(FORM_WIDTH - 230, 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 122, 255),
            ForeColor = Color.White,
            Font = new Font("SF Pro Display", 10),
            Enabled = false
        };
        editButton.Click += EditButton_Click;

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

        bottomPanel.Controls.AddRange(new Control[] { addNewButton, editButton, exitButton });
        Controls.Add(bottomPanel);
    }

    private void InitializeSelectedPasswordPanel()
    {
        selectedPasswordPanel = new Panel
        {
            Location = new Point(SIDEBAR_WIDTH, 60),
            Size = new Size(FORM_WIDTH - SIDEBAR_WIDTH - 20, 90),
            BackColor = Color.White,
            Visible = false
        };

        selectedPasswordPanel.Paint += (s, e) =>
        {
            ControlPaint.DrawBorder(e.Graphics, selectedPasswordPanel.ClientRectangle,
                Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid);
        };

        selectedServiceLabel = new Label
        {
            Location = new Point(15, 10),
            Size = new Size(400, 20),
            Font = new Font("SF Pro Display", 12, FontStyle.Bold)
        };

        selectedEmailLabel = new Label
        {
            Location = new Point(15, 35),
            Size = new Size(400, 20),
            Font = new Font("SF Pro Display", 10)
        };

        selectedPasswordBox = new TextBox
        {
            Location = new Point(15, 60),
            Size = new Size(300, 25),
            UseSystemPasswordChar = true,
            ReadOnly = true,
            Font = new Font("SF Pro Display", 10)
        };

        showPasswordButton = new Button
        {
            Text = "👁",
            Size = new Size(30, 25),
            Location = new Point(320, 60),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(0, 122, 255)
        };
        showPasswordButton.Click += (s, e) => 
            selectedPasswordBox.UseSystemPasswordChar = !selectedPasswordBox.UseSystemPasswordChar;

        copyPasswordButton = new Button
        {
            Text = "📋",
            Size = new Size(30, 25),
            Location = new Point(355, 60),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(0, 122, 255)
        };
        copyPasswordButton.Click += (s, e) =>
        {
            if (selectedPasswordBox.Text != "")
            {
                Clipboard.SetText(selectedPasswordBox.Text);
                MessageBox.Show("Password copied to clipboard!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        selectedPasswordPanel.Controls.AddRange(new Control[] 
        { 
            selectedServiceLabel, 
            selectedEmailLabel,
            selectedPasswordBox,
            showPasswordButton,
            copyPasswordButton
        });

        Controls.Add(selectedPasswordPanel);
        selectedPasswordPanel.BringToFront();
    }

    private void CategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (e.Node?.Text == "All Items")
        {
            passwordList.Visible = true;
            suggestedList.Visible = false;
            keysListInSuggested.Visible = false;
            LoadAllPasswords().Wait();
        }
        else if (e.Node?.Text == "Suggested")
        {
            passwordList.Visible = false;
            suggestedList.Visible = true;
            keysListInSuggested.Visible = true;
            LoadSuggestedPasswords().Wait();
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
                currentPasswords = await repository.GetGrainsByUserId(user.Id);
                UpdatePasswordList(currentPasswords);
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
            currentPasswords = await repository.GetGrainsByUserId(user.Id);
            
            var now = DateTime.Now;
            var suggestedUpdates = GetSuggestedPasswords();
            var regularKeys = GetRegularPasswords();

            UpdateSuggestedList(suggestedUpdates);
            UpdateKeysList(regularKeys);
        }
    }

    private void UpdatePasswordList(List<Grain> passwords, string searchText = "")
    {
        passwordList.Items.Clear();
        foreach (var pwd in passwords)
        {
            if (string.IsNullOrEmpty(searchText) || 
                pwd.ServiceName.ToLower().Contains(searchText) || 
                pwd.Email.ToLower().Contains(searchText))
            {
                var item = new ListViewItem();
                
                if (!string.IsNullOrEmpty(searchText) && pwd.ServiceName.ToLower().Contains(searchText))
                {
                    item.Text = pwd.ServiceName;
                    item.BackColor = Color.FromArgb(230, 240, 255);
                }
                else
                {
                    item.Text = pwd.ServiceName;
                }

                var emailItem = item.SubItems.Add(pwd.Email);
                if (!string.IsNullOrEmpty(searchText) && pwd.Email.ToLower().Contains(searchText))
                {
                    emailItem.BackColor = Color.FromArgb(230, 240, 255);
                }

                item.SubItems.Add(new string('•', pwd.Password.Length));
                item.SubItems.Add(pwd.CreatedAt.ToString("MM/dd/yyyy"));
                item.Tag = pwd;
                passwordList.Items.Add(item);
            }
        }
    }

    private void UpdateSuggestedList(List<Grain> passwords, string searchText = "")
    {
        suggestedList.Items.Clear();
        foreach (var pwd in passwords)
        {
            if (string.IsNullOrEmpty(searchText) || 
                pwd.ServiceName.ToLower().Contains(searchText) || 
                pwd.Email.ToLower().Contains(searchText))
            {
                var item = new ListViewItem();
                
                if (!string.IsNullOrEmpty(searchText) && pwd.ServiceName.ToLower().Contains(searchText))
                {
                    item.Text = pwd.ServiceName;
                    item.BackColor = Color.FromArgb(230, 240, 255);
                }
                else
                {
                    item.Text = pwd.ServiceName;
                }

                var emailItem = item.SubItems.Add(pwd.Email);
                if (!string.IsNullOrEmpty(searchText) && pwd.Email.ToLower().Contains(searchText))
                {
                    emailItem.BackColor = Color.FromArgb(230, 240, 255);
                }

                item.SubItems.Add(new string('•', pwd.Password.Length));
                var daysAgo = (DateTime.Now - pwd.UpdatedAt).Days;
                item.SubItems.Add($"Updated {daysAgo} days ago");
                item.Tag = pwd;
                suggestedList.Items.Add(item);
            }
        }
    }

    private void UpdateKeysList(List<Grain> passwords, string searchText = "")
    {
        keysListInSuggested.Items.Clear();
        foreach (var pwd in passwords)
        {
            if (string.IsNullOrEmpty(searchText) || 
                pwd.ServiceName.ToLower().Contains(searchText) || 
                pwd.Email.ToLower().Contains(searchText))
            {
                var item = new ListViewItem();
                
                if (!string.IsNullOrEmpty(searchText) && pwd.ServiceName.ToLower().Contains(searchText))
                {
                    item.Text = pwd.ServiceName;
                    item.BackColor = Color.FromArgb(230, 240, 255);
                }
                else
                {
                    item.Text = pwd.ServiceName;
                }

                var emailItem = item.SubItems.Add(pwd.Email);
                if (!string.IsNullOrEmpty(searchText) && pwd.Email.ToLower().Contains(searchText))
                {
                    emailItem.BackColor = Color.FromArgb(230, 240, 255);
                }

                item.SubItems.Add(new string('•', pwd.Password.Length));
                item.SubItems.Add(pwd.CreatedAt.ToString("MM/dd/yyyy"));
                item.Tag = pwd;
                keysListInSuggested.Items.Add(item);
            }
        }
    }

    private List<Grain> GetSuggestedPasswords()
    {
        var now = DateTime.Now;
        return currentPasswords
            .Where(p => (now - p.UpdatedAt).TotalDays > 90)
            .OrderByDescending(p => now - p.UpdatedAt)
            .ToList();
    }

    private List<Grain> GetRegularPasswords()
    {
        var now = DateTime.Now;
        return currentPasswords
            .Where(p => (now - p.UpdatedAt).TotalDays <= 90)
            .ToList();
    }

    private void SearchBox_TextChanged(object sender, EventArgs e)
    {
        searchTimer.Stop();
        searchTimer.Start();
    }

    private void SearchTimer_Tick(object sender, EventArgs e)
    {
        searchTimer.Stop();
        PerformSearch(searchBox.Text);
    }

    private void PerformSearch(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            UpdatePasswordList(currentPasswords);
            if (categoryTree.SelectedNode?.Text == "Suggested")
            {
                UpdateSuggestedList(GetSuggestedPasswords());
                UpdateKeysList(GetRegularPasswords());
            }
            return;
        }

        searchText = searchText.ToLower();
        
        if (categoryTree.SelectedNode?.Text == "Suggested")
        {
            var suggestedPasswords = GetSuggestedPasswords();
            var regularPasswords = GetRegularPasswords();
            
            UpdateSuggestedList(suggestedPasswords, searchText);
            UpdateKeysList(regularPasswords, searchText);
        }
        else
        {
            UpdatePasswordList(currentPasswords, searchText);
        }
    }

    private void PasswordList_SelectedIndexChanged(object sender, EventArgs e)
    {
        var listView = sender as ListView;
        if (listView?.SelectedItems.Count > 0)
        {
            var selectedItem = listView.SelectedItems[0];
            if (selectedItem.Tag is Grain grain)
            {
                selectedServiceLabel.Text = grain.ServiceName;
                selectedEmailLabel.Text = grain.Email;
                selectedPasswordBox.Text = grain.Password;
                selectedPasswordPanel.Visible = true;
                editButton.Enabled = true;
            }
        }
        else
        {
            selectedPasswordPanel.Visible = false;
            editButton.Enabled = false;
        }
    }

    private void AddNewButton_Click(object sender, EventArgs e)
    {
        var addForm = new AddPasswordForm(currentUsername);
        if (addForm.ShowDialog() == DialogResult.OK)
        {
            if (categoryTree.SelectedNode?.Text == "Suggested")
                LoadSuggestedPasswords().Wait();
            else
                LoadAllPasswords().Wait();
        }
    }

    private void EditButton_Click(object sender, EventArgs e)
    {
        EditSelectedPassword();
    }

    private void EditSelectedPassword()
    {
        ListView? activeList = null;
        
        if (passwordList.SelectedItems.Count > 0)
            activeList = passwordList;
        else if (suggestedList?.SelectedItems.Count > 0)
            activeList = suggestedList;
        else if (keysListInSuggested?.SelectedItems.Count > 0)
            activeList = keysListInSuggested;

        if (activeList?.SelectedItems[0].Tag is Grain selectedGrain)
        {
            var editForm = new EditPasswordForm(currentUsername, selectedGrain);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                if (categoryTree.SelectedNode?.Text == "Suggested")
                    LoadSuggestedPasswords().Wait();
                else
                    LoadAllPasswords().Wait();
            }
        }
    }

    private async void DeleteSelectedPassword()
    {
        ListView? activeList = null;
        
        if (passwordList.SelectedItems.Count > 0)
            activeList = passwordList;
        else if (suggestedList?.SelectedItems.Count > 0)
            activeList = suggestedList;
        else if (keysListInSuggested?.SelectedItems.Count > 0)
            activeList = keysListInSuggested;

        if (activeList?.SelectedItems[0].Tag is Grain selectedGrain)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this password?", 
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var repository = new GrainRepository();
                    await repository.DeleteGrain(selectedGrain.Id);
                    
                    if (categoryTree.SelectedNode?.Text == "Suggested")
                        await LoadSuggestedPasswords();
                    else
                        await LoadAllPasswords();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting password: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 