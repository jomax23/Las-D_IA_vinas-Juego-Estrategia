using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FieldObstacleGeneration : MonoBehaviour
{
    [Header("DIMENSIONES DEL MAPA")]
    public GeneradorMapa generadorDelMapa;

    private Tile[,] arrayTile;

    [Header("OBSTACULOS")]
    public List<GeneradorObstaculos> generadorDeObstaculos;

    [Header("PERSONAJES")]
    public GeneradorPersonajes generadorDePersonajes;
    


    void Awake()
    {

        //GENERACION DEL TERRENO

        arrayTile = new Tile[generadorDelMapa.anchura, generadorDelMapa.altura];
        GameObject item;

        for (int col = 0; col < generadorDelMapa.anchura; col++)
        {
            for (int row = 0; row < generadorDelMapa.altura; row++)
            {
                item = Instantiate(generadorDelMapa.prefab, new Vector2(col, row), Quaternion.identity);
                Tile tileScript = item.GetComponent<Tile>();
                arrayTile[col, row] = tileScript;
            }
        }

        //PARA LOS TILES EN LOS QUE SE COLOCAN LOS REYES DE AMBOS JUGADORS, ESTOS NO HARA FALTA INCLUIRLOS EN EL ARRAY
        item = Instantiate(generadorDelMapa.prefab, new Vector2(-1, generadorDelMapa.altura / 2), Quaternion.identity);
        item = Instantiate(generadorDelMapa.prefab, new Vector2(generadorDelMapa.anchura, generadorDelMapa.altura / 2), Quaternion.identity);

        //GENERACION DE OBSTACULOS

        int i = 0;

        int randPosY;
        int randPosX;

        //TRONCOS
        if(generadorDeObstaculos.Count > 1)
        {
            if (generadorDelMapa.anchura > 9 && generadorDelMapa.altura > generadorDeObstaculos[0].cantidad)
            {
                while (i < generadorDeObstaculos[0].cantidad)
                {
                    randPosY = Random.Range(0, generadorDelMapa.altura);
                    randPosX = Random.Range(0, generadorDelMapa.anchura);

                    GameObject obstacle;

                    if (randPosX > 3 && randPosX < generadorDelMapa.anchura - 4)
                    {

                        if (arrayTile[randPosX, randPosY].IsClear() && arrayTile[randPosX - 1, randPosY].IsClear() && arrayTile[randPosX + 1, randPosY].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[0].prefab, new Vector3(randPosX, randPosY, -5), Quaternion.identity);
                            i++;
                        }
                    }
                }

                i = 0;
            }
        }

        //ROCAS

        for (int index = 1; index < generadorDeObstaculos.Count; index++)
        {

            if (generadorDelMapa.anchura > 6 && generadorDelMapa.altura > generadorDeObstaculos[index].cantidad)
            {

                while (i < generadorDeObstaculos[index].cantidad)
                {
                    randPosY = Random.Range(0, generadorDelMapa.altura);
                    randPosX = Random.Range(0, generadorDelMapa.anchura);

                    GameObject obstacle;

                    if (randPosX > 3 && randPosX < generadorDelMapa.anchura - 3)
                    {

                        if (arrayTile[randPosX, randPosY].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, -5), Quaternion.identity);
                            i++;
                        }
                    }
                }

                i = 0;
            }
        }

        //GENERACION DE PERSONAJES

        //ALIADOS

        //REY ALIADO

        item = Instantiate(generadorDePersonajes.prefabReyAliado, new Vector3(-1, generadorDelMapa.altura / 2, -5), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS
        for(int indiceAliado = 0; indiceAliado < generadorDePersonajes.listaDeAliados.Count; indiceAliado++)
        {
            int cantidadAliadoActual = 0;

            while(cantidadAliadoActual < generadorDePersonajes.listaDeAliados[indiceAliado].cantidad)
            {
                randPosY = Random.Range(0, generadorDelMapa.altura);
                randPosX = Random.Range(0, 3);

                if (arrayTile[randPosX, randPosY].IsClear())
                {
                    item = Instantiate(generadorDePersonajes.listaDeAliados[indiceAliado].prefab, new Vector3(randPosX, randPosY, -5), Quaternion.identity);
                    cantidadAliadoActual++;
                }
            }
        }


        //ENEMIGOS

        //REY ENEMIGOS

        item = Instantiate(generadorDePersonajes.prefabReyEnemigo, new Vector3(generadorDelMapa.anchura, generadorDelMapa.altura / 2, -5), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS

        for (int indiceEnemigo = 0; indiceEnemigo < generadorDePersonajes.listaDeEnemigos.Count; indiceEnemigo++)
        {
            int cantidadEnemigoActual = 0;

            while (cantidadEnemigoActual < generadorDePersonajes.listaDeEnemigos[indiceEnemigo].cantidad)
            {
                randPosY = Random.Range(0, generadorDelMapa.altura);
                randPosX = Random.Range(generadorDelMapa.anchura - 3, generadorDelMapa.anchura);

                if (arrayTile[randPosX, randPosY].IsClear())
                {
                    item = Instantiate(generadorDePersonajes.listaDeEnemigos[indiceEnemigo].prefab, new Vector3(randPosX, randPosY, -5), Quaternion.identity);
                    cantidadEnemigoActual++;
                }
            }
        }

        //i = 0;


    }   
}
[System.Serializable]
public class GeneradorMapa
{
    public int anchura; //SERIA LA MAX COLUMNA y LA MAS A LA DERECHA
    public int altura; //SERIA LA MAX FILA y LA MAS ALTA
    public GameObject prefab;
}

[System.Serializable]
public class GeneradorObstaculos
{
    public GameObject prefab;
    public int cantidad;
}

[System.Serializable]
public class GeneradorPersonajes
{
    public GameObject prefabReyAliado;
    public GameObject prefabReyEnemigo;
    public List<GeneradorAliados> listaDeAliados;
    public List<GeneradorEnemigos> listaDeEnemigos;
}

[System.Serializable]
public class GeneradorAliados
{
    public GameObject prefab;
    public int cantidad;
}

[System.Serializable]
public class GeneradorEnemigos
{
    public GameObject prefab;
    public int cantidad;
}



