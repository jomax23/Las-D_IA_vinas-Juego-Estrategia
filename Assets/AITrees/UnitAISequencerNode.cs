using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAISequencerNode : UnitAINode
{
    private List<UnitAINode> children;

    public UnitAISequencerNode()
    {
        children = new List<UnitAINode>();
    }

    public void AddChild(UnitAINode child)
    {
        children.Add(child);
    }

    public override bool Execute(Unit u)
    {
        //AND
        foreach(UnitAINode c in children)
        {
            if(!c.Execute(u))
            {
                return false;
            }
        }

        return true;
    }
}
