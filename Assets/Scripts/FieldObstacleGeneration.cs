using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FieldObstacleGeneration : MonoBehaviour
{
    [Header("DIMENSIONES DEL MAPA")]
    public GeneradorMapa generadorDelMapa = new GeneradorMapa();

    private Tile[,] arrayTile;

    [Header("OBSTACULOS")]
    public List<GeneradorObstaculos> generadorDeObstaculos;

    [Header("PERSONAJES")]
    public GeneradorPersonajes generadorDePersonajes;


    void Awake()
    {

        //GENERACION DEL TERRENO
        generadorDelMapa.CrearMapa();
        arrayTile = new Tile[generadorDelMapa.GetAnchura(), generadorDelMapa.GetAltura()];
        GameObject item;

        for (int col = 0; col < generadorDelMapa.GetAnchura(); col++)
        {
            for (int row = 0; row < generadorDelMapa.GetAltura(); row++)
            {
                item = Instantiate(generadorDelMapa.prefab, new Vector3(col, row, transform.position.z), Quaternion.identity);
                Tile tileScript = item.GetComponent<Tile>();
                arrayTile[col, row] = tileScript;
            }
        }

        //PARA LOS TILES EN LOS QUE SE COLOCAN LOS REYES DE AMBOS JUGADORS, ESTOS NO HARA FALTA INCLUIRLOS EN EL ARRAY
        item = Instantiate(generadorDelMapa.prefab, new Vector3(-1, generadorDelMapa.GetAltura() / 2, transform.position.z), Quaternion.identity);
        item = Instantiate(generadorDelMapa.prefab, new Vector3(generadorDelMapa.GetAnchura(), generadorDelMapa.GetAltura() / 2, transform.position.z), Quaternion.identity);

        //GENERACION DE OBSTACULOS

        int midFieldWidth = (generadorDelMapa.GetAnchura() - generadorDelMapa.GetNumColumnasCreacion() * 2);
        
        int numCasillasMidField = midFieldWidth * generadorDelMapa.GetAltura();
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

                if (casillasQueOcupa <= midFieldWidth && generadorDelMapa.GetAltura() > 0)// && generadorDeObstaculos[index].cantidad < generadorDelMapa.GetAltura())
                {
                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField / casillasQueOcupa)
                    {
                        randPosY = Random.Range(0, generadorDelMapa.GetAltura());
                        randPosX = Random.Range(generadorDelMapa.GetNumColumnasCreacion() + 1, generadorDelMapa.GetAnchura() - generadorDelMapa.GetNumColumnasCreacion() - 1);

                        GameObject obstacle;

                        if (arrayTile[randPosX, randPosY].IsClear() && arrayTile[randPosX - 1, randPosY].IsClear() && arrayTile[randPosX + 1, randPosY].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, generadorDeObstaculos[index].GetZCoord()), Quaternion.identity);
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

                if (casillasQueOcupa <= generadorDelMapa.GetAltura() && generadorDelMapa.GetAnchura() > 0)// && generadorDeObstaculos[index].cantidad < generadorDelMapa.GetAltura())
                {
                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField / casillasQueOcupa)
                    {
                        randPosY = Random.Range(1, generadorDelMapa.GetAltura() -1);
                        randPosX = Random.Range(generadorDelMapa.GetNumColumnasCreacion(), generadorDelMapa.GetAnchura() - generadorDelMapa.GetNumColumnasCreacion());

                        GameObject obstacle;

                        Debug.Log(randPosY);
                        Debug.Log(randPosY - 1);
                        Debug.Log(randPosY + 1);

                        if (arrayTile[randPosX, randPosY].IsClear() && arrayTile[randPosX, randPosY - 1].IsClear() && arrayTile[randPosX, randPosY + 1].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, generadorDeObstaculos[index].GetZCoord()), Quaternion.identity);
                            obstacle.transform.Rotate(0.0f, 0.0f, -90.0f);
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

                if(casillasQueOcupa <= midFieldWidth && casillasQueOcupa <= generadorDelMapa.GetAltura())
                {
                    while (contadorCasillasSinObstaculos < numCasillasMidField && i < generadorDeObstaculos[index].cantidad)
                    {
                        randPosY = Random.Range(0, generadorDelMapa.GetAltura());
                        randPosX = Random.Range(generadorDelMapa.GetNumColumnasCreacion(), generadorDelMapa.GetAnchura() - generadorDelMapa.GetNumColumnasCreacion());

                        GameObject obstacle;

                        if (arrayTile[randPosX, randPosY].IsClear())
                        {
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3(randPosX, randPosY, generadorDeObstaculos[index].GetZCoord()), Quaternion.identity);
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

        item = Instantiate(generadorDePersonajes.prefabReyAliado, new Vector3(-1, generadorDelMapa.GetAltura() / 2, generadorDePersonajes.GetZCoord()), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS
        for(int indiceAliado = 0; indiceAliado < generadorDePersonajes.listaDeAliados.Count; indiceAliado++)
        {
            int cantidadAliadoActual = 0;

            while(cantidadAliadoActual < generadorDePersonajes.listaDeAliados[indiceAliado].cantidad)
            {
                randPosY = Random.Range(0, generadorDelMapa.GetAltura());
                randPosX = Random.Range(0, generadorDelMapa.GetNumColumnasCreacion());

                if (arrayTile[randPosX, randPosY].IsClear())
                {
                    item = Instantiate(generadorDePersonajes.listaDeAliados[indiceAliado].prefab, new Vector3(randPosX, randPosY, generadorDePersonajes.GetZCoord()), Quaternion.identity);
                    cantidadAliadoActual++;
                }
            }
        }


        //ENEMIGOS

        //REY ENEMIGO

        item = Instantiate(generadorDePersonajes.prefabReyEnemigo, new Vector3(generadorDelMapa.GetAnchura(), generadorDelMapa.GetAltura() / 2, generadorDePersonajes.GetZCoord()), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ENEMIGOS

        for (int indiceEnemigo = 0; indiceEnemigo < generadorDePersonajes.listaDeEnemigos.Count; indiceEnemigo++)
        {
            int cantidadEnemigoActual = 0;

            while (cantidadEnemigoActual < generadorDePersonajes.listaDeEnemigos[indiceEnemigo].cantidad)
            {
                randPosY = Random.Range(0, generadorDelMapa.GetAltura());
                randPosX = Random.Range(generadorDelMapa.GetAnchura() - generadorDelMapa.GetNumColumnasCreacion(), generadorDelMapa.GetAnchura());

                if (arrayTile[randPosX, randPosY].IsClear())
                {
                    item = Instantiate(generadorDePersonajes.listaDeEnemigos[indiceEnemigo].prefab, new Vector3(randPosX, randPosY, generadorDePersonajes.GetZCoord()), Quaternion.identity);
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
        if(anchurasPosibles.Count > 0 && alturasPosibles.Count > 0)
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


