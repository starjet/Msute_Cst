using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Mステ_CST
{
    public partial class Form1 : Form
    {
        string p1 = "gwDdwaG3REda3mypwdgGZewisuzhGTFnGZcNciezG_Sa8rNn";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] pEncrypted = File.ReadAllBytes(@"LocalData\UserClass\CLIENT_DATA.json");
            p1 = HpCrypt.DecryptString(pEncrypted, p1).Replace("{\"password\":\"", "").Replace("\"}", "");
            if (Environment.GetCommandLineArgs().Contains("-diff"))
            {
                string[] old = File.ReadAllLines(Environment.GetCommandLineArgs()[2]);
                string[] new1 = File.ReadAllLines(Environment.GetCommandLineArgs()[3]);
                foreach (string s in new1)
                {
                    bool exists = false;
                    string temp = s.Split('/').Last();
                    foreach (string s2 in old)
                    {
                        string temp2 = s2.Split('/').Last();
                        if (temp2 == temp)
                        {
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        File.AppendAllText("diff.txt", s + "\r\n");
                    }
                }
                Environment.Exit(0);
            }
            if (Environment.GetCommandLineArgs().Contains("-diffmydesk"))
            {
                string[] old = File.ReadAllLines(Environment.GetCommandLineArgs()[2]);
                string[] new1 = File.ReadAllLines(Environment.GetCommandLineArgs()[3]);
                foreach (string s in new1)
                {
                    bool exists = false;
                    string temp = s.Split('/').Last();
                    foreach (string s2 in old)
                    {
                        string temp2 = s2.Split('/').Last();
                        if (temp2 == temp)
                        {
                            exists = true;
                        }
                    }
                    if (!exists && s.Contains("mydesk"))
                    {
                        File.AppendAllText("diffmydesk.txt", s + "\r\n");
                    }
                }
                Environment.Exit(0);
            }
            //CSTtoCSV(Directory.GetCurrentDirectory());
            CSTtoAssetURLs(Directory.GetCurrentDirectory());
            //CSTtoMasterURLs(Directory.GetCurrentDirectory());
            Environment.Exit(0);
        }

        private void CSTtoCSV(string folder)
        {
            foreach (string file in Directory.EnumerateFiles(folder, "*.cst"))
            {
                HpClientAssetInfo[] array = CSTtoClientAssetInfoArray(file);
                string CsvOut = "id,name,hash,filesize\r\n";
                foreach (HpClientAssetInfo clientAssetInfo in array)
                {
                    CsvOut += clientAssetInfo.id + "," + clientAssetInfo.name + "," + clientAssetInfo.hash + "," + clientAssetInfo.filesize + "\r\n";
                }
                File.WriteAllText(file.Replace(".cst", ".csv"), CsvOut);
            }
        }

        private HpClientAssetInfo[] CSTtoClientAssetInfoArray(string file)
        {
            byte[] CstIn = File.ReadAllBytes(file);
            byte[] CstDecrypted = HpCrypt.DecryptBytes(CstIn, p1);
            Dictionary<long, byte[]> dictionary = HpMasterCluster.Decode_ToMasterBins(CstDecrypted, false);
            List<HpClientAssetInfo> list = new List<HpClientAssetInfo>();
            foreach (KeyValuePair<long, byte[]> current in dictionary)
            {
                list.Add(HpMasterBin.Decode<HpClientAssetInfo>(current.Value, false));
            }
            return list.ToArray();
        }

        private void CSTtoAssetURLs(string folder)
        {
            string baseURL = File.ReadAllLines("baseurl.txt")[0].Split('#')[0].Trim();

            string[] assetCSTs = new string[] { "Audio.cst", "Sprite.cst", "System.cst" };
            string[] bundleCSTs = new string[] { "Card.cst", "Chara.cst", "Resources.cst", "Unit.cst" };
            foreach (string assetCST in assetCSTs)
            {
                try
                {
                    new System.Net.WebClient().DownloadFile(baseURL + "assets/" + assetCST, assetCST);
                    string urls = "";
                    string file = Path.Combine(folder, assetCST);
                    HpClientAssetInfo[] array = CSTtoClientAssetInfoArray(file);
                    foreach (HpClientAssetInfo clientAssetInfo in array)
                    {
                        urls += baseURL + "assets/" + clientAssetInfo.name + "." + clientAssetInfo.hash + "\r\n";
                    }
                    File.WriteAllText(file.Replace(".cst", "-urls.txt"), urls);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            foreach (string bundleCST in bundleCSTs)
            {
                try
                {
                    new System.Net.WebClient().DownloadFile(baseURL + "asset_bundles/android/" + bundleCST, bundleCST);
                    string urls = "";
                    string file = Path.Combine(folder, bundleCST);
                    HpClientAssetInfo[] array = CSTtoClientAssetInfoArray(file);
                    foreach (HpClientAssetInfo clientAssetInfo in array)
                    {
                        urls += baseURL + "asset_bundles/android/" + clientAssetInfo.name + "." + clientAssetInfo.hash + "\r\n";
                    }
                    File.WriteAllText(file.Replace(".cst", "-urls.txt"), urls);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }

        private void CSTtoMasterURLs(string folder)
        {
            //Set baseURL; Changes with each new update
            string baseURL = "https://d2qkdj9w29igl.cloudfront.net/v1/2017092401-tiFSGnPog5JoGU3ZTGFUdVlansubWyAh/";
            string[] masterCSTs = new string[] { "csts.cst" };
            foreach (string masterCST in masterCSTs)
            {
                try
                {
                    string urls = "";
                    string file = Path.Combine(folder, masterCST);
                    HpClientAssetInfo[] array = CSTtoClientAssetInfoArray(file);
                    foreach (HpClientAssetInfo clientAssetInfo in array)
                    {
                        urls += baseURL + "masters/" + clientAssetInfo.name + "." + clientAssetInfo.hash + "\r\n";
                    }
                    File.WriteAllText(file.Replace(".cst", "-urls.txt"), urls);
                }
                catch { }
            }
        }
    }
}
