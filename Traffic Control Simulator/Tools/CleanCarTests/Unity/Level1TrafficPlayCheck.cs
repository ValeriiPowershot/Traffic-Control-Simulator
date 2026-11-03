using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficControl.CleanVehicles;
public sealed class Level1TrafficPlayCheck : MonoBehaviour
{
    public static void Begin()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/_Project/Sandbox/Scenes/ControllerTests/Level1CleanTraffic.unity");
        new GameObject("Level 1 play check").AddComponent<Level1TrafficPlayCheck>();
        UnityEditor.EditorApplication.isPlaying = true;
    }
    private IEnumerator Start()
    {
        yield return null;
        Time.timeScale = 8;
        var preview=FindObjectOfType<CityTrafficPreview>();
        if(!Check(preview && preview.Spawners.Length==4 && preview.Signals.Length==2,"Preview has four flows and only two active signals")) yield break;
        if(!Check(BaseCode.Logic.City.GameManager.ForScene(gameObject.scene)==null,"Campaign disabled in preview"))yield break;
        foreach(var light in preview.Signals) light.SwitchRedGreen();
        yield return new WaitForSeconds(2.3f);
        foreach(var spawn in preview.Spawners)
        {
            int before=spawn.Completed; float maxError=0; spawn.StartAutomatically=true;
            float deadline=Time.time+100;
            while(Time.time<deadline)
            {
                foreach(var car in CityCarController.All)
                {
                    if(!Check(car.State!=CityCarController.DrivingState.Crashed && car.SameStreamContacts==0,"No contacts in isolated level flow"))yield break;
                    maxError=Mathf.Max(maxError,Mathf.Abs(car.LaneError));
                    if(!Check(Mathf.Abs(car.transform.position.y-car.Road.At(car.RouteDistance).Position.y)<3,"Car stays on actual road surface"))yield break;
                }
                yield return new WaitForSeconds(0.25f);
            }
            if(!Check(spawn.Completed-before>=5 && maxError<0.85f,"Actual Level 1 "+spawn.name+": completed="+(spawn.Completed-before)+"; error="+maxError))yield break;
            spawn.StartAutomatically=false;
            foreach(var car in new List<CityCarController>(CityCarController.All))Destroy(car.gameObject);
            yield return null;
        }
        foreach(var car in FindObjectsOfType<Realistic_Traffic_Controller.Scripts.RTC_CarController>())
            if(!Check(false,"Legacy car active in preview"))yield break;
        Debug.Log("LEVEL1_PLAY_PASS: all four streams on real Level 1 geometry, original active lights, no same-stream contacts, campaign disabled.");
        UnityEditor.EditorApplication.Exit(0);
    }
    private static bool Check(bool ok,string text)
    {
        if(!ok){Debug.LogError("LEVEL1_PLAY_FAIL "+text);UnityEditor.EditorApplication.Exit(1);}
        else if(text.StartsWith("Actual") || text.StartsWith("Preview") || text.StartsWith("Campaign"))Debug.Log("PASS "+text);
        return ok;
    }
}