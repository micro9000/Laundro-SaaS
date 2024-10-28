using Laundro.Core.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundro.Core.Storage;
public static class FilePaths
{
    public static string GenerateStoreProfileFilePath(InputFileStorageInformation fileInfo)
    {
        var folder = TenantStorageConstants.StoreProfileFolder;
        var extension = Path.GetExtension(fileInfo.FileName);
        var formattedDateUploaded = fileInfo.DateUploaded.ToDateTimeUtc().ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var fileName = $"{fileInfo.Id.ToString()}-{formattedDateUploaded}";

        return $"{folder}/{fileName}{extension}";
    }
}
