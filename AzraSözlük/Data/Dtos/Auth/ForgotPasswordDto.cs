using CaferVerifierLib;

namespace AzraSözlük.Data.Dtos;

public class ForgotPasswordDto
{
    public string UserEmail { get; set; }

    public ForgotPasswordDto(string userEmail)
    {
        UserEmail = userEmail;
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
        return (errors.Count == 0, errors);
    }
    
}