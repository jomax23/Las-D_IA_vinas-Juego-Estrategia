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

    private GameMaster gm;

    void Awake()
    {
        grid = GetComponent<FieldObstacleGeneration>();
        gm = GameObject.Find("/GameMaster").GetComponent<GameMaster>();
    }
    /*
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
    */

    public void FindPathMovement(Unit unit, Vector3 startPos, Vector3 targetPos)
    {
        Tile startTile = grid.arrayTile[(int)startPos.x, (int)startPos.y];
        Tile targetTile = grid.arrayTile[(int)targetPos.x, (int)targetPos.y];

        if(unit.unitType == Unit.UnitType.Flyer)
        {
            pathMovement = new List<Tile>();
            pathMovement.Add(startTile);
            pathMovement.Add(targetTile);
        }

        else
        {
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
                    if(unit.unitType == Unit.UnitType.Flyer)
                    {
                        if (closedSet.Contains(neighbour))
                        {
                            continue;
                        }
                    }

                    else if(!neighbour.IsClear() || neighbour.hasUnit || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newCostToNeighbour = tile.gCost + GetDistance(tile, neighbour);
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

    }
    /*
    public void FindPathMovementAI(Unit unit, Vector3 startPos, Vector3 targetPos)
    {
        Tile startTile = grid.arrayTile[(int)startPos.x, (int)startPos.y];
        Tile targetTile = grid.arrayTile[(int)targetPos.x, (int)targetPos.y];

        if (unit.unitType == Unit.UnitType.Flyer)
        {
            pathMovement = new List<Tile>();
            pathMovement.Add(startTile);
            pathMovement.Add(targetTile);
        }

        else
        {
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
                    if (unit.unitType == Unit.UnitType.Flyer)
                    {
                        if (closedSet.Contains(neighbour))
                        {
                            continue;
                        }
                    }

                    else if (!neighbour.IsClear() || neighbour.hasUnit || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newCostToNeighbour = tile.gCost * tile.influenceValue + GetDistance(tile, neighbour);
                    if (newCostToNeighbour < neighbour.gCost * neighbour.influenceValue || !openSet.Contains(neighbour))
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

    }
    */
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
        Queue<Tile> queue = new Queue<Tile>();
        Queue<Tile> auxiliarQueue = new Queue<Tile>();

        int myCol;
        int myRow;

        //SOLO PARA LOS TILES VECINOS A LA UNIDAD

        if (unit.unitType == Unit.UnitType.Flyer)
        {
            //SOLO PARA LOS TILES VECINOS A LA UNIDAD

            for (int i = 1; i <= unit.tileSpeed; i++)
            {
                myCol = (int)unit.transform.position.x - i;
                myRow = (int)unit.transform.position.y + i;

                if (myRow >= 0 && myRow < unit.myField.altura)
                {
                    for (int col = myCol; col < (int)unit.transform.position.x + i; col++)
                    {
                        if (col >= 0 && col < unit.myField.anchura && !unit.myField.arrayTile[col, myRow].hasUnit)// && myField.arrayTile[col, myRow].IsClear())
                        {
                            if(!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[col, myRow].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[col, myRow]);
                        }

                    }
                }

                myCol = (int)unit.transform.position.x + i;

                if (myCol >= 0 && myCol < unit.myField.anchura)
                {
                    for (int row = myRow; row > (int)unit.transform.position.y - i; row--)
                    {
                        if (row >= 0 && row < unit.myField.altura && !unit.myField.arrayTile[myCol, row].hasUnit)// && myField.arrayTile[myCol, row].IsClear())
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myCol, row].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[myCol, row]);
                        }
                    }
                }

                myRow = (int)unit.transform.position.y - i;

                if (myRow >= 0 && myRow < unit.myField.altura)
                {
                    for (int col = myCol; col > (int)unit.transform.position.x - i; col--)
                    {
                        if (col >= 0 && col < unit.myField.anchura && !unit.myField.arrayTile[col, myRow].hasUnit)// && myField.arrayTile[col, myRow].IsClear())
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[col, myRow].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[col, myRow]);
                        }
                    }
                }

                myCol = (int)unit.transform.position.x - i;

                if (myCol >= 0 && myCol < unit.myField.anchura)
                {
                    for (int row = myRow; row < (int)unit.transform.position.y + i; row++)
                    {
                        if (row >= 0 && row < unit.myField.altura && !unit.myField.arrayTile[myCol, row].hasUnit)// && myField.arrayTile[myCol, row].IsClear())
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myCol, row].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[myCol, row]);
                        //Debug.Log("COL : " + myCol + ", ROW : " + row + ", VALOR DE INFLUENCIA: " + myField.arrayTile[myCol, row].influenceValue);
                        }
                    }
                }

                //myRow = (int)transform.position.y + i;
            }

                //myField.decisionMakingValues.kingsInfo.myKingCoord = 
        }

        else
        {
            if (1 <= unit.tileSpeed)
            {
                myCol = (int)unit.transform.position.x - 1;
                myRow = (int)unit.transform.position.y + 1;

                if (myRow >= 0 && myRow < unit.myField.altura)
                {
                    for (int col = myCol; col < (int)unit.transform.position.x + 1; col++)
                    {
                        if (col >= 0 && col < unit.myField.anchura && unit.myField.arrayTile[col, myRow].IsClear() && !unit.myField.arrayTile[col, myRow].isWalkable)
                        {
                            queue.Enqueue(unit.myField.arrayTile[col, myRow]);
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[col, myRow].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[col, myRow]);
                        }

                    }
                }

                myCol = (int)unit.transform.position.x + 1;

                if (myCol >= 0 && myCol < unit.myField.anchura)
                {
                    for (int row = myRow; row > (int)unit.transform.position.y - 1; row--)
                    {
                        if (row >= 0 && row < unit.myField.altura && unit.myField.arrayTile[myCol, row].IsClear() && !unit.myField.arrayTile[myCol, row].isWalkable)
                        {
                            queue.Enqueue(unit.myField.arrayTile[myCol, row]);
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myCol, row].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[myCol, row]);
                        }
                    }
                }

                myRow = (int)unit.transform.position.y - 1;

                if (myRow >= 0 && myRow < unit.myField.altura)
                {
                    for (int col = myCol; col > (int)unit.transform.position.x - 1; col--)
                    {
                        if (col >= 0 && col < unit.myField.anchura && unit.myField.arrayTile[col, myRow].IsClear() && !unit.myField.arrayTile[col, myRow].isWalkable)
                        {
                            queue.Enqueue(unit.myField.arrayTile[col, myRow]);
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[col, myRow].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[col, myRow]);
                        }
                    }
                }

                myCol = (int)unit.transform.position.x - 1;

                if (myCol >= 0 && myCol < unit.myField.anchura)
                {
                    //Debug.Log("COL : " + myCol);
                    for (int row = myRow; row < (int)unit.transform.position.y + 1; row++)
                    {
                        if (row >= 0 && row < unit.myField.altura && unit.myField.arrayTile[myCol, row].IsClear() && !unit.myField.arrayTile[myCol, row].isWalkable)
                        {
                            queue.Enqueue(unit.myField.arrayTile[myCol, row]);
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myCol, row].Highlight();
                            unit.reachableTileList.Add(unit.myField.arrayTile[myCol, row]);
                        }
                    }
                }
            }

            for (int i = 2; i <= unit.tileSpeed; i++)
            {
                Tile myTile;

                while (queue.Count > 0)
                {
                    myTile = queue.Peek();
                    queue.Dequeue();

                    int myTileX = (int)myTile.transform.position.x;
                    int myTileY = (int)myTile.transform.position.y;

                    //Debug.Log("COL : " + myTileX + ", ROW : " + myTileY);

                    //ESQUINA SUP IZQUIERDA
                    if (myTileX - 1 >= 0 && myTileY + 1 < unit.myField.altura)
                    {
                        if (unit.myField.arrayTile[myTileX - 1, myTileY + 1].IsClear() && !unit.myField.arrayTile[myTileX - 1, myTileY + 1].isWalkable && (myTileX - 1 != unit.transform.position.x || myTileY + 1 != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX - 1, myTileY + 1].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX - 1, myTileY + 1]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX - 1, myTileY + 1]);
                        }

                    }

                    //ARRIBA
                    if (myTileY + 1 < unit.myField.altura)
                    {
                        if (unit.myField.arrayTile[myTileX, myTileY + 1].IsClear() && !unit.myField.arrayTile[myTileX, myTileY + 1].isWalkable && (myTileX != unit.transform.position.x || myTileY + 1 != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX, myTileY + 1].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX, myTileY + 1]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX, myTileY + 1]);
                        }

                    }

                    //ESQUINA SUP DERECHA
                    if (myTileX + 1 < unit.myField.anchura && myTileY + 1 < unit.myField.altura)
                    {
                        if (unit.myField.arrayTile[myTileX + 1, myTileY + 1].IsClear() && !unit.myField.arrayTile[myTileX + 1, myTileY + 1].isWalkable && (myTileX + 1 != unit.transform.position.x || myTileY + 1 != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX + 1, myTileY + 1].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX + 1, myTileY + 1]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX + 1, myTileY + 1]);
                        }

                    }

                    //DERECHA
                    if (myTileX + 1 < unit.myField.anchura)
                    {
                        if (unit.myField.arrayTile[myTileX + 1, myTileY].IsClear() && !unit.myField.arrayTile[myTileX + 1, myTileY].isWalkable && (myTileX + 1 != unit.transform.position.x || myTileY != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX + 1, myTileY].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX + 1, myTileY]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX + 1, myTileY]);
                        }

                    }

                    //ESQUINA INF DERECHA
                    if (myTileX + 1 < unit.myField.anchura && myTileY - 1 >= 0)
                    {
                        if (unit.myField.arrayTile[myTileX + 1, myTileY - 1].IsClear() && !unit.myField.arrayTile[myTileX + 1, myTileY - 1].isWalkable && (myTileX + 1 != unit.transform.position.x || myTileY - 1 != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX + 1, myTileY - 1].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX + 1, myTileY - 1]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX + 1, myTileY - 1]);
                        }

                    }

                    //ABAJO
                    if (myTileY - 1 >= 0)
                    {
                        if (unit.myField.arrayTile[myTileX, myTileY - 1].IsClear() && !unit.myField.arrayTile[myTileX, myTileY - 1].isWalkable && (myTileX != unit.transform.position.x || myTileY - 1 != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX, myTileY - 1].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX, myTileY - 1]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX, myTileY - 1]);
                        }

                    }

                    //ESQUINA INF IZQUIERDA
                    if (myTileX - 1 >= 0 && myTileY - 1 >= 0)
                    {
                        if (unit.myField.arrayTile[myTileX - 1, myTileY - 1].IsClear() && !unit.myField.arrayTile[myTileX - 1, myTileY - 1].isWalkable && (myTileX - 1 != unit.transform.position.x || myTileY - 1 != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX - 1, myTileY - 1].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX - 1, myTileY - 1]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX - 1, myTileY - 1]);
                        }
                    }

                    //IZQUIERDA
                    if (myTileX - 1 >= 0)
                    {
                        if (unit.myField.arrayTile[myTileX - 1, myTileY].IsClear() && !unit.myField.arrayTile[myTileX - 1, myTileY].isWalkable && (myTileX - 1 != unit.transform.position.x || myTileY != unit.transform.position.y))
                        {
                            if (!(gm.IAactive && gm.playerTurn == 2))
                                unit.myField.arrayTile[myTileX - 1, myTileY].Highlight();
                            auxiliarQueue.Enqueue(unit.myField.arrayTile[myTileX - 1, myTileY]);
                            unit.reachableTileList.Add(unit.myField.arrayTile[myTileX - 1, myTileY]);
                        }
                    }
                }
                /*
                foreach (Tile tile in auxiliarQueue)
                {
                    Debug.Log("AUXILIAR QUEUE IT " + i + " TILE -> COL : " + tile.transform.position.x + ", ROW : " + tile.transform.position.y);
                }
                */


                while (auxiliarQueue.Count > 0)
                {
                    queue.Enqueue(auxiliarQueue.Peek());
                    auxiliarQueue.Dequeue();
                }

            }
        }

        queue.Clear();
        auxiliarQueue.Clear();
    }
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