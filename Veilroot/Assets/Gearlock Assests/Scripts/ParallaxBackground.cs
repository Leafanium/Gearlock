using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxEffect = 10f;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        float moveX = (mousePosition.x / Screen.width - 0.5f) * -parallaxEffect;
        float moveY = (mousePosition.y / Screen.height - 0.5f) * -parallaxEffect;

        transform.position = initialPosition + new Vector3(moveX, moveY, 0);
    }
}
