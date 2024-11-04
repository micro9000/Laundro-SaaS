namespace Laundro.API.Features.Stores.UpdateStore;

internal class UpdateStoreRequest
{
    public int StoreId { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
}
