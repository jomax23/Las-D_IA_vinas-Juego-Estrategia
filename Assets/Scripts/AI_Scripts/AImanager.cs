using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AImanager : MonoBehaviour
{

    [HideInInspector] public List<GameObject> units;
    [HideInInspector] public List<GameObject> unitsPlayer;

    private TankBehavior _tank;
    private ArcherBehavior _archer;

    private GameMaster gm;
    private Barrack bar;
    private FieldObstacleGeneration fog;

    [Header("TIENDA ACCESIBLE POR LA IA")]
    [Tooltip("Ordena de menor a mayor coste los items de forma manual. Como no pretendemos añadir muchos esto facilita trabajar con la IA.")]
    public List<BarrackItem> purchasable;

    // Start is called before the first frame update
    void Start()
    {
        _tank = GetComponent<TankBehavior>();
        _archer = GetComponent<ArcherBehavior>();
        //ShowEnemiesList();

        gm  = GameObject.Find("/GameMaster").GetComponent<GameMaster>();
        bar = GameObject.Find("/GameMaster").GetComponent<Barrack>();
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
        for(int i = 0; i < units.Count; i++)
        {
            if (units[i] == null)
            {
                units.Remove(units[i]);
                continue;
            }

            else
            {
                Unit u = units[i].GetComponent<Unit>();
                switch (u.unitType)
                {
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

                yield return new WaitUntil(() => !gm.somethingIsMoving);
            } 
        }

        // Antes de que termine el turno, la IA gasta el dinero de tener dinero
        PurchaseDecision();

        // El turno de la IA termina cuando todas sus unidades han tomado sus respectivas acciones
        gm.AIendTurn();

        yield return null;
    }

    public void MoveToKing(Unit mySelf)
    {
        Vector2 target = fog.decisionMakingValues.kingsInfo.myKingCoord;

        // una vez tengo las casillas a las quye puedo llegar, quiero obtener la casilla más cercana al objetivo
        target = ClosestToTarget(mySelf, target);

        mySelf.MoveAIUnit(target);
    }

    public void MoveToTarget(Unit mySelf, Unit myEnemy)
    {
        Vector2 target = myEnemy.transform.position;

        // una vez tengo las casillas a las quye puedo llegar, quiero obtener la casilla más cercana al objetivo
        target = ClosestToTarget(mySelf, target);

        mySelf.MoveAIUnit(target);
    }

    public void Flee(Unit mySelf, Unit myEnemy)
    {
        Vector2 target = myEnemy.transform.position;

        // una vez tengo las casillas a las quye puedo llegar, quiero obtener la casilla más alejada del objetivo
        target = FurthestFromTarget(mySelf, target);

        mySelf.MoveAIUnit(target);
    }

    public Vector2 ClosestToTarget(Unit unit, Vector2 target)
    {
        Vector2 newTarget = unit.transform.position;
        float influence = 1000;

        unit.GetWalkableTiles();

        /*
        Para que la ia no se quede atascada con obstáculos necesito usar en vez de 
                reachableTileList
        debo usar 
                reachableTileList && inPathToTarget --> la intersección de ambas listas
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
        No le pasa lo mismo que al método anterior, aquí quieres ir al punto más alejado
        Aun así, en teoría se puede usar el mapa de influencias para escoger un mejor objetivo al que moverse
        dependiendo de cuentos enemigos halla al rededor y aliados. Para una primera implementación no es 
        horrible pero está bastante mal.
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

    private void PurchaseDecision()
    {
        BarrackItem b = null;

        // Si sobra dinero, es porque hemos ahorrado en las rondas anteriores para comprar la unidad cara
        if (gm.player2Gold >= purchasable[^1].cost)
            b = purchasable[^1];

        // Si el jugador tiene más tropas, compra la tropa más cara que te puedas permitir
        else if(unitsPlayer.Count > units.Count)
        {
            foreach (BarrackItem i in purchasable)
            {
                if (gm.player2Gold >= purchasable[^1].cost)
                    b = i;
            }
        }

        // Si tenemos el mismo número de tropas, como IA quiero comprar lo más barato que pueda para ganar ventaja
        else if (unitsPlayer.Count == units.Count)
        {
            foreach (BarrackItem i in purchasable)
            {
                if (gm.player2Gold >= purchasable[^1].cost)
                {
                    b = i;
                    break;
                }
            }
        }

        // Si tengo la ventaja me espero hasta poder comprar la unidad más cara
        if (b != null)
        {
            bar.BuyItem(b);
        }
    }
}
