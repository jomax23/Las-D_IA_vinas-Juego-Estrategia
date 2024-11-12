using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

        arrayTile[0, altura -1].canCreateObstacle = false;
        arrayTile[anchura -1, altura - 1].canCreateObstacle = false;

        int columna = columnasPersonajesInicial;
        int fila = altura / 2;

        //string[] direcciones = { "Abajo", "Derecha", "Arriba" };

        int randomClearPathValue;
        
        int iterationCounter = 0;

        if((anchura > columnasPersonajesInicial * 2 && altura > 2))
        {
            while (columna < anchura - columnasPersonajesInicial)
            {

                arrayTile[columna, fila].canCreateObstacle = false;

                if (columna < anchura - 1)
                {
                    if (fila == altura - 1)
                    {
                        randomClearPathValue = Random.Range(0, 2); //0 == Abajo, 1 == Derecha

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
                        randomClearPathValue = Random.Range(0, 2); //0 == Arriba, 1 == Derecha

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
                        randomClearPathValue = Random.Range(0, 3); //0 == Arriba, 1 == Abajo, 2 == Derecha

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
        int i = 0;

        int randPosY;
        int randPosX;

        int contadorCasillasSinObstaculos = 0;

        int casillasQueOcupa = 0;

        //OBSTACULOS

        int randIndex;

        List<Tile> lista = new List<Tile>(); //LA HE LLAMADO LISTA POR SIMPLIFICAR EL NOMBRE, PERO SU FUNCION ES ALMACENAR LAS POSIBLES POSICIONES DE LOS OBSTACULOS Y LOS PERSONAJES

        for (int index = 0; index < generadorDeObstaculos.Count; index++)
        {
            if (generadorDeObstaculos[index].orientacion == Orientacion.Horizontal)
            {
                casillasQueOcupa = 3;
                
                if (casillasQueOcupa <= midFieldWidth && altura > 0)// && generadorDeObstaculos[index].cantidad < altura)
                {
                    for (int col = columnasPersonajesInicial + 1; col < anchura - columnasPersonajesInicial - 1; col++)
                    {
                        for (int row = 0; row < altura; row++)
                        {
                            lista.Add(arrayTile[col, row]); // IMPORTANTE PARA ENTENDER LO DE ABAJO, EN UNITY, LOS OBJETOS SE PASAN POR DEFECTO POR REFERENCIA, A DIFERENCIA DE ENTEROS, STRINGS, ETC.
                        }
                    }

                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField)
                    {

                        randIndex = Random.Range(0, lista.Count);
                        GameObject obstacle;

                        if (lista[randIndex].IsClear() && lista[randIndex].canCreateObstacle && arrayTile[(int)lista[randIndex].transform.position.x - 1, (int)lista[randIndex].transform.position.y].canCreateObstacle && arrayTile[(int)lista[randIndex].transform.position.x + 1, (int)lista[randIndex].transform.position.y].canCreateObstacle)
                        {

                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y, depth), Quaternion.identity);
                            arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y].canCreateObstacle = false;
                            arrayTile[(int)lista[randIndex].transform.position.x - 1, (int)lista[randIndex].transform.position.y].canCreateObstacle = false;
                            arrayTile[(int)lista[randIndex].transform.position.x + 1, (int)lista[randIndex].transform.position.y].canCreateObstacle = false;

                            lista.RemoveAt(randIndex);
                            contadorCasillasSinObstaculos += 3;

                            i++;
                            
                        }

                        //PARA LOS TILES CONTIGUOS, ME GUSTARIA HACERLO CON ALGUNA FUNCION QUE ME PERMITA BORRAR LOS CONTIGUOS A LA POSICION DEL OBSTACULO GENERADO DE LA LISTA, PERO DE MOMENTO SOLO SE ME OCURRE USAR UN
                        //BUCLE LARGO DE COJONES, MAS ADELANTE PENSARE COMO JUNTARLO CON EL IF DE ARRIBA SI ME DA TIEMPO ^^. 
                        else
                        {
                            lista.RemoveAt(randIndex);
                            contadorCasillasSinObstaculos++;
                        }
                    }

                    lista.Clear();
                }

                i = 0;
            }

            else if (generadorDeObstaculos[index].orientacion == Orientacion.Vertical)
            {
                casillasQueOcupa = 3;

                if (casillasQueOcupa <= altura && midFieldWidth > 0)// && generadorDeObstaculos[index].cantidad < altura)
                {
                    for (int col = columnasPersonajesInicial; col < anchura - columnasPersonajesInicial; col++)
                    {
                        for (int row = 1; row < altura - 1; row++)
                        {
                            if (arrayTile[col, row].IsClear())
                            {
                                lista.Add(arrayTile[col, row]); // IMPORTANTE PARA ENTENDER LO DE ABAJO, EN UNITY, LOS OBJETOS SE PASAN POR DEFECTO POR REFERENCIA, A DIFERENCIA DE ENTEROS, STRINGS, ETC.
                            }
                        }
                    }

                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField)
                    {
                        randIndex = Random.Range(0, lista.Count);

                        GameObject obstacle;

                        if (lista[randIndex].IsClear() && lista[randIndex].canCreateObstacle && arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y - 1].canCreateObstacle && arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y + 1].canCreateObstacle)
                        {

                            
                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y, depth), Quaternion.identity);
                            arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y].canCreateObstacle = false;
                            arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y - 1].canCreateObstacle = false;
                            arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y + 1].canCreateObstacle = false;

                            lista.RemoveAt(randIndex);
                            contadorCasillasSinObstaculos += 3;

                            i++;

                        }

                        //PARA LOS TILES CONTIGUOS, ME GUSTARIA HACERLO CON ALGUNA FUNCION QUE ME PERMITA BORRAR LOS CONTIGUOS A LA POSICION DEL OBSTACULO GENERADO DE LA LISTA, PERO DE MOMENTO SOLO SE ME OCURRE USAR UN
                        //BUCLE LARGO DE COJONES, MAS ADELANTE PENSARE COMO JUNTARLO CON EL IF DE ARRIBA SI ME DA TIEMPO ^^. 

                        else
                        {
                            lista.RemoveAt(randIndex);
                            contadorCasillasSinObstaculos++;
                        }
                    }

                    lista.Clear();
                }

                i = 0;
            }

            else if (generadorDeObstaculos[index].orientacion == Orientacion.UnoXUno)
            {
                casillasQueOcupa = 1;

                if (casillasQueOcupa <= midFieldWidth && casillasQueOcupa <= altura)
                {
                    for (int col = columnasPersonajesInicial; col < anchura - columnasPersonajesInicial; col++)
                    {
                        for (int row = 0; row < altura; row++)
                        {
                            if (arrayTile[col, row].IsClear() && arrayTile[col, row].canCreateObstacle)
                            {
                                lista.Add(arrayTile[col, row]); // IMPORTANTE PARA ENTENDER LO DE ABAJO, EN UNITY, LOS OBJETOS SE PASAN POR DEFECTO POR REFERENCIA, A DIFERENCIA DE ENTEROS, STRINGS, ETC.
                            }
                        }
                    }

                    randIndex = Random.Range(0, lista.Count);

                    GameObject obstacle;
                    while (i < generadorDeObstaculos[index].cantidad && contadorCasillasSinObstaculos < numCasillasMidField)
                    {
                        if (lista[randIndex].IsClear() && lista[randIndex].canCreateObstacle)
                        {
                            randIndex = Random.Range(0, lista.Count);

                            obstacle = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y, depth), Quaternion.identity);
                            arrayTile[(int)lista[randIndex].transform.position.x, (int)lista[randIndex].transform.position.y].canCreateObstacle = false;

                            lista.RemoveAt(randIndex);
                            contadorCasillasSinObstaculos++;

                            i++;

                        }

                        else
                        {
                            //lista.RemoveAt(randIndex);
                            contadorCasillasSinObstaculos++;
                        }
                    }

                    lista.Clear();

                }

                i = 0;
            }
        }



        //GENERACION DE PERSONAJES

        //ALIADOS

        //REY ALIADO

        item = Instantiate(generadorDePersonajes.prefabReyAliado, new Vector2(-1, altura / 2), Quaternion.identity);

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS

        for (int col = 0; col < anchura - columnasPersonajesInicial; col++)
        {
            for (int row = 0; row < altura; row++)
            {
                if (arrayTile[col, row].IsClear())
                {
                    lista.Add(arrayTile[col, row]); // IMPORTANTE PARA ENTENDER LO DE ABAJO, EN UNITY, LOS OBJETOS SE PASAN POR DEFECTO POR REFERENCIA, A DIFERENCIA DE ENTEROS, STRINGS, ETC.
                }
            }
        }

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