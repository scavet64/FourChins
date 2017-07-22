using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FourChins
{
    class WalletController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            string response = null;

            try
            {
                response = HTTPUtil.PostJsonString(Properties.Settings.Default.WalletServerUsername, Properties.Settings.Default.WalletServerPassword, url, jsonToSend);
            }
            catch (WebException ex)
            {
                logger.Fatal(string.Format("Error sending award[{3}] to wallet[{0}] for post[{1}]: {2}", walletAddress, postNumber, ex.Message, amount), ex);
            }
            return response;
        }
    }
}
