namespace Identity.Contracts;

public interface IActivationService
{
    Task<string> GenerateActivationTokenAsync(
        Guid userId,
        CancellationToken ct = default);
}