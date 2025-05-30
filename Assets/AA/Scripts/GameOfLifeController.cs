using System.Collections;
using UnityEngine;
using UnityEngine.UI;      
using TMPro;               

public class GameOfLifeController : MonoBehaviour
{
    // Referencia al script que maneja el grid.
    public GridManager gridManager;

    // Se usa para controlar la velocidad del "GameOfLife".
    public Slider speedSlider;

    // Texto para mostrar cuántas celdas están vivas.
    public TextMeshProUGUI aliveCounterText;

    // Texto del botón Play/Pause (para que cambie dinámicamente).
    public TextMeshProUGUI playButtonText;

    // Valor por defecto si no hay slider asignado.
    public float defaultInterval = 0.1f;

    // Se usa para saber si se esta corriendo.
    private bool isRunning = false;

    // Referencia a la corotina que simula el paso del tiempo.
    private Coroutine simulationCoroutine;

    // Este método se llama desde el botón "Play/Pause".
    public void ToggleSimulation()
    {
        if (isRunning)
        {
            // Si está corriendo, detenemos la coroutine.
            if (simulationCoroutine != null)
            {
                StopCoroutine(simulationCoroutine);
                simulationCoroutine = null;
            }

            isRunning = false;

            // Se cambia el texto del botón a "Play".
            if (playButtonText != null) playButtonText.text = "Play";
        }
        else
        {
            // Si no estaba corriendo, la empezamos.
            simulationCoroutine = StartCoroutine(Simulate());
            isRunning = true;

            // Se cambia el texto del botón a "Pause".
            if (playButtonText != null) playButtonText.text = "Pause";
        }
    }

    // Esto es una corutina que corre indefinidamente mientras está en "Play".
    // Va llamando a Step() cada cierto tiempo.
    IEnumerator Simulate()
    {
        while (true)
        {
            Step(); // Se ejecuta una generación.

            // Se espera un tiempo.
            float delay = (speedSlider != null) ? speedSlider.value : defaultInterval;
            yield return new WaitForSeconds(delay);
        }
    }

    // Este método se llama desde el botón "Clear".
    public void ClearGrid()
    {
        var grid = gridManager.GetGrid();
        if (grid == null) return;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Se reecorre cada celda y la apagamos (estado: muerta).
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].SetAlive(false);
            }
        }
    }

    // Este método ejecuta una generación del autómata.
    void Step()
    {
        var grid = gridManager.GetGrid();
        if (grid == null) return;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Se crea una copia para almacenar los nuevos estados.
        bool[,] nextStates = new bool[width, height];

        // Se aplica las reglas del Game of Life.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(grid, x, y);
                bool isAlive = grid[x, y].isAlive;

                // Se aplica las reglas de vida o muerte
                nextStates[x, y] = isAlive switch
                {
                    true when aliveNeighbors < 2 => false,     // Soledad.
                    true when aliveNeighbors > 3 => false,     // Sobrepoblación.
                    true => true,                              // Sobrevive.
                    false when aliveNeighbors == 3 => true,    // Reproducción.
                    _ => false
                };
            }
        }

        // Se aplica los nuevos estados al grid real.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].SetAlive(nextStates[x, y]);
            }
        }

        // Se cuenta cuántas celdas siguen vivas y actualizamos el contador.
        int aliveCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].isAlive) aliveCount++;
            }
        }

        if (aliveCounterText != null)
            aliveCounterText.text = "Celdas vivas: " + aliveCount;
    }

    // Se cuenta cuántas celdas vivas hay alrededor de una posición.
    int CountAliveNeighbors(Cell[,] grid, int x, int y)
    {
        int count = 0;

        // Se recorre los vecinos de 3x3 (excepto a sí mismo).
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                // Se asegura de no salirnos del grid.
                if (nx >= 0 && ny >= 0 && nx < grid.GetLength(0) && ny < grid.GetLength(1))
                {
                    if (grid[nx, ny].isAlive)
                        count++;
                }
            }
        }

        return count;
    }
}