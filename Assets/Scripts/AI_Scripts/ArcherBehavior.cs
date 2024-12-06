using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBehavior : MonoBehaviour
{
    public void PlayActions(Unit unit)
    {
        print("SOY EL CUPIDO DE LA MUERTE");

        unit.GetEnemies();
        foreach (var u in unit.enemiesClose)
        {
            if (u != null)
            {
                // El arquero se aleja de la zona
                print("El arquero HUYE");
                unit.GetEnemies();

                break;
            }
        }

        Unit target = GetLowestHealthEnemy(unit);

        if(target != null)
        {
            unit.Attack(target);
        }

        if (target == null && !unit.hasMoved)   // Si no tiene un objetivo al que atacar y aun no se ha movido
        {
            // el archero se acerca al enemigo más próximo
            unit.GetEnemies();

            target = GetLowestHealthEnemy(unit);
            if (target != null) // Si después de moverse encuentra un objetivo le ataca
            {
                unit.Attack(target);
            }
        }

    }

    private Unit GetLowestHealthEnemy(Unit unit)
    {
        Unit unit_lowestHealth = null;

        foreach (var u in unit.enemiesInRange)
        {
            if (u != null && (unit_lowestHealth == null || unit_lowestHealth.health > u.health))
            {
                // El objetivo del arquero pasa a ser el de menor vida
                unit_lowestHealth = u;
            }
        }

        return unit_lowestHealth;
    }
}
