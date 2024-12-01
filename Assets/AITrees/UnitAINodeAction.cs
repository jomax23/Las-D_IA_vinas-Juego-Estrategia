using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAINodeAction : UnitAINode
{
    public ActionType actionType;

    public enum ActionType
    {
        Huir,
        ProtegerRey,
        Pushear, //No se como expresarlo en español xd
        AtacarRey,
        AtacarEnemigo,
        IrACurarse, //Si esta a poca vida y el enemigo puede matarlo, ira
    }

    public UnitAINodeAction(ActionType a)
    {
        actionType = a;
    }

    public override bool Execute(Unit u)
    {
        if(u.actionRequested != null)
        {
            return false;
        }

        Debug.Log(u.actionRequested);

        u.actionRequested = actionType;
        return true;
    }
}
