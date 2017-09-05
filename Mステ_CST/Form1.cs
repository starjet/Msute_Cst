using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MsuteClasses;
using System.IO;

namespace Mステ_CST
{
    public partial class Form1 : Form
    {
        string p1 = "QofIGLgHeiX+uRLQ7MACPQJuvmq9GOad";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CSTtoCSV(Directory.GetCurrentDirectory());
            CSTtoAssetURLs(Directory.GetCurrentDirectory());
            CSTtoMasterURLs(Directory.GetCurrentDirectory());
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
            //Set baseURL; Changes with each new update
            string baseURL = "https://d2qkdj9w29igl.cloudfront.net/v1/2017090201-wLLcYQihf2XSdH8k8cr34HIbLZxFOn8I/";
            string[] assetCSTs = new string[] { "Audio.cst", "Sprite.cst", "System.cst" };
            string[] bundleCSTs = new string[] { "Card.cst", "Chara.cst", "Resources.cst", "Unit.cst" };
            foreach (string assetCST in assetCSTs)
            {
                try
                {
                    string urls = "";
                    string file = Path.Combine(folder, assetCST);
                    HpClientAssetInfo[] array = CSTtoClientAssetInfoArray(file);
                    foreach (HpClientAssetInfo clientAssetInfo in array)
                    {
                        urls += baseURL + "assets/" + clientAssetInfo.name + "." + clientAssetInfo.hash + "\r\n";
                    }
                    File.WriteAllText(file.Replace(".cst", "-urls.txt"), urls);
                }
                catch { }
            }
            foreach (string bundleCST in bundleCSTs)
            {
                try
                {
                    string urls = "";
                    string file = Path.Combine(folder, bundleCST);
                    HpClientAssetInfo[] array = CSTtoClientAssetInfoArray(file);
                    foreach (HpClientAssetInfo clientAssetInfo in array)
                    {
                        urls += baseURL + "asset_bundles/android/" + clientAssetInfo.name + "." + clientAssetInfo.hash + "\r\n";
                    }
                    File.WriteAllText(file.Replace(".cst", "-urls.txt"), urls);
                }
                catch { }
            }
        }

        private void CSTtoMasterURLs(string folder)
        {
            //Set baseURL; Changes with each new update
            string baseURL = "https://d2qkdj9w29igl.cloudfront.net/v1/2017090201-ZMKwlvnT3ZmlaEDPxvOUhdAqmMecVN9a/";
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
