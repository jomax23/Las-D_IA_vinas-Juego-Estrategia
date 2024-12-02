using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAIConditionalNode : UnitAINode
{

    public enum ConditionalType { LowHealth, EnemiesAround, KingLowHealth}

    public ConditionalType ct;

    public UnitAIConditionalNode(ConditionalType conditionType)
    {
        ct = conditionType;
    }
    public override bool Execute(Unit u)
    {
        switch (ct)
        {
            case ConditionalType.LowHealth: 
                return u.health < 4;

            case ConditionalType.KingLowHealth:
                return true;

            default:
                return true;

/*  
            case EnemiesAround:
            {

            }
                */

        }
    }
}
