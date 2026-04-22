namespace GoodHamburger.Web.Services;

public interface ITokenService
{
    string? Token { get; set; }
}

public class TokenService : ITokenService
{
    public string? Token { get; set; }
}
