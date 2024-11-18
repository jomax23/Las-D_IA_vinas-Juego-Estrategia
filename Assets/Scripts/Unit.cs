using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool selected;

    private GameMaster gm;
    private Pathfinding pathFinding;

    public int tileSpeed;
    public bool hasMoved;
    public float moveSpeed;

    public int playerNumber;
    public int attackRange;

    private List<Unit> enemiesInRange = new List<Unit>();

    public bool hasAttacked;
    public GameObject weaponIcon;

    public int health;
    public int attackStat;
    public int defenseStat;

    public DamageIcon damageIcon;
    public Text kingHealth;

    public bool isKing;
    public int layer = 0;

    private FieldObstacleGeneration myField;
    private List<Tile> myPathToObjective;

    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        myField = FindObjectOfType<FieldObstacleGeneration>();
        pathFinding = FindObjectOfType<Pathfinding>();
        UpdateKingHealth();
    }

    public void UpdateKingHealth()
    {
        if (isKing)
        {
            // Un-comment this line if you want to display king's health in the UI
            // kingHealth.text = health.ToString();
        }
    }

    private void OnMouseDown()
    {
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
    }

    void GetWalkableTiles()
    {
        if (hasMoved) return;

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if (transform.position.x == -1)
            {
                Debug.Log("POSICION DEL TILE ES -1");
            }

            if (transform.position.x == myField.anchura)
            {
                Debug.Log("POSICION DEL TILE ES ANCHURA");
            }

            if (Mathf.Abs(transform.position.x - tile.transform.position.x) <= tileSpeed && Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed && tile.IsClear())
            {
                pathFinding.seekerUnit = this;
                pathFinding.seeker = transform;
                pathFinding.target = tile.transform;

                pathFinding.FindPath(transform.position, tile.transform.position);

                if (pathFinding.pathCounter <= tileSpeed)
                {
                    tile.Highlight();
                }
            }
        }
    }

    public void GetEnemies()
    {
        enemiesInRange.Clear();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            float distance = Vector2.Distance(transform.position, unit.transform.position);
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange && Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange && unit.playerNumber != gm.playerTurn && !hasAttacked)
            {
                enemiesInRange.Add(unit);
                unit.weaponIcon.SetActive(true);

                myField.arrayTile[(int)unit.transform.position.x, (int)unit.transform.position.y].HighlightAttackTile(this.playerNumber);
            }
        }
    }

    void ResetWeaponIcons()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }

    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        pathFinding.FindPathMovement(transform.position, new Vector3(tilePos.x, tilePos.y, myField.depth));
        StartCoroutine(StartMovement(tilePos));
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

        layer = (int)tilePos.y;

        ResetWeaponIcons();

        GetEnemies();
    }
}