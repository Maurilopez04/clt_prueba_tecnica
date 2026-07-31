namespace Clt.Api.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PasswordHash { get; set; }
    public ICollection<Address> Addresses { get; set; } = [];
}
