using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MsuteClasses
{
    public class HpMasterBin
    {
        public enum eDATATYPE
        {
            INT,
            FLOAT,
            STRING,
            BOOL,
            LONG
        }

        public const int c_NULLVALUE_INT = 0;

        public const float c_NULLVALUE_FLOAT = 0f;

        public const string c_NULLVALUE_STRING = "";

        public const bool c_NULLVALUE_BOOL = false;

        public const long c_NULLVALUE_LONG = 0L;

        public static string cEXTENSION_FORMAT = ".mst";

        public static float ByteToFloat(byte[] pFloat32, bool isLittleEndian)
        {
            if (pFloat32 == null)
            {
                return 0f;
            }
            if (isLittleEndian)
            {
                Array.Reverse(pFloat32, 0, 4);
            }
            return BitConverter.ToSingle(pFloat32, 0);
        }

        public static int ByteToInt(byte[] pInt32, bool isLittleEndian)
        {
            if (pInt32 == null)
            {
                return 0;
            }
            if (isLittleEndian)
            {
                Array.Reverse(pInt32, 0, 4);
            }
            return BitConverter.ToInt32(pInt32, 0);
        }

        public static string ByteToString(byte[] pData)
        {
            if (pData == null)
            {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(pData);
        }

        public static bool ByteToBool(byte[] pBool, bool isLittleEndian)
        {
            if (pBool == null)
            {
                return false;
            }
            if (isLittleEndian)
            {
                Array.Reverse(pBool, 0, 4);
            }
            return BitConverter.ToBoolean(pBool, 0);
        }

        public static long ByteToLong(byte[] pLong, bool isLittleEndian)
        {
            if (pLong == null)
            {
                return 0L;
            }
            if (isLittleEndian)
            {
                Array.Reverse(pLong, 0, 8);
            }
            return BitConverter.ToInt64(pLong, 0);
        }

        public static int ReadByte(byte[] pBinary, int sIndexNow, int sReadLength, out byte[] pReaded)
        {
            if (sReadLength > 0)
            {
                pReaded = new byte[sReadLength];
                Array.Copy(pBinary, sIndexNow, pReaded, 0, sReadLength);
            }
            else
            {
                pReaded = null;
            }
            return sIndexNow + sReadLength;
        }

        public static byte[] FloatToByte(float val)
        {
            return BitConverter.GetBytes(val);
        }

        public static byte[] IntToByte(int val)
        {
            return BitConverter.GetBytes(val);
        }

        public static byte[] BoolToByte(bool val)
        {
            return BitConverter.GetBytes(val);
        }

        public static byte[] StringToByte(string val)
        {
            return Encoding.UTF8.GetBytes(val);
        }

        public static byte[] LongToByte(long val)
        {
            return BitConverter.GetBytes(val);
        }

        public static byte[] CombineByte(byte[] pOriginal, byte[] pAdd)
        {
            byte[] array = new byte[pOriginal.Length + pAdd.Length];
            Array.Copy(pOriginal, array, pOriginal.Length);
            Array.Copy(pAdd, 0, array, pOriginal.Length, pAdd.Length);
            return array;
        }

        public static T Decode<T>(string pFilePath, bool isLittleEndian = false)
        {
            byte[] pBinary = File.ReadAllBytes(pFilePath);
            return HpMasterBin.Decode<T>(pBinary, isLittleEndian);
        }

        public static T Decode<T>(byte[] pBinary, bool isLittleEndian = false)
        {
            T t = Activator.CreateInstance<T>();
            int num = 0;
            byte[] array = null;
            if (num < pBinary.Length)
            {
                num = HpMasterBin.ReadByte(pBinary, num, 4, out array);
                int num2 = HpMasterBin.ByteToInt(array, isLittleEndian);
                List<int> list = new List<int>();
                for (int i = 0; i < num2; i++)
                {
                    num = HpMasterBin.ReadByte(pBinary, num, 4, out array);
                    list.Add(HpMasterBin.ByteToInt(array, isLittleEndian));
                }
                List<string> list2 = new List<string>();
                for (int j = 0; j < num2; j++)
                {
                    num = HpMasterBin.ReadByte(pBinary, num, 4, out array);
                    int sReadLength = HpMasterBin.ByteToInt(array, isLittleEndian);
                    num = HpMasterBin.ReadByte(pBinary, num, sReadLength, out array);
                    list2.Add(HpMasterBin.ByteToString(array));
                }
                List<object> list3 = new List<object>();
                for (int k = 0; k < num2; k++)
                {
                    num = HpMasterBin.ReadByte(pBinary, num, 4, out array);
                    int sReadLength2 = HpMasterBin.ByteToInt(array, isLittleEndian);
                    num = HpMasterBin.ReadByte(pBinary, num, sReadLength2, out array);
                    if (list[k] == 1)
                    {
                        list3.Add(HpMasterBin.ByteToFloat(array, isLittleEndian));
                    }
                    else if (list[k] == 0)
                    {
                        list3.Add(HpMasterBin.ByteToInt(array, isLittleEndian));
                    }
                    else if (list[k] == 2)
                    {
                        list3.Add(HpMasterBin.ByteToString(array));
                    }
                    else if (list[k] == 3)
                    {
                        list3.Add(HpMasterBin.ByteToBool(array, isLittleEndian));
                    }
                    else if (list[k] == 4)
                    {
                        list3.Add(HpMasterBin.ByteToLong(array, isLittleEndian));
                    }
                }
                Type typeFromHandle = typeof(T);
                FieldInfo[] fields = typeFromHandle.GetFields();
                for (int l = 0; l < list2.Count; l++)
                {
                    for (int m = 0; m < fields.Length; m++)
                    {
                        if (list2[l] == fields[m].Name)
                        {
                            fields[m].SetValue(t, list3[l]);
                            break;
                        }
                    }
                }
                list.Clear();
                list2.Clear();
                list3.Clear();
            }
            return t;
        }

        public static byte[] Encode<T>(T pMasterClass)
        {
            Type typeFromHandle = typeof(T);
            FieldInfo[] fields = typeFromHandle.GetFields();
            List<byte[]> list = new List<byte[]>();
            List<byte[]> list2 = new List<byte[]>();
            List<byte[]> list3 = new List<byte[]>();
            if (fields.Length <= 0)
            {
                return null;
            }
            for (int i = 0; i < fields.Length; i++)
            {
                bool flag = false;
                if (fields[i].FieldType == typeof(float))
                {
                    list.Add(HpMasterBin.IntToByte(1));
                    flag = true;
                    list3.Add(HpMasterBin.FloatToByte((float)fields[i].GetValue(pMasterClass)));
                }
                else if (fields[i].FieldType == typeof(int))
                {
                    list.Add(HpMasterBin.IntToByte(0));
                    flag = true;
                    list3.Add(HpMasterBin.IntToByte((int)fields[i].GetValue(pMasterClass)));
                }
                else if (fields[i].FieldType == typeof(string))
                {
                    list.Add(HpMasterBin.IntToByte(2));
                    flag = true;
                    list3.Add(HpMasterBin.StringToByte((string)fields[i].GetValue(pMasterClass)));
                }
                else if (fields[i].FieldType == typeof(bool))
                {
                    list.Add(HpMasterBin.IntToByte(3));
                    flag = true;
                    list3.Add(HpMasterBin.BoolToByte((bool)fields[i].GetValue(pMasterClass)));
                }
                else if (fields[i].FieldType == typeof(long))
                {
                    list.Add(HpMasterBin.IntToByte(4));
                    flag = true;
                    list3.Add(HpMasterBin.LongToByte((long)fields[i].GetValue(pMasterClass)));
                }
                if (flag)
                {
                    list2.Add(HpMasterBin.StringToByte(fields[i].Name));
                }
            }
            byte[] array = HpMasterBin.IntToByte(list3.Count);
            byte[] array2 = array;
            for (int j = 0; j < list.Count; j++)
            {
                array2 = HpMasterBin.CombineByte(array2, list[j]);
            }
            for (int k = 0; k < list2.Count; k++)
            {
                byte[] pAdd = HpMasterBin.IntToByte(list2[k].Length);
                array2 = HpMasterBin.CombineByte(array2, pAdd);
                array2 = HpMasterBin.CombineByte(array2, list2[k]);
            }
            for (int l = 0; l < list3.Count; l++)
            {
                byte[] pAdd2 = HpMasterBin.IntToByte(list3[l].Length);
                array2 = HpMasterBin.CombineByte(array2, pAdd2);
                array2 = HpMasterBin.CombineByte(array2, list3[l]);
            }
            list.Clear();
            list2.Clear();
            list3.Clear();
            return array2;
        }
    }
}