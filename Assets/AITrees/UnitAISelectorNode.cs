using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAISelectorNode : UnitAINode
{
    private List<UnitAINode> children;

    public UnitAISelectorNode()
    {
        children = new List<UnitAINode>();
    }

    public void AddChild(UnitAINode child)
    {
        children.Add(child);
    }

    public override bool Execute(Unit u)
    {
        //Selector ejecuta todos los hijos hasta encontrar un true, sino encuentra ninguno devuelve false

        foreach(UnitAINode c in children)
        {
            if(c.Execute(u))
            {
                return true;
            }
        }

        return false;
    }
}
