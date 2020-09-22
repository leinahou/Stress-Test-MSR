using System;
using POSLink;
using DUKPTCore;
using System.Text;
using System.IO;

namespace ConsoleApp2
{
	class Program
	{
		static PosLink poslink;
        static StringBuilder sb;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter test rounds:");
            int iterr = Convert.ToInt32(Console.ReadLine());
            sb = new StringBuilder();

            for (int i = 0; i < iterr; i++) {
                try { 
                    poslink = new PosLink();
                    commset();
                    inputaccount();
                }
                catch (Exception e)
                {
                    sb.Append(e);
                    sb.AppendLine();
                    sb.AppendLine();
                }
                WorkThreadFunction();
            }
            File.AppendAllText(Environment.CurrentDirectory + DateTime.Now.ToString("MMddyyyy h.mmtt") + "Log.txt", sb.ToString());
            sb.Clear();
            Console.ReadLine();
		}

        public static void commset()
        {
            CommSetting commSetting = new CommSetting();
            commSetting.CommType = "TCP";
            commSetting.DestIP = "172.16.1.109";
            commSetting.DestPort = "10009";
            commSetting.TimeOut = "-1";
            poslink.CommSetting = commSetting;
        }

        public static void inputaccount()
        {
            ManageRequest manageRequest = new ManageRequest();
            manageRequest.TransType = manageRequest.ParseTransType("INPUTACCOUNTWITHEMV");
            manageRequest.EDCType = manageRequest.ParseEDCType("CREDIT");
            manageRequest.Trans = manageRequest.ParseTrans("SALE");
            manageRequest.Amount = "100";
            manageRequest.MagneticSwipeEntryFlag = "1";
            manageRequest.ManualEntryFlag = "1";
            manageRequest.ContactEMVEntryFlag = "1";
            manageRequest.ContactlessEntryFlag = "1";
            manageRequest.EncryptionFlag = "1";
            manageRequest.KeySlot = "1";
            manageRequest.Timeout = "300";
            poslink.ManageRequest = manageRequest;
        }

		public static void WorkThreadFunction()
		{
            string bdk = "0123456789ABCDEFFEDCBA9876543210";
            ProcessTransResult transResult = poslink.ProcessTrans();
            ManageResponse manageResponse = poslink.ManageResponse;
            Console.WriteLine("BDK: " + bdk);
            sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "BDK: " + bdk);
            sb.AppendLine();

            if (transResult.Code.Equals(ProcessTransResultCode.OK))
            {
                Console.WriteLine("From POSLink Encryption: ");
                sb.Append("From POSLink Encryption: ");
                sb.AppendLine();
                Console.WriteLine(manageResponse.ResultCode);
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "ProcessTransResult code: " + manageResponse.ResultCode);
                sb.AppendLine();
                Console.WriteLine(manageResponse.ResultTxt);
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "ProcessTransResult msg: " + manageResponse.ResultTxt);
                sb.AppendLine();
                Console.WriteLine("KSN: " + manageResponse.KSN);
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "KSN: " + manageResponse.KSN);
                sb.AppendLine();
                Console.WriteLine("PAN: " + manageResponse.PAN);
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "PAN: " + manageResponse.PAN);
                sb.AppendLine();
                Console.WriteLine("Masked PAN: " + manageResponse.MaskedPAN);
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "Masked PAN: " + manageResponse.MaskedPAN);
                sb.AppendLine();
                Console.WriteLine("From third party Decryption: ");
                sb.Append("From third party Decryption: ");
                sb.AppendLine();

                /*
                byte[] superSecretMessage = Encoding.UTF8.GetBytes("5147501000000018"); //hard coded test card PAN
                byte[] encryptedData = DUKPT.Encrypt(bdk, ksn, superSecretMessage, DUKPTVariant.Data);
                //Console.WriteLine(String.Join("",encryptedData));
                String hexres = BitConverter.ToString(encryptedData);
                hexres = hexres.Replace("-", "");
                Console.WriteLine("Encrypted Data: " + hexres);
                */

                string ksn = manageResponse.KSN;
                byte[] num = StringToByteArray(manageResponse.PAN);
                try { 
                    byte[] decryptedData = DUKPT.Decrypt(bdk, ksn, num, DUKPTVariant.Data);
                    string res = Encoding.UTF8.GetString(decryptedData);
                    Console.WriteLine("Decrypted Data: " + res + "\n");
                    sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "Decrypted Data: " + res + "\n");
                    sb.AppendLine();
                }
                catch (Exception e)
                {
                    sb.Append(e);
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }
            else
            {
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "ProcessTransResult code: " + manageResponse.ResultCode);
                sb.AppendLine();
                sb.Append("[" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + "]" + "ProcessTransResult msg: " + manageResponse.ResultTxt);
                sb.AppendLine();
            }
		}

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
