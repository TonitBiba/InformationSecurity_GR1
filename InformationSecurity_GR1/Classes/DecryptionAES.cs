using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InformationSecurity_GR1.Classes
{
    public class DecryptionAES:MainWindow
    {
        private RijndaelManaged rijndaelManaged;
        private List<byte[]> listOfFilteredKeysByte;
        private List<string> keysIndex;

        public List<string> listOfKeys { get; set; }

        public string CiperText { get; set; }

        public DecryptionAES(List<string> listOfKeys, string CiperText)
        {
            this.listOfKeys = listOfKeys;
            this.CiperText = CiperText;
            keysIndex = new List<string> { "K0", "K1", "K2", "K3", "K4", "K5", "K6", "K7", "K8", "K9", "K10", "K11", "K12", "K13"};

            rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.ECB;
            rijndaelManaged.GenerateIV();
            rijndaelManaged.KeySize = 128;
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.Padding = PaddingMode.None;
        }

        public List<byte[]> GetFilteredKeys()
        {
            List<byte[]> keys = listOfKeys.Select(k => ConvertHexToByte(k)).ToList();
            listOfFilteredKeysByte = keys.Where(
                K => K[2] > Convert.ToInt32("DD", 16) &&
                     K[5] > K[13] &&
                     K[10] < Convert.ToInt32("1E", 16)
                ).ToList();
            return listOfFilteredKeysByte;
        }

        public List<byte[]> GetFilteredKeys(List<StatisticsCondition> statisticsConditions)
        {
            List<byte[]> keys = listOfKeys.Select(k => ConvertHexToByte(k)).ToList();

            foreach (var condition in statisticsConditions)
            {
                if (condition.sign == 1)
                    listOfFilteredKeysByte = keys.Where(K =>
                        K[keysIndex.IndexOf(condition.condition[0])] > 
                            (keysIndex.Where(ki => ki.Contains(condition.condition[1])).Any() == true ?
                            K[keysIndex.IndexOf(condition.condition[1])] : 
                            Convert.ToInt32(condition.condition[1], 16))
                    ).ToList();
                else if(condition.sign == 2)
                    listOfFilteredKeysByte = keys.Where(K =>
                        K[keysIndex.IndexOf(condition.condition[0])] <
                            (keysIndex.Where(ki => ki.Contains(condition.condition[1])).Any() == true ?
                            K[keysIndex.IndexOf(condition.condition[1])] :
                            Convert.ToInt32(condition.condition[1], 16))
                    ).ToList();
                keys = listOfFilteredKeysByte;
            }
            return listOfFilteredKeysByte;
        }

        public ResultOfDecryption resultOfDecryption(ListBox lstGeneratedKey)
        {
            string plainText = null;
            byte[] finalKey = new byte[16];
            try
            {
                //Thread thread = new Thread(() => {
                foreach (var key in listOfFilteredKeysByte)
                {
                    for (int i = 0; i <= 65535; i++)
                    {
                        finalKey = key.Concat(BitConverter.GetBytes(i)).Take(16).ToArray();
                            Dispatcher.Invoke(() =>
                            {
                                lstGeneratedKey.Items.Add(ByteArrayToString(finalKey));
                            });
                        plainText = Decryption(ConvertHexToByte(CiperText), finalKey);
                        if (plainText != null)
                            if (plainText.Contains("KODI"))
                                return new ResultOfDecryption
                                {
                                    Key = ByteArrayToString(finalKey),
                                    PlainText = plainText
                                };
                    }
                }
                //});
                //thread.Priority = ThreadPriority.Highest;
                //thread.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
            return null;
        }

        private string Decryption(byte[] ciperText, byte[] key)
        {
            string plaintText = null;
            rijndaelManaged.Key = key;

            using(ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream(ciperText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            try
                            {
                                plaintText = streamReader.ReadToEnd();
                                    return plaintText;
                            }
                            catch
                            {
                                plaintText = null;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private byte[] ConvertHexToByte(string hexadecimal)
        {
            int bytesLength = (hexadecimal.Length)/2;
            byte[] hexBytes = new byte[bytesLength];
            for (int i = 0; i < bytesLength; i++)
                hexBytes[i] = Convert.ToByte(hexadecimal.Substring(i*2, 2), 16);
            return hexBytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }
    }
}
