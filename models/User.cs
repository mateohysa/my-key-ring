public class User
{
    private string _username = string.Empty;
    private string _masterPassword = string.Empty;

    public int Id { get; set; }
    
    public string Username 
    { 
        get => _username;
        set => _username = value ?? throw new ArgumentNullException(nameof(Username));
    }
    
    public string MasterPassword 
    { 
        get => _masterPassword;
        set => _masterPassword = value ?? throw new ArgumentNullException(nameof(MasterPassword));
    }
}