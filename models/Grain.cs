public class Grain
{
    private string _serviceName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;

    public int Id { get; set; }
    
    public string ServiceName 
    { 
        get => _serviceName;
        set => _serviceName = value ?? throw new ArgumentNullException(nameof(ServiceName));
    }

    public string Email
    {
        get => _email;
        set => _email = value ?? throw new ArgumentNullException(nameof(Email));
    }
    
    public string Password 
    { 
        get => _password;
        set => _password = value ?? throw new ArgumentNullException(nameof(Password));
    }
    
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
