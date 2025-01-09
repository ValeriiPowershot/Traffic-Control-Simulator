using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.ScoringSystem
{
    public class ScoringMaterials : MonoBehaviour
    {
        public MeshRenderer indicatorOfScore;

        public Material good;
        public Material neutral;
        public Material bad;
        
        public void SetNewMaterial(ScoreType scoreType)
        {
            switch (scoreType)
            {
                case ScoreType.Good:
                    indicatorOfScore.material = good;
                    break;
                case ScoreType.Neuteral:
                    indicatorOfScore.material = neutral;
                    break;
                case ScoreType.Bad:
                    indicatorOfScore.material = bad;
                    break;
            }
        }
    }
}
