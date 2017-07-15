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

        /// <summary>
        /// Send an award of coins to a wallet.
        /// </summary>
        /// <param name="walletAddress">Wallet to send coins to</param>
        /// <param name="amount">amount of coins</param>
        /// <param name="postNumber">post that is awarded</param>
        /// <param name="url">URL of bot's hosted wallet</param>
        /// <returns></returns>
        public static string SendAwardToWallet(string walletAddress, double amount, int postNumber, string url)
        {
            string jsonToSend = "{\"method\": \"sendtoaddress\", \"params\":[\"" + walletAddress + "\"," + amount.ToString() + ",\"A tip for post #" + postNumber.ToString() + ".\"]}";
            return HTTPUtil.PostJsonString(Properties.Settings.Default.WalletServerUsername, Properties.Settings.Default.WalletServerPassword, url, jsonToSend);
        }
    }
}
