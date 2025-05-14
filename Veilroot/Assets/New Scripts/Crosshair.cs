using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairTexture;   // Drag your crosshair texture in the inspector
    public float crosshairSize = 10f;    // Size of the crosshair

    private Rect crosshairPosition;

    void Start()
    {
        // Set the position to the center of the screen
        float x = (Screen.width - crosshairSize) / 2;
        float y = (Screen.height - crosshairSize) / 2;
        crosshairPosition = new Rect(x, y, crosshairSize, crosshairSize);
    }

    void OnGUI()
    {
        if (crosshairTexture != null)
        {
            // Draw the crosshair texture at the calculated position
            GUI.DrawTexture(crosshairPosition, crosshairTexture);
        }
        else
        {
            // Fallback: Draw a simple "+" symbol if no texture is set
            int size = (int)crosshairSize;
            GUI.Label(new Rect((Screen.width - size) / 2, (Screen.height - size) / 2, size, size), "+");
        }
    }
}
