using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapHandler : MonoBehaviour
{
    /*
    public FieldObstacleGeneration fieldObstacleGeneration; // Referencia al script de generación de obstáculos
    public int influenceRange = 3; // Rango de influencia (en tiles)
    public int maxInfluence = 5; // Máximo valor de influencia

    private float[,] influenceMap;

    void Start()
    {
        // Inicializamos el mapa de influencias
        influenceMap = new float[fieldObstacleGeneration.anchura, fieldObstacleGeneration.altura];
        UpdateInfluenceMap();
    }

    public void UpdateInfluenceMap()
    {
        // Limpiar el mapa de influencias
        System.Array.Clear(influenceMap, 0, influenceMap.Length);

        // Recalcular las influencias para cada unidad activa
        foreach (var unit in GetActiveUnits())
        {
            PropagateInfluence(unit.Position, unit.Direction);
        }
    }

    private void PropagateInfluence(Vector2Int origin, Vector2Int direction)
    {
        // Ajustar los límites del cuadrado de influencia según la dirección
        int startX = Mathf.Max(0, origin.x - (direction.x == -1 ? influenceRange : 0));
        int endX = Mathf.Min(fieldObstacleGeneration.anchura - 1, origin.x + (direction.x == 1 ? influenceRange : 0));
        int startY = Mathf.Max(0, origin.y - (direction.y == -1 ? influenceRange : 0));
        int endY = Mathf.Min(fieldObstacleGeneration.altura - 1, origin.y + (direction.y == 1 ? influenceRange : 0));

        // Recorrer y actualizar las tiles dentro del área de influencia
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                int distance = Mathf.Max(Mathf.Abs(x - origin.x), Mathf.Abs(y - origin.y));
                influenceMap[x, y] += Mathf.Max(0, maxInfluence - distance);
            }
        }
    }
    private List<Unit> GetActiveUnits()
    {
        // Aquí debes implementar cómo obtener todas las unidades activas en el campo
        // Esto puede ser un listado de objetos Unit que tengan propiedades como Position y Direction.
        return new List<Unit>(); // Ejemplo vacío
    }

    public void OnUnitMoved(Vector2Int newPosition, Vector2Int newDirection)
    {
        // Actualiza el mapa de influencias cuando una unidad se mueve
        UpdateInfluenceMap();
    }

    void OnDrawGizmos()
    {
        if (influenceMap == null) return;

        for (int x = 0; x < fieldObstacleGeneration.anchura; x++)
        {
            for (int y = 0; y < fieldObstacleGeneration.altura; y++)
            {
                if (influenceMap[x, y] > 0)
                {
                    Color color = Color.Lerp(Color.green, Color.red, Mathf.Clamp01(influenceMap[x, y] / maxInfluence));
                    Gizmos.color = color;
                    Gizmos.DrawWireCube(new Vector3(x, y, 0), Vector3.one);
                }
            }
        }
    }
    */
}