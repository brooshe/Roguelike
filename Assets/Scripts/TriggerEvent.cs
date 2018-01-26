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
        int pre = pawn.CurMovePointLev;
        int lev = Mathf.Clamp(pawn.CurMovePointLev + value, 0, 7);
        //TODO: maybe this trigger make move-point level less than 0, which would kill player
        pawn.CurMovePointLev = lev;
        if(pre > lev)
        {
            UIManager.Instance.QuestLog(string.Format("Your move-point has decreased {0} level", pre - lev));
        }
        else if(pre < lev)
        {
            UIManager.Instance.QuestLog(string.Format("Your move-point has increased {0} level", lev - pre));
        }
        else
        {
            UIManager.Instance.QuestLog("Your move-point level stays the same.");
        }
    }

    public static void PlayEffect(CharacterPawn pawn, Trigger trigger, string name)
    {
        UIManager.Instance.QuestLog("Your touched a trigger.");

        GameObject prefab = Resources.Load<GameObject>(name);
        Object.Instantiate<GameObject>(prefab, trigger.mono.transform);        
    }
}
