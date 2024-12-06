using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AImanager : MonoBehaviour
{

    [HideInInspector]
    public List<GameObject> units;

    private TankBehavior _tank;
    private ArcherBehavior _archer;

    // Start is called before the first frame update
    void Start()
    {
        _tank = GetComponent<TankBehavior>();
        _archer = GetComponent<ArcherBehavior>();
        ShowEnemiesList();
    }

    void ShowEnemiesList()
    {
        foreach (var unit in units)
        {
            print(unit.GetComponent<Unit>().unitType);
        }
    }

    public void PlayEnemyTurn()
    {
        foreach (var unit in units)
        {
            switch (unit.GetComponent<Unit>().unitType){
                case Unit.UnitType.Tank:
                    _tank.PlayActions(unit.GetComponent<Unit>());
                    break;
                case Unit.UnitType.Archer:
                    _archer.PlayActions(unit.GetComponent<Unit>());
                    break;
                default:
                    break;
            }
        }
    }

}
