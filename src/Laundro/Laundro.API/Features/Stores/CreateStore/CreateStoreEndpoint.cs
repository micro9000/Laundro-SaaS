﻿using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.Stores.ProfileStorage;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Laundro.Core.Storage;

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
        Policies(PolicyName.CanCreateUpdateRetrieveAllStore);
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreateStoreRequest request, CancellationToken c)
    {
        try
        {
            var currentUser = _currentUserAccessor.GetCurrentUser();
            var tenantId = currentUser?.Tenant?.Id;
            var tenantGuid = currentUser?.Tenant?.TenantGuid;

            if (tenantId == null || tenantGuid == null)
            {
                AddError("Unable to proceed creating your store due to internal server error");
                _logger.LogError("Unable to create new store due to missing tenant id in the User Context {@UserContext}", currentUser);
            }

            if (request.StoreImage is not null)
            {
                var imageFileValidationResult = ValidateFile(request.StoreImage);
                if (imageFileValidationResult.ErrorOccured)
                {
                    AddError(imageFileValidationResult.ErrorMessage);
                    _logger.LogError("Unable to create new store due to {ErrorMessage}", imageFileValidationResult.ErrorMessage);
                }
            }

            ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point

            await _storeProfileImagesStorage.EnsureTenantContainerExists((Guid)tenantGuid!);
            string? imageFileUrl = null;
            string? imageFileContentType = null;
            if (request.StoreImage is not null)
            {
                imageFileContentType = request.StoreImage.ContentType;
                var fileContent = GetFileContent(request.StoreImage);
                imageFileUrl = await _storeProfileImagesStorage.Store(
                (Guid)tenantGuid!,
                new InputFileStorageInformation
                {
                    Id = Guid.NewGuid(),
                    FileName = request.StoreImage?.FileName,
                    DateUploaded = _clock.Now
                }, fileContent);
            }

            var newStore = new Store
            {
                Name = request.Name,
                CreatedAt = _clock.Now,
                TenantId = (int)tenantId!,
                ProfileImageUrl = imageFileUrl,
                ProfileImageContentType = imageFileContentType
            };

            _dbContext.Stores.Add(newStore);
            await _dbContext.SaveChangesAsync();

            await _userStoresRepository.RefreshAndGetCachedStoresByTenant(currentUser!.UserId);

            await SendAsync(new()
            {
                Store = newStore
            });
        }
        catch (Exception ex)
        {
            AddError("Unable to fetch all stores due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }

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

internal class CreateStoreValidator : Validator<CreateStoreRequest>
{
    public CreateStoreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store Name is required")
            .MinimumLength(3).WithMessage("Your Store name is too short!");
    }
}

internal sealed class CreateStoreRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public IFormFile? StoreImage { get; set; }
}

internal sealed class CreateStoreResponse
{
    public Store? Store { get; set; }
}