using ModbusTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace SendPDFViaFTP
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }
        string UserId = "hmi";
        string Password = "123";
        string folderLayout = @"D:\PDF\Layout\";
        string folderWS = @"D:\PDF\WS\";
        string folderLayoutBK = @"D:\PDF_Backup\Layout\";
        string folderWSBK = @"D:\PDF_Backup\WS\";

        private void btSend_Click(object sender, EventArgs e)
        {
            lb601.Text = "Downloading...";
            //lb604.Text = "Downloading...";
            //lb605.Text = "Downloading...";
            //lb5M.Text = "Downloading...";

            lbBTD2.Text = "Downloading...";
            lbBTD3.Text = "Downloading...";
            lbBTD4.Text = "Downloading...";
            lbBTD5.Text = "Downloading...";

            lbD650.Text = "Downloading...";
            lbD750.Text = "Downloading...";
            lbD1000.Text = "Downloading...";
            lbD1100.Text = "Downloading...";

            //lbGMC1.Text = "Downloading...";
            //lbGMC2.Text = "Downloading...";
            //lbSCL.Text = "Downloading...";

            P601.Instance().Processing();
            //P604.Instance().Processing();
            //P605.Instance().Processing();
            //P5M.Instance().Processing();

            BTD2.Instance().Processing();
            BTD3.Instance().Processing();
            BTD4.Instance().Processing();
            BTD5.Instance().Processing();

            D650.Instance().Processing();
            D750.Instance().Processing();
            D1000.Instance().Processing();
            D1100.Instance().Processing();

            //GMC1.Instance().Processing();
            //GMC2.Instance().Processing();
            //SCL.Instance().Processing();

            lb601.Text = P601.Instance().status;
            //lb604.Text = P604.Instance().status;
            //lb605.Text = P605.Instance().status;
            //lb5M.Text = P5M.Instance().status;

            lbBTD2.Text = BTD2.Instance().status;
            lbBTD3.Text = BTD3.Instance().status;
            lbBTD4.Text = BTD4.Instance().status;
            lbBTD5.Text = BTD5.Instance().status;

            lbD650.Text = D650.Instance().status;
            lbD750.Text = D750.Instance().status;
            lbD1000.Text = D1000.Instance().status;
            lbD1100.Text = D1100.Instance().status;

            //lbGMC1.Text = GMC1.Instance().status;
            //lbGMC2.Text = GMC2.Instance().status;
            //lbSCL.Text = SCL.Instance().status;

            #region Move all file PDF to folder backup           
            try
            {
                //folder Layout
                System.IO.DirectoryInfo di = new DirectoryInfo(folderLayout);
                foreach (FileInfo file in di.GetFiles())
                {
                    Directory.Move(folderLayout + file, folderLayoutBK + file);
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                //folder WS
                di = new DirectoryInfo(folderWS);
                foreach (FileInfo file in di.GetFiles())
                {
                    Directory.Move(folderWS + file, folderWSBK + file);
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch
            {}                  
            #endregion
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

        void UploadFile(string machineIP, string file)
        {
            try
            {
                //string From = @"E:\PDF\" + file + ".pdf";
                //string From = @"D:\Layout\" + file + ".pdf";
                string From = @"D:\Layout\" + file;
                //string To = machineIP + file + ".pdf";
                string To = machineIP + file;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(UserId, Password);
                    client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, From);
                }
            }
            catch
            {

            }
        }

        
        private void Form_ListFile_Load(object sender, EventArgs e)
        {
            
        }

        private void btDeleteAll_Click(object sender, EventArgs e)
        {
            P601.Instance().DeletePDF();
            //P604.Instance().DeletePDF();
            //P605.Instance().DeletePDF();
            //P5M.Instance().DeletePDF();

            BTD2.Instance().DeletePDF();
            BTD3.Instance().DeletePDF();
            BTD4.Instance().DeletePDF();
            BTD5.Instance().DeletePDF();

            D650.Instance().DeletePDF();
            D750.Instance().DeletePDF();
            D1000.Instance().DeletePDF();
            D1100.Instance().DeletePDF();

            //GMC1.Instance().DeletePDF();
            //GMC2.Instance().DeletePDF();
            //SCL.Instance().DeletePDF();

            MessageBox.Show("All Delete Success");
        }
    }
}
