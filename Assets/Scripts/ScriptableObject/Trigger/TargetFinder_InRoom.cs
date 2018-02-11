using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "TargetFinder/AllPlayerInRoom")]
    public class TargetFinder_InRoom : TargetFinder
    {
        public override void FindTarget(PlayerController controller, ActorInstance.Trigger triggerInst, ref List<CharacterPawn> targets)
        {
            if(triggerInst.parentRoom != null && triggerInst.parentRoom.PawnsInRoom != null)
                targets.AddRange(triggerInst.parentRoom.PawnsInRoom);
        }
    }

}