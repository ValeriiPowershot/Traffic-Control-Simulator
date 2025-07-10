using System.Collections.Generic;
using System.Linq;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Stores all active drive actions for a vehicle
    /// </summary>
    public struct DriveActions
    {
        public List<DriveAction> activeActions;


        public void Add(DriveAction newAction)
        {
            if (!activeActions.Contains(newAction))
            {
                activeActions.Add(newAction);
            }
        }


        public void Remove(TrafficSystem.DriveActions actionType)
        {
            activeActions.RemoveAll(cond => cond.GetActionType() == actionType);
        }


        public void RemoveAll(TrafficSystem.DriveActions[] movingActions)
        {
            activeActions.RemoveAll(cond => movingActions.Contains(cond.GetActionType()));
        }


        public void Insert(int position, DriveAction newAction)
        {
            activeActions.Insert(position, newAction);
        }


        public bool Contains(TrafficSystem.DriveActions actionType)
        {
            return activeActions.Any(cond => cond.GetActionType() == actionType);
        }
    }


    public struct DriveAction
    {
        TrafficSystem.DriveActions actionType;
        RoadSide side;

        public DriveAction(TrafficSystem.DriveActions actionType, RoadSide side)
        {
            this.actionType = actionType;
            this.side = side;
        }


        public TrafficSystem.DriveActions GetActionType()
        {
            return actionType;
        }


        public RoadSide GetSide()
        {
            return side;
        }
    }
}