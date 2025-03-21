﻿using FastEndpoints;
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
using System.IO;

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
        Post("create-new-store");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreateStoreRequest request, CancellationToken ct)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, ct))
            {
                try
                {
                    var tenantId = currentUser?.Tenant?.Id;
                    var tenantGuid = currentUser?.Tenant?.TenantGuid;

                    if (tenantId == null || tenantGuid == null)
                    {
                        _logger.LogError("Unable to create new store due to missing tenant id in the User Context {@UserContext}", currentUser);
                        ThrowError("Unable to proceed creating your store due to internal server error");
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
                            var imageFileValidationResult = StoreImageUtilities.ValidateFile(file);
                            if (imageFileValidationResult.ErrorOccured)
                            {
                                _logger.LogError("Unable to create new store due to {ErrorMessage}", imageFileValidationResult.ErrorMessage);
                                ThrowError(imageFileValidationResult.ErrorMessage);
                            }

                            var imageId = Guid.NewGuid();
                            var extension = Path.GetExtension(file?.FileName);
                            var newFileName = $"{imageId}{extension}";

                            var fileContent = StoreImageUtilities.GetFileContent(file!);
                            var imageFileUrl = await _storeProfileImagesStorage.Store(
                                new InputFileStorageInformation
                                {
                                    Id = imageId,
                                    TenantGuid = (Guid)tenantGuid!,
                                    FileName = file?.FileName,
                                    DateUploaded = _clock.Now
                                }, fileContent);

                            _dbContext.StoreImages.Add(new StoreImage
                            {
                                StoreId = newStore.Id,
                                Url = imageFileUrl,
                                ContentType = file?.ContentType,
                                CreatedAt = _clock.Now,
                                Filename = newFileName
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
                    AddError("Unable to create new store due to internal server error");
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        });

        ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point
    }

}
