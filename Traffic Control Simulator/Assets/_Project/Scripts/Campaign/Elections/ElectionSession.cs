using System;

namespace TrafficControl.Elections
{
    public enum ElectionPhase { Preparation, Counting, Results }
    public enum ElectionOutcome { MayorWon, OpponentWon, Tie, NoVotes }

    public sealed class ElectionResult
    {
        public int Electorate { get; }
        public int MayorVotes { get; }
        public int OpponentVotes { get; }
        public int CastVotes => MayorVotes + OpponentVotes;
        public int Abstained => Electorate - CastVotes;
        public double MayorShare => CastVotes == 0 ? 0 : (double)MayorVotes / CastVotes;
        public double Turnout => (double)CastVotes / Electorate;
        public ElectionOutcome Outcome { get; }
        internal ElectionResult(int electorate, int mayor, int opponent)
        {
            Electorate = electorate; MayorVotes = mayor; OpponentVotes = opponent;
            Outcome = CastVotes == 0 ? ElectionOutcome.NoVotes : mayor == opponent ? ElectionOutcome.Tie :
                mayor > opponent ? ElectionOutcome.MayorWon : ElectionOutcome.OpponentWon;
        }
    }

    // Independent test election. Votes are not the campaign's reputation score.
    public sealed class ElectionSession
    {
        public int Electorate { get; }
        public int MayorVotes { get; private set; }
        public int OpponentVotes { get; private set; }
        public int Unassigned => Electorate - MayorVotes - OpponentVotes;
        public ElectionPhase Phase { get; private set; }
        public ElectionResult Result { get; private set; }
        public ElectionSession(int electorate = 10000)
        { if (electorate < 1) throw new ArgumentOutOfRangeException(nameof(electorate)); Electorate = electorate; Reset(); }
        public void Reset()
        { Phase = ElectionPhase.Preparation; Result = null; SetVotes((int)((long)Electorate * 42 / 100), (int)((long)Electorate * 38 / 100)); }
        public bool SetVotes(int mayor, int opponent)
        {
            if (Phase != ElectionPhase.Preparation) return false;
            MayorVotes = Clamp(mayor, Electorate);
            OpponentVotes = Clamp(opponent, Electorate - MayorVotes);
            return true;
        }
        public bool AdjustMayor(int delta)
        {
            if (Phase != ElectionPhase.Preparation) return false;
            MayorVotes = Clamp((long)MayorVotes + delta, Electorate - OpponentVotes); return true;
        }
        public bool AdjustOpponent(int delta)
        {
            if (Phase != ElectionPhase.Preparation) return false;
            OpponentVotes = Clamp((long)OpponentVotes + delta, Electorate - MayorVotes); return true;
        }
        public bool BeginCount()
        {
            if (Phase != ElectionPhase.Preparation) return false;
            Result = new ElectionResult(Electorate, MayorVotes, OpponentVotes);
            Phase = ElectionPhase.Counting; return true;
        }
        public bool CompleteCount()
        { if (Phase != ElectionPhase.Counting) return false; Phase = ElectionPhase.Results; return true; }
        private static int Clamp(long value, int upper) => (int)Math.Max(0L, Math.Min(upper, value));
    }
}
