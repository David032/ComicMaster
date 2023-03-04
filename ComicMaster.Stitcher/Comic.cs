namespace ComicMaster.Stitcher
{
    class Comic
    {
        string name;
        string publisher;
        DateTime publishDate;
        List<Page> pages;

        public Comic()
        {

        }

        public void addPageToComic(Page pageToAdd)
        {
            pages.Add(pageToAdd);
        }
    }
}
