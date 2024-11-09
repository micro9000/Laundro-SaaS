namespace Laundro.API.Features.Stores;

public static class StoreImageUtilities
{
    public static (bool ErrorOccured, string ErrorMessage) ValidateFile(IFormFile file)
    {
        if (file == null)
        {
            return (ErrorOccured: true, ErrorMessage: "No file added");
        }

        var imageExtensions = new List<string>() { ".png", ".jpeg", ".jpg", ".svg" };
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

    public static byte[] GetFileContent(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        return ms.ToArray();
    }
}
