using System.Text;
using System.Text.RegularExpressions;

namespace CaferVerifierLib;

public class Verifier
{
    public static bool IsValidEmailFormat(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var emailRegex = @"^[^@\s]+(\.[^@\s]+)*[^.\s]@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            return false;
        }

        var domainPart = email.Split('@').Last();
        if (domainPart.Contains(".."))
        {
            return false;
        }

        return true;
    }
    
    public static (bool IsValid, string Errors) IsValidPasswordFormat(string password,
        int minLength, int maxLength, bool requireUppercase, bool requireLowercase, bool requireNumber, bool requireSpecialCharacter)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Password cannot be null or empty.");
        }

        var errors = new StringBuilder();

        if (password.Length < minLength || password.Length > maxLength)
        {
            errors.AppendLine($"Password must be between {minLength} and {maxLength} characters.");
        }

        var hasUppercase = new Regex(@"[A-Z]");
        var hasLowercase = new Regex(@"[a-z]");
        var hasNumber = new Regex(@"\d");
        var hasSpecialCharacter = new Regex(@"[\W_]");

        if (!hasUppercase.IsMatch(password) && requireUppercase)
        {
            errors.AppendLine("Password must contain at least one uppercase letter.");
        }
        
        if (!hasLowercase.IsMatch(password) && requireLowercase)
        {
            errors.AppendLine("Password must contain at least one lowercase letter.");
        }
        
        if (!hasNumber.IsMatch(password) && requireNumber)
        {
            errors.AppendLine("Password must contain at least one numeric digit.");
        }
        if (!hasSpecialCharacter.IsMatch(password) && requireSpecialCharacter)
        {
            errors.AppendLine("Password must contain at least one special character.");
        }

        return (errors.Length == 0, errors.ToString().Trim());
    }
}