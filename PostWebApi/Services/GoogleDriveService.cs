namespace PostWebApi.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Drive.v3.Data;
using System.IO;

public class GoogleDriveService
{
    private readonly string[] _scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive };
    private readonly string _applicationName = "FacebookClone";

    public DriveService GetDriveService()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var credentialPath = Path.Combine(basePath, "Services", "ultimate-balm-439008-h4-5ac62cc83f52.json"); // Service Account key

        GoogleCredential credential;
        using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
        }
        return new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName,
        });
    }
    

    public string UploadImage(Stream imageStream, string imageName, string mimeType)
    {
        var driveService = GetDriveService();

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = imageName,
            Parents = new List<string> { "1379sLHziGCnIjhGdifl_t4V7_GoAXG6A" } // Optional: Set a parent folder
        };

        var request = driveService.Files.Create(fileMetadata, imageStream, mimeType);
        request.Fields = "id";
        var file = request.Upload();

        if (file.Status == UploadStatus.Failed)
        {
            throw new Exception($"Failed to upload file: {file.Exception.Message}");
        }

        // Make file public
        var fileId = request.ResponseBody.Id;
        SetFilePublic(driveService, fileId);

        // Generate public URL
        return $"https://drive.google.com/thumbnail?id={fileId}&sz=w1000";
    }

    private void SetFilePublic(DriveService service, string fileId)
    {
        var permission = new Permission()
        {
            Role = "reader",
            Type = "anyone"
        };
        var request = service.Permissions.Create(permission, fileId);
        request.Execute();
    }


    public bool DeleteImageFile(string fileImageId)
    {
        try
        {
            var driveService = GetDriveService();
            // Delete the file
            var request = driveService.Files.Delete(fileImageId);
            request.Execute();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Return false to indicate failure
            return false;
        }
    }

}
