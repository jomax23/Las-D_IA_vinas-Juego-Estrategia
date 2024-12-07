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

    [HideInInspector]
    public List<Unit> enemiesInRange = new List<Unit>();

    [HideInInspector]
    public List<Unit> enemiesClose = new List<Unit>();

    [HideInInspector]
    public FieldObstacleGeneration myField;

    [HideInInspector]
    public List<Tile> reachableTileList = new List<Tile>();

    //private GameObject miItem;
    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        myField = FindObjectOfType<FieldObstacleGeneration>();
        pathFinding = FindObjectOfType<Pathfinding>();
        SetInfluenceTiles();

        /*
        if (unitType == UnitType.King && playerNumber == 1)
        {
            kingHealth = GameObject.Find("Health1 Text").GetComponent<Text>();
        }
        */
        if (unitType == UnitType.King)
        {
            if (playerNumber == 1)
            {
                myField.decisionMakingValues.kingsInfo.myKing = this;
                myField.decisionMakingValues.kingsInfo.myKingCoord = transform.position;
                kingHealth = GameObject.Find("Health1 Text").GetComponent<Text>();
                //ESTO LO HE PUESTO SOLO PARA QUE VEAIS EL VALOR DE LA CASILLA DEL IV EN EL QUE ESTA EL REY ALIADO, NO ES EL VALOR REAL DE SU TILE EL QUE SE MUESTRA EN EL INSPECTOR INICIALMENTE , DESPUES DE LA PRIMERA ACCION YA SE MUESTRA CORRECTAMENTE
                //myField.decisionMakingValues.kingsInfo.myKingTileIV = myField.arrayTile[(int)myField.decisionMakingValues.kingsInfo.myKingCoord.x, (int)myField.decisionMakingValues.kingsInfo.myKingCoord.y].influenceValue;
            }
            else
            {
                myField.decisionMakingValues.kingsInfo.enemyKing = this;
                myField.decisionMakingValues.kingsInfo.enemyKingCoord = transform.position;
                kingHealth = GameObject.Find("Health Text").GetComponent<Text>();
                //ESTO LO HE PUESTO SOLO PARA QUE VEAIS EL VALOR DE LA CASILLA DEL IV EN EL QUE ESTA EL REY ENEMIGO, NO ES EL VALOR REAL DE SU TILE EL QUE SE MUESTRA EN EL INSPECTOR INICIALMENTE , DESPUES DE LA PRIMERA ACCION YA SE MUESTRA CORRECTAMENTE
                //myField.decisionMakingValues.kingsInfo.enemyKingTileIV = myField.arrayTile[(int)myField.decisionMakingValues.kingsInfo.enemyKingCoord.x, (int)myField.decisionMakingValues.kingsInfo.enemyKingCoord.y].influenceValue;
            }
        }
        /*
        if(unitType == UnitType.Archer)
        {
            miItem = misObjetos.flecha;
        }
        */
        UpdateKingHealth();
    }

    public enum UnitType
    {
        King,
        Tank,
        Archer,
        Flyer
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
        else if (playerNumber == gm.playerTurn && (gm.playerTurn != 2 || !gm.IAactive))
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

    public void Attack(Unit enemy)
    {
        hasAttacked = true;

        int damageDealt = attackStat - enemy.defenseStat;

        if (unitType == UnitType.Archer)
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
                myField.arrayTile[(int)enemy.transform.position.x, (int)enemy.transform.position.y].hasUnit = false;
                Destroy(enemy.gameObject);
                GetWalkableTiles();
                myField.arrayTile[(int)enemy.transform.position.x, (int)enemy.transform.position.y].Reset();

            }

            UpdateInfluenceMap();
        }

        //gm.ResetTiles(); //Reset tiles es solo para los booleanos de walkable, creatable y el color del tile, por si se mata a un enemigo que estaba en ese tile


    }

    //ESTO ESTABA EN GetWalkableTiles() ANTES DEL CAMBIO EN LA FORMA DE AVERIGUAR LOS TILES ALCANZABLES 
    //List<Tile> reachableTileList = new List<Tile>();

    /*
    for (int i = (int)transform.position.x - tileSpeed; i <= transform.position.x + tileSpeed; i++)
    {
        for (int j = (int)transform.position.y - tileSpeed; j <= transform.position.y + tileSpeed; j++)
        {
            if (i >= 0 && j >= 0 && i < myField.anchura && j < myField.altura && myField.arrayTile[i, j].IsClear())
            {
                if ((int)transform.position.x != i || (int)transform.position.y != j)
                {
                    //Debug.Log(i + ", " + j);
                    //ESTA LISTA CREO QUE SE PODRIA QUITAR Y METERLE DIRECTAMENTE LO DE DENTRO DEL BUCLE FOR QUE TIENE ABAJO
                    reachableTileList.Add(myField.arrayTile[i, j]);
                }
            }
        }
    }


    foreach (Tile tile in reachableTileList)
    {
        pathFinding.FindPath(this, tile.transform.position);
    }
    */

    void GetWalkableTiles()
    {
        if (hasMoved) return;

        reachableTileList = new List<Tile>();

        if(unitType == UnitType.Flyer)
        {
            pathFinding.ReachableTilesDijkstra(this);
        }

        else
        {
            pathFinding.ReachableTilesDijkstra(this);
        }
        //pathFinding.ReachableTilesDijkstra(this);

        //ESTO ES POR SI QUEREIS VER LOS TILES QUE SE HAN A�ADIDO A LA LISTA REACHABLE TILES
        /*
        foreach(Tile tile in reachableTileList)
        {
            Debug.Log(tile.transform.position.x + ", " + tile.transform.position.y);
        }
        */
    }

    public void SetInfluenceTiles()
    {

        int myCol;
        int myRow;

        //SOLO PARA LOS TILES VECINOS A LA UNIDAD
        myCol = (int)transform.position.x - 1;
        myRow = (int)transform.position.y + 1;


        myField.arrayTile[(int)transform.position.x, (int)transform.position.y].influenceValue += influenceStrength;

        for (int i = 1; i <= influenceAreaRadius; i++)
        {
            myCol = (int)transform.position.x - i;
            myRow = (int)transform.position.y + i;

            if (myRow >= 0 && myRow < myField.altura)
            {
                for (int col = myCol; col < (int)transform.position.x + i; col++)
                {
                    if (col >= 0 && col < myField.anchura)// && myField.arrayTile[col, myRow].IsClear())
                    {
                        myField.arrayTile[col, myRow].influenceValue += influenceStrength / (i + 1);
                        //Debug.Log("COL : " + col + ", ROW : " + myRow + ", VALOR DE INFLUENCIA: " + myField.arrayTile[col, myRow].influenceValue);
                    }

                }
            }

            myCol = (int)transform.position.x + i;

            if (myCol >= 0 && myCol < myField.anchura)
            {
                for (int row = myRow; row > (int)transform.position.y - i; row--)
                {
                    if (row >= 0 && row < myField.altura)// && myField.arrayTile[myCol, row].IsClear())
                    {
                        myField.arrayTile[myCol, row].influenceValue += influenceStrength / (i + 1);
                        //Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                    }
                }
            }

            myRow = (int)transform.position.y - i;

            if (myRow >= 0 && myRow < myField.altura)
            {
                for (int col = myCol; col > (int)transform.position.x - i; col--)
                {
                    if (col >= 0 && col < myField.anchura)// && myField.arrayTile[col, myRow].IsClear())
                    {
                        myField.arrayTile[col, myRow].influenceValue += influenceStrength / (i + 1);
                        //Debug.Log("COL : " + col + ", ROW : " + myRow + ", VALOR DE INFLUENCIA: " + myField.arrayTile[col, myRow].influenceValue);
                    }
                }
            }

            myCol = (int)transform.position.x - i;

            if (myCol >= 0 && myCol < myField.anchura)
            {
                for (int row = myRow; row < (int)transform.position.y + i; row++)
                {
                    if (row >= 0 && row < myField.altura)// && myField.arrayTile[myCol, row].IsClear())
                    {
                        myField.arrayTile[myCol, row].influenceValue += influenceStrength / (i + 1);
                        //Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                    }
                }
            }

            //myRow = (int)transform.position.y + i;
        }

        //ACTUALIZAMOS VALOR DE LA INFLUENCIA QUE HAY EN LA CASILLA DEL REY, ESTO PODRIA CAMBIARSE POR LA VIDA DEL REY COMO CONDICION PARA TOMAR UNA DECISION U OTRA EN EL ARBOL DE DECISIONES Y NO USAR EL MAPA DE INFLUENCIAS AQUI,
        //SOLO LO HAGO PARA QUE SE VEA POR SI QUEREIS USARLO MAS ADELANTE PARA ALGO
        //myField.decisionMakingValues.myKingIV = myField.arrayTile[0, myField.altura / 2].influenceValue;

        //CONSIDERAMOS DE MOMENTO UNA CASILLA ARBITRARIA COMO MINIMO Y MAXIMO, EN  MI CASO HE ESCOGIDO LA CASILLA INICIAL DEL REY ALIADO, LA CUAL ESTA EN LA POSICION 0, altura / 2
        myField.decisionMakingValues.minIV = myField.arrayTile[0, myField.altura / 2].influenceValue;
        myField.decisionMakingValues.maxIV = myField.arrayTile[0, myField.altura / 2].influenceValue;

        for (int col = 0; col < myField.anchura; col++)
        {
            for (int row = 0; row < myField.altura; row++)
            {
                //CAMBIAR MINIMO
                if (myField.decisionMakingValues.minIV > myField.arrayTile[col, row].influenceValue)
                {
                    myField.decisionMakingValues.minIV = myField.arrayTile[col, row].influenceValue;
                    myField.decisionMakingValues.minIVCoord.x = col;
                    myField.decisionMakingValues.minIVCoord.y = row;
                }

                //CAMBIAR MAXIMO
                else if (myField.decisionMakingValues.maxIV < myField.arrayTile[col, row].influenceValue)
                {
                    myField.decisionMakingValues.maxIV = myField.arrayTile[col, row].influenceValue;
                    myField.decisionMakingValues.maxIVCoord.x = col;
                    myField.decisionMakingValues.maxIVCoord.y = row;
                }
            }
        }

        //myField.decisionMakingValues.kingsInfo.myKingCoord = 

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

            if (Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange/2 && Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange/2 && unit.playerNumber != gm.playerTurn && !hasAttacked)
            {
                enemiesClose.Add(unit);
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
        pathFinding.FindPathMovement(this, transform.position, new Vector3(tilePos.x, tilePos.y, myField.depth));
        StartCoroutine(StartMovement(tilePos));

        //gm.somethingIsMoving = false;
    }

    public void MoveAIUnit(Vector2 tileTarget)
    {
        gm.somethingIsMoving = true;
        //gm.SomethingIsMoving(true);
        gm.ResetTiles();
        pathFinding.FindPathMovement(this, transform.position, new Vector3(tileTarget.x, tileTarget.y, myField.depth));
        StartCoroutine(StartMovement(tileTarget));

        //gm.somethingIsMoving = false;
    }

    IEnumerator StartMovement(Vector2 tilePos)
    {
        myField.arrayTile[(int) transform.position.x, (int) transform.position.y].hasUnit = false;

        foreach (Tile tile in pathFinding.pathMovement)
        {
            while (transform.position.x != tile.transform.position.x || transform.position.y != tile.transform.position.y)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(tile.transform.position.x, tile.transform.position.y), moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        myField.arrayTile[(int) transform.position.x, (int) transform.position.y].hasUnit = true;


        hasMoved = true;
        //gm.somethingIsMoving = false;
        layer = (int)tilePos.y + 1;

        ResetWeaponIcons();

        GetEnemies();
        
        gm.somethingIsMoving = false;

        //SetInfluenceTiles();
        UpdateInfluenceMap();

        myField.decisionMakingValues.kingsInfo.SetKingsIVCoords();

        //ESTO LO HE PUESTO SOLO PARA QUE VEAIS EL VALOR DE LA CASILLA DEL IV EN EL QUE ESTA EL REY ALIADO
        myField.decisionMakingValues.kingsInfo.myKingTileIV = myField.arrayTile[(int)myField.decisionMakingValues.kingsInfo.myKingCoord.x, (int)myField.decisionMakingValues.kingsInfo.myKingCoord.y].influenceValue;
        //ESTO LO HE PUESTO SOLO PARA QUE VEAIS EL VALOR DE LA CASILLA DEL IV EN EL QUE ESTA EL REY ENEMIGO
        myField.decisionMakingValues.kingsInfo.enemyKingTileIV = myField.arrayTile[(int)myField.decisionMakingValues.kingsInfo.enemyKingCoord.x, (int)myField.decisionMakingValues.kingsInfo.enemyKingCoord.y].influenceValue;
    }

    //DAVID, HE HECHO ESTA FUNCION, CREO QUE IRA BIEN, SI NO ESTA BIEN, EL PLANTEAMIENTO CREO QUE SERVIRA, Y CREO QUE DE MOMENTO SOLO NO IRA BIEN SI EL TARGET ESTA RODEADO POR ENEMIGOS Y/O OBSTACULOS
    
    IEnumerator StartMovementAI(Vector2 tilePos)
    {
        myField.arrayTile[(int)transform.position.x, (int)transform.position.y].hasUnit = false;

        int contadorPasos = 0;

        foreach (Tile tile in pathFinding.pathMovement)
        {      
            if(contadorPasos < tileSpeed && (transform.position.x != pathFinding.pathMovement[pathFinding.pathMovement.Count - 2].transform.position.x || transform.position.y != pathFinding.pathMovement[pathFinding.pathMovement.Count - 2].transform.position.y))
            {
                while (transform.position.x != pathFinding.pathMovement[pathFinding.pathMovement.Count - 2].transform.position.x || transform.position.y != pathFinding.pathMovement[pathFinding.pathMovement.Count - 2].transform.position.y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(tile.transform.position.x, tile.transform.position.y), moveSpeed * Time.deltaTime);
                    
                    yield return null;
                }

                contadorPasos++;
            }

            else
            {
                break;
            }
        }       

        myField.arrayTile[(int)transform.position.x, (int)transform.position.y].hasUnit = true;


        hasMoved = true;
        //gm.somethingIsMoving = false;
        layer = (int)tilePos.y + 1;

        ResetWeaponIcons();

        GetEnemies();

        gm.somethingIsMoving = false;

        //SetInfluenceTiles();
        UpdateInfluenceMap();

        myField.decisionMakingValues.kingsInfo.SetKingsIVCoords();

        //ESTO LO HE PUESTO SOLO PARA QUE VEAIS EL VALOR DE LA CASILLA DEL IV EN EL QUE ESTA EL REY ALIADO
        myField.decisionMakingValues.kingsInfo.myKingTileIV = myField.arrayTile[(int)myField.decisionMakingValues.kingsInfo.myKingCoord.x, (int)myField.decisionMakingValues.kingsInfo.myKingCoord.y].influenceValue;
        //ESTO LO HE PUESTO SOLO PARA QUE VEAIS EL VALOR DE LA CASILLA DEL IV EN EL QUE ESTA EL REY ENEMIGO
        myField.decisionMakingValues.kingsInfo.enemyKingTileIV = myField.arrayTile[(int)myField.decisionMakingValues.kingsInfo.enemyKingCoord.x, (int)myField.decisionMakingValues.kingsInfo.enemyKingCoord.y].influenceValue;
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
            myField.arrayTile[(int)enemy.transform.position.x, (int)enemy.transform.position.y].hasUnit = false;
            Destroy(enemy.gameObject);
            GetWalkableTiles();
            myField.arrayTile[(int)enemy.transform.position.x, (int)enemy.transform.position.y].Reset();
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

    public bool GetPathToP1KingAI()
    {
        return false;
        //ESTO DE AQUI ES SIMILAR A LO QUE HARIA GETWALKABLETILES
        /*
        if (hasMoved) return false;

        List<Tile> reachableTileList = new List<Tile>();


        for (int i = (int)transform.position.x - tileSpeed; i <= transform.position.x + tileSpeed; i++)
        {
            for (int j = (int)transform.position.y - tileSpeed; j <= transform.position.y + tileSpeed; j++)
            {
                if (i >= 0 && j >= 0 && i < myField.anchura && j < myField.altura && myField.arrayTile[i, j].IsClear())
                {
                    if ((int)transform.position.x != i || (int)transform.position.y != j)
                    {
                        //Debug.Log(i + ", " + j);
                        //ESTA LISTA CREO QUE SE PODRIA QUITAR Y METERLE DIRECTAMENTE LO DE DENTRO DEL BUCLE FOR QUE TIENE ABAJO
                        reachableTileList.Add(myField.arrayTile[i, j]);
                    }
                }
            }
        }


        foreach (Tile tile in reachableTileList)
        {
            pathFinding.FindPath(this, tile.transform.position);
        }

        
        pathFinding.FindPathMovement(transform.position, new Vector3(myField.decisionMakingValues.kingsInfo.enemyKingCoord.x, myField.decisionMakingValues.kingsInfo.enemyKingCoord.y, myField.depth));


        if (reachableTileList.Count == 0)
        {
            return false;
        }

        return true;
        */
    }

    public bool GetEnemiesAI()
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

                //myField.arrayTile[(int)unit.transform.position.x, (int)unit.transform.position.y].HighlightAttackTile(this.playerNumber);
            }
        }

        if (enemiesInRange.Count == 0)
        {
            return false;
        }
            
        return true;
    }

    public bool CompareMyTileInfluenceValueAI()
    {
        //EL CERO SE PODRIA SUSTITUIR POR OTRO VALOR, PARA QUE NO SE ACOJONE TAN FACILMENTE EN FUNCION DE CIERTOS VALORES Y HUYA
        if (myField.arrayTile[(int)transform.position.x, (int)transform.position.y].influenceValue <= 0) //ESTAMOS SEGUROS, PODEMOS SEGUIR AVANZANDO O ATACANDO
        {
            return true;
        }

        return false;
    }
}