namespace Payroll.Infrastructure.Auth;

public static class PasswordHasher
{
    public static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }

    public static bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
        var computed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computed.SequenceEqual(hash);
    }
}
