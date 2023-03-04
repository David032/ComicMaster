namespace ComicMaster.Stitcher
{
    class Page
    {
        int number;
        Image image; //Probably won't support webp?

        public Page(int pageNumber, Image pageImage = null)
        {
            number = pageNumber;
            image = pageImage;
        }

        public void setPageImage(Image pageImage)
        {
            image = pageImage;
        }
    }
}
