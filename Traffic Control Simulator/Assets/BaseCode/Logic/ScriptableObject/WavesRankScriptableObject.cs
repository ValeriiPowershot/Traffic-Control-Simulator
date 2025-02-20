using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseCode.Logic.ScriptableObject
{
    
    public enum WaveRank
    {
        Easy,
        Medium,
        Hard,
        Insane
    }
    
    [CreateAssetMenu(fileName = "WavesRankSo", menuName = "So/WavesRankSo", order = 0)]
    public class WavesRankScriptableObject : UnityEngine.ScriptableObject
    {
        public List<WaveWord> waveWords = new List<WaveWord>()
        {
            new() { waveRank = WaveRank.Easy, waveWord = new List<string> { "Easy", "You mom does better", "Piece of cake", "Warm-up round" } },
            new() { waveRank = WaveRank.Medium, waveWord = new List<string> { "Medium", "Getting there", "A bit tougher", "Not bad" } },
            new() { waveRank = WaveRank.Hard, waveWord = new List<string> { "Hard", "Challenge accepted", "Bring it on", "Show your skills" } },
            new() { waveRank = WaveRank.Insane, waveWord = new List<string> { "Insane", "Are you sure?", "Good luck with that", "Total madness" } }
        };
        public string GetWaveWord(WaveRank waveRank)
        {
            foreach (var waveWord in waveWords)
            {
                if(waveWord.waveRank == waveRank)
                    return waveWord.GetRandomWord();
            }
            return "";
        }
    }
    
    [Serializable]
    public class WaveWord
    {
        public List<string> waveWord = new List<string>();
        public WaveRank waveRank;

        public string GetRandomWord()
        {
            return waveWord[Random.Range(0, waveWord.Count)];
        }
    }
}