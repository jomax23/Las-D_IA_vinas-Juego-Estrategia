using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FieldObstacleGeneration : MonoBehaviour
{
    [Header("DIMENSIONES DEL MAPA")]
    public GeneradorMapa generadorDelMapa = new GeneradorMapa();

    private int anchura; //ANCHURA DEL MAPA
    private int altura; //ALTURA DEL MAPA

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

        for (int col = 0; col < anchura; col++)
        {
            for (int row = 0; row < altura; row++)
            {
                item = Instantiate(generadorDelMapa.prefab, new Vector3(col, row, depth), Quaternion.identity);
                Tile tileScript = item.GetComponent<Tile>();
                arrayTile[col, row] = tileScript;
            }
        }

        //PARA LOS TILES EN LOS QUE SE COLOCAN LOS REYES DE AMBOS JUGADORS, ESTOS NO HARA FALTA INCLUIRLOS EN EL ARRAY
        item = Instantiate(generadorDelMapa.prefab, new Vector3(-1, altura / 2, depth), Quaternion.identity);
        item = Instantiate(generadorDelMapa.prefab, new Vector3(anchura, altura / 2, depth), Quaternion.identity);

        //GENERACION DE OBSTACULOS

        int midFieldWidth = anchura - columnasPersonajesInicial * 2;

        int numCasillasMidField = midFieldWidth * altura;
        int i = 0;

        int randPosY;
        int randPosX;

        int contadorCasillasSinObstaculos = 0;

        int casillasQueOcupa = 0;

        //OBSTACULOS

        for (int index = 0; index < generadorDeObstaculos.Count; index++)
        {
            if (generadorDeObstaculos[index].orientacion == Orientacion.Horizontal)
            {
                casillasQueOcupa = 3;

                if (casillasQueOcupa <= midFieldWidth && altura > 0)// && generadorDeObstaculos[index].cantidad < altura)
                {
                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField / casillasQueOcupa)
                    {
                        randPosY = Random.Range(0, altura);
                        randPosX = Random.Range(columnasPersonajesInicial + 1, anchura - columnasPersonajesInicial - 1);

                        GameObject obstacle;

                        if (arrayTile[randPosX, randPosY].IsClear() && arrayTile[randPosX - 1, randPosY].IsClear() && arrayTile[randPosX + 1, randPosY].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, depth), Quaternion.identity);
                            i++;
                        }

                        else if (arrayTile[randPosX, randPosY].CanCreateObstacle == true)
                        {
                            arrayTile[randPosX, randPosY].CanCreateObstacle = false;
                            contadorCasillasSinObstaculos++;
                        }
                    }
                }

                i = 0;
            }

            else if (generadorDeObstaculos[index].orientacion == Orientacion.Vertical)
            {
                casillasQueOcupa = 3;

                if (casillasQueOcupa <= altura && anchura > 0)// && generadorDeObstaculos[index].cantidad < altura)
                {
                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField / casillasQueOcupa)
                    {
                        randPosY = Random.Range(1, altura - 1);
                        randPosX = Random.Range(columnasPersonajesInicial, anchura - columnasPersonajesInicial);

                        GameObject obstacle;

                        if (arrayTile[randPosX, randPosY].IsClear() && arrayTile[randPosX, randPosY - 1].IsClear() && arrayTile[randPosX, randPosY + 1].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, depth), Quaternion.identity);
                            i++;
                        }

                        else if (arrayTile[randPosX, randPosY].CanCreateObstacle == true)
                        {
                            arrayTile[randPosX, randPosY].CanCreateObstacle = false;
                            contadorCasillasSinObstaculos++;
                        }
                    }
                }

                i = 0;
            }

            else if (generadorDeObstaculos[index].orientacion == Orientacion.UnoXUno)
            {
                casillasQueOcupa = 1;

                if (casillasQueOcupa <= midFieldWidth && casillasQueOcupa <= altura)
                {
                    while (contadorCasillasSinObstaculos < numCasillasMidField && i < generadorDeObstaculos[index].cantidad)
                    {
                        randPosY = Random.Range(0, altura);
                        randPosX = Random.Range(columnasPersonajesInicial, anchura - columnasPersonajesInicial);

                        GameObject obstacle;

                        if (arrayTile[randPosX, randPosY].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, depth), Quaternion.identity);
                            i++;
                        }

                        else if (arrayTile[randPosX, randPosY].CanCreateObstacle == true)
                        {
                            arrayTile[randPosX, randPosY].CanCreateObstacle = false;
                            contadorCasillasSinObstaculos++;
                        }

                    }
                }

                i = 0;
            }
        }



        //GENERACION DE PERSONAJES

        //ALIADOS

        //REY ALIADO

        item = Instantiate(generadorDePersonajes.prefabReyAliado, new Vector2(-1, altura / 2), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS
        for (int indiceAliado = 0; indiceAliado < generadorDePersonajes.listaDeAliados.Count; indiceAliado++)
        {
            int cantidadAliadoActual = 0;

            while (cantidadAliadoActual < generadorDePersonajes.listaDeAliados[indiceAliado].cantidad)
            {
                randPosY = Random.Range(0, altura);
                randPosX = Random.Range(0, columnasPersonajesInicial);

                if (arrayTile[randPosX, randPosY].IsClear())
                {
                    item = Instantiate(generadorDePersonajes.listaDeAliados[indiceAliado].prefab, new Vector2(randPosX, randPosY), Quaternion.identity);

                    cantidadAliadoActual++;
                }
            }
        }


        //ENEMIGOS

        //REY ENEMIGO

        item = Instantiate(generadorDePersonajes.prefabReyEnemigo, new Vector2(anchura, altura / 2), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ENEMIGOS

        for (int indiceEnemigo = 0; indiceEnemigo < generadorDePersonajes.listaDeEnemigos.Count; indiceEnemigo++)
        {
            int cantidadEnemigoActual = 0;

            while (cantidadEnemigoActual < generadorDePersonajes.listaDeEnemigos[indiceEnemigo].cantidad)
            {
                randPosY = Random.Range(0, altura);
                randPosX = Random.Range(anchura - columnasPersonajesInicial, anchura);

                if (arrayTile[randPosX, randPosY].IsClear())
                {
                    item = Instantiate(generadorDePersonajes.listaDeEnemigos[indiceEnemigo].prefab, new Vector2(randPosX, randPosY), Quaternion.identity);

                    cantidadEnemigoActual++;
                }
            }
        }
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

    private int numColumnasCreacion = 3;

    private int randomWidthIndex;
    private int randomHeightIndex;

    private int anchura; //SERIA LA MAX COLUMNA y LA MAS A LA DERECHA
    private int altura; //SERIA LA MAX FILA y LA MAS ALTA

    public void CrearMapa()
    {
        if (anchurasPosibles.Count > 0 && alturasPosibles.Count > 0)
        {
            randomWidthIndex = Random.Range(0, anchurasPosibles.Count);
            randomHeightIndex = Random.Range(0, alturasPosibles.Count);

            anchura = anchurasPosibles[randomWidthIndex];
            altura = alturasPosibles[randomHeightIndex];
        }
    }
    public int GetNumColumnasCreacion()
    {
        return numColumnasCreacion;  // Devuelve el valor de zCoord
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