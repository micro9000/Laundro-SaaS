using Laundro.Core.Constants;
using System.Globalization;

namespace Laundro.Core.Storage;
public static class FilePaths
{
    public static string GenerateStoreProfileFilePath(InputFileStorageInformation fileInfo)
    {
        var folder = $"tenant-{fileInfo.TenantGuid.ToString()}/{TenantStorageConstants.StoreProfileFolder}";
        var extension = Path.GetExtension(fileInfo.FileName);
        var formattedDateUploaded = fileInfo.DateUploaded.ToDateTimeUtc().ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var fileName = $"{fileInfo.Id.ToString()}-{formattedDateUploaded}";

        return $"{folder}/{fileName}{extension}";
    }
}
