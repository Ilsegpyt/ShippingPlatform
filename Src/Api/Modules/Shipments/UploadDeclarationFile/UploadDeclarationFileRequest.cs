namespace Api.Modules.Shipments.UploadDeclarationFile;

public sealed class UploadDeclarationFileRequest
{
    public IFormFile File { get; set; } = null!;
}