using System.IO.Compression;

namespace ComicMaster.Stitcher;

public partial class CreateComic : ContentPage
{
    List<FileResult> selectedFiles;

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

    private void StitchComic_Clicked(object sender, EventArgs e)
    {

        List<(string, string)> filesUsed = new List<(string, string)>();
        string cacheDir = FileSystem.Current.CacheDirectory;

        ZipFile.CreateFromDirectory(cacheDir + "/ComicTemp", cacheDir + "/ComicTemp.zip");

        foreach (var item in selectedFiles)
        {
            File.Copy(item.FullPath, cacheDir + "/" + item.FileName);
            string file = Path.Combine(cacheDir, item.FileName);
            filesUsed.Add((file, item.FileName));
        }


        using (FileStream tempComicZip = new FileStream(cacheDir + "/ComicTemp.zip", FileMode.Open))
        {
            using (ZipArchive archive = new ZipArchive(tempComicZip, ZipArchiveMode.Update))
            {
                foreach (var item in filesUsed)
                {
                    ZipArchiveEntry file = archive.CreateEntryFromFile(item.Item1, item.Item2);
                }
            }
        }

    }
}