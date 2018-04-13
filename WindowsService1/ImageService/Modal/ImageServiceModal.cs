using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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


        static DateTime getDate(string file)
        {
            //Creates a varialble with the current time.
            DateTime thisTime = DateTime.Now;
            //calculate the local time
            TimeSpan calc = (thisTime - thisTime.ToUniversalTime());
            DateTime finalDate = (File.GetLastWriteTimeUtc(file) + calc);
            return finalDate;
        }

        public string AddFile(string filePath, out bool creationResult)
        {

            string imageYear, imageMonth, logMessaage = String.Empty;

            try
            {
                if (File.Exists(filePath))
                {
                    //extract the year and month from the date given by the path
                    DateTime currentDate = getDate(filePath);
                    imageYear = currentDate.Year.ToString();
                    imageMonth = currentDate.Month.ToString();
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //create a new directory for the output folder
                    DirectoryInfo output = Directory.CreateDirectory(m_OutputFolder);
                    output.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                    Directory.CreateDirectory(m_OutputFolder + "\\" + "ImageThumbnails");
                    string folderMessage = this.CreateFolders(m_OutputFolder, imageYear, ImageMonth);
                    string thumbailMessage = this.CreateFolders(m_OutputFolder + "\\" + "ImageThumbnails", ImgYear, ImgMonth);
                    if (folderMessage != string.Empty || thumbailMessage != string.Empty)
                    {
                        throw new Exception("Folder Creation Error");
                    }
                    string currentFolderPath = m_OutputFolder + "\\" + ImgYear + "\\" + ImgMonth + "\\";
                    if (!File.Exists(currentFolderPath + Path.GetFileName(filePath)))
                    {
                        File.Copy(filePath, currentFolderPath + Path.GetFileName(filePath));
                        logMessaage = Path.GetFileName(filePath) + "Now Added to " + currentFolderPath;
                    }
                    if (!File.Exists((m_OutputFolder + "\\" + "ImageThumbnails" + "\\" + ImgYear + "\\" + ImgMonth + "\\" + Path.GetFileName(filePath))))
                    {
                    //    Image Imgthumbnail = Image.FromFile(filePath);
                      //  Imgthumbnail = (Image)(new Bitmap(Imgthumbnail, new Size(this.m_thumbnailSize, this.m_thumbnailSize)));
                      //  Imgthumbnail.Save(m_OutputFolder + "\\" + "ImageThumbnails" + "\\" + ImgYear + "\\" + ImgMonth + "\\" + Path.GetFileName(filePath));
                        logMessaage += " And thumbnail - " + Path.GetFileName(filePath);
                    }
                    creationResult = true;
                    return logMessaage;
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else
                {
                    throw new Exception("Exception: fail to add file because path do not exist");
                }

            }
            catch (Exception e)
            {
                creationResult = false;
                return e.ToString();
            }
        }
    }
}
