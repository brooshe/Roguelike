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

    public static void BonusAttribute(CharacterPawn pawn, ActorBase actor, Property.CharacterAtrribute attrib, int value)
    {
        int pre = 0;
        switch (attrib)
        {
        case Property.CharacterAtrribute.STRENGTH:
            pre = pawn.CurStrengthLev;
        	break;
        case Property.CharacterAtrribute.AGILITY:
            pre = pawn.CurAgilityLev;
            break;
        case Property.CharacterAtrribute.INTEL:
            pre = pawn.CurIntellLev;
            break;
        case Property.CharacterAtrribute.SPIRIT:
            pre = pawn.CurSpiritLev;
            break;
        }        
        int lev = Mathf.Clamp(pre + value, 0, 7);
        //TODO: maybe this trigger make attribute level less than 0, which would kill player
        switch (attrib)
        {
            case Property.CharacterAtrribute.STRENGTH:
                pawn.CurStrengthLev = lev;
                break;
            case Property.CharacterAtrribute.AGILITY:
                pawn.CurAgilityLev = lev;
                break;
            case Property.CharacterAtrribute.INTEL:
                pawn.CurIntellLev = lev;
                break;
            case Property.CharacterAtrribute.SPIRIT:
                pawn.CurSpiritLev = lev;
                break;
        }

        if(pre > lev)
        {
            UIManager.Instance.QuestLog(string.Format("Your {0} has decreased {1} level", attrib,  pre - lev));
        }
        else if(pre < lev)
        {
            UIManager.Instance.QuestLog(string.Format("Your {0} has increased {1} level", attrib, lev - pre));
        }
        else
        {
            UIManager.Instance.QuestLog(string.Format("Your {0} level stays the same.", attrib));
        }
    }

    public static void ConsumeMovePoint(CharacterPawn pawn, ActorBase actor, int value)
    {
        pawn.ConsumeMovePoint(value);
    }

    public static void ConsumeAllMovePoint(CharacterPawn pawn, ActorBase actor)
    {
        pawn.ConsumeMovePoint(pawn.RemainMovePoint);
    }

    public static void PlayEffect(CharacterPawn pawn, ActorBase actor, string name)
    {
        UIManager.Instance.QuestLog("Your touched a trigger.");

        GameObject prefab = Resources.Load<GameObject>(name);
        Object.Instantiate<GameObject>(prefab, actor.actorTrans);        
    }

    public static void DemonPace(CharacterPawn pawn, ActorBase actor)
    {
        UIManager.Instance.Message("你无法离开此房间");
    }

    public static void TriggerRemoteTriggers(CharacterPawn pawn, ActorBase actor, string filterName)
    {
        Property.TriggerFilter filter = Resources.Load<Property.TriggerFilter>(string.Format("Trigger/TriggerFilter/{0}", filterName));
        if(filter != null)
        {
            foreach (Trigger trig in GameLoader.Instance.RemoteTriggers)
            {
                if(filter.Check(trig))
                {
                    trig.OnTriggerSuccess(pawn.controller);
                }
            }
            
        }
    }

    public static void TriggerRemoteTriggers(CharacterPawn pawn, ActorBase actor, string filterName, string finderName, string sequenceName)
    {
        Property.TriggerFilter filter = Resources.Load<Property.TriggerFilter>(string.Format("Trigger/TriggerFilter/{0}", filterName));
        Property.TargetFinder finder = Resources.Load<Property.TargetFinder>(string.Format("Trigger/TargetFinder/{0}", finderName));
        Property.EventSequence sequence = Resources.Load<Property.EventSequence>(string.Format("Events/EventSequence/{0}", sequenceName));
        if (filter != null && sequence != null)
        {
            foreach (Trigger trig in GameLoader.Instance.RemoteTriggers)
            {
                if (filter.Check(trig))
                {
                    trig.CallEventSequence(pawn.controller, finder, sequence);
                }
            }

        }
    }

    public static void PlayAnimation(CharacterPawn pawn, ActorBase actor, string anim)
    {
        if(anim.Equals("rushtowindow"))
        {
            UIManager.Instance.Message("rushtowindow");
        }
        else if (anim.Equals("rushtowindowStop"))
        {
            UIManager.Instance.Message("rushtowindowStop");
        }
    }

    public static void DelayTransportToRoom(CharacterPawn pawn, ActorBase actor, string room, float delay)
    {

    }

    public static void TransportToRoom(CharacterPawn pawn, ActorBase actor, string room)
    {

    }
}
