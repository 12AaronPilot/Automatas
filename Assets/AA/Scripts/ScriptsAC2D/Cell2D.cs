using UnityEngine;

public class Cell2D : MonoBehaviour
{
    public int x;
    public int y;
    public int state = 0;
    public Automata2DManager manager;

    public void UpdateColor()
    {
        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            renderer.color = state > 0 ? Color.black : Color.white;
        }
    }

    private void OnMouseDown()
    {
        if (!manager.toggleRandom.isOn)
        {
            state = state == 0 ? 1 : 0;
            UpdateColor();
        }
    }
}
