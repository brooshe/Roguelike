using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Property;

public class EventTimerInst
{
    public EventFunction funcToExecute;
    public CharacterPawn pawn;
    public ActorInstance.ActorBase actor;
    public EventTimer timer;


    public float timeRemain;
    public float timeScale = 1.0f;

    public EventTimerInst(EventTimer timer, EventFunction eventFunc, CharacterPawn pawn, ActorInstance.ActorBase actor)
    {
        this.timer = timer;
        this.funcToExecute = eventFunc;
        this.pawn = pawn;
        this.actor = actor;
        timer.Init(this);        
    }

    public bool Update(float deltaTime)
    {
        if (timer.Interupt(pawn))
        {
            UIManager.Instance.CloseCasting();
            return true;
        }

        timeRemain -= deltaTime * timeScale;
        if (timeRemain <= 0)
        {
            funcToExecute.Execute(pawn, actor);
            timer.Clear(this);
            UIManager.Instance.CloseCasting();
            return true;
        }
        else
            UIManager.Instance.ShowCasting(timeRemain / timer.DelayTime);

        return false;
    }

    public void Pause()
    {
        timeScale = 0;
    }
    public void Resume()
    {
        timeScale = 1;
    }
    public void Stop()
    {
        timeRemain = 0;
        Debug.LogWarning("Timer Stopped!");
    }
}
