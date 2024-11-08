using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FieldObstacleGeneration : MonoBehaviour
{
    // Start is called before the first frame update

    public int numColumnas; //SERIA X ( DESPLAZAMIENTO COLUMNA )
    public int numFilas; //SERIA Y ( ALTURA FILA )

    private Tile[,] arrayTile;
    public List<Tile> listOfTiles = new List<Tile>();

    public List<Tile> listOfTilesPlayerCharacters = new List<Tile>();
    public List<Tile> listOfTilesEnemyCharacters = new List<Tile>();

    int randObstacleIndex;

    public int ObstacleTreeAmount = 3;
    public int ObstacleRockAmount = 3;

    public GameObject tilePrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;

    private bool esDivisible = false;


    void Awake()
    {
        arrayTile = new Tile[numColumnas, numFilas];
        GameObject itemTile;
        

        for (int row = 0; row < numFilas; row++)
        {
            for (int col = 0; col < numColumnas; col++)
            {
                //item = Instantiate(tilePrefab, new Vector2(i, listOfTiles[randObstacleIndex].transform.position.y), Quaternion.identity);
                itemTile = Instantiate(tilePrefab, new Vector2(row, col), Quaternion.identity);
            }
        }

        itemTile = Instantiate(tilePrefab, new Vector2(-1, numColumnas / 2), Quaternion.identity);
        itemTile = Instantiate(tilePrefab, new Vector2(numFilas, numColumnas / 2), Quaternion.identity);

        int i = 0;

        while (i < ObstacleTreeAmount)
        {
            randObstacleIndex = Random.Range(1, listOfTiles.Count -1);

            GameObject item;
            /*
            Debug.Log("NUMERO RANDOM " + randObstacleIndex);
            Debug.Log(randObstacleIndex % numColumnas);
            Debug.Log(randObstacleIndex % (numColumnas - 1));
            
            if (listOfTiles[randObstacleIndex].transform.position.y != listOfTiles[0].transform.position.y || listOfTiles[randObstacleIndex].transform.position.y != listOfTiles[numColumnas - 1].transform.position.y)
            {
                Debug.Log("ES DIVISIBLE");
                esDivisible = true;
            }
            */
            //if (esDivisible && listOfTiles[randObstacleIndex].IsClear() == true && listOfTiles[randObstacleIndex + 1].IsClear() == true && listOfTiles[randObstacleIndex + 1].IsClear() == true)
            if (listOfTiles[randObstacleIndex].IsClear() == true && listOfTiles[randObstacleIndex - 1].IsClear() == true && listOfTiles[randObstacleIndex + 1].IsClear() == true)
            {
                item = Instantiate(treePrefab, new Vector2(listOfTiles[randObstacleIndex].transform.position.x, listOfTiles[randObstacleIndex].transform.position.y), Quaternion.identity);
                i++;
            }

            esDivisible = false;
        }

        i = 0;

        while (i < ObstacleRockAmount)
        {
            randObstacleIndex = Random.Range(0, listOfTiles.Count);
            GameObject item;
            if (listOfTiles[randObstacleIndex].IsClear() == true)
            {
                item = Instantiate(rockPrefab, new Vector2(listOfTiles[randObstacleIndex].transform.position.x, listOfTiles[randObstacleIndex].transform.position.y), Quaternion.identity);
                i++;
            }
        }
    }
}
