using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTree : MonoBehaviour
{

    public Unit u;
    // Start is called before the first frame update
    void Start()
    {
        //UnitAI
        UnitAISelectorNode raiz = new UnitAISelectorNode();
        UnitAISequencerNode hijo1 = new UnitAISequencerNode();
        UnitAISequencerNode hijo2 = new UnitAISequencerNode();
        UnitAIConditionalNode subHijo1 = new UnitAIConditionalNode(UnitAIConditionalNode.ConditionalType.LowHealth);
        UnitAINodeAction subHijo2 = new UnitAINodeAction(UnitAINodeAction.ActionType.Pushear);

        raiz.AddChild(hijo1);
        raiz.AddChild(hijo2);

        hijo1.AddChild(subHijo1);
        hijo2.AddChild(subHijo2);

        raiz.Execute(u);
    }
}
