using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    public MeshRenderer model;

    private Vector3 fromPos = Vector3.zero;
    public Vector3 toPos = Vector3.zero;
    private float lastTime;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    public void SetPosition(Vector3 position)
    {
        fromPos = toPos;
        toPos = position;
        lastTime = Time.time;
    }

    private void Update()
    {
        if (Client.instance.myId != id)
        {
            // smooth motion for non-local player
            transform.position = Vector3.Lerp(fromPos, toPos, (Time.time - lastTime) / (1.0f / Constants.TICKS_PER_SEC));
        }
    }

    public void Die(int id)
    {
        if (Client.instance.myId == id)
        {
            GameObject.Find("Menu").GetComponent<UIManager>().OnDie();
        }
        model.enabled = false;
    }

    public void Respawn(int id)
    {
        if (Client.instance.myId == id)
        {
            GameObject.Find("Menu").GetComponent<UIManager>().OnRespawn();
        }
        model.enabled = true;
    }
}
