using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Automata1DManager : MonoBehaviour
{
    [Header("Prefabs y Layout")]
    public GameObject cellPrefab;
    public Transform gridParent;

    [Header("UI Inputs")]
    public TMP_InputField inputWidth;
    public TMP_InputField inputHeight;
    public TMP_InputField inputRule;
    public Toggle toggleRandomStart;
    public Toggle toggleStepped;
    public float stepDelay = 0.1f;

    private int width, height, rule;
    private bool[] currentGeneration;
    private Dictionary<string, bool> ruleSet;

    public void Generate()
    {
        Debug.Log("Generate fue llamado");
        Debug.Log("inputWidth: " + inputWidth.text);
        Debug.Log("inputHeight: " + inputHeight.text);
        Debug.Log("inputRule: " + inputRule.text);
        Debug.Log("Random? " + toggleRandomStart.isOn);
        Debug.Log("Stepped? " + toggleStepped.isOn);

        if (string.IsNullOrEmpty(inputWidth.text) ||
            string.IsNullOrEmpty(inputHeight.text) ||
            string.IsNullOrEmpty(inputRule.text))
        {
            Debug.LogWarning("Todos los campos deben estar llenos.");
            return;
        }

        if (!int.TryParse(inputWidth.text, out width) ||
            !int.TryParse(inputHeight.text, out height) ||
            !int.TryParse(inputRule.text, out rule))
        {
            Debug.LogWarning("Asegúrate de ingresar solo números válidos (enteros).");
            return;
        }

        bool isRandom = toggleRandomStart.isOn;
        bool isStepped = toggleStepped.isOn;

        // Limpiar celdas anteriores
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        ruleSet = GenerateRuleSet(rule);
        currentGeneration = new bool[width];

        // Primera generación
        for (int i = 0; i < width; i++)
        {
            currentGeneration[i] = isRandom ? Random.value > 0.5f : i == width / 2;
        }

        if (isStepped)
        {
            StartCoroutine(Simulate());
        }
        else
        {
            for (int y = 0; y < height; y++)
            {
                DrawRow(y, currentGeneration);
                currentGeneration = GetNextGeneration(currentGeneration);
            }
        }
    }

    IEnumerator Simulate()
    {
        for (int y = 0; y < height; y++)
        {
            DrawRow(y, currentGeneration);
            currentGeneration = GetNextGeneration(currentGeneration);
            yield return new WaitForSeconds(stepDelay);
        }
    }

    void DrawRow(int y, bool[] generation)
    {
        float cellSize = 1f;
        float offsetX = -(width * cellSize) / 2f + cellSize / 2f;

        // Aumentamos aún más el valor de offsetY para subir el patrón
        float offsetY = (height * cellSize) / 2f - cellSize / 2f + 4f;

        for (int x = 0; x < generation.Length; x++)
        {
            GameObject cell = Instantiate(cellPrefab, gridParent);
            cell.transform.localPosition = new Vector3((x * cellSize) + offsetX, -(y * cellSize) + offsetY, 0);
            cell.transform.localScale = Vector3.one;
            cell.GetComponent<SpriteRenderer>().color = generation[x] ? Color.black : Color.white;
        }
    }

    bool[] GetNextGeneration(bool[] current)
    {
        bool[] next = new bool[current.Length];

        for (int i = 0; i < current.Length; i++)
        {
            bool left = i > 0 ? current[i - 1] : false;
            bool center = current[i];
            bool right = i < current.Length - 1 ? current[i + 1] : false;

            string pattern = (left ? "1" : "0") + (center ? "1" : "0") + (right ? "1" : "0");
            next[i] = ruleSet.ContainsKey(pattern) ? ruleSet[pattern] : false;
        }

        return next;
    }

    Dictionary<string, bool> GenerateRuleSet(int rule)
    {
        Dictionary<string, bool> result = new Dictionary<string, bool>();
        string binary = System.Convert.ToString(rule, 2).PadLeft(8, '0');

        string[] keys = new string[]
        {
            "111", "110", "101", "100",
            "011", "010", "001", "000"
        };

        for (int i = 0; i < keys.Length; i++)
        {
            result[keys[i]] = binary[i] == '1';
        }

        return result;
    }
}