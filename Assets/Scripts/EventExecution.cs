using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorInstance;

public static class EventExecution
{
    /*
     * A general event should have two parameters: pawn & actor
     */
	public static void ShowPanel(CharacterPawn pawn, ActorBase actor, int panelID)
	{
        Debug.LogFormat("Show panel {0}", panelID);
	}

    public static void BonusMovePoint(CharacterPawn pawn, ActorBase actor, int value)
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

    public static void ConsumeMovePoint(CharacterPawn pawn, ActorBase actor, int value)
    {
        pawn.ConsumeMovePoint(value);
    }

    public static void PlayEffect(CharacterPawn pawn, ActorBase actor, string name)
    {
        UIManager.Instance.QuestLog("Your touched a trigger.");

        GameObject prefab = Resources.Load<GameObject>(name);
        Object.Instantiate<GameObject>(prefab, actor.actorTrans);        
    }
}
