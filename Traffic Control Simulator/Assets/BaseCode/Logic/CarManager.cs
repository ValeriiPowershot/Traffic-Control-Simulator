using System;
using BaseCode.Logic.ScoringSystem;
using BaseCode.Logic.Ways;
using BaseCode.Logic.EntityHandler;
using BaseCode.Logic.Services.Handler.Car;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic
{
    public class CarManager : MonoBehaviour
    {
        public AllWaysContainer allWaysContainer;
        
        [SerializeField] private ScoringManager scoreManager;
        [Space] 
        [SerializeField] private CarSpawnServiceHandler carSpawnServiceHandler;

        private void Awake()
        {
            carSpawnServiceHandler.Initialize(this);
        }

        private void Update()
        {
            carSpawnServiceHandler.Update();
        }
        public ScoringManager ScoringManager => scoreManager;
    }
}

/*private void TestBus()
{
    var subscriptionManager = new EventBusSubscriptionManager();
    var eventBus = new EventBus(subscriptionManager);

    eventBus.Subscribe<UserRegisteredNewEvent, UserRegisteredEventHandler>();

    var userRegisteredEvent = new UserRegisteredNewEvent("Tolga", "Tolga.Konat.12@gmail.com");
    eventBus.Publish(userRegisteredEvent);
}*/