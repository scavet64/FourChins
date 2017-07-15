using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FourChins
{
    class WalletController
    {

        public static string SendAwardToWallet(string walletAddress, double amount, string postNumber, string url)
        {
            string jsonToSend = "{\"method\": \"sendtoaddress\", \"params\":[\"" + walletAddress + "\"," + amount.ToString() + ",\"A tip for post #" + postNumber + ".\"]}";
            return HTTPUtil.PostJsonString(Properties.Settings.Default.WalletServerUsername, Properties.Settings.Default.WalletServerPassword, url, jsonToSend);
        }
    }
}
