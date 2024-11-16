using System.Text;

namespace ContactWebApi.Utils;

public static class EncryptData
{
    public static string Encrypt(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Decrypt(string cipherText)
    {
        var cipherTextBytes = Convert.FromBase64String(cipherText);
        return Encoding.UTF8.GetString(cipherTextBytes);
    }
}