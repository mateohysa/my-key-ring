using System.Security.Cryptography;
using System.Text;

namespace password_manager_project.encryption;

public class AesEncryption
{
    private readonly byte[] key;
    private readonly byte[] iv;

    public AesEncryption(string masterPassword)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(masterPassword, 32, 10000, HashAlgorithmName.SHA256))
        {
            key = deriveBytes.GetBytes(32); // 256 bits for AES-256
            iv = deriveBytes.GetBytes(16);  // 128 bits for IV
        }
    }

    public string EncryptCredential(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using (var encryptor = aes.CreateEncryptor())
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string DecryptCredential(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
} 