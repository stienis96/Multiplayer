using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera localPlayerCamera;

    void Start()
    {
        localPlayerCamera = GameObject.FindWithTag("LocalPlayer").GetComponentInChildren<Camera>();
    }

    void Update()
    {
        transform.LookAt(localPlayerCamera.transform);
    }
}
