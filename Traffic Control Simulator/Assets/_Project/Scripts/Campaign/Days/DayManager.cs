using System;
using UnityEngine;

namespace BaseCode.Logic.City
{
    public class DayManager : MonoBehaviour
    {
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private ReputationSystem reputationSystem;
        [SerializeField] private TrafficGameSettings settings;
        private int sessionDay;
        private bool complete;
        public string LastError { get; private set; }
        public void Configure(SaveSystem save, TrafficGameSettings config)
        { saveSystem = save; settings = config; sessionDay = save.Data.CurrentDay; complete = false; }
        public bool TryFinishDay(CityStatistics statistics)
        {
            LastError = null;
            if (complete) { LastError = "Результат этой смены уже сохранён."; return false; }
            if (saveSystem == null || saveSystem.Data == null || settings == null || sessionDay == 0)
            { LastError = "Системы дня не инициализированы."; return false; }
            try
            {
                var candidate = CampaignRules.Complete(saveSystem.Data, statistics, sessionDay, settings.CampaignLengthDays,
                    settings.ElectionThreshold, settings.ScorePenaltyPerAccident, settings.ScorePerReputationPoint, settings.ReputationPenaltyPerAccident);
                if (!saveSystem.TryCommit(candidate)) { LastError = saveSystem.LastError; return false; }
                complete = true; return true;
            }
            catch (Exception exception) when (exception is InvalidOperationException || exception is ArgumentException)
            { LastError = exception.Message; return false; }
        }
        public void FinishDay(CityStatistics statistics)
        { if (!TryFinishDay(statistics)) Debug.LogWarning(LastError); }
    }
}
