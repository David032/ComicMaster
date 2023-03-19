using System.IO.Compression;

namespace ComicMaster.Stitcher;

public partial class CreateComic : ContentPage
{
    List<FileResult> selectedFiles = new();

    public CreateComic()
    {
        InitializeComponent();
    }

    private void OnAddPage(object sender, EventArgs e)
    {
        Application.Current.Dispatcher.Dispatch(() =>
        {
            var btn = new Button();
            btn.Text = "Page";
            btn.Clicked += AddNewPage;
            btn.HorizontalOptions = LayoutOptions.Center;
            CreationWindow.Add(btn);
        });
    }

    async void AddNewPage(object sender, EventArgs e)
    {
        var result = await PickAndShow(PickOptions.Default);
        var button = (Button)sender;
        button.Text = result.FileName.ToString();
        selectedFiles.Add(result);
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    var image = ImageSource.FromStream(() => stream);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// TODO: What happens if the process fails? Need to clean up the files
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StitchComic_Clicked(object sender, EventArgs e)
    {
        List<(string, int)> filesUsed = new List<(string, int)>();
        string cacheDir = FileSystem.Current.CacheDirectory;
        DirectoryInfo folder = Directory.CreateDirectory(cacheDir + "/ComicTemp");
        string zipFolder = cacheDir + "/ComicTemp.zip";

        ZipFile.CreateFromDirectory(folder.FullName, zipFolder);

        for (int i = 0; i < selectedFiles.Count; i++)
        {
            var image = selectedFiles[i];
            string tempFilePath = cacheDir + "/" + i + Path.GetExtension(image.FullPath);
            File.Copy(image.FullPath, tempFilePath);
            filesUsed.Add((tempFilePath, i));
        }

        //Create the zip folder
        using (FileStream tempComicZip = new FileStream(cacheDir + "/ComicTemp.zip", FileMode.Open))
        {
            using (ZipArchive archive = new ZipArchive(tempComicZip, ZipArchiveMode.Update))
            {
                foreach (var item in filesUsed)
                {
                    ZipArchiveEntry file = archive.CreateEntryFromFile(item.Item1, item.Item2.ToString() + ".png");
                }
            }
        }

        //Delete copied images & temp folder
        foreach (var item in filesUsed)
        {
            File.Delete(item.Item1);
        }
        filesUsed.Clear();

        Directory.Delete(folder.FullName, true);

        //Rename to cbz format
        File.Move(zipFolder, zipFolder + ".cbz");

        //Then hand over to new page for file naming, tagging, date, etc?
    }
}