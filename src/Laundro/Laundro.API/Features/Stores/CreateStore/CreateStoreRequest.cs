namespace Laundro.API.Features.Stores.CreateStore;

internal sealed class CreateStoreRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public List<IFormFile>? StoreImages { get; set; }
}
