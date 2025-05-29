using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Automata2DManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 50;
    public GameObject cellPrefab;
    public Transform gridParent;

    [Header("UI Inputs")]
    public TMP_InputField inputWidth;
    public TMP_InputField inputHeight;
    public TMP_InputField inputRuleAlive;
    public TMP_InputField inputRuleDeath;
    public TMP_InputField inputStates;
    public Toggle toggleMoore;
    public Toggle toggleRandom;
    public TMP_InputField inputSpeed;
    public Button generateButton;
    public Button playButton;

    private Cell2D[,] grid;
    private int states;
    private float speed = 0.5f;
    private bool isRunning = false;

    void Start()
    {
        generateButton.onClick.AddListener(Generate);
        playButton.onClick.AddListener(() => StartCoroutine(RunSimulation()));
    }

    void Generate()
    {
        if (!int.TryParse(inputWidth.text, out width)) width = 50;
        if (!int.TryParse(inputHeight.text, out height)) height = 50;
        if (!int.TryParse(inputStates.text, out states)) states = 2;
        if (!float.TryParse(inputSpeed.text, out speed)) speed = 0.5f;

        // Limpiar el grid anterior
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        grid = new Cell2D[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                cellObj.transform.localPosition = new Vector3(x * 0.2f, -y * 0.2f, 0);

                Cell2D cell = cellObj.AddComponent<Cell2D>();
                cell.x = x;
                cell.y = y;
                cell.manager = this;

                if (toggleRandom.isOn)
                    cell.state = Random.Range(0, states);
                else
                    cell.state = 0;

                cell.UpdateColor();
                grid[x, y] = cell;
            }
        }
    }

    IEnumerator RunSimulation()
    {
        isRunning = true;
        while (isRunning)
        {
            StepSimulation();
            yield return new WaitForSeconds(speed);
        }
    }

    void StepSimulation()
    {
        int[,] newStates = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                int currentState = grid[x, y].state;
                int newState = currentState;

                string[] aliveRules = inputRuleAlive.text.Split(',');
                string[] deathRules = inputRuleDeath.text.Split(',');

                if (currentState > 0 && Contains(deathRules, aliveNeighbors))
                    newState = 0;
                else if (currentState == 0 && Contains(aliveRules, aliveNeighbors))
                    newState = 1;

                newStates[x, y] = newState;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].state = newStates[x, y];
                grid[x, y].UpdateColor();
            }
        }
    }

    bool Contains(string[] list, int val)
    {
        foreach (string s in list)
        {
            if (int.TryParse(s.Trim(), out int num) && num == val)
                return true;
        }
        return false;
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        int[,] offsets = toggleMoore.isOn ?
            new int[,] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } } :
            new int[,] { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = x + offsets[i, 0];
            int ny = y + offsets[i, 1];

            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
            {
                if (grid[nx, ny].state > 0)
                    count++;
            }
        }
        return count;
    }
}

