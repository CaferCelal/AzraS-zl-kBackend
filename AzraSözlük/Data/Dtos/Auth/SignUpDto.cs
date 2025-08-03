using AzraSözlük.Constants;
using CaferVerifierLib;

namespace AzraSözlük.Data.Dtos;

public class SignUpDto
{
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }

    public SignUpDto(string userName, string userEmail, string userPassword)
    {
        UserName = userName;
        UserEmail = userEmail;
        UserPassword = userPassword;
    }
    
    public (bool isValid, List<string> errors) IsValid()
    {
        List<string> errors = new();
        
        if (string.IsNullOrWhiteSpace(UserName))
        {
            errors.Add("Username cannot be empty.");
        }

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
        
        if( UserName.Length < ProgramConstants.MinUserNameLength)
        {
            errors.Add($"Username must be at least {ProgramConstants.MinUserNameLength} characters long.");
        }
        
        if (UserName.Length > ProgramConstants.MaxUserNameLength)
        {
            errors.Add($"Username must not exceed {ProgramConstants.MaxUserNameLength} characters.");
        }

        var passwordValidation = Verifier.IsValidPasswordFormat(
            password:UserPassword,
            minLength:ProgramConstants.MinPasswordLength,
            maxLength:ProgramConstants.MaxPasswordLength,
            requireUppercase:true,
            requireLowercase:true,
            requireNumber:true,
            requireSpecialCharacter:false
        );

        if (passwordValidation.IsValid == false)
        {
            errors.AddRange(passwordValidation.Errors.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
        }
        
        return (isValid: errors.Count == 0, errors: errors);
    }
}