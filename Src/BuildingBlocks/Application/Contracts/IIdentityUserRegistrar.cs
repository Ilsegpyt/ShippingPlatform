namespace BuildingBlocks.Application.Contracts;

public interface IIdentityUserRegistrar
{
    Task<Guid> CreateUserAsync(string email, CancellationToken ct); 
    string GetDefaultPassword();
}