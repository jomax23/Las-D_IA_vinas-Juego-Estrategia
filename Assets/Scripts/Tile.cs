using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public float influenceValue;

    private SpriteRenderer rend;
    public Sprite[] tileGraphics;
    public float hoverAmount;

    public LayerMask obstacleLayer;

    public Color highlightedColor;
    public bool isWalkable;

    public GameObject obstacleTree;
    public GameObject obstacleRock;

    public Vector3 worldPosition;
    public int posX;
    public int posY;

    private GameMaster gm;

    public Color creatableColor;
    public bool isCreatable;

    [HideInInspector]
    public bool canCreateObstacle = true;
    public bool isObstacleUncreatable = false;

    public int gCost;
    public int hCost;
    public Tile parent;

    public bool visited;

    private Pathfinding pathFinding;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = tileGraphics[Random.Range(0, tileGraphics.Length)];

        gm = FindObjectOfType<GameMaster>();
        worldPosition = transform.position;

        pathFinding = FindObjectOfType<Pathfinding>();
    }

    public int fCost => gCost + hCost;

    private void OnMouseEnter() => transform.localScale += Vector3.one * hoverAmount;

    private void OnMouseExit() => transform.localScale -= Vector3.one * hoverAmount;

    public bool IsClear() => Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer) == null;


    public void Highlight()
    {
        if (pathFinding != null)
        {
            rend.color = highlightedColor;
            
            isWalkable = true;
            //visited = true;
        }
        else
        {
            rend.color = Color.black;
        }

        // Reset pathfinding-related data
        pathFinding.target = null;
        pathFinding.seekerUnit = null;
        pathFinding.seeker = null;
        pathFinding.pathCounter = 0;
    }

    public void HighlightInfluence(int value)
    {
        if (pathFinding != null)
        {
            rend.color = highlightedColor / value;

            isWalkable = true;

            //rend.color = highlightedColor;
            //visited = true;
        }
        else
        {
            rend.color = Color.black;
        }

        // Reset pathfinding-related data
        pathFinding.target = null;
        pathFinding.seekerUnit = null;
        pathFinding.seeker = null;
        pathFinding.pathCounter = 0;
    }

    public void HighlightAttackTile(int enemyNumber)
    {
            rend.color = Color.red;
    }

    public void HighlightHealthTile(int allyNumber)
    {
        rend.color = Color.blue;
    }

    public void Reset()
    {
        rend.color = Color.white;
        isWalkable = false;
        isCreatable = false;
    }

    public void SetCreatable()
    {
        rend.color = creatableColor;
        isCreatable = true;
    }

    private void OnMouseDown()
    {
        if (isWalkable && gm.selectedUnit != null)
        {
            gm.selectedUnit.Move(transform.position);
        }
        else if (isCreatable)
        {
            CreateBarrackItem();
        }
    }

    private void CreateBarrackItem()
    {
        BarrackItem item = Instantiate(gm.purchasedItem, new Vector3(transform.position.x, transform.position.y, -5), Quaternion.identity);
        gm.ResetTiles();

        Unit unit = item.GetComponent<Unit>();
        if (unit != null)
        {
            unit.hasMoved = true;
            unit.hasAttacked = true;
        }
    }
}