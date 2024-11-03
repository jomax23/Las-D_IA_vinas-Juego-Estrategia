using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObstacleGeneration : MonoBehaviour
{
    // Start is called before the first frame update

    public int numColumnas = 13;
    public int numFilas = 7;

    public List<Tile> listOfTiles = new List<Tile>();

    int randObstacleIndex;

    public int maxObstacleAmount = 3;
    void Awake()
    {

        for (int i = 0; i < maxObstacleAmount; i++)
        {
            randObstacleIndex = Random.Range(0, numColumnas * numFilas);

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
}
