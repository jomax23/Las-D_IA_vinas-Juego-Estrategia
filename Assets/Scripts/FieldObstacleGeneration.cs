using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FieldObstacleGeneration : MonoBehaviour
{
    [HideInInspector]
    public int depth = 5; //Coordenada z que tendran z los tiles y los obstaculos
    private int depthObstacle = 6;

    [Header("DIMENSIONES DEL MAPA")]
    public GeneradorMapa generadorDelMapa = new GeneradorMapa();

    [HideInInspector]
    public int anchura; //ANCHURA DEL MAPA

    [HideInInspector]
    public int altura; //ALTURA DEL MAPA

    [HideInInspector]
    public const int columnasGeneracionPersonajes = 3;

    //[HideInInspector]
    public Tile[,] arrayTile;

    [HideInInspector]
    public List<Tile> path;

    [Header("OBSTACULOS")]
    public List<GeneradorObstaculos> generadorDeObstaculos;

    [Header("PERSONAJES")]
    public GeneradorPersonajes generadorDePersonajes;

    [Header("VALORES PARA TOMA DE DECISIONES")]
    public DecisionMakingValues decisionMakingValues;

    [Header("REFERENCIA AL AI MANAGER")]
    public AImanager AImanager;

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

                if((c == 0 || c == anchura -1) && r != altura / 2)
                {
                    item = Instantiate(generadorDeObstaculos[2].prefab, new Vector3(c, r, depthObstacle), Quaternion.identity);
                    //tileScript.isWalkable = false;
                    //item.SetActive(false);
                }
                
                arrayTile[c, r] = tileScript;
            }
        }

        int columna = columnasGeneracionPersonajes;
        int fila = altura / 2;

        //string[] direcciones = { "Abajo", "Derecha", "Arriba" };

        int randomClearPathValue;

        int iterationCounter = 0;

        if ((anchura > columnasGeneracionPersonajes * 2 && altura > 2))
        {

            lista.Remove(new Vector2(columna, fila));

            while (columna < anchura - columnasGeneracionPersonajes)
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
                        else if (arrayTile[columna + 1, fila].canCreateObstacle)
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

        //GENERACION DE OBSTACULOS

        int midFieldWidthStartIndex = columnasGeneracionPersonajes + 1; //(Columnas Generacion De Personajes Aliados) + (columna Rey Aliado)
        int midFieldWidthEndIndex = anchura - columnasGeneracionPersonajes - 1; //(Columnas Generacion De Personajes Aliados) + (columna Rey Aliado)

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
                    for (int c = midFieldWidthStartIndex + 1; c < midFieldWidthEndIndex - 1; c++) //sumamos 1 al inicio y restamos 1 al final para que no se generen obstaculos horizontales en esa posicion
                    {
                        if (arrayTile[c, r].canCreateObstacle && arrayTile[c - 1, r].canCreateObstacle && arrayTile[c + 1, r].canCreateObstacle)
                        {
                            lista.Add(new Vector2(c, r));
                        }
                    }
                }

                generadorDeObstaculos[index].SetCantidadRandom();

                for (int miCantidad = 0; miCantidad < generadorDeObstaculos[index].GetCantidad(); miCantidad++)
                {
                    if (lista.Count > 0)
                    {
                        randIndex = UnityEngine.Random.Range(0, lista.Count);

                        col = (int)lista[randIndex].x;
                        row = (int)lista[randIndex].y;

                        item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[col, row].transform.position.x, (int)arrayTile[col, row].transform.position.y, depthObstacle), Quaternion.identity);


                        if (col + 2 < anchura - columnasGeneracionPersonajes)
                        {
                            lista.Remove(new Vector2(col + 2, row));
                        }

                        if (col - 2 > columnasGeneracionPersonajes)
                        {
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
                    for (int c = midFieldWidthStartIndex + 1; c < midFieldWidthEndIndex - 1; c++)
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
                for (int c = midFieldWidthStartIndex; c < midFieldWidthEndIndex; c++)
                {
                    for (int r = 1; r < altura - 1; r++) //quitamos 1 fila arriba y abajo para que no se generen obstaculos verticales en esa posicion
                    {
                        if (arrayTile[c, r].canCreateObstacle && arrayTile[c, r - 1].canCreateObstacle && arrayTile[c, r + 1].canCreateObstacle)
                        {
                            lista.Add(new Vector2(c, r));
                        }
                    }
                }

                generadorDeObstaculos[index].SetCantidadRandom();

                for (int miCantidad = 0; miCantidad < generadorDeObstaculos[index].GetCantidad(); miCantidad++)
                {
                    if (lista.Count > 0)
                    {
                        randIndex = UnityEngine.Random.Range(0, lista.Count);

                        col = (int)lista[randIndex].x;
                        row = (int)lista[randIndex].y;

                        item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[col, row].transform.position.x, (int)arrayTile[col, row].transform.position.y, depthObstacle), Quaternion.identity);

                        if (row < altura - 2)
                        {
                            if (arrayTile[col, row + 2].canCreateObstacle)
                            {
                                //arrayTile[col, row + 2].canCreateObstacle = false;
                                lista.Remove(new Vector2(col, row + 2));
                            }
                        }

                        if (row > 2)
                        {
                            if (arrayTile[col, row - 2].canCreateObstacle)
                            {
                                //arrayTile[col, row - 2].canCreateObstacle = false;
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

                for (int c = midFieldWidthStartIndex; c < midFieldWidthEndIndex; c++)
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
                    for (int c = midFieldWidthStartIndex; c < midFieldWidthEndIndex; c++)
                    {
                        if (arrayTile[c, r].canCreateObstacle)
                        {
                            lista.Add(new Vector2(c, r));
                        }
                    }
                }

                generadorDeObstaculos[index].SetCantidadRandom();

                for (int miCantidad = 0; miCantidad < generadorDeObstaculos[index].GetCantidad(); miCantidad++)
                {
                    if (lista.Count > 0)
                    {
                        randIndex = UnityEngine.Random.Range(0, lista.Count);

                        col = (int)lista[randIndex].x;
                        row = (int)lista[randIndex].y;

                        item = Instantiate(generadorDeObstaculos[index].prefab, new Vector3((int)arrayTile[col, row].transform.position.x, (int)arrayTile[col, row].transform.position.y, depthObstacle), Quaternion.identity);

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

        //CANTIDADES

        generadorDePersonajes.generadorTanques.SetCantidadRandom();
        int cantidadTanques = generadorDePersonajes.generadorTanques.GetCantidad();

        generadorDePersonajes.generadorArqueros.SetCantidadRandom();
        int cantidadArqueros = generadorDePersonajes.generadorArqueros.GetCantidad();

        generadorDePersonajes.generadorVoladores.SetCantidadRandom();
        int cantidadVoladores = generadorDePersonajes.generadorVoladores.GetCantidad();

        //ALIADOS

        //EN LA CASILLA DE CREACION DE PERSONAJES ALIADOS NO SE PODRA CREAR UN ALIADO INICIALMENTE, AL INICIO DEBERA ESTAR VACIA
        arrayTile[1, altura - 1].canCreateObstacle = false;
        arrayTile[1, altura - 1].hasUnit = true; //PARA ESTO CONSIDERAREMOS QUE YA TIENE UNA UNIDAD ENCIMA DE EL, AUNQUE ESO OBCVIAMENTE NO SEA CIERTO

        //REY ALIADO

        item = Instantiate(generadorDePersonajes.generadorReyes.prefabReyAliado, new Vector2(0, altura / 2), Quaternion.identity);
        arrayTile[0, altura / 2].hasUnit = true;
        AImanager.unitsPlayer.Add(item);

        //LISTA DE VECTORES DE POSICIONES

        for (int c = 1; c <= columnasGeneracionPersonajes; c++)
        {
            for (int r = 0; r < altura; r++)
            {
                if (c != 1 || r != altura - 1)
                {
                    lista.Add(new Vector2(c, r));
                }
            }
        }

        //INSTANCIAS DEL RESTO DE PERSONAJES ALIADOS

        for (int i = 0; i < cantidadTanques; i++)
        {
            if (lista.Count > 0)
            {
                randIndex = UnityEngine.Random.Range(0, lista.Count);

                col = (int)lista[randIndex].x;
                row = (int)lista[randIndex].y;

                item = Instantiate(generadorDePersonajes.generadorTanques.prefabTanqueAliado, new Vector2(col, row), Quaternion.identity);
                arrayTile[col, row].hasUnit = true;
                AImanager.unitsPlayer.Add(item);

                lista.Remove(new Vector2(col, row));
            }
        }

        for (int i = 0; i < cantidadArqueros; i++)
        {
            if (lista.Count > 0)
            {
                randIndex = UnityEngine.Random.Range(0, lista.Count);

                col = (int)lista[randIndex].x;
                row = (int)lista[randIndex].y;

                item = Instantiate(generadorDePersonajes.generadorArqueros.prefabArqueroAliado, new Vector2(col, row), Quaternion.identity);
                arrayTile[col, row].hasUnit = true;
                AImanager.unitsPlayer.Add(item);

                lista.Remove(new Vector2(col, row));
            }
        }

        for (int i = 0; i < cantidadVoladores; i++)
        {
            if (lista.Count > 0)
            {
                randIndex = UnityEngine.Random.Range(0, lista.Count);

                col = (int)lista[randIndex].x;
                row = (int)lista[randIndex].y;

                item = Instantiate(generadorDePersonajes.generadorVoladores.prefabVoladorAliado, new Vector2(col, row), Quaternion.identity);
                arrayTile[col, row].hasUnit = true;
                AImanager.unitsPlayer.Add(item);

                lista.Remove(new Vector2(col, row));
            }
        }

        lista.Clear();

        //COMO YA HEMOS CREADO A LOS ALIADOS, AHORA SI SE PODRAN CREAR ALIADOS EN ESA CASILLA
        arrayTile[1, altura - 1].canCreateObstacle = true;
        arrayTile[1, altura - 1].hasUnit = false;

        //ENEMIGOS

        //EN LA CASILLA DE CREACION DE PERSONAJES ENEMIGOS NO SE PODRA CREAR UN ENEMIGO INICIALMENTE, AL INICIO DEBERA ESTAR VACIA
        arrayTile[anchura - 1, altura - 1].canCreateObstacle = false;

        //REY ENEMIGO

        item = Instantiate(generadorDePersonajes.generadorReyes.prefabReyEnemigo, new Vector2(anchura - 1, altura / 2), Quaternion.identity);
        arrayTile[anchura - 1, altura / 2].hasUnit = true;
        AImanager.units.Add(item);

        //LISTA DE VECTORES DE POSICIONES

        for (int c = anchura - columnasGeneracionPersonajes - 1; c <= anchura - 2; c++)
        {
            for (int r = 0; r < altura; r++)
            {
                if (c != anchura - 2 || r != altura - 1)
                {
                    lista.Add(new Vector2(c, r));
                }
            }
        }

        //INSTANCIAS DEL RESTO DE PERSONAJES ENEMIGOS

        for (int i = 0; i < cantidadTanques; i++)
        {
            if (lista.Count > 0)
            {
                randIndex = UnityEngine.Random.Range(0, lista.Count);

                col = (int)lista[randIndex].x;
                row = (int)lista[randIndex].y;

                item = Instantiate(generadorDePersonajes.generadorTanques.prefabTanqueEnemigo, new Vector2(col, row), Quaternion.identity);
                arrayTile[col, row].hasUnit = true;

                AImanager.units.Add(item);

                lista.Remove(new Vector2(col, row));
            }
        }

        for (int i = 0; i < cantidadArqueros; i++)
        {
            if (lista.Count > 0)
            {
                randIndex = UnityEngine.Random.Range(0, lista.Count);

                col = (int)lista[randIndex].x;
                row = (int)lista[randIndex].y;

                item = Instantiate(generadorDePersonajes.generadorArqueros.prefabArqueroEnemigo, new Vector2(col, row), Quaternion.identity);
                arrayTile[col, row].hasUnit = true;

                AImanager.units.Add(item);

                lista.Remove(new Vector2(col, row));
            }
        }

        for (int i = 0; i < cantidadVoladores; i++)
        {
            if (lista.Count > 0)
            {
                randIndex = UnityEngine.Random.Range(0, lista.Count);

                col = (int)lista[randIndex].x;
                row = (int)lista[randIndex].y;

                item = Instantiate(generadorDePersonajes.generadorVoladores.prefabVoladorEnemigo, new Vector2(col, row), Quaternion.identity);
                arrayTile[col, row].hasUnit = true;

                AImanager.units.Add(item);

                lista.Remove(new Vector2(col, row));
            }
        }

        lista.Clear();

        //COMO YA HEMOS CREADO A LOS ENEMIGOS, AHORA SI SE PODRAN CREAR ENEMIGOS EN ESA CASILLA
        arrayTile[anchura - 1, altura - 1].canCreateObstacle = true;
        arrayTile[anchura - 1, altura - 1].hasUnit = false;
    }

    public enum Orientacion
    {
        Horizontal,
        Vertical,
        UnoXUno
    }


    public List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        // Priorizar movimientos horizontales y verticales antes que diagonales
        int x = (int)tile.transform.position.x;
        int y = (int)tile.transform.position.y;

        // Movimientos horizontales (izquierda y derecha)
        if (x - 1 >= 0) neighbours.Add(arrayTile[x - 1, y]); // izquierda
        if (x + 1 < anchura) neighbours.Add(arrayTile[x + 1, y]); // derecha

        // Movimientos verticales (arriba y abajo)
        if (y - 1 >= 0) neighbours.Add(arrayTile[x, y - 1]); // abajo
        if (y + 1 < altura) neighbours.Add(arrayTile[x, y + 1]); // arriba

        // Movimientos diagonales (abajo izquierda, abajo derecha, arriba izquierda, arriba derecha)
        if (x - 1 >= 0 && y - 1 >= 0) neighbours.Add(arrayTile[x - 1, y - 1]); // abajo izquierda
        if (x + 1 < anchura && y - 1 >= 0) neighbours.Add(arrayTile[x + 1, y - 1]); // abajo derecha
        if (x - 1 >= 0 && y + 1 < altura) neighbours.Add(arrayTile[x - 1, y + 1]); // arriba izquierda
        if (x + 1 < anchura && y + 1 < altura) neighbours.Add(arrayTile[x + 1, y + 1]); // arriba derecha

        return neighbours;
    }
    

    [System.Serializable]
    public class GeneradorMapa
    {
        public GameObject prefab;

        public List<int> anchurasPosibles;
        public List<int> alturasPosibles;

        private int randomWidthIndex;
        private int randomHeightIndex;

        private int anchura; //anchura del campo sin considerar las columnas de creacion de personajes y reyes, es decir, NUMERO DE COLUMNAS DEL MEDIO DEL CAMPO, DONDE SE GENERARAN LOS OBSTACULOS
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
            if (anchura >= 0)
            {
                return anchura + columnasGeneracionPersonajes * 2 + 2;
            }

            anchura = columnasGeneracionPersonajes * 2 + 2;
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

        private int cantidad;

        public int minimaCantidad;
        public int maximaCantidad;

        private int zCoord = -5;

        public int GetZCoord()
        {
            return zCoord;  // Devuelve el valor de zCoord
        }

        public void SetCantidadRandom()
        {

            if (minimaCantidad >= 0 && minimaCantidad <= maximaCantidad)
            {
                cantidad = UnityEngine.Random.Range(minimaCantidad, maximaCantidad + 1);
            }

            else if (minimaCantidad > maximaCantidad || minimaCantidad < 0 || maximaCantidad < 0)
            {
                cantidad = 0;
            }

            else
            {
                cantidad = minimaCantidad;
            }
        }

        public int GetCantidad()
        {
            return cantidad;
        }

        public Orientacion orientacion;
    }

    [System.Serializable]
    public class GeneradorPersonajes
    {
        public GeneradorReyes generadorReyes;

        //PARA HACER ESTO PODRIA HABER USADO HERENCIA, ALMACENAR EN UNA LISTA LOS ITEMS Y RECORRERLA ARRIBA PARA QUE QUEDE GUAPO, PERO COMO ES IGUAL DE OPTIMO PARA GENERAR A LOS PERSONAJES NO LO HE INTENTADO, SI ESO LO HARE MAS ADELANTE PERO NO CREO QUE SEA NECESARIO
        public GeneradorTanques generadorTanques;
        public GeneradorArqueros generadorArqueros;
        public GeneradorVoladores generadorVoladores;

        private int zCoord = -5;

        public int GetZCoord()
        {
            return zCoord;  // Devuelve el valor de zCoord
        }
    }

    [System.Serializable]
    public class GeneradorReyes
    {
        public GameObject prefabReyAliado;
        public GameObject prefabReyEnemigo;
    }

    [System.Serializable]
    public class GeneradorTanques
    {
        public GameObject prefabTanqueAliado;
        public GameObject prefabTanqueEnemigo;

        private int cantidad;

        public int minimaCantidad;
        public int maximaCantidad;
        public void SetCantidadRandom()
        {
            if (minimaCantidad >= 0 && minimaCantidad <= maximaCantidad)
            {
                cantidad = UnityEngine.Random.Range(minimaCantidad, maximaCantidad + 1);
            }

            else if (minimaCantidad > maximaCantidad || minimaCantidad < 0 || maximaCantidad < 0)
            {
                cantidad = 0;
            }

            else
            {
                cantidad = minimaCantidad;
            }
        }

        public int GetCantidad()
        {
            return cantidad;
        }
    }

    [System.Serializable]
    public class GeneradorArqueros
    {
        public GameObject prefabArqueroAliado;
        public GameObject prefabArqueroEnemigo;

        private int cantidad;

        public int minimaCantidad;
        public int maximaCantidad;
        public void SetCantidadRandom()
        {

            if (minimaCantidad >= 0 && minimaCantidad <= maximaCantidad)
            {
                cantidad = UnityEngine.Random.Range(minimaCantidad, maximaCantidad + 1);
            }

            else if (minimaCantidad > maximaCantidad || minimaCantidad < 0 || maximaCantidad < 0)
            {
                cantidad = 0;
            }

            else
            {
                cantidad = minimaCantidad;
            }
        }

        public int GetCantidad()
        {
            return cantidad;
        }
    }

    [System.Serializable]
    public class GeneradorVoladores
    {
        public GameObject prefabVoladorAliado;
        public GameObject prefabVoladorEnemigo;

        private int cantidad;

        public int minimaCantidad;
        public int maximaCantidad;
        public void SetCantidadRandom()
        {

            if (minimaCantidad >= 0 && minimaCantidad <= maximaCantidad)
            {
                cantidad = UnityEngine.Random.Range(minimaCantidad, maximaCantidad + 1);
            }

            else if (minimaCantidad > maximaCantidad || minimaCantidad < 0 || maximaCantidad < 0)
            {
                cantidad = 0;
            }

            else
            {
                cantidad = minimaCantidad;
            }
        }

        public int GetCantidad()
        {
            return cantidad;
        }
    }

    [System.Serializable]
    public class GeneradorAliados
    {
        public GameObject prefab;

        private int cantidad;

        public int minimaCantidad;
        public int maximaCantidad;
        public void SetCantidadRandom()
        {

            if (minimaCantidad >= 0 && minimaCantidad <= maximaCantidad)
            {
                cantidad = UnityEngine.Random.Range(minimaCantidad, maximaCantidad + 1);
            }

            else if (minimaCantidad > maximaCantidad || minimaCantidad < 0 || maximaCantidad < 0)
            {
                cantidad = 0;
            }

            else
            {
                cantidad = minimaCantidad;
            }
        }

        public int GetCantidad()
        {
            return cantidad;
        }
    }

    [System.Serializable]
    public class GeneradorEnemigos
    {
        public GameObject prefab;

        private int cantidad;

        public int minimaCantidad;
        public int maximaCantidad;
        public void SetCantidadRandom()
        {
            if (minimaCantidad >= 0 && minimaCantidad <= maximaCantidad)
            {
                cantidad = UnityEngine.Random.Range(minimaCantidad, maximaCantidad + 1);
            }

            else if (minimaCantidad > maximaCantidad || minimaCantidad < 0)
            {
                cantidad = 0;
            }

            else
            {
                cantidad = minimaCantidad;
            }
        }
        public int GetCantidad()
        {
            return cantidad;
        }
    }

    [System.Serializable]
    public class DecisionMakingValues
    {
        public Vector2 minIVCoord;
        public float minIV;

        public Vector2 maxIVCoord;
        public float maxIV;

        public KingsInfo kingsInfo;

        //IV == INFLUENCE VALUE ABREVIADO
    }

    [System.Serializable]
    public class KingsInfo
    {
        public Vector2 myKingCoord;
        //ESTO ES SOLO PARA QUE LO VEAIS EN EL INSPECTOR
        public float myKingTileIV;

        public Vector2 enemyKingCoord;
        //ESTO ES SOLO PARA QUE LO VEAIS EN EL INSPECTOR
        public float enemyKingTileIV;

        [HideInInspector]
        public Unit myKing;

        [HideInInspector]
        public Unit enemyKing;

        public void SetKingsIVCoords()
        {
            myKingCoord = myKing.transform.position;
            enemyKingCoord = enemyKing.transform.position;
        }
    }
}