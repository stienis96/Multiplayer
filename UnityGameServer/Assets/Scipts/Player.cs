using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float massiveJumpSpeed = 1f;
    private bool isMassiveJumpTriggered;

    public Vector3 slideDirection;

    private bool[] inputs;
    public float yVelocity = 0;
    public bool IsAlive;
    public bool IsFinished;

    public long lastAppliedTick = -1;
    public float lastAppliedTickTime;
    internal float serverPacketTime;

    private void Start()
    {
        IsAlive = true;
        IsFinished = false;
        lastAppliedTick = 0;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
        isMassiveJumpTriggered = false;
        slideDirection = Vector3.zero;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        inputs = new bool[5];
    }

    //void FixedUpdate()
    //{
    //    DecideOnMovement();
    //}

    private void ApplyInputs()
    {
        Vector2 inputdirection = Vector2.zero;
        if (inputs[0])
        {
            inputdirection.y += 1;
        }
        if (inputs[1])
        {
            inputdirection.y -= 1;
        }
        if (inputs[2])
        {
            inputdirection.x -= 1;
        }
        if (inputs[3])
        {
            inputdirection.x += 1;
        }

        if (controller.enabled)
        {
            Move(inputdirection);
        }
    }

    private void Move(Vector2 inputdirection)
    {
        Vector3 moveDirection = transform.right * inputdirection.x + transform.forward * inputdirection.y;
        moveDirection *= moveSpeed;
        moveDirection += slideDirection;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }

        if (isMassiveJumpTriggered)
        {
            yVelocity = massiveJumpSpeed * Time.fixedDeltaTime;
            isMassiveJumpTriggered = false;
        }

        yVelocity += gravity;
        moveDirection.y = yVelocity;

        controller.Move(moveDirection);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation, long tick, float tickTime, float serverPacketWriteTime)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
        lastAppliedTick = tick;
        lastAppliedTickTime = tickTime;
        serverPacketTime = serverPacketWriteTime;

        // hack but we shouldn't wait for the fixed update to take place, otherwise the information is sent back too late to the client
        ApplyInputs();
    }

    public void Die()
    {
        controller.enabled = false;
        transform.position = new Vector3(5f, 25f, 0f);
        lastAppliedTick++;
        ServerSend.PlayerPosition(this);
        StartCoroutine(Respawn());
        ServerSend.SendPlayerDeath(id);
        IsAlive = false;

        RoundManager.CheckForEndOfRound();
    }

    public void Finish()
    {
        controller.enabled = false;
        transform.position = new Vector3(5f, 25f, 0f);
        lastAppliedTick++;
        ServerSend.PlayerPosition(this);
        StartCoroutine(Respawn());
        //ServerSend.SendPlayerDeath(id); other package for score board?
        IsFinished = true;

        RoundManager.CheckForEndOfRound();
    }

    private IEnumerator Respawn()
    {
        yield return new WaitUntil(() => RoundManager.IsNewRoundStarting());

        Debug.Log("New round starting, waiting for player");
        RoundManager roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        roundManager.JoinRound(this);
        
        yield return new WaitUntil(() => roundManager.IsRoundFull());

        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    public void MassiveJump(float jumpForce)
    {
        isMassiveJumpTriggered = true;
        massiveJumpSpeed = jumpForce;
    }
}
