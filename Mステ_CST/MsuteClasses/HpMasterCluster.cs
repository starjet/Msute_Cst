using System;
using System.Collections.Generic;
using System.IO;

namespace MsuteClasses
{
    public class HpMasterCluster
    {
        public static string cEXTENSION_FORMAT = ".cst";

        public static byte[] Encode_ToMasterCluster<T>(Dictionary<long, T> pMasterDatas) where T : HpMasterData
        {
            List<byte[]> list = new List<byte[]>();
            List<long> list2 = new List<long>();
            foreach (KeyValuePair<long, T> current in pMasterDatas)
            {
                if (current.Value != null)
                {
                    list.Add(HpMasterBin.Encode<T>(current.Value));
                    list2.Add(current.Key);
                }
            }
            byte[] array = new byte[8];
            byte[] array2 = HpMasterBin.LongToByte((long)list.Count);
            array = array2;
            for (int i = 0; i < list.Count; i++)
            {
                array2 = HpMasterBin.LongToByte(list2[i]);
                array = HpMasterBin.CombineByte(array, array2);
                array2 = HpMasterBin.IntToByte(list[i].Length);
                array = HpMasterBin.CombineByte(array, array2);
                array = HpMasterBin.CombineByte(array, list[i]);
            }
            return array;
        }

        public static Dictionary<long, byte[]> Decode_ToMasterBins(byte[] pMasterClusterData, bool isLittleEndian = false)
        {
            Dictionary<long, byte[]> dictionary = new Dictionary<long, byte[]>();
            int num = 0;
            if (num < pMasterClusterData.Length)
            {
                long num2 = HpMasterCluster.Decode_MstBinNum(pMasterClusterData, ref num, isLittleEndian);
                for (long num3 = 0L; num3 < num2; num3 += 1L)
                {
                    long key = HpMasterCluster.Decode_MstBinID(pMasterClusterData, ref num, isLittleEndian);
                    byte[] value = HpMasterCluster.Decode_MstBinData(pMasterClusterData, ref num, isLittleEndian);
                    dictionary[key] = value;
                }
            }
            return dictionary;
        }

        public static Dictionary<long, byte[]> Decode_ToMasterBins(string pFilePath, bool isLittleEndian = false)
        {
            if (File.Exists(pFilePath))
            {
                byte[] pMasterClusterData = File.ReadAllBytes(pFilePath);
                return HpMasterCluster.Decode_ToMasterBins(pMasterClusterData, isLittleEndian);
            }
            return null;
        }

        public static long Decode_MstBinNum(byte[] pMasterClusterData, ref int sIndex, bool bIsLittleEndian = false)
        {
            byte[] pLong = null;
            sIndex = HpMasterBin.ReadByte(pMasterClusterData, sIndex, 8, out pLong);
            return HpMasterBin.ByteToLong(pLong, bIsLittleEndian);
        }

        public static long Decode_MstBinID(byte[] pMasterClusterData, ref int sIndex, bool bIsLittleEndian = false)
        {
            byte[] pLong = null;
            sIndex = HpMasterBin.ReadByte(pMasterClusterData, sIndex, 8, out pLong);
            return HpMasterBin.ByteToLong(pLong, bIsLittleEndian);
        }

        public static byte[] Decode_MstBinData(byte[] pMasterClusterData, ref int sIndex, bool bIsLittleEndian = false)
        {
            byte[] pInt = null;
            sIndex = HpMasterBin.ReadByte(pMasterClusterData, sIndex, 4, out pInt);
            int sReadLength = HpMasterBin.ByteToInt(pInt, bIsLittleEndian);
            byte[] result = null;
            sIndex = HpMasterBin.ReadByte(pMasterClusterData, sIndex, sReadLength, out result);
            return result;
        }
    }
}