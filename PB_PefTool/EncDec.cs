using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PB_PefTool
{
    public class EncDec
    {
        public static byte[] Decrypt(byte[] data, int shift)
        {
            if (data.Length > 0)
            {
                byte num = data[data.Length - 1];
                for (int index = data.Length - 1; index > 0; index--)
                {
                    data[index] = (byte)(((int)data[index - 1] & (int)byte.MaxValue) << 8 - shift | ((int)data[index] & (int)byte.MaxValue) >> shift);
                }
                data[0] = (byte)((int)num << 8 - shift | ((int)data[0] & (int)byte.MaxValue) >> shift);
                return data;
            }
            return data;
        }

        public static byte[] Encrypt(byte[] data, byte shift)
        {
            byte lastElement = data[data.Length - 1];
            for (int index = data.Length - 1; index > 0; index--)
            {
                data[index] = (byte)(((int)data[index - 1] & (int)byte.MaxValue) << 8 - shift | ((int)data[index] & (int)byte.MaxValue) >> shift);
            }
            data[0] = (byte)((int)lastElement << 8 - shift | ((int)data[0] & (int)byte.MaxValue) >> shift);

            //первый элемент в полученом массиве переносим в конец массива
            byte[] datatemp = new byte[data.Length];
            for (int i = 0; i < data.Length - 1; i++)
            {
                datatemp[i] = data[i + 1];
            }
            datatemp[data.Length - 1] = data[0];
            datatemp.CopyTo(data, 0);
            return data;
        }
    }
}
