using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayShooter : MonoBehaviour
{

    //Private field; stores a reference to the camera
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // OnGUI method; for drawing a crosshair
    private void OnGUI()
    {
        int size = 24;

        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;

        GUI.Label(new Rect(posX, posY, size, size), "+");
    }



    // coroutine
    // Place down a spehere at a location, which then disappears after one second
    private IEnumerator SphereIndicator(Vector3 pos)
    {
        // Create a new sphere game object
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Place sphere at pos passed in 
        sphere.transform.position = pos;

        // Wait one second
        yield return new WaitForSeconds(1);

        // Destory the sphere
        Destroy(sphere);

    }

    // Update is called once per frame
    void Update()
    {
        // When the player left-clicks, perferm a raycast
        if (Input.GetMouseButtonDown(0))
        {
            // Calculare the center of the screen
            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);

            // Create a ray whose starting point is the middle of the screen
            Ray ray = cam.ScreenPointToRay(point);

            // Create a raycast object to figure out what was hit
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Print out the coords of where the ray hit
                Debug.Log("Hit: " + hit.point);

                //If the object hit was a reactive target, say that it was hit
                // Otherwise, place down a sphere
                GameObject hitObject = hit.transform.gameObject;
                EnemyTarget target = hitObject.GetComponent<EnemyTarget>();
                if (target != null)
                {
                    target.ReactToHit();
                    Debug.Log("Target hit!");
                }
                else
                {
                    StartCoroutine(SphereIndicator(hit.point));
                }
            }
        }
    }
}
