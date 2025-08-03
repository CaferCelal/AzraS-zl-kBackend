using CaferVerifierLib;

namespace AzraSözlük.Data.Dtos;

public class SignInDto
{
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }

    public SignInDto(string userEmail, string userPassword)
    {
        UserEmail = userEmail;
        UserPassword = userPassword;
    }

    public (bool isValid, List<string> errors) IsValid()
    {
        List<string> errors = new();
        
        if (string.IsNullOrWhiteSpace(UserEmail))
        {
            errors.Add("Email cannot be empty.");
        }

        if (!Verifier.IsValidEmailFormat(UserEmail))
        {
            errors.Add("Email format is invalid.");
        }

        if (string.IsNullOrWhiteSpace(UserPassword))
        {
            errors.Add("Password cannot be empty.");
        }
        
        return (errors.Count == 0, errors);
    }
}