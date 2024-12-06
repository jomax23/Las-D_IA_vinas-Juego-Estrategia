using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour
{
    public void PlayActions(Unit unit)
    {
        print("ME MOVI PUTO");

        unit.GetEnemies();
        foreach(var u in unit.enemiesInRange)
        {
            if (u != null) { 
                unit.Attack(u);

                if (u == null)  // la unidad rival ha muerto
                {
                    // mover hacia el rey rival
                    
                }

                break;
            }
        }

        if(!unit.hasAttacked && !unit.hasMoved)   // No había nadie a quien atacar y aun puede moverse
        {
            // mover hacia el rey rival

            unit.GetEnemies();
            foreach (var u in unit.enemiesInRange)
            {
                if (u != null)  // se ha acercado lo bastante a una unidad nueva como para atacarla
                {
                    unit.Attack(u);

                    break;
                }
            }
        }
    }
}
