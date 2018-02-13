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

    public static void InvokeEventSequence(CharacterPawn pawn, ActorBase actor, string sequenceName)
    {
        Property.EventSequence sequence = Resources.Load<Property.EventSequence>(string.Format("Events/EventSequence/{0}", sequenceName));

        sequence.Init();
        sequence.CheckAndExecute(pawn, actor);
    }

    public static void TransportToRoom(CharacterPawn pawn, ActorBase actor, string roomFilter)
    {
        Connector from = actor as Connector;
        if (from == null)
            Debug.LogError("TransportToRoom: from null connector!");

        Property.RoomFilter filter = Resources.Load<Property.RoomFilter>(string.Format("Room/RoomFilter/{0}", roomFilter));

        Room destRoom = GameLoader.Instance.FindRoom(filter);
        if (destRoom == null)
            destRoom = GameLoader.Instance.CreateRoomByFilter(filter);
        if (destRoom != null)
        {
            foreach (Connector conn in destRoom.ConnectorList)
            {
                if (conn.connectorProp.IsDynamic)
                {
                    if (conn.TryGetThrough(pawn, from.LogicPosition))
                    {
                        from.parentRoom.Show(false);
                        return;
                    }
                }
            }
            Debug.LogErrorFormat("Transport to {0} fail!  Cannot connect to dest room!", roomFilter);
        }
        else
        {
            Debug.LogErrorFormat("Transport to {0} fail! No dest room", roomFilter);
        }
    }

    public static void DicePhysicalDamage(CharacterPawn pawn, ActorBase actor, int numDice)
    {
        int damage = Dice.RandomDice(numDice);
        UIManager.Instance.QuestLog(string.Format("You take {0} physical damage.", damage));

        if (damage <= 0)
            return;

        int str = pawn.CurStrengthLev;
        int agi = pawn.CurAgilityLev;
        while(damage-- > 0)
        {
            if (str >= agi)
                --str;
            else
                --agi;
            if(str <= 0 || agi <= 0)
            {                
                pawn.CurStrengthLev = str;
                pawn.CurAgilityLev = agi;
                //TODO: player die
                UIManager.Instance.Message("You die!");
                return;
            }
        }
        pawn.CurStrengthLev = str;
        pawn.CurAgilityLev = agi;
    }
}
