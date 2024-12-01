using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAIInverterNode : UnitAINode
{
    UnitAINode child;

    public UnitAIInverterNode()
    {
        child = null;
    }

    public void SetChild(UnitAINode c)
    {
        child = c;
    }

    public override bool Execute(Unit u)
    {
        return !child.Execute(u);
    }
}
