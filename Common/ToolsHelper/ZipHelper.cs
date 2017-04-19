using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Configuration;
using Common.CryptHelper;

namespace Common
{
    public class ZipHelper
    {
        /// <summary>
        /// ZIP:壓縮單個檔
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="FileToZip">需要壓縮的檔（絕對路徑）</param>
        /// <param name="ZipedPath">壓縮後的檔路徑（絕對路徑）</param>
        /// <param name="ZipedFileName">壓縮後的檔案名稱（檔案名，默認 同原始檔案同名）</param>
        /// <param name="CompressionLevel">壓縮等級（0 無 - 9 最高，默認 5）</param>
        /// <param name="BlockSize">緩存大小（每次寫入文件大小，默認 2048）</param>
        /// <param name="IsEncrypt">是否加密（默認 加密）</param>
        public static void ZipFile(string FileToZip, string ZipedPath, string ZipedFileName = "", int CompressionLevel = 5, int BlockSize = 2048, bool IsEncrypt = true)
        {
            //如果檔沒有找到，則報錯
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要壓縮的檔: " + FileToZip + " 不存在!");
            }

            //檔案名稱（默認同原始檔案名稱相同）
            string ZipFileName = string.IsNullOrEmpty(ZipedFileName) ? ZipedPath + "\\" + new FileInfo(FileToZip).Name.Substring(0, new FileInfo(FileToZip).Name.LastIndexOf('.')) + ".zip" : ZipedPath + "\\" + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    using (System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        string fileName = FileToZip.Substring(FileToZip.LastIndexOf("\\") + 1);

                        ZipEntry ZipEntry = new ZipEntry(fileName);

                        if (IsEncrypt)
                        {
                            //壓縮檔加密
                            ZipStream.Password = "123";
                        }

                        ZipStream.PutNextEntry(ZipEntry);

                        //設置壓縮層級
                        ZipStream.SetLevel(CompressionLevel);

                        //緩存大小
                        byte[] buffer = new byte[BlockSize];

                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }

                        StreamToZip.Close();
                    }

                    ZipStream.Finish();
                    ZipStream.Close();
                }

                ZipFile.Close();
            }
        }


        public static void ZipDirectory(string DirectoryToZip, string ZipedPath, string ZipedFileName = "", bool IsEncrypt = true, System.Collections.Generic.List<string> IgNoreFiles = null)
        {
            if (!Directory.Exists(DirectoryToZip))
            {
                throw new FileNotFoundException("指定的目錄: " + DirectoryToZip + " 不存在!");
            }
            string path = string.IsNullOrEmpty(ZipedFileName) ? (ZipedPath + "\\" + new DirectoryInfo(DirectoryToZip).Name + ".zip") : (ZipedPath + "\\" + ZipedFileName + ".zip");
            using (FileStream fileStream = File.Create(path))
            {
                using (ZipOutputStream zipOutputStream = new ZipOutputStream(fileStream))
                {
                    if (IsEncrypt)
                    {
                        zipOutputStream.Password = ((ConfigurationManager.AppSettings["ZipPassword"] != null) ? new AESCrypt().Decrypt(ConfigurationManager.AppSettings["ZipPassword"].ToString()) : "guodongbudingxizhilang");
                    }
                    ZipSetp(DirectoryToZip, zipOutputStream, "", IgNoreFiles);
                }
            }
        }
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath, System.Collections.Generic.List<string> IgNoreFiles = null)
        {
            if (strDirectory[strDirectory.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                strDirectory += System.IO.Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();
            string[] fileSystemEntries = Directory.GetFileSystemEntries(strDirectory);
            string[] array = fileSystemEntries;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if (IgNoreFiles == null || IgNoreFiles.Count <= 0 || !IgNoreFiles.Contains(text + "\\"))
                {
                    if (Directory.Exists(text))
                    {
                        string text2 = parentPath + text.Substring(text.LastIndexOf("\\") + 1);
                        text2 += "\\";
                        ZipSetp(text, s, text2, null);
                    }
                    else
                    {
                        using (FileStream fileStream = File.OpenRead(text))
                        {
                            byte[] array2 = new byte[fileStream.Length];
                            fileStream.Read(array2, 0, array2.Length);
                            string name = parentPath + text.Substring(text.LastIndexOf("\\") + 1);
                            ZipEntry zipEntry = new ZipEntry(name);
                            zipEntry.DateTime = DateTime.Now;
                            zipEntry.Size = fileStream.Length;
                            fileStream.Close();
                            crc.Reset();
                            crc.Update(array2);
                            zipEntry.Crc = crc.Value;
                            s.PutNextEntry(zipEntry);
                            s.Write(array2, 0, array2.Length);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ZIP：壓縮檔夾
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="DirectoryToZip">需要壓縮的資料夾（絕對路徑）</param>
        /// <param name="ZipedPath">壓縮後的檔路徑（絕對路徑）</param>
        /// <param name="ZipedFileName">壓縮後的檔案名稱（檔案名，默認 同原始檔案夾同名）</param>
        /// <param name="IsEncrypt">是否加密（默認 加密）</param>
        public static void ZipDirectory(string DirectoryToZip, string ZipedPath, string ZipedFileName = "", bool IsEncrypt = true)
        {
            //如果目錄不存在，則報錯
            if (!System.IO.Directory.Exists(DirectoryToZip))
            {
                throw new System.IO.FileNotFoundException("指定的目錄: " + DirectoryToZip + " 不存在!");
            }

            //檔案名稱（默認同原始檔案名稱相同）
            string ZipFileName = string.IsNullOrEmpty(ZipedFileName) ? ZipedPath + "\\" + new DirectoryInfo(DirectoryToZip).Name + ".zip" : ZipedPath + "\\" + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    if (IsEncrypt)
                    {
                        //壓縮檔加密
                        s.Password = "123";
                    }
                    ZipSetp(DirectoryToZip, s, "");
                }
            }
        }
        /// <summary>
        /// 遞迴遍歷目錄
        /// add yuangang by 2016-06-13
        /// </summary>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames)// 遍歷所有的檔和目錄
            {

                if (Directory.Exists(file))// 先當作目錄處理如果存在這個目錄就遞迴Copy該目錄下面的檔
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                }

                else // 否則直接壓縮檔
                {
                    //打開壓縮檔
                    using (FileStream fs = File.OpenRead(file))
                    {

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;

                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);

                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        /// <summary>
        /// ZIP:解壓一個zip檔
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="ZipFile">需要解壓的Zip檔（絕對路徑）</param>
        /// <param name="TargetDirectory">解壓到的目錄</param>
        /// <param name="Password">解壓密碼</param>
        /// <param name="OverWrite">是否覆蓋已存在的檔</param>
        public static void UnZip(string ZipFile, string TargetDirectory, string Password, bool OverWrite = true)
        {
            //如果解壓到的目錄不存在，則報錯
            if (!System.IO.Directory.Exists(TargetDirectory))
            {
                throw new System.IO.FileNotFoundException("指定的目錄: " + TargetDirectory + " 不存在!");
            }
            //目錄結尾
            if (!TargetDirectory.EndsWith("\\")) { TargetDirectory = TargetDirectory + "\\"; }

            using (ZipInputStream zipfiles = new ZipInputStream(File.OpenRead(ZipFile)))
            {
                zipfiles.Password = Password;
                ZipEntry theEntry;

                while ((theEntry = zipfiles.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(TargetDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(TargetDirectory + directoryName + fileName) && OverWrite) || (!File.Exists(TargetDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(TargetDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipfiles.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                zipfiles.Close();
            }
        }
    }
}
