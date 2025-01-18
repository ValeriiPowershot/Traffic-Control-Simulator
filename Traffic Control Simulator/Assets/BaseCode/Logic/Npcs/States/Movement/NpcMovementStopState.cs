using System.Collections;
using BaseCode.Logic.Npcs.Controllers;
using BaseCode.Logic.Npcs.Controllers;
using BaseCode.Logic.Npcs.Npc;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Npcs.States.Movement
{
    public class NpcMovementStopState : INpcMovementState
    {
        private readonly NpcMovementStateController _stateController;
        private bool _isWaiting;
        public NpcMovementStopState(NpcMovementStateController npcController)
        {
            _stateController = npcController;
        }
        
        public void MovementEnter()
        {
            Debug.Log("Stop");
            _stateController.Controller.SetToIdle();

            if (IsItGreen())
            {
                float distanceOfA = Vector3.Distance(NpcBase.transform.position, NpcController.a.position);
                float distanceOfB = Vector3.Distance(NpcBase.transform.position, NpcController.b.position);

                NpcController.target = distanceOfA > distanceOfB ? NpcController.b : NpcController.a;
                
                Vector3 directionToTarget = (NpcController.target.position - NpcBase.transform.position).normalized;
                NpcBase.transform.rotation = Quaternion.LookRotation(directionToTarget);
                _stateController.Controller.SetToRun();
            }

            _isWaiting = false;
        }

        public void MovementUpdate()
        {
            if (_isWaiting == false && IsCloseEnough() && IsItRed())
                NpcBase.StartCoroutine(WaitForSecondsAndChange());
            
            if (IsCloseEnough() == false)
            {
                MoveTowardsTarget();
            }
            else
            {
                _stateController.Controller.SetToIdle();
            }
        }

        private IEnumerator WaitForSecondsAndChange()
        {
            _stateController.Controller.SetToIdle();

            _isWaiting = true;
             yield return new WaitForSeconds(1f);
            _isWaiting = false;
            _stateController.SetState<NpcMovementGoState>();
        }

        private bool IsItRed()
        {
            return NpcBase.npcController.currentLight == LightState.Red;
        }
        private bool IsItGreen()
        {
            return NpcBase.npcController.currentLight == LightState.Green;
        }


        private bool IsCloseEnough()
        {
            return Vector3.Distance(NpcBase.transform.position, NpcController.target.position) <= NpcSo.stopDistance;
        }

        public void MovementExit()
        { 
        }

        private void MoveTowardsTarget()
        {
            Vector3 direction = (NpcController.target.position - NpcBase.transform.position).normalized;
            NpcBase.transform.position += direction * (NpcSo.moveSpeed * Time.deltaTime);
        }
        
        private NpcBase NpcBase => _stateController.Controller.NpcBase;
        private NpcScriptableObject NpcSo => NpcBase.npcController.npcScriptableObject;
        private NpcController NpcController => _stateController.Controller;

    }
}