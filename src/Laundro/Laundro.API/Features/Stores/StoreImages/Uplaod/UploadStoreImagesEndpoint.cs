using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.CreateStore;
using Laundro.API.Features.Stores.StoreImages.Get;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.Stores.ProfileStorage;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Laundro.Core.Storage;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.StoreImages.Uplaod;

internal class UploadStoreImagesEndpoint : Endpoint<UploadStoreImagesRequest>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IStoreProfileImagesStorage _storeProfileImagesStorage;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public UploadStoreImagesEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IClockService clock,
        IStoreProfileImagesStorage storeProfileImagesStorage,
        ILogger<CreateStoreEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _clock = clock;
        _storeProfileImagesStorage = storeProfileImagesStorage;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("upload");
        Group<StoreImagesGroup>();
        Policies(PolicyName.IsTenantOwner);
        AllowFileUploads();
    }

    public override async Task HandleAsync(UploadStoreImagesRequest request, CancellationToken ct)
    {
        if (request.Images is null || !request.Images.Any())
        {
            ThrowError("Invalid Request: No uploaded file/image");
        }

        var currentUser = _currentUserAccessor.GetCurrentUser();

        var tenantId = currentUser?.Tenant?.Id;
        var tenantGuid = currentUser?.Tenant?.TenantGuid;

        if (tenantId == null || tenantGuid == null)
        {
            _logger.LogError("Unable to create new store due to missing tenant id in the User Context {@UserContext}", currentUser);
            ThrowError("Unable to proceed creating your store due to internal server error");
        }

        // TODO: Find a way to make this global and default if there we are passing StoreId in any API request
        // Need to check if the store is belongs to the user's tenant
        var store = await _dbContext.Stores
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == request.StoreId, ct);
        if (store == null)
        {
            ThrowError("Invalid store id");
        }

        var existingActiveImages = await _dbContext.StoreImages.Where(img => img.StoreId == store.Id).ToListAsync();
        if (existingActiveImages.Count() >= 4)
        {
            ThrowError("A store can only have a maximum of 4 images");
        }

        if (request.Images is not null && request.Images.Any())
        {
            foreach(var file in request.Images)
            {
                var imageFileValidationResult = StoreImageUtilities.ValidateFile(file);
                if (imageFileValidationResult.ErrorOccured)
                {
                    _logger.LogError("Invalid image: {ErrorMessage}", imageFileValidationResult.ErrorMessage);
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

                var image = new StoreImage
                {
                    StoreId = store.Id,
                    Url = imageFileUrl,
                    ContentType = file?.ContentType,
                    CreatedAt = _clock.Now,
                    Filename = newFileName
                };
                _dbContext.StoreImages.Add(image);

                await _dbContext.SaveChangesAsync();
                await SendCreatedAtAsync<GetStoreImageContentEndpoint>(new GetStoreImageContentRequest
                {
                    ImageId = image.Id,
                    StoreId = store.Id,
                    tenantGuid = tenantGuid.ToString()
                }, "Uploaded");
            }
        }

    }
}

internal class UploadStoreImagesRequest
{
    public int StoreId { get; set; }
    public List<IFormFile>? Images { get; set; }
}

internal class UploadStoreImagesValidator : Validator<UploadStoreImagesRequest>
{
    public UploadStoreImagesValidator()
    {
        RuleFor(x => x.Images)
            .Must(imgs => imgs == null || imgs.Count == 0)
            .WithMessage("Upload at least 1 image");

        RuleFor(x => x.Images)
            .Must(imgs => imgs != null && imgs.Count <= 4)
            .WithMessage("Maximum store images is 4");

        RuleFor(x => x.StoreId)
            .NotEqual(0)
            .WithMessage("Store Id is required");
    }
}