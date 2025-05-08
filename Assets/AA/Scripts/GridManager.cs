using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // Prefab que representa una celda (con color y colisión).
    public int width = 50;        // Número de columnas del grid.
    public int height = 30;       // Número de filas del grid.

    private Cell[,] grid;         // Arreglo 2D que guarda todas las celdas.

    void Start()
    {
        // Se crea la matriz de celdas lógicas del tamaño especificado.
        grid = new Cell[width, height];

        // Se calcula un "desplazamiento" para centrar todo el grid en pantalla.
        Vector3 offset = new Vector3(-width / 2f + 0.5f, -height / 2f + 0.5f, 0);

        // Se recorre cada posición en el grid.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Se calcula la posición real en el mundo, aplicando el offset.
                Vector3 position = new Vector3(x, y, 0) + offset;

                // Se instancia una celda del prefab en esa posición.
                GameObject obj = Instantiate(cellPrefab, position, Quaternion.identity);

                // Se organiza como hijo de este objeto para mantener limpio el Hierarchy.
                obj.transform.parent = this.transform;

                // Se obtiene el componente Cell para poder configurarla.
                Cell cell = obj.GetComponent<Cell>();
                cell.gridPos = new Vector2Int(x, y); // Se le asigna su posición en el grid.

                // Se guarda en la matriz.
                grid[x, y] = cell;
            }
        }

        // Centramos la cámara al centro del grid (necesario para que se vea todo).
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.orthographicSize = Mathf.Max(width, height) / 2f;
    }

    // Método para obtener la matriz de celdas desde otros scripts.
    public Cell[,] GetGrid() => grid;
}