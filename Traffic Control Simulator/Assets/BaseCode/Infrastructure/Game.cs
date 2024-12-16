using BaseCode.Infrastructure;
using Infrastructure.States;
using Logic;
using Services;
using UnityEngine;

namespace Infrastructure
{
  public class Game
  {
    public GameStateMachine StateMachine;

    public Game(ICoroutineRunner coroutineRunner, LoadingCurtain curtain)
    {
      if (Application.isEditor)
      {
        
      }
        
      StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), curtain, AllServices.Container);
    }
  }
}