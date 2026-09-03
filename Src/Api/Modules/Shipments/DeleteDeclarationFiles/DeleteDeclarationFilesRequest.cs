namespace Api.Modules.Shipments.DeleteDeclarationFiles.DeleteDeclarationFiles;

public sealed class DeleteDeclarationFilesRequest
{
    public List<Guid> DeclarationFileIds { get; set; } = [];
}