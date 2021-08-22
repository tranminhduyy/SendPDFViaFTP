using System.Collections.Generic;
using System.Net;
using System.IO;

namespace SendPDFViaFTP
{
    public class GMC1
    {
        string host = "ftp://192.168.20.11:21/";
        string UserId = "hmi";
        string Password = "123";
        List<string> FileInServer = new List<string>();
        public string status;
        string folderLayout = @"D:\PDF\Layout\";
        string folderWS = @"D:\PDF\WS\";
        string[] layoutStatus;
        string[] wsStatus;
        int numberFailed;

        protected GMC1()
        {

        }
        private static GMC1 _instance;
        public static GMC1 Instance()
        {
            if (_instance == null)
            {
                _instance = new GMC1();
            }
            return _instance;
        }

        public void Processing()
        {
            try
            {
                //upload file from folder Layout
                string[] filesInFolderLayout = Directory.GetFiles(folderLayout);
                int listLenght = filesInFolderLayout.Length;
                layoutStatus = new string[listLenght];
                for (int i = 0; i < listLenght; i++)
                {
                    filesInFolderLayout[i] = filesInFolderLayout[i].Replace(@"D:\PDF\Layout\", "");
                    UploadFileLayout(host, filesInFolderLayout[i], i);
                }
          
                for (int i = 0; i < listLenght; i++)
                {
                    if (layoutStatus[i] == "Failed")
                        numberFailed += 1;
                }

                //upload file from folder WS
                string[] filesInFolderWS = Directory.GetFiles(folderWS);
                listLenght = filesInFolderWS.Length;
                wsStatus = new string[listLenght];
                for (int i = 0; i < listLenght; i++)
                {
                    filesInFolderWS[i] = filesInFolderWS[i].Replace(@"D:\PDF\WS\", "");
                    UploadFileWS(host, filesInFolderWS[i], i);
                }

                for (int i = 0; i < listLenght; i++)
                {
                    if (wsStatus[i] == "Failed")
                        numberFailed += 1;
                }

                //return status
                if (numberFailed == 0)
                    status = "Success";
            }
            catch
            {
                status = "Failed";
            }
        }

        public void DeletePDF()
        {
            //delete file in FTP server
            List<string> filesInFTPServer = new List<string>();
            filesInFTPServer = DirectoryListing(host, UserId, Password);
            int length = filesInFTPServer.Count;
            for (int i = 0; i < length; i++)
            {
                DeleteFTPFile(filesInFTPServer[i], host, UserId, Password);
            }
        }

        public static List<string> DirectoryListing(string ServerAdress, string Login, string Password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ServerAdress);
            request.Credentials = new NetworkCredential(Login, Password);

            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> result = new List<string>();

            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
            response.Close();

            return result;
        }

        public static void DeleteFTPFile(string file, string ServerAdress, string Login, string Password)
        {
            FtpWebRequest clsRequest = (System.Net.FtpWebRequest)WebRequest.Create(ServerAdress + file);
            clsRequest.Credentials = new System.Net.NetworkCredential(Login, Password);

            clsRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            string result = string.Empty;
            FtpWebResponse response = (FtpWebResponse)clsRequest.GetResponse();
            long size = response.ContentLength;
            Stream datastream = response.GetResponseStream();
            StreamReader sr = new StreamReader(datastream);
            result = sr.ReadToEnd();
            sr.Close();
            datastream.Close();
            response.Close();
        }

        void UploadFileLayout(string machineIP, string file, int number)
        {
            try
            {
                string From = @"D:\PDF\Layout\" + file;
                string To = machineIP + file;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(UserId, Password);
                    client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, From);
                }
                layoutStatus[number] = "Success";
            }
            catch
            {
                layoutStatus[number] = "Failed";
            }
        }

        void UploadFileWS(string machineIP, string file, int number)
        {
            try
            {
                string From = @"D:\PDF\WS\" + file;
                string To = machineIP + file;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(UserId, Password);
                    client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, From);
                }
                wsStatus[number] = "Success";
            }
            catch
            {
                wsStatus[number] = "Failed";
            }
        }
    }
}
