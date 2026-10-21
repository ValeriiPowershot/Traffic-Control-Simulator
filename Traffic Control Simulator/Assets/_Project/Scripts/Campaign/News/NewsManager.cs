using UnityEngine;

namespace BaseCode.Logic.City
{
    public class NewsManager : MonoBehaviour
    {
        public string GetNews(int reputation)
        {
            if(reputation > 70)
                return "Жители довольны движением в городе.";

            if(reputation < 40)
                return "Горожане жалуются на движение и ждут улучшений.";

            return "Город оценивает работу мэра спокойно.";
        }
    }
}
