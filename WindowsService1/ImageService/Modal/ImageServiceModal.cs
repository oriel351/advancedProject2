
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


        private string CreateFolders(string currentPath, string imageYear, string imageMonth)
        {
            try
            {
                //we add two "\\" because one doesnt count
                Directory.CreateDirectory(currentPath + "\\" + imageYear);
                Directory.CreateDirectory(currentPath + "\\" + imageYear + "\\" + imageMonth);
                //succesed returns empty
                return string.Empty;

            }
            catch (Exception e)
            {
                //failed - throws exception
                return e.ToString();
            }
        }


        private static Regex r = new Regex(":");

        /*
         * this function retrieves the image date, anc converts it to local ( machine) time
         */
        public static DateTime getImageDate(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (Image img = Image.FromStream(fs, false, false))
                {
                    Console.WriteLine("trying to get date took pic");
                    try
                    {
                        PropertyItem propItem = img.GetPropertyItem(36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        return DateTime.Parse(dateTaken).ToLocalTime();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("could not get date....taking last modified instead");
                        return File.GetLastWriteTime(path).ToLocalTime();
                    }
                } // end of using
            }
        }

        public string AddFile(string filePath, out bool creationResult)
        {

            creationResult = false;
            string imageYear, imageMonth, logMessaage = String.Empty;

            //extract the year and month from the date given by the path
            DateTime date = getImageDate(filePath);

            String yearFolder = m_OutputFolder + "\\" + date.Year.ToString();
            String monthFolder = m_OutputFolder + "\\" + date.Month.ToString();
            String thumbYearFolder = m_OutputFolder + "\\" + "Thumbnails" + "\\" + date.Year.ToString();
            String thumbMonthFolder = m_OutputFolder + "\\" + "Thumbnails" + "\\" + date.Month.ToString();

            DirectoryInfo y = new DirectoryInfo(yearFolder);
            FileStream fs = null;
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

                fs = File.Create(filePath);
                File.Move(filePath, monthFolder);
                Console.WriteLine("{0} file  was moved to {1}.", filePath, monthFolder);

                Image image = Image.FromFile(monthFolder);
                Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(thumbMonthFolder, "thumb"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not move the file" + e.StackTrace);
            } finally
            {
                fs.Close();
            }
        }
    }
}
