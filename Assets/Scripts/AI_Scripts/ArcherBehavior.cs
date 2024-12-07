using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherBehavior : MonoBehaviour
{
    private AImanager ai_mg;
    private GameMaster gm;

    private void Awake()
    {
        ai_mg = GetComponent<AImanager>();
        gm    = GameObject.Find("/GameMaster").GetComponent<GameMaster>();
    }

    public IEnumerator PlayActions(Unit unit)
    {
        unit.GetEnemies();
        foreach (var u in unit.enemiesClose)
        {
            if (u != null)
            {
                // El arquero se aleja de la zona
                ai_mg.Flee(unit, u);
                yield return new WaitUntil(() => !gm.somethingIsMoving);
                unit.GetEnemies();

                break;
            }
        }

        Unit target = GetLowestHealthEnemy(unit);

        if(target != null)
        {
            unit.Attack(target);
            yield return new WaitUntil(() => !gm.somethingIsMoving); ;
        }

        if (target == null && !unit.hasMoved)   // Si no tiene un objetivo al que atacar y aun no se ha movido
        {
            // el archero se acerca al enemigo más próximo
            ai_mg.MoveToTarget(unit, GetClosestEnemy(unit));
            yield return new WaitUntil(() => !gm.somethingIsMoving);
            unit.GetEnemies();

            target = GetLowestHealthEnemy(unit);
            if (target != null) // Si después de moverse encuentra un objetivo le ataca
            {
                unit.Attack(target);
                yield return new WaitUntil(() => !gm.somethingIsMoving);
            }
        }

        yield return null;
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

    private Unit GetClosestEnemy(Unit unit)
    {
        Vector2 target = Vector2.one * 1000;
        Vector2 origin = unit.transform.position;
        Unit unitClosest = null;

        foreach (var e in ai_mg.unitsPlayer)
        {
            if (e != null)
            {
                Vector2 v = new Vector2(e.transform.position.x, e.transform.position.y);
                if (Vector2.Distance(origin, target) > Vector2.Distance(origin, v))
                {
                    target = v;
                    unitClosest = e.GetComponent<Unit>();
                }
            }
        }

        return unitClosest;
    }
}
