namespace AzraSözlük.Data.Dtos;

public class ResetPasswordDto
{
    public string NewPassword { get; set; }

    public ResetPasswordDto(string newPassword)
    {
        NewPassword = newPassword;
    }
}