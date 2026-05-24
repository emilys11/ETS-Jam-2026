using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public Camera DebugOnlyCam;
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            Ray ray = DebugOnlyCam.ScreenPointToRay(currentMousePos);
            Plane gridPlane = new Plane(Vector3.forward, Vector3.zero);
            if (gridPlane.Raycast(ray, out float enterDistance))
            {
                Vector3 worldIntersectionPoint = ray.GetPoint(enterDistance);
                Debug.Log(ValidSpawnHelper.Instance.ValidSpawnLocation(worldIntersectionPoint));
            }
        }
    }
}