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

    //---------------------------------------------------------------------------------------------------------------------
    public void ReachableTilesDijkstra(Unit unit)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        Queue<Vector2> queue = new Queue<Vector2>();
        int myCol;
        int myRow;

        //queue.Enqueue(unit.myField.arrayTile[(int)unit.transform.position.x, (int)unit.transform.position.y]);  // Añadimos el nodo de inicio a la cola
        myCol = (int)unit.transform.position.x - 1;
        myRow = (int)unit.transform.position.y + 1;

        //SOLO PARA LOS TILES VECINOS A LA UNIDAD

        for (int i = 1; i <= unit.tileSpeed; i++)
        {
            
            myCol = (int)transform.position.x - i;
            myRow = (int)transform.position.y + i;
            
            if(i == 1)
            {
                if (myRow >= 0 && myRow < myField.altura)
                {
                    for (int col = myCol; col < (int)transform.position.x + i; col++)
                    {
                        if (col >= 0 && col < myField.anchura && myField.arrayTile[col, myRow].IsClear())// && myField.arrayTile[col, myRow].IsClear())
                        {
                            //myField.arrayTile[col, myRow].influenceValue += influenceStrength / (i + 1);
                            //Debug.Log("COL : " + col + ", ROW : " + myRow + ", VALOR DE INFLUENCIA: " + myField.arrayTile[col, myRow].influenceValue);
                            queue.Enqueue(col, myRow);
                            unit.myField.arrayTile[col, myRow].Highlight();
                        }

                    }
                }

                myCol = (int)transform.position.x + i;

                if (myCol >= 0 && myCol < myField.anchura)
                {
                    for (int row = myRow; row > (int)transform.position.y - i; row--)
                    {
                        if (row >= 0 && row < myField.altura && myField.arrayTile[myCol, row].IsClear())// && myField.arrayTile[myCol, row].IsClear())
                        {
                            //myField.arrayTile[myCol, row].influenceValue += influenceStrength / (i + 1);
                            //Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                            queue.Enqueue(myCol, row);
                            myField.arrayTile[myCol, row].Highlight();
                        }
                    }
                }

                myRow = (int)transform.position.y - i;

                if (myRow >= 0 && myRow < myField.altura)
                {
                    for (int col = myCol; col > (int)transform.position.x - i; col--)
                    {
                        if (col >= 0 && col < myField.anchura && myField.arrayTile[col, myRow].IsClear())// && myField.arrayTile[col, myRow].IsClear())
                        {
                            //myField.arrayTile[col, myRow].influenceValue += influenceStrength / (i + 1);
                            //Debug.Log("COL : " + col + ", ROW : " + myRow + ", VALOR DE INFLUENCIA: " + myField.arrayTile[col, myRow].influenceValue);
                            queue.Enqueue(col, myRow);
                            unit.myField.arrayTile[col, myRow].Highlight();
                        }
                    }
                }

                myCol = (int)transform.position.x - i;

                if (myCol >= 0 && myCol < myField.anchura)
                {
                    for (int row = myRow; row < (int)transform.position.y + i; row++)
                    {
                        if (row >= 0 && row < myField.altura && myField.arrayTile[myCol, row].IsClear())// && myField.arrayTile[myCol, row].IsClear())
                        {
                            //myField.arrayTile[myCol, row].influenceValue += influenceStrength / (i + 1);
                            //Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                            queue.Enqueue(myCol, row);
                            unit.myField.arrayTile[myCol, row].Highlight();
                        }
                    }
                }
            }

            while(queue.Count != 0)
            {
                Vector2 miVector = queue.Dequeue();
                
            }

            //myRow = (int)transform.position.y + i;
        }

        //ACTUALIZAMOS VALOR DE LA INFLUENCIA QUE HAY EN LA CASILLA DEL REY, ESTO PODRIA CAMBIARSE POR LA VIDA DEL REY COMO CONDICION PARA TOMAR UNA DECISION U OTRA EN EL ARBOL DE DECISIONES Y NO USAR EL MAPA DE INFLUENCIAS AQUI,
        //SOLO LO HAGO PARA QUE SE VEA POR SI QUEREIS USARLO MAS ADELANTE PARA ALGO
        //myField.decisionMakingValues.myKingIV = myField.arrayTile[0, myField.altura / 2].influenceValue;

        //CONSIDERAMOS DE MOMENTO UNA CASILLA ARBITRARIA COMO MINIMO Y MAXIMO, EN  MI CASO HE ESCOGIDO LA CASILLA INICIAL DEL REY ALIADO, LA CUAL ESTA EN LA POSICION 0, altura / 2
        

        //-----------------------------------------------------------------------
        /*
        

        queue.Enqueue(start);  // Añadimos el nodo de inicio a la cola

        while (queue.Count > 0)
        {
            int currentNode = queue.Dequeue();  // Extraemos el primer nodo de la cola

             // Revisamos los vecinos del nodo actual
            queue.Enqueue(neighborNode);  // Añadimos el vecino a la cola

        }
          // Ordenamos la cola según las distancias para simular una cola de prioridad
        var sortedQueue = new Queue<int>(queue.OrderBy(node => distances[node]));
        queue = sortedQueue;
        */
    }
    
}