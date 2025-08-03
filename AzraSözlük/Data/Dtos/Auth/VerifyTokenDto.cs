namespace AzraSözlük.Data.Dtos;

public class VerifyTokenDto
{
    public string UserEmail { get; set; }
    public string UserToken { get; set; }

    public VerifyTokenDto(string userEmail, string userToken)
    {
        UserEmail = userEmail;
        UserToken = userToken;
    }
}