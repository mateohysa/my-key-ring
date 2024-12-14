public class Grain
{
    private string _serviceName = string.Empty;
    private string _password = string.Empty;

    public int Id { get; set; }
    
    public string ServiceName 
    { 
        get => _serviceName;
        set => _serviceName = value ?? throw new ArgumentNullException(nameof(ServiceName));
    }
    
    public string Password 
    { 
        get => _password;
        set => _password = value ?? throw new ArgumentNullException(nameof(Password));
    }
    
    public int UserId { get; set; }
}
