using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{
    public int width = 10;  // Ancho del mapa
    public int height = 10; // Altura del mapa
    public float influenceRadius = 3.0f; // Alcance máximo de influencia
    public int maxInfluence = 5; // Valor máximo de influencia

    private float[,] influenceValues;  // Matriz que guarda los valores de influencia

    void Start()
    {
        // Inicializamos el mapa de influencias
        influenceValues = new float[width, height];
        GenerateInfluenceMap(new Vector2Int(5, 5));  // Generar influencia con origen en (5,5)
    }

    // Generar el mapa de influencias desde una posición de origen
    void GenerateInfluenceMap(Vector2Int origin)
    {
        // Limpiar el mapa de influencias
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                influenceValues[x, y] = 0;
            }
        }

        // Propagar la influencia desde el origen
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculamos la distancia entre la celda (x, y) y el origen
                float distance = Vector2Int.Distance(new Vector2Int(x, y), origin);

                // Si la celda está dentro del radio de influencia, calculamos su valor
                if (distance <= influenceRadius)
                {
                    // Usamos una caída de influencia proporcional a la distancia
                    influenceValues[x, y] = Mathf.Max(0, maxInfluence - Mathf.FloorToInt(distance));
                }
            }
        }

        // Opcional: Mostrar el mapa de influencias en la consola para depurar
        PrintInfluenceMap();
    }

    // Mostrar el mapa de influencias en la consola
    void PrintInfluenceMap()
    {
        string map = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map += influenceValues[x, y] + " ";
            }
            map += "\n";
        }
        Debug.Log(map);
    }

    // Dibujar el mapa de influencias visualmente en Unity (solo en el editor)
    void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Usar el valor de la influencia para determinar el color
                Color color = Color.Lerp(Color.green, Color.red, influenceValues[x, y] / (float)maxInfluence);
                Gizmos.color = color;
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
            }
        }
    }
}