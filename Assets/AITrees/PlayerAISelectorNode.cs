using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAISelectorNode : PlayerAINode
{
    private List<PlayerAINode> children;

    public PlayerAISelectorNode()
    {
        children = new List<PlayerAINode>();
    }

    public void AddChild(PlayerAINode child)
    {
        children.Add(child);
    }

    public override bool Execute()
    {
        //Selector ejecuta todos los hijos hasta encontrar un true, sino encuentra ninguno devuelve false

        foreach(PlayerAINode c in children)
        {
            if(c.Execute())
            {
                return true;
            }
        }

        return false;
    }
}
