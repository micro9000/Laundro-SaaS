using Laundro.Core.Domain.Entities;

namespace Laundro.API.Features.Stores.GetStores;

internal sealed class GetStoresResponse
{
    public IEnumerable<Store>? Stores { get; set; }
}