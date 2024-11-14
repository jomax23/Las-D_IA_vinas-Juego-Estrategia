using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FieldObstacleGeneration : MonoBehaviour
{
    [Header("DIMENSIONES DEL MAPA")]
    public GeneradorMapa generadorDelMapa = new GeneradorMapa();

    [HideInInspector]
    public int anchura; //ANCHURA DEL MAPA

    [HideInInspector]
    public int altura; //ALTURA DEL MAPA

    private int depth = 5; //Coordenada z que tendran z los tiles y los obstaculos

    private const int columnasPersonajesInicial = 3;

    [HideInInspector]
    public Tile[,] arrayTile;

    [Header("OBSTACULOS")]
    public List<GeneradorObstaculos> generadorDeObstaculos;

    [Header("PERSONAJES")]
    public GeneradorPersonajes generadorDePersonajes;

    void Awake()
    {

        //GENERACION DEL TERRENO
        generadorDelMapa.CrearMapa();

        anchura = generadorDelMapa.GetAnchura();
        altura = generadorDelMapa.GetAltura();

        arrayTile = new Tile[anchura, altura];
        GameObject item;

        List<Vector2> lista = new List<Vector2>();

        for (int c = 0; c < anchura; c++)
        {
            for (int r = 0; r < altura; r++)
            {
                item = Instantiate(generadorDelMapa.prefab, new Vector3(c, r, depth), Quaternion.identity);
                Tile tileScript = item.GetComponent<Tile>();
                arrayTile[c, r] = tileScript;
            }
        }

        arrayTile[0, altura -1].canCreateObstacle = false;
        arrayTile[anchura -1, altura - 1].canCreateObstacle = false;

        int columna = columnasPersonajesInicial;
        int fila = altura / 2;

        //string[] direcciones = { "Abajo", "Derecha", "Arriba" };

        int randomClearPathValue;
        
        int iterationCounter = 0;

        if((anchura > columnasPersonajesInicial * 2 && altura > 2))
        {
            
            lista.Remove(new Vector2(columna, fila));

            while (columna < anchura - columnasPersonajesInicial)
            {
                
                arrayTile[columna, fila].canCreateObstacle = false;
                arrayTile[columna, fila].isObstacleUncreatable = true;

                if (columna < anchura - 1)
                {
                    if (fila == altura - 1)
                    {
                        randomClearPathValue = UnityEngine.Random.Range(0, 2); //0 == Abajo, 1 == Derecha

                        if (randomClearPathValue == 0 && arrayTile[columna, fila - 1].canCreateObstacle)
                        {
                            fila--;
                        }
                        else if(arrayTile[columna + 1, fila].canCreateObstacle)
                        {
                            columna++;
                        }
                    }

                    else if (fila == 0)
                    {
                        randomClearPathValue = UnityEngine.Random.Range(0, 2); //0 == Arriba, 1 == Derecha

                        if (randomClearPathValue == 0 && arrayTile[columna, fila + 1].canCreateObstacle)
                        {
                            fila++;
                        }
                        else if (arrayTile[columna + 1, fila].canCreateObstacle)
                        {
                            columna++;
                        }
                    }

                    else
                    {
                        randomClearPathValue = UnityEngine.Random.Range(0, 3); //0 == Arriba, 1 == Abajo, 2 == Derecha

                        if (randomClearPathValue == 0 && arrayTile[columna, fila + 1].canCreateObstacle)
                        {
                            fila++;
                        }

                        else if (randomClearPathValue == 1 && arrayTile[columna, fila - 1].canCreateObstacle)
                        {
                            fila--;
                        }

                        else if (arrayTile[columna + 1, fila].canCreateObstacle)
                        {
                            columna++;
                        }
                    }

                }

                iterationCounter++;
            }
        }
        //PARA LOS TILES EN LOS QUE SE COLOCAN LOS REYES DE AMBOS JUGADORS, ESTOS NO HARA FALTA INCLUIRLOS EN EL ARRAY
        item = Instantiate(generadorDelMapa.prefab, new Vector3(-1, altura / 2, depth), Quaternion.identity);
        item = Instantiate(generadorDelMapa.prefab, new Vector3(anchura, altura / 2, depth), Quaternion.identity);

        //GENERACION DE OBSTACULOS

        int midFieldWidth = anchura - columnasPersonajesInicial * 2;

        int numCasillasMidField = midFieldWidth * altura;

        //OBSTACULOS

        int randIndex;

        int col;
        int row;
        for (int index = 0; index < generadorDeObstaculos.Count; index++)
        {
            if (generadorDeObstaculos[index].orientacion == Orientacion.Horizontal)
            {
                for (int r = 0; r < altura; r++)
                {
                    for (int c = columnasPersonajesInicial + 1; c < anchura - columnasPersonajesInicial - 1; c++)
                    {
                        if(arrayTile[c, r].canCreateObstacle && arrayTile[c - 1, r].canCreateObstacle && arrayTile[c + 1, r].canCreateObstacle)
                        {
                            lista.Add(new Vector2(c, r));
                        }
                    }
                }

                for (int miCantidad = 0; miCantidad < generadorDeObstaculos[index].cantidad; miCantidad++)
                {
                    if(lista.Count > 0)
                    {
                        randIndex = UnityEngine.Random.Range(0, lista.Count);

                        col = (int)lista[randIndex].x;
                        row = (int)lista[randIndex].y;

                        item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[col, row].transform.position.x, (int)arrayTile[col, row].transform.position.y, depth), Quaternion.identity);


                        if (col + 2 < anchura - columnasPersonajesInicial)
                        {
                            //arrayTile[col + 2, row].canCreateObstacle = false;
                            lista.Remove(new Vector2(col + 2, row));
                        }

                        if (col - 2 > columnasPersonajesInicial)
                        {
                            //arrayTile[col - 2, row].canCreateObstacle = false;
                            lista.Remove(new Vector2(col - 2, row));
                        }

                        arrayTile[col + 1, row].canCreateObstacle = false;
                        arrayTile[col, row].canCreateObstacle = false;
                        arrayTile[col - 1, row].canCreateObstacle = false;


                        lista.Remove(new Vector2(col + 1, row));
                        lista.Remove(new Vector2(col, row));
                        lista.Remove(new Vector2(col - 1, row));
                    }
                }

                for (int r = 0; r < altura; r++)
                {
                    for (int c = columnasPersonajesInicial + 1; c < anchura - columnasPersonajesInicial - 1; c++)
                    {
                        if (!arrayTile[c, r].isObstacleUncreatable && arrayTile[c, r].IsClear())
                        {
                            arrayTile[c, r].canCreateObstacle = true;
                        }
                    }
                }

                lista.Clear();
            }

            if (generadorDeObstaculos[index].orientacion == Orientacion.Vertical)
            {
                for (int c = columnasPersonajesInicial; c < anchura - columnasPersonajesInicial; c++)
                {
                    for (int r = 1; r < altura - 1; r++)
                    {
                        if (arrayTile[c, r].canCreateObstacle && arrayTile[c, r - 1].canCreateObstacle && arrayTile[c, r + 1].canCreateObstacle)
                        {
                            lista.Add(new Vector2(c, r));

                           // item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[c, r].transform.position.x, (int)arrayTile[c, r].transform.position.y, depth), Quaternion.identity);
                        }
                    }
                }

                for (int miCantidad = 0; miCantidad < generadorDeObstaculos[index].cantidad; miCantidad++)
                {
                    if (lista.Count > 0)
                    {
                        randIndex = UnityEngine.Random.Range(0, lista.Count);

                        col = (int)lista[randIndex].x;
                        row = (int)lista[randIndex].y;

                        item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[col, row].transform.position.x, (int)arrayTile[col, row].transform.position.y, depth), Quaternion.identity);

                        if (row < altura - 2)
                        {
                            if (arrayTile[col, row + 2].canCreateObstacle)
                            {
                                arrayTile[col, row + 2].canCreateObstacle = false;
                                lista.Remove(new Vector2(col, row + 2));
                            }
                        }

                        if(row > 2)
                        {
                            if (arrayTile[col, row - 2].canCreateObstacle)
                            {
                                arrayTile[col, row - 2].canCreateObstacle = false;
                                lista.Remove(new Vector2(col, row - 2));
                            }
                        }
                                
                        arrayTile[col, row - 1].canCreateObstacle = false;
                        arrayTile[col, row].canCreateObstacle = false;
                        arrayTile[col, row + 1].canCreateObstacle = false;

                        lista.Remove(new Vector2(col, row - 1));
                        lista.Remove(new Vector2(col, row));
                        lista.Remove(new Vector2(col, row + 1));
                    }
                }

                lista.Clear();

                for (int c = columnasPersonajesInicial; c < anchura - columnasPersonajesInicial; c++)
                {
                    for (int r = 1; r < altura - 1; r++)
                    {
                        if (!arrayTile[c, r].isObstacleUncreatable && arrayTile[c, r].IsClear())
                        {
                            arrayTile[c, r].canCreateObstacle = true;
                        }
                    }
                }
            }

            if (generadorDeObstaculos[index].orientacion == Orientacion.UnoXUno)
            {
                for (int r = 0; r < altura; r++)
                {
                    for (int c = columnasPersonajesInicial; c < anchura - columnasPersonajesInicial; c++)
                    {
                        if (arrayTile[c, r].canCreateObstacle)
                        {
                            lista.Add(new Vector2(c, r));
                        }
                    }
                }



                for (int miCantidad = 0; miCantidad < generadorDeObstaculos[index].cantidad; miCantidad++)
                {
                    if (lista.Count > 0)
                    {
                        randIndex = UnityEngine.Random.Range(0, lista.Count);

                        col = (int)lista[randIndex].x;
                        row = (int)lista[randIndex].y;

                        item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[col, row].transform.position.x, (int)arrayTile[col, row].transform.position.y, depth), Quaternion.identity);

                        arrayTile[col, row].canCreateObstacle = false;
                        lista.Remove(new Vector2(col, row));
                    }
                }

                lista.Clear();

                for (int r = 0; r < altura; r++)
                {
                    for (int c = 0; c < anchura; c++)
                    {
                        if (!arrayTile[c, r].isObstacleUncreatable && arrayTile[c, r].IsClear())
                        {
                            arrayTile[c, r].canCreateObstacle = true;
                        }
                    }
                }
            }
        
        }



        //GENERACION DE PERSONAJES

        //ALIADOS

        //REY ALIADO

        item = Instantiate(generadorDePersonajes.prefabReyAliado, new Vector2(-1, altura / 2), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS
        for (int c = 0; c < columnasPersonajesInicial; c++)
        {
            for (int r = 0; r < altura; r++)  
            {
                lista.Add(new Vector2(c, r));
            }
        }

        for (int indiceAliado = 0; indiceAliado < generadorDePersonajes.listaDeAliados.Count; indiceAliado++)
        {
            for (int miCantidad = 0; miCantidad < generadorDePersonajes.listaDeAliados[indiceAliado].cantidad; miCantidad++)
            {
                if (lista.Count > 0)
                {
                    randIndex = UnityEngine.Random.Range(0, lista.Count);

                    col = (int)lista[randIndex].x;
                    row = (int)lista[randIndex].y;

                    item = Instantiate(generadorDePersonajes.listaDeAliados[indiceAliado].prefab, new Vector2(col, row), Quaternion.identity);

                    lista.Remove(new Vector2(col, row));
                }
            }
        }

        lista.Clear();


        //ENEMIGOS

        //REY ENEMIGO

        item = Instantiate(generadorDePersonajes.prefabReyEnemigo, new Vector2(anchura, altura / 2), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ENEMIGOS

        for (int c = midFieldWidth + columnasPersonajesInicial; c < anchura; c++)
        {
            for (int r = 0; r < altura; r++)
            {
                lista.Add(new Vector2(c, r));
            }
        }

        for (int indiceEnemigo = 0; indiceEnemigo < generadorDePersonajes.listaDeEnemigos.Count; indiceEnemigo++)
        {
            for (int miCantidad = 0; miCantidad < generadorDePersonajes.listaDeEnemigos[indiceEnemigo].cantidad; miCantidad++)
            {
                if (lista.Count > 0)
                {
                    randIndex = UnityEngine.Random.Range(0, lista.Count);

                    col = (int)lista[randIndex].x;
                    row = (int)lista[randIndex].y;

                    item = Instantiate(generadorDePersonajes.listaDeEnemigos[indiceEnemigo].prefab, new Vector2(col, row), Quaternion.identity);

                    lista.Remove(new Vector2(col, row));
                }
            }
        }

        lista.Clear();
    }
}

public enum Orientacion
{
    Horizontal,
    Vertical,
    UnoXUno
}

[System.Serializable]
public class GeneradorMapa
{
    public GameObject prefab;

    public List<int> anchurasPosibles;
    public List<int> alturasPosibles;

    private int randomWidthIndex;
    private int randomHeightIndex;

    private int anchura; //SERIA LA MAX COLUMNA y LA MAS A LA DERECHA
    private int altura; //SERIA LA MAX FILA y LA MAS ALTA

    public void CrearMapa()
    {
        if (anchurasPosibles.Count > 0 && alturasPosibles.Count > 0)
        {
            randomWidthIndex = UnityEngine.Random.Range(0, anchurasPosibles.Count);
            randomHeightIndex = UnityEngine.Random.Range(0, alturasPosibles.Count);

            anchura = anchurasPosibles[randomWidthIndex];
            altura = alturasPosibles[randomHeightIndex];
        }
    }

    public int GetAnchura()
    {
        return anchura;
    }

    public int GetAltura()
    {
        return altura;
    }
}

[System.Serializable]
public class GeneradorObstaculos
{
    public GameObject prefab;

    public int cantidad;

    private int zCoord = -5;

    public int GetZCoord()
    {
        return zCoord;  // Devuelve el valor de zCoord
    }

    public Orientacion orientacion;
}

[System.Serializable]
public class GeneradorPersonajes
{
    public GameObject prefabReyAliado;
    public GameObject prefabReyEnemigo;

    public List<GeneradorAliados> listaDeAliados;
    public List<GeneradorEnemigos> listaDeEnemigos;

    private int zCoord = -5;

    public int GetZCoord()
    {
        return zCoord;  // Devuelve el valor de zCoord
    }

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