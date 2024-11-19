using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public void FindPath(Vector3 startPos, Vector3 targetPos)
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
                RetracePath(startTile, targetTile);
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

    void RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        
        path.Reverse();
        //Debug.Log(path.Count);
        pathCounter = path.Count;

        //Debug.Log("CAMINO");
        /*
        foreach (Tile tile in path)
        {
            Debug.Log(tile.transform.position.x + ", " + tile.transform.position.y);
        }
        */

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