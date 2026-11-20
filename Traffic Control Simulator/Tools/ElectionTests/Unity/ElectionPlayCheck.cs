// Temporary-project integration check. Run ElectionPlayCheck.Begin in the editor.
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TrafficControl.Elections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#endif
public sealed class ElectionPlayCheck : MonoBehaviour
{
    private int errors;
    private static string Output=>Path.Combine(Application.temporaryCachePath,"ElectionUI");
    public static void Begin()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/_Project/Sandbox/Scenes/UI/ElectionDemo.unity");
        new GameObject("Election UI verification").AddComponent<ElectionPlayCheck>();
        UnityEditor.EditorApplication.isPlaying=true;
    }
    private void OnEnable(){Application.logMessageReceived+=OnLog;}
    private void OnDisable(){Application.logMessageReceived-=OnLog;}
    private void OnLog(string message,string stack,LogType type){if((type==LogType.Error || type==LogType.Exception) && !stack.Contains("UnityEditor.Search"))errors++;}
    private T Field<T>(ElectionDemo demo,string name)=> (T)typeof(ElectionDemo).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(demo);
    private void Click(ElectionDemo demo,string name)
    {
        var button=Field<Button>(demo,name);
        ExecuteEvents.Execute(button.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);
    }
    private IEnumerator Start()
    {
        yield return null;
        var demo=FindObjectOfType<ElectionDemo>();
        if(!Check(demo && demo.Session.Phase==ElectionPhase.Preparation,"Saved scene initializes independent election"))yield break;
        Click(demo,"mayorPlus");Click(demo,"opponentMinus");
        if(!Check(demo.Session.MayorVotes==4300 && demo.Session.OpponentVotes==3700,"Mouse buttons update displayed vote allocation"))yield break;
#if ENABLE_INPUT_SYSTEM
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        var keyboard=InputSystem.AddDevice<Keyboard>("ElectionTestKeyboard"); InputSystem.EnableDevice(keyboard); keyboard.MakeCurrent();
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.UpArrow));yield return new WaitForSecondsRealtime(0.05f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        if(!Check(demo.Session.MayorVotes==4400,"Up-arrow input adds 100 mayor votes; actual="+demo.Session.MayorVotes))yield break;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.LeftShift,Key.DownArrow));yield return new WaitForSecondsRealtime(0.05f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        if(!Check(demo.Session.MayorVotes==3900,"Shift-down input subtracts 500 mayor votes"))yield break;
#endif
        demo.ResetElection();
        yield return null; Capture(demo,"election-preparation.png");
        Click(demo,"winPreset");Click(demo,"primary");
        if(!Check(demo.Session.Phase==ElectionPhase.Counting && !Field<Button>(demo,"mayorPlus").interactable,"Start count freezes vote editing"))yield break;
        demo.ChangeMayor(-6000);Time.timeScale=0;
        yield return new WaitForSecondsRealtime(1.5f);
        if(!Check(demo.CountProgress>0 && demo.CountProgress<1 && demo.Session.MayorVotes==6000,"Count animates while game time is paused; snapshot is frozen"))yield break;
        yield return null;Capture(demo,"election-counting.png");
        yield return new WaitForSecondsRealtime(2.2f);
        if(!Check(demo.Session.Phase==ElectionPhase.Results && demo.Session.Result.Outcome==ElectionOutcome.MayorWon,"Automatic count reaches mayor victory"))yield break;
        yield return null;Capture(demo,"election-victory.png");
        Click(demo,"primary");Click(demo,"losePreset");Click(demo,"primary");demo.FinishElection();
        if(!Check(demo.Session.Result.Outcome==ElectionOutcome.OpponentWon && Field<Text>(demo,"resultHeading").text.Contains("соперник"),"Defeat has its own result screen"))yield break;
        yield return null;Capture(demo,"election-defeat.png");
        demo.ResetElection();Click(demo,"tiePreset");demo.BeginElection();demo.FinishElection();
        if(!Check(demo.Session.Result.Outcome==ElectionOutcome.Tie && Field<Text>(demo,"verdictBadge").text.Contains("ПОВТОРНОЕ"),"Tie requests a new ballot"))yield break;
        demo.ResetElection();Click(demo,"emptyPreset");demo.BeginElection();demo.FinishElection();
        if(!Check(demo.Session.Result.Outcome==ElectionOutcome.NoVotes,"Empty election does not fabricate a winner"))yield break;
#if ENABLE_INPUT_SYSTEM
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.R));yield return new WaitForSecondsRealtime(0.05f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        if(!Check(demo.Session.Phase==ElectionPhase.Preparation,"R restarts from final results"))yield break;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));yield return new WaitForSecondsRealtime(0.05f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        if(!Check(demo.Session.Phase==ElectionPhase.Counting,"Enter begins final election"))yield break;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));yield return new WaitForSecondsRealtime(0.05f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        if(!Check(demo.Session.Phase==ElectionPhase.Results,"Space skips the count animation"))yield break;
#endif
        // The same layout must fit a small 4:3 viewport without cropping.
        Capture(demo,"election-compact.png",1024,768);
        Time.timeScale=1;
        if(!Check(errors==0,"No runtime errors in standalone election UI"))yield break;
        Debug.Log("ELECTION_PLAY_PASS: saved UI; buttons; keyboard arrows/shift/enter/space/R; all outcomes; unscaled animation; responsive captures.");
        UnityEditor.EditorApplication.Exit(0);
    }
    private static void Capture(ElectionDemo demo,string name,int width=1600,int height=900)
    {
        var canvas=demo.GetComponentInChildren<Canvas>();var camera=FindObjectOfType<Camera>();
        var previousMode=canvas.renderMode;var previousCamera=canvas.worldCamera;
        var target=new RenderTexture(width,height,24);camera.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;
        canvas.scaleFactor=Mathf.Min(width/1600f,height/900f);
        foreach(var label in canvas.GetComponentsInChildren<Text>()) label.SetAllDirty();
        Canvas.ForceUpdateCanvases();camera.Render();RenderTexture previous=RenderTexture.active;RenderTexture.active=target;
        var texture=new Texture2D(width,height,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();
        Directory.CreateDirectory(Output); File.WriteAllBytes(Path.Combine(Output,name),texture.EncodeToPNG());
        RenderTexture.active=previous;camera.targetTexture=null;canvas.renderMode=previousMode;canvas.worldCamera=previousCamera;
        Destroy(texture);target.Release();Destroy(target);
    }
    private static bool Check(bool condition,string message)
    {
        if(condition){Debug.Log("PASS "+message);return true;}
        Debug.LogError("ELECTION_PLAY_FAIL "+message);Time.timeScale=1;UnityEditor.EditorApplication.Exit(1);return false;
    }
}