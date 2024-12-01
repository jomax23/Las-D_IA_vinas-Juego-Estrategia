using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FieldObstacleGeneration;
using static UnityEditor.PlayerSettings;

public class Unit : MonoBehaviour
{
    public UnitAINodeAction.ActionType actionRequested;

    private GameMaster gm;
    private Pathfinding pathFinding;

    //influenceStrength unidades aliadas sera negativa, y la de los enemigos positiva
    public float influenceStrength;
    public int influenceAreaRadius;
    /*
    public Vector2Int Position { get; set; }
    public Vector2Int Direction { get; set; }
    */
    [Header("EQUIPO DE LA UNIDAD")]
    public int playerNumber;

    [Header("TIPO DE UNIDAD")]
    public UnitType unitType;

    [Header("STATS")]
    public int tileSpeed;

    public int health;
    public int attackStat;
    public int defenseStat;

    public int attackRange;

    [Header("OBJETOS DE ATAQUE/CURACION")]
    public ObjetosAtaqueYCuracion misObjetos;

    [Header("EFECTOS TRAS ATACAR")]
    //public GameObject weaponIcon;
    public DamageIcon damageIcon;

    [Header("TEXTO DE VIDA DEL REY")]
    public Text kingHealth;

    [HideInInspector]
    public bool hasMoved;

    [HideInInspector]
    public float moveSpeed;

    [HideInInspector]
    public bool hasAttacked;

    [HideInInspector]
    public bool selected;

    private int layer = 0;

    private List<Unit> enemiesInRange = new List<Unit>();

    private FieldObstacleGeneration myField;

    //private GameObject miItem;
    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        myField = FindObjectOfType<FieldObstacleGeneration>();
        pathFinding = FindObjectOfType<Pathfinding>();
        SetInfluenceTiles();
        /*
        if(unitType == UnitType.Archer)
        {
            miItem = misObjetos.flecha;
        }
        */
        //UpdateKingHealth();
    }

    public enum UnitType
    {
        King,
        Tank,
        Archer,
        Healer
    }

    public void UpdateKingHealth()
    {
        if (unitType == UnitType.King)
        {
            // Un-comment this line if you want to display king's health in the UI
            kingHealth.text = health.ToString();
        }
    }

    private void OnMouseDown()
    {
        //gm.SomethingIsMoving();
        ResetWeaponIcons();

        if (selected)
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
        }
        else if (playerNumber == gm.playerTurn)
        {
            if (gm.selectedUnit != null)
            {
                gm.selectedUnit.selected = false;
            }

            selected = true;
            gm.selectedUnit = this;
            gm.ResetTiles();
            GetEnemies();
            GetWalkableTiles();
        }

        // Check for enemy attack
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit unit = col?.GetComponent<Unit>();

        if (gm.selectedUnit != null && unit != null && gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.hasAttacked)
        {
            gm.selectedUnit.Attack(unit);
        }
    }

    void Attack(Unit enemy)
    {
        hasAttacked = true;

        int damageDealt = attackStat - enemy.defenseStat;

        if(unitType == UnitType.Archer)
        {
            GameObject instance = Instantiate(misObjetos.flecha, transform.position, Quaternion.identity);
            gm.somethingIsMoving = true;
            StartCoroutine(StartMovementItem(instance, enemy, damageDealt));
            //if(StartMovementItem.hasEnded)
            
        }

        else
        {
            if (damageDealt > 0)
            {
                DamageIcon instance = Instantiate(enemy.damageIcon, enemy.transform.position, Quaternion.identity);
                instance.Setup(damageDealt);
                enemy.health -= damageDealt;
                enemy.UpdateKingHealth();
            }

            if (enemy.health <= 0)
            {
                Destroy(enemy.gameObject);
                GetWalkableTiles();
            }

            if (health <= 0)
            {
                gm.ResetTiles();
                Destroy(gameObject);
            }

            UpdateInfluenceMap();
        }

        gm.ResetTiles();


    }

    void GetWalkableTiles()
    {
        if (hasMoved) return;

        List<Tile> reachableTileList = new List<Tile>();

        
        for(int i = (int)transform.position.x - tileSpeed; i <= transform.position.x + tileSpeed; i++)
        {
            for (int j = (int)transform.position.y - tileSpeed; j <= transform.position.y + tileSpeed; j++)
            {
                if(i >= 0 && j >= 0 && i < myField.anchura && j < myField.altura && myField.arrayTile[i, j].IsClear())
                {
                    if ((int)transform.position.x != i || (int)transform.position.y != j)
                    {
                        //Debug.Log(i + ", " + j);
                        reachableTileList.Add(myField.arrayTile[i, j]);
                    }
                }
            }
        }

        
        
        foreach (Tile tile in reachableTileList)
        {
            pathFinding.FindPath(this, tile.transform.position);
        }
    }

    public void SetInfluenceTiles()
    {
        
        int myCol;
        int myRow;

        //SOLO PARA LOS TILES VECINOS A LA UNIDAD
        myCol = (int)transform.position.x - 1;
        myRow = (int)transform.position.y + 1;

        for (int i = 1; i <= influenceAreaRadius; i++)
        {
            myCol = (int)transform.position.x - i;
            myRow = (int)transform.position.y + i;

            if (myRow >= 0 && myRow < myField.altura)
            {
                for (int col = myCol; col < (int)transform.position.x + i; col++)
                {
                    if (col >= 0 && col < myField.anchura && myField.arrayTile[col, myRow].IsClear())
                    {
                        myField.arrayTile[col, myRow].influenceValue += influenceStrength/i;
                        Debug.Log("COL : " + col + ", ROW : " + myRow + ", VALOR DE INFLUENCIA: " + myField.arrayTile[col, myRow].influenceValue);
                    }
                    
                }
            }

            myCol = (int)transform.position.x + i;

            if (myCol >= 0 && myCol < myField.anchura)
            {
                for (int row = myRow; row > (int)transform.position.y - i; row--)
                {
                    if (row >= 0 && row < myField.altura && myField.arrayTile[myCol, row].IsClear())
                    {
                        myField.arrayTile[myCol, row].influenceValue += influenceStrength/i;
                        Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                    }
                }
            }

            myRow = (int)transform.position.y - i;

            if (myRow >= 0 && myRow < myField.altura)
            {
                for (int col = myCol; col > (int)transform.position.x - i; col--)
                {
                    if (col >= 0 && col < myField.anchura && myField.arrayTile[col, myRow].IsClear())
                    {
                        myField.arrayTile[col, myRow].influenceValue += influenceStrength / i;
                        Debug.Log("COL : " + col + ", ROW : " + myRow + ", VALOR DE INFLUENCIA: " + myField.arrayTile[col, myRow].influenceValue);
                    }
                }
            }

            myCol = (int)transform.position.x - i;

            if (myCol >= 0 && myCol < myField.anchura)
            {
                for (int row = myRow; row < (int)transform.position.y + i; row++)
                {
                    if (row >= 0 && row < myField.altura && myField.arrayTile[myCol, row].IsClear())
                    {
                        myField.arrayTile[myCol, row].influenceValue += influenceStrength / i;
                        Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                    }
                }
            }

            //myRow = (int)transform.position.y + i;
        }
    }
    public void GetEnemies()
    {
        enemiesInRange.Clear();
        
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            float distance = Vector2.Distance(transform.position, unit.transform.position);
            //if (Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange && Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange && unit.playerNumber != gm.playerTurn && !hasAttacked)
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange && Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange && unit.playerNumber != gm.playerTurn && !hasAttacked)
            {
                enemiesInRange.Add(unit);
                //unit.weaponIcon.SetActive(true);

                myField.arrayTile[(int)unit.transform.position.x, (int)unit.transform.position.y].HighlightAttackTile(this.playerNumber);
            }
        }
        
        //ESTO DE AQUI ES PARA QUE VEAIS COMO PUEDE AFECTAR EL ATAQUE A UN AREA CIRCULAR EN LUGAR DE UN AREA CUADRADA COMO EL MOVIMIENTO, YA QUE EL PROBLEMA DE LA OTRA ES QUE PUEDE ATACAR A UN ENEMIGO EN DIAGONAL QUE ESTA MAS LEJOS QUE UNO QUE PUEDA ESTAR
        //UNA CASILLA MAS ALEJADA DE SU RANGO DE ATAQUE EN EL EJE VERTICAL Y HORIZONTAL, YA QUE POR PITAGORAS SE SABE QUE LA DIAGONAL MIDE LADO * SQRT(2). LO PODEMOS CAMBIAR COMO QUERAIS QUE ES UN SEGUNDO HACERLO, ESTO LO PUEDO OPTIMIZAR TAMBIEN EN UN SEGUNDO QUE DE MOMENTO ESTA POCHA
        //ESTE CODIGO QUE NO SE QUITE QUE PUEDE SERVIR ALGUNA COSILLA DE AQUI PARA HACER EL MAPA DE INFLUENCIAS
        /*
        foreach (Tile unit in FindObjectsOfType<Tile>())
        {
            float distance = Vector2.Distance(transform.position, unit.transform.position);
            //if (Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange && Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange && unit.playerNumber != gm.playerTurn && !hasAttacked)
            if (distance <= attackRange)
            {
                //enemiesInRange.Add(unit);
                //unit.weaponIcon.SetActive(true);

                myField.arrayTile[(int)unit.transform.position.x, (int)unit.transform.position.y].HighlightAttackTile(this.playerNumber);
            }
        }
        */



    }

    void ResetWeaponIcons()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            //unit.weaponIcon.SetActive(false);
        }
    }

    public void Move(Vector2 tilePos)
    {
        gm.somethingIsMoving = true;
        //gm.SomethingIsMoving(true);
        gm.ResetTiles();
        pathFinding.FindPathMovement(transform.position, new Vector3(tilePos.x, tilePos.y, myField.depth));
        StartCoroutine(StartMovement(tilePos));
        //gm.somethingIsMoving = false;
    }

    IEnumerator StartMovement(Vector2 tilePos)
    {
        foreach (Tile tile in pathFinding.pathMovement)
        {
            while (transform.position.x != tile.transform.position.x || transform.position.y != tile.transform.position.y)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(tile.transform.position.x, tile.transform.position.y), moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        hasMoved = true;
        //gm.somethingIsMoving = false;
        layer = (int)tilePos.y + 1;

        ResetWeaponIcons();

        GetEnemies();
        
        gm.somethingIsMoving = false;

        //SetInfluenceTiles();

        UpdateInfluenceMap();
    }

    IEnumerator StartMovementItem(GameObject item, Unit enemy, int damageDealt)
    {
        gm.somethingIsMoving = true;

        Vector2 tilePos = enemy.transform.position;

        while (item.transform.position.x != tilePos.x || item.transform.position.y != tilePos.y)
        {
            item.transform.position = Vector2.MoveTowards(item.transform.position, tilePos, moveSpeed * 2 * Time.deltaTime);
            yield return null;
        }

        layer = (int)tilePos.y + 1;
        Destroy(item);

        if (damageDealt > 0)
        {
            DamageIcon instance = Instantiate(enemy.damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(damageDealt);
            enemy.health -= damageDealt;
            enemy.UpdateKingHealth();
        }

        if (enemy.health <= 0)
        {
            Destroy(enemy.gameObject);
            GetWalkableTiles();
        }

        if (health <= 0)
        {
            gm.ResetTiles();
            Destroy(gameObject);
        }

        gm.somethingIsMoving = false;

        UpdateInfluenceMap();
        //gm.somethingIsMoving = false;
    }

    public void UpdateInfluenceMap()
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.ResetInfluences();
        }

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            //unit.weaponIcon.SetActive(false);
            unit.SetInfluenceTiles();
        }
    }
    [System.Serializable]
    public class ObjetosAtaqueYCuracion
    {
        public GameObject flecha;
        public GameObject curacion;
    }
}