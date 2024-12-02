using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Pathfinding : MonoBehaviour
{

    public Transform seeker, target;
    FieldObstacleGeneration grid;
    public Unit seekerUnit;

    public int pathCounter = 0;

    public List<Tile> pathMovement;

    void Awake()
    {
        grid = GetComponent<FieldObstacleGeneration>();
    }

    public void FindPath(Unit unit, Vector3 targetPos)
    {
        //pathCounter = 0;
        Tile startTile = grid.arrayTile[(int)unit.transform.position.x, (int)unit.transform.position.y];
        Tile targetTile = grid.arrayTile[(int)targetPos.x, (int)targetPos.y];

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile tile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < tile.fCost || openSet[i].fCost == tile.fCost)
                {
                    if (openSet[i].hCost < tile.hCost)
                        tile = openSet[i];
                }
            }

            openSet.Remove(tile);
            closedSet.Add(tile);

            if (tile == targetTile)
            {
                RetracePath(startTile, targetTile, unit.tileSpeed);
                return;
            }

            foreach (Tile neighbour in grid.GetNeighbours(tile))
            {
                if (!neighbour.IsClear() || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = tile.gCost + GetDistance(tile, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetTile);
                    neighbour.parent = tile;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }

                    else if (tile != targetTile && pathCounter >= unit.tileSpeed)
                    {
                        pathCounter = 0;
                        return;
                    }
                }
            }
        }
    }

    void RetracePath(Tile startTile, Tile endTile, int tileSpeed)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            pathCounter++;
            currentTile = currentTile.parent;
        }

        
        path.Reverse();

        pathCounter = path.Count;

        if(pathCounter <= tileSpeed)
        {
            endTile.Highlight();
        }

        seekerUnit = null;
        seeker = null;
        target = null;
    }

    public void FindPathMovement(Vector3 startPos, Vector3 targetPos)
    {
        Tile startTile = grid.arrayTile[(int)startPos.x, (int)startPos.y];
        Tile targetTile = grid.arrayTile[(int)targetPos.x, (int)targetPos.y];

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile tile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < tile.fCost || openSet[i].fCost == tile.fCost)
                {
                    if (openSet[i].hCost < tile.hCost)
                        tile = openSet[i];
                }
            }

            openSet.Remove(tile);
            closedSet.Add(tile);

            if (tile == targetTile)
            {
                RetracePathMovement(startTile, targetTile);
                return;
            }

            foreach (Tile neighbour in grid.GetNeighbours(tile))
            {
                if (!neighbour.IsClear() || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = tile.gCost + GetDistance(tile, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetTile);
                    neighbour.parent = tile;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePathMovement(Tile startTile, Tile endTile)
    {
        pathMovement = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            pathMovement.Add(currentTile);
            currentTile = currentTile.parent;
        }
        pathMovement.Reverse();
        /*
        foreach (Tile tile in pathMovement)
        {
            Debug.Log(tile.transform.position.x + ", " + tile.transform.position.y);
        }
        */
    }

    int GetDistance(Tile tileA, Tile tileB)
    {
        int dstX = Mathf.Abs(tileA.posX - tileB.posX);
        int dstY = Mathf.Abs(tileA.posY - tileB.posY);

        // Si el movimiento es diagonal
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // Movimiento diagonal (usamos 14 para las diagonales)
        return 14 * dstX + 10 * (dstY - dstX);  // Movimiento ortogonal (usamos 10 para los movimientos ortogonales)
    }
}