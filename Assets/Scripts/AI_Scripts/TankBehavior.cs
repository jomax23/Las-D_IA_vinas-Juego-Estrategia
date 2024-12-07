using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour
{
    // este código lo estoy usando tantopara el tanque como para el volador


    private AImanager ai_mg;

    private void Awake()
    {
        ai_mg = GetComponent<AImanager>();
    }

    public IEnumerator PlayActions(Unit unit)
    {
        unit.GetEnemies();
        foreach(var u in unit.enemiesInRange)
        {
            if (u != null) { 
                unit.Attack(u);
                yield return new WaitForSecondsRealtime(1);

                if (u == null)  // la unidad rival ha muerto
                {
                    // mover hacia el rey rival
                    ai_mg.MoveToKing(unit);
                    yield return new WaitForSecondsRealtime(1);
                }

                break;
            }
        }

        if(!unit.hasAttacked && !unit.hasMoved)   // No había nadie a quien atacar y aun puede moverse
        {
            // mover hacia el rey rival
            ai_mg.MoveToKing(unit);
            yield return new WaitForSecondsRealtime(1);

            unit.GetEnemies();
            foreach (var u in unit.enemiesInRange)
            {
                if (u != null)  // se ha acercado lo bastante a una unidad nueva como para atacarla
                {
                    unit.Attack(u);
                    yield return new WaitForSecondsRealtime(1);

                    break;
                }
            }
        }

        yield return null;
    }
}
