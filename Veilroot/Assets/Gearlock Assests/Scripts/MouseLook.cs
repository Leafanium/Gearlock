using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseX = 0, MouseY = 1, MouseXAndY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float sensitivityHoriz = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float verticalRot = 0f;

    void Update()
    {
        if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHoriz, 0);
        }
        else if (axes == RotationAxes.MouseY)
        {
            verticalRot -= Input.GetAxis("Mouse Y") * sensitivityVert;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);

            float horizontalRot = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
        }
        else
        {
            verticalRot -= Input.GetAxis("Mouse Y") * sensitivityVert;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);

            float delta = Input.GetAxis("Mouse X") * sensitivityHoriz;
            float horizontalRot = transform.localEulerAngles.y + delta;
            
            transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
        }
    }
}
