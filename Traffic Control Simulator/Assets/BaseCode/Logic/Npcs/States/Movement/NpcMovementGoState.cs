using System.Collections;
using BaseCode.Logic.Npcs.Controllers;
using BaseCode.Logic.Npcs.Npc;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEditor.Animations;
using UnityEngine;

namespace BaseCode.Logic.Npcs.States.Movement
{
    public class NpcMovementGoState : INpcMovementState
    {
        private readonly NpcMovementStateController _stateController; // Reference to the state controller
        private bool _selectingATarget = false;
        
        public NpcMovementGoState(NpcMovementStateController stateController)
        {
            _stateController = stateController;
        }

        public void MovementEnter()
        {
            Debug.Log("Enter Go");
            SelectTarget();
        }

        private void SelectTarget()
        {
            float distanceOfA = Vector3.Distance(NpcBase.transform.position, NpcController.a.position);
            float distanceOfB = Vector3.Distance(NpcBase.transform.position, NpcController.b.position);

            NpcController.target = distanceOfA > distanceOfB ? NpcController.a : NpcController.b;
            
            Vector3 directionToTarget = (NpcController.target.position - NpcBase.transform.position).normalized;
            if(directionToTarget != Vector3.zero)
                NpcBase.transform.rotation = Quaternion.LookRotation(directionToTarget);
            _stateController.Controller.SetToWalk();
        }
        private IEnumerator SelectTargetWithTime()
        {
            _selectingATarget = true;
            _stateController.Controller.SetToIdle();
            yield return new WaitForSeconds(Random.Range(1f,3f));
            _stateController.Controller.SetToWalk();
            SelectTarget();
            _selectingATarget = false;
        }

        public void MovementUpdate() 
        {
            if(IsItGreen())
                _stateController.SetState<NpcMovementStopState>();

            if (IsCloseEnough() == false)
                MoveTowardsTarget();
            else
                if(_selectingATarget == false)
                    NpcBase.StartCoroutine(SelectTargetWithTime());
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
        private NpcController NpcController => _stateController.Controller;
        
        private NpcScriptableObject NpcSo => NpcBase.npcController.npcScriptableObject;
        
    }
}
