using UnityEngine;

public class TreeColliderAdder : MonoBehaviour
{
    public Terrain terrain;
    public GameObject colliderPrefab;  // An empty GameObject with a collider attached

    void Start()
    {
        AddColliders();
    }

    void AddColliders()
    {
        foreach (TreeInstance tree in terrain.terrainData.treeInstances)
        {
            Vector3 worldPosition = Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.transform.position;
            GameObject newCollider = Instantiate(colliderPrefab, worldPosition, Quaternion.identity);
            newCollider.transform.parent = terrain.transform; // Keep colliders organized
        }
    }
}
