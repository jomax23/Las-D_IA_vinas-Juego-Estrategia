using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIInverterNode : PlayerAINode
{
    PlayerAINode child;

    public PlayerAIInverterNode()
    {
        child = null;
    }

    public void SetChild(PlayerAINode c)
    {
        child = c;
    }

    public override bool Execute()
    {
        return !child.Execute();
    }
}
