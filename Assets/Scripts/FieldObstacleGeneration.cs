using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObstacleGeneration : MonoBehaviour
{
    // Start is called before the first frame update

    public int numColumnas = 11;
    public int numFilas = 7;

    public List<Tile> listOfTiles = new List<Tile>();

    int randObstacleIndex;
    public List<int> usedObstacleIndex = new List<int>();
    bool isUsed = false;

    public int maxObstacleAmount = 3;
    void Awake()
    {
        for (int i = 0; i < maxObstacleAmount; i++)
        {
            randObstacleIndex = Random.Range(0, listOfTiles.Count);

            if(usedObstacleIndex.Count == 0)
            {
                usedObstacleIndex.Add(randObstacleIndex);
                usedObstacleIndex.Add(randObstacleIndex + 1);
                usedObstacleIndex.Add(randObstacleIndex - 1);
            }
            else
            {
                while (isUsed)
                {
                    if (!usedObstacleIndex.Contains(randObstacleIndex))
                    {
                        usedObstacleIndex.Add(randObstacleIndex);
                        usedObstacleIndex.Add(randObstacleIndex + 1);
                        usedObstacleIndex.Add(randObstacleIndex - 1);
                        isUsed = false;
                    }
                    else
                    {
                        randObstacleIndex = Random.Range(0, listOfTiles.Count);
                    }
                }
                isUsed = true;
                
            }
            if (!listOfTiles[randObstacleIndex].obstaculo.activeSelf)
            {
                listOfTiles[randObstacleIndex].obstaculo.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SeeIfUsedObstacleIndex()
    {

    }
}
