using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAISequencerNode : PlayerAINode
{
    private List<PlayerAINode> children;

    public PlayerAISequencerNode()
    {
        children = new List<PlayerAINode>();
    }

    public void AddChild(PlayerAINode child)
    {
        children.Add(child);
    }

    public override bool Execute()
    {
        foreach(PlayerAINode c in children)
        {
            if(!c.Execute())
            {
                return false;
            }
        }

        return true;
    }
}
