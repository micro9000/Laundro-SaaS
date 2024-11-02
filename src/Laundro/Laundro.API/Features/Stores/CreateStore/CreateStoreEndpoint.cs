using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.Stores.ProfileStorage;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Laundro.Core.Storage;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Laundro.API.Features.Stores.CreateStore;

internal class CreateStoreEndpoint : Endpoint<CreateStoreRequest, CreateStoreResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IUserStoresRepository _userStoresRepository;
    private readonly IStoreProfileImagesStorage _storeProfileImagesStorage;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public CreateStoreEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IClockService clock,
        IUserStoresRepository userStoresRepository,
        IStoreProfileImagesStorage storeProfileImagesStorage,
        ILogger<CreateStoreEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _clock = clock;
        _userStoresRepository = userStoresRepository;
        _storeProfileImagesStorage = storeProfileImagesStorage;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("api/store/create");
        Policies(PolicyName.IsTenantOwner);
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreateStoreRequest request, CancellationToken c)
    {

        var currentUser = _currentUserAccessor.GetCurrentUser();

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, c))
            {
                try
                {
                    var tenantId = currentUser?.Tenant?.Id;
                    var tenantGuid = currentUser?.Tenant?.TenantGuid;

                    if (tenantId == null || tenantGuid == null)
                    {
                        AddError("Unable to proceed creating your store due to internal server error");
                        _logger.LogError("Unable to create new store due to missing tenant id in the User Context {@UserContext}", currentUser);
                        ThrowIfAnyErrors(); // Fail fast
                    }

                    var newStore = new Store
                    {
                        Name = request.Name,
                        Location = request.Location,
                        CreatedAt = _clock.Now,
                        TenantId = (int)tenantId!
                    };

                    _dbContext.Stores.Add(newStore);
                    await _dbContext.SaveChangesAsync();

                    if (request.StoreImages is not null && request.StoreImages.Any())
                    {
                        foreach (var file in request.StoreImages)
                        {
                            var imageFileValidationResult = ValidateFile(file);
                            if (imageFileValidationResult.ErrorOccured)
                            {
                                AddError(imageFileValidationResult.ErrorMessage);
                                _logger.LogError("Unable to create new store due to {ErrorMessage}", imageFileValidationResult.ErrorMessage);
                                ThrowIfAnyErrors(); // Fail fast
                            }

                            var fileContent = GetFileContent(file);
                            var imageFileUrl = await _storeProfileImagesStorage.Store(
                                new InputFileStorageInformation
                                {
                                    Id = Guid.NewGuid(),
                                    TenantGuid = (Guid) tenantGuid!,
                                    FileName = file?.FileName,
                                    DateUploaded = _clock.Now
                                }, fileContent);

                            _dbContext.StoreImages.Add(new StoreImage
                            {
                                StoreId = newStore.Id,
                                Url = imageFileUrl,
                                ContentType = file?.ContentType,
                                CreatedAt = _clock.Now
                            });
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    await _userStoresRepository.RefreshAndGetCachedStoresByTenant(currentUser!.UserId);

                    await SendAsync(new()
                    {
                        Store = newStore
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    AddError("Unable to fetch all stores due to internal server error");
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        });

        ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point
    }

    private (bool ErrorOccured, string ErrorMessage) ValidateFile(IFormFile file)
    {
        if (file == null)
        {
            return (ErrorOccured: true, ErrorMessage: "No file added");
        }

        var imageExtensions = new List<string>() { ".png", ".jpeg", ".svg" };
        var fileIsImage = imageExtensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase);

        if (!fileIsImage)
        {
            return (ErrorOccured: true, ErrorMessage: "File is not an image");
        }

        var fileHaveContent = file.Length > 0;

        if (!fileHaveContent)
        {
            return (ErrorOccured: true, ErrorMessage: "File have no content");
        }


        return (ErrorOccured: false, ErrorMessage: string.Empty);
    }

    private static byte[] GetFileContent(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        return ms.ToArray();
    }
}
