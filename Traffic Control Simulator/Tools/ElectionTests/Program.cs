using System;
using TrafficControl.Elections;
static class Program
{
    static int passed;
    static void Check(bool condition,string name){if(!condition)throw new Exception(name);passed++;Console.WriteLine("PASS "+name);}
    static void Main()
    {
        var s=new ElectionSession();
        Check(s.MayorVotes==4200 && s.OpponentVotes==3800 && s.Unassigned==2000,"Default electorate balances");
        s.AdjustMayor(100);Check(s.MayorVotes==4300 && s.Unassigned==1900,"Add mayor votes from unassigned pool");
        s.AdjustOpponent(50000);Check(s.OpponentVotes==5700 && s.Unassigned==0,"Opponent cannot exceed remaining electorate");
        s.AdjustMayor(int.MaxValue);Check(s.MayorVotes==4300,"Full electorate cannot be overallocated");
        s.AdjustOpponent(int.MinValue);Check(s.OpponentVotes==0,"Vote removal clamps at zero without overflow");
        s.SetVotes(-10,int.MaxValue);Check(s.MayorVotes==0 && s.OpponentVotes==10000,"Invalid presets stay inside electorate");
        Check(!s.CompleteCount(),"Cannot complete a count that never began");
        s.SetVotes(6000,3000);Check(s.BeginCount(),"Begin count");var frozen=s.Result;
        Check(frozen.Outcome==ElectionOutcome.MayorWon && frozen.CastVotes==9000 && frozen.Abstained==1000,"Winning result and turnout snapshot");
        Check(Math.Abs(frozen.MayorShare-2d/3)<0.00001,"Share uses cast votes, not total electorate");
        Check(!s.AdjustMayor(-6000) && !s.AdjustOpponent(100) && !s.SetVotes(0,0),"Freeze vote editing during count");
        Check(!s.BeginCount(),"Cannot restart an active count");
        Check(s.CompleteCount() && s.Phase==ElectionPhase.Results,"Publish final result");
        Check(!s.CompleteCount() && !s.AdjustMayor(100),"Final result is stable");
        s.Reset();Check(s.Phase==ElectionPhase.Preparation && s.Result==null && s.MayorVotes==4200,"Reset returns to editable preparation");
        Check(frozen.MayorVotes==6000,"Reset does not mutate prior result snapshot");
        s.SetVotes(3200,5800);s.BeginCount();Check(s.Result.Outcome==ElectionOutcome.OpponentWon,"Defeat");
        s.Reset();s.SetVotes(4500,4500);s.BeginCount();Check(s.Result.Outcome==ElectionOutcome.Tie,"Tie is not a mayor victory");
        s.Reset();s.SetVotes(0,0);s.BeginCount();Check(s.Result.Outcome==ElectionOutcome.NoVotes && s.Result.MayorShare==0,"Empty election has no winner or division by zero");
        s.Reset();s.SetVotes(5001,4999);s.BeginCount();Check(s.Result.Outcome==ElectionOutcome.MayorWon,"One-vote majority determines winner");
        var large=new ElectionSession(int.MaxValue);Check(large.MayorVotes>0 && large.Unassigned>=0,"Large electorate initializes without integer overflow");
        bool rejected=false;try{new ElectionSession(0);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"Reject zero electorate");
        Console.WriteLine(passed+" election checks passed");
    }
}
