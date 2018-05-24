
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder { get; set; }            // The Output Folder
        private int m_thumbnailSize { get; set; }              // The Size Of The Thumbnail Size

        #endregion


        public ImageServiceModal(string output, int thumbnailSize)
        {
            this.m_OutputFolder = output;
            this.m_thumbnailSize = thumbnailSize;
        }

        private string CreateFolders(string currentPath, string imageYear, string imageMonth)
        {
            try
            {
                //we add two "\\" because one doesnt count
                Directory.CreateDirectory(Path.Combine(currentPath,imageYear));
                Directory.CreateDirectory(Path.Combine(currentPath, imageYear, imageMonth));
                //succesed returns empty
                return string.Empty;

            }
            catch (Exception e)
            {
                //failed - throws exception
                return e.Message;
            }
        }

       private static Regex r = new Regex(":");

        /*
         * this function retrieves the image date, anc converts it to local ( machine) time
         */
        public static DateTime getImageDate(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (Image img = Image.FromStream(fs, false, false))
                    {
                        PropertyItem propItem = img.GetPropertyItem(36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        return DateTime.Parse(dateTaken).ToLocalTime();
                    } // end of using
                }
            }
            catch (Exception e)
            {
                return File.GetLastWriteTime(path).ToLocalTime();
            }
        }
        
        public string AddFile(string filePath, out bool creationResult)
        {
            creationResult = false;
            string logMessaage = string.Empty;

            //extract the year and month from the date given by the path
            DateTime date = getImageDate(filePath);

            string yearFolder = m_OutputFolder + "\\" + date.Year.ToString();
            string monthFolder = yearFolder + "\\" + date.Month.ToString();
            string thumbYearFolder = m_OutputFolder + "\\" + "Thumbnails" + "\\" + date.Year.ToString();
            string thumbMonthFolder = thumbYearFolder + "\\" + date.Month.ToString();

            DirectoryInfo y = new DirectoryInfo(yearFolder);            
            try
            {
                // validate folder in  output for image
                if (!y.Exists)
                {
                    y.Create();
                }
                DirectoryInfo m = new DirectoryInfo(monthFolder);
                if (!m.Exists)
                {
                    m.Create();
                }

                // validate folder int thumbnails
                DirectoryInfo ty = new DirectoryInfo(thumbYearFolder);
                if (!ty.Exists)
                {
                    ty.Create();
                }
                DirectoryInfo tm = new DirectoryInfo(thumbMonthFolder);
                if (!tm.Exists)
                {
                    tm.Create();
                }
                string fileName = Path.GetFileName(filePath);
                string newFilePath = monthFolder + "\\" + fileName;
                String newThumbPath = thumbMonthFolder + "\\" + fileName;
                // generating thumbnail:
                int count = 1;
                while (File.Exists(newFilePath)) // check image doesnt exist already. if it does - add numeration
                {
                    newFilePath = monthFolder + "\\" + count.ToString() + fileName ;
                    newThumbPath = thumbMonthFolder + "\\" + count.ToString() + fileName;
                    count++;
                }              

                Image image = Image.FromFile(filePath);
                Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(newThumbPath, "jpg"));
                image.Dispose();
                thumb.Dispose();
                File.Move(filePath, newFilePath);        
            }                       
            catch (Exception e)
            {
                return e.Message;
            }          
            creationResult = true;
            return "Moving File Succeeded";
        }
    }
}
