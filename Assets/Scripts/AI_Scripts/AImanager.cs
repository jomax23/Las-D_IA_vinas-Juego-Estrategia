using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AImanager : MonoBehaviour
{

    [HideInInspector]
    public List<GameObject> units;
    public List<GameObject> unitsPlayer;

    private TankBehavior _tank;
    private ArcherBehavior _archer;

    private GameMaster gm;
    private FieldObstacleGeneration fog;

    // Start is called before the first frame update
    void Start()
    {
        _tank = GetComponent<TankBehavior>();
        _archer = GetComponent<ArcherBehavior>();
        //ShowEnemiesList();

        gm  = GameObject.Find("/GameMaster").GetComponent<GameMaster>();
        fog = GameObject.Find("/FieldGenerator").GetComponent<FieldObstacleGeneration>();
    }

    void ShowEnemiesList()
    {
        foreach (var unit in units)
        {
            print(unit.GetComponent<Unit>().unitType);
        }
    }

    public IEnumerator PlayEnemyTurn()
    {
        foreach (var unit in units)
        {
            Unit u = unit.GetComponent<Unit>();
            switch (u.unitType){
                case Unit.UnitType.Tank:
                    StartCoroutine(_tank.PlayActions(u));
                    break;
                case Unit.UnitType.Flyer:
                    StartCoroutine(_tank.PlayActions(u));

                    break;
                case Unit.UnitType.Archer:
                    StartCoroutine(_archer.PlayActions(u));
                    break;
                default:
                    break;
            }

            yield return new WaitForSecondsRealtime(1);
        }

        // El turno de la IA termina cuando todas sus unidades han tomado sus respectivas acciones
        gm.AIendTurn();

        yield return null;
    }

    public void MoveToKing(Unit mySelf)
    {
        Vector2 target = fog.decisionMakingValues.kingsInfo.myKingCoord;

        // una vez tengo las casillas a las quye puedo llegar, quiero obtener la casilla m�s cercana al objetivo
        target = ClosestToTarget(mySelf, target);

        mySelf.MoveAIUnit(target);
    }

    public void MoveToTarget(Unit mySelf, Unit myEnemy)
    {
        Vector2 target = myEnemy.transform.position;

        // una vez tengo las casillas a las quye puedo llegar, quiero obtener la casilla m�s cercana al objetivo
        target = ClosestToTarget(mySelf, target);

        mySelf.MoveAIUnit(target);
    }

    public void Flee(Unit mySelf, Unit myEnemy)
    {
        Vector2 target = myEnemy.transform.position;

        // una vez tengo las casillas a las quye puedo llegar, quiero obtener la casilla m�s alejada del objetivo
        target = FurthestFromTarget(mySelf, target);

        mySelf.MoveAIUnit(target);
    }

    public Vector2 ClosestToTarget(Unit unit, Vector2 target)
    {
        Vector2 newTarget = unit.transform.position;
        float influence = 1000;

        unit.GetWalkableTiles();

        /*
        Para que la ia no se quede atascada con obst�culos necesito usar en vez de 
                reachableTileList
        debo usar 
                reachableTileList && inPathToTarget --> la intersecci�n de ambas listas
        tengo que programar inPathToTarget que devuelve una lista con las tiles de un camino
        ,sin importar si tiene suficiente velocidad para llegar o no, que llegue desde el 
        origen hasta el objetivo.
        */

        // NEW ahora tiene en cuenta el coste de moverse

        Vector2 v;
        foreach (Tile t in unit.reachableTileList)
        {
            v = new Vector2(t.transform.position.x, t.transform.position.y);
            if (t.fCost < influence && Vector2.Distance(newTarget, target) > Vector2.Distance(v, target))
            {
                newTarget = v;
                influence = t.influenceValue;
            }
        }

        v = unit.transform.position;
        if (Vector2.Distance(newTarget, target) > Vector2.Distance(v, target))
            newTarget = v;

        return newTarget;
    }

    public Vector2 FurthestFromTarget(Unit unit, Vector2 target)
    {
        Vector2 newTarget = unit.transform.position;
        float influence = 1000;

        unit.GetWalkableTiles();

        /*
        No le pasa lo mismo que al m�todo anterior, aqu� quieres ir al punto m�s alejado
        Aun as�, en teor�a se puede usar el mapa de influencias para escoger un mejor objetivo al que moverse
        dependiendo de cuentos enemigos halla al rededor y aliados. Para una primera implementaci�n no es 
        horrible pero est� bastante mal.
        */

        // NEW ahora tiene en cuenta el coste de moverse

        Vector2 v;
        foreach (Tile t in unit.reachableTileList)
        {
            v = new Vector2(t.transform.position.x, t.transform.position.y);
            if (t.fCost < influence && Vector2.Distance(newTarget, target) < Vector2.Distance(v, target))
            {
                newTarget = v;
                influence = t.influenceValue;
            }
        }

        v = unit.transform.position;
        if (Vector2.Distance(newTarget, target) < Vector2.Distance(v, target))
            newTarget = v;

        return newTarget;
    }
}
