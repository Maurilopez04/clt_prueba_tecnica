namespace Clt.Api.Application.Common.Security;

public interface IPasswordHasher
{
    string Hash(string password);
}
