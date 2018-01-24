using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerEvent
{
    /*
     * A general trigger event should have two parameters: pawn & trigger
     */
	public static void ShowPanel(CharacterPawn pawn, Trigger trigger, int panelID)
	{
        Debug.LogFormat("Show panel {0}", panelID);
	}

    public static void BonusMovePoint(CharacterPawn pawn, Trigger trigger, int value)
    {
        int lev = Mathf.Clamp(pawn.CurMovePointLev + value, 0, 7);
        //TODO: maybe this trigger make move-point level less than 0, which would kill player
        pawn.CurMovePointLev = lev;
    }

    public static void PlayEffect(CharacterPawn pawn, Trigger trigger, string name)
    {
        GameObject prefab = Resources.Load<GameObject>(name);
        Object.Instantiate<GameObject>(prefab, trigger.mono.transform);        
    }
}
