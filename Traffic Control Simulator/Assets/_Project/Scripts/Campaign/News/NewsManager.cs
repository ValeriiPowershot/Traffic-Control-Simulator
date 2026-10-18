using UnityEngine;

namespace BaseCode.Logic.City
{
    public class NewsManager : MonoBehaviour
    {
        public string GetNews(int reputation)
        {
            if(reputation > 70)
                return "Citizens are happy with traffic.";

            if(reputation < 40)
                return "Citizens complain about traffic.";

            return "The city is stable.";
        }
    }
}
