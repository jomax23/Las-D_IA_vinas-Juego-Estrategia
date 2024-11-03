using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Unit : MonoBehaviour
{
    public bool selected;

    GameMaster gm;

    public int tileSpeed;
    public bool hasMoved;

    public float moveSpeed;

    public int playerNumber;

    public int attackRange;

    List<Unit> enemiesInRange = new List<Unit>();

    public bool hasAttacked;

    public GameObject weaponIcon;

    public int health;

    public int attackDamage;

    public int defenseDamage;

    public int armor;

    public DamageIcon damageIcon;

    public Text kingHealth;

    public bool isKing;

    public int layer = 0;

    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        UpdateKingHealth();
    }

    public void UpdateKingHealth()
    {
        if (isKing == true)
        {
            kingHealth.text = health.ToString();
        }
    }

    private void OnMouseDown()
    {
        ResetWeaponIcons();

        if(selected == true)
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
        }

        else
        {
            if(playerNumber == gm.playerTurn)
            {
                if(gm.selectedUnit != null)
                {
                    gm.selectedUnit.selected = false;
                }

                selected = true;
                gm.selectedUnit = this;

                gm.ResetTiles();
                GetEnemies();
                GetWalkableTiles();
            }
        }

        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit unit = col.GetComponent<Unit>();

        if(gm.selectedUnit != null)
        {
            //Debug.Log("HOLA");
            if (gm.selectedUnit.enemiesInRange.Contains(unit) && gm.selectedUnit.hasAttacked == false)
            {
                gm.selectedUnit.Attack(unit);
            }
        }

    }

    void Attack(Unit enemy)
    {
        hasAttacked = true;

        int enemyDamage = attackDamage - enemy.armor;

        int myDamage = enemy.defenseDamage - armor;

        if(enemyDamage >= 1)
        {
            DamageIcon instance = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(attackDamage);

            enemy.health -= enemyDamage;

            enemy.UpdateKingHealth();
        }

        if(myDamage >= 1)
        {
            DamageIcon instance = Instantiate(damageIcon, transform.position, Quaternion.identity);
            instance.Setup(defenseDamage);

            health -= myDamage;

            enemy.UpdateKingHealth();
        }

        if(enemy.health <= 0)
        {
            Destroy(enemy.gameObject);
            GetWalkableTiles();
        }

        if(health <= 0)
        {
            gm.ResetTiles();
            Destroy(this.gameObject);
        }
    }

    void GetWalkableTiles()
    {
        if(hasMoved == true)
        {
            return;
        }

        foreach(Tile tile in FindObjectsOfType<Tile>())
        {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x)  + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            {
                if(tile.IsClear() == true)
                {
                    tile.Highlight();
                    //Debug.Log("mensaje");
                }
            }
        }
    }

    void GetEnemies()
    {
        enemiesInRange.Clear();

        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange)
            {
                if(unit.playerNumber != gm.playerTurn && hasAttacked == false)
                {
                    enemiesInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }
            }
        }
    }

    public void ResetWeaponIcons()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
            /*
            if (unit.weaponIcon != null)
            {
                unit.weaponIcon.SetActive(false);
            }
            */
        }
    }
    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(tilePos));
    }

    IEnumerator StartMovement(Vector2 tilePos) {

        while(transform.position.x != tilePos.x)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(tilePos.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;

        }

        while (transform.position.y != tilePos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, tilePos.y), moveSpeed * Time.deltaTime);

            yield return null;

        }

        hasMoved = true;

        layer = (int)tilePos.y;

        ResetWeaponIcons();

        GetEnemies();
    }
}
