using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using System.Text;

namespace Pinwheel.MeshToFile
{
    public static class NetUtils
    {
        private const string AFF_ID = "1100l3QbW";

        public static string ModURL(string url, string utmCampaign = "", string utmSource = "", string utmMedium = "")
        {
            const string PINWHEELSTUDIO = "pinwheelstud.io";
            const string ASSETSTOREUNITYCOM = "assetstore.unity.com";

            if (url.Contains(PINWHEELSTUDIO) ||
                url.Contains(ASSETSTOREUNITYCOM))
            {
                string queryString = "";
                int queryStart = url.IndexOf('?');
                if (queryStart > 0)
                {
                    queryString = url.Substring(queryStart).Remove(0, 1);
                    url = url.Remove(queryStart);
                }

                Dictionary<string, string> queries = new Dictionary<string, string>();
                ParseQuery(queryString, queries);

                if (url.Contains(PINWHEELSTUDIO))
                {
                    queries["utm_campaign"] = utmCampaign;
                    queries["utm_source"] = utmSource;
                    queries["utm_medium"] = utmMedium;
                }
                else if (url.Contains(ASSETSTOREUNITYCOM))
                {
                    queries["aid"] = AFF_ID;
                    queries["pubref"] = $"{utmCampaign}_{utmSource}_{utmMedium}";
                }

                url = CombinePathAndQuery(url, queries);
                return url;
            }
            else
            {
                return url;
            }
        }

        private static void ParseQuery(string queryString, Dictionary<string, string> pairs)
        {
            string[] elements = queryString.Split('=', '&');
            int numPair = elements.Length / 2;
            for (int i = 0; i < numPair; ++i)
            {
                string key = elements[i * 2 + 0];
                string value = elements[i * 2 + 1];
                pairs[key] = value;
            }
        }

        private static string CombinePathAndQuery(string url, Dictionary<string, string> queries)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(url).Append('?');
            foreach (var pair in queries)
            {
                sb.Append(pair.Key).Append('=').Append(pair.Value).Append('&');
            }
            return sb.ToString();
        }
    }
}
