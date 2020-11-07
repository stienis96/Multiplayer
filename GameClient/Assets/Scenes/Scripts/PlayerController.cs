using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static List<ClientPlayerState> stateBuffer;
    public static List<ServerWorldState> serverStateBuffer;

    public CharacterController controller;

    public bool isMassiveJumpTriggered;
    public Vector3 slideDirection; // FIXME rotating platform

    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float massiveJumpSpeed = 1f;  // FIXME jump platform
    public float yVelocity = 0;

    void Awake()
    {
        stateBuffer = new List<ClientPlayerState>();
        serverStateBuffer = new List<ServerWorldState>();

        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
        isMassiveJumpTriggered = false;
        slideDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Debug.Log($"Current tick {GameManager.tick} - {transform.position}");
        SendInputToServer();
        ProcessServerState();

        GameManager.tick++;
    }

    private void ProcessServerState()
    {
        ServerWorldState[] copyServerState = new ServerWorldState[serverStateBuffer.Count];
        serverStateBuffer.CopyTo(copyServerState);
        serverStateBuffer.Clear();
        foreach (ServerWorldState serverState in copyServerState)
        {
            if (GameManager.players.TryGetValue(serverState.clientId, out PlayerManager player))
            {
                CharacterController characterController = player.GetComponent<CharacterController>();
                if (player.id == Client.instance.myId)
                {
                    ClientPlayerState clientStateAtTick = stateBuffer.Find(state => state.tick == serverState.tick);
                    if (clientStateAtTick != null && AreEqualPositionsWithMargin(serverState.position, clientStateAtTick.position))
                    {
                        Debug.Log($"Positions are equal at tick {serverState.tick} - {serverState.position}");
                        continue; // Skip when they're the same
                    }
                    else if (clientStateAtTick != null)
                        Debug.Log($"Positions at tick {serverState.tick} diverged with server at {serverState.position} and client at {clientStateAtTick.position}");
                    // Set server position to player
                    ApplyMovementDelta(GetDeltaPosition(serverState.position, player.transform));
                    transform.rotation = serverState.rotation;

                    if (!controller.isGrounded)
                    {
                        // Then take over the yVelocity based on the server
                        yVelocity = serverState.serverYVelocity;
                    }

                    // Remove all states from the buffer these have now been made obsolete by the server
                    stateBuffer.RemoveAll(state => state.tick <= serverState.tick); // fixme clean remove?
                    //PrintBuffer(serverState);
                    for (int i = 0; i < stateBuffer.Count; i++)
                    {
                        // Apply buffered states that haven't been ack
                        ClientPlayerState bufferedState = stateBuffer[i];

                        // Predict based on the new ack state
                        Vector3 predictedDeltaPos = GetMovementVector(bufferedState.inputs);
                        yVelocity = bufferedState.yVelocity;
                        ApplyMovementDelta(predictedDeltaPos);
                    }
                }
                else // This is always a new position packet (because it's form a different player), therefore apply it
                {
                    player.SetPosition(serverState.position);
                }
            }
        }
    }

    private bool AreEqualPositionsWithMargin(Vector3 position1, Vector3 position2)
    {
        const int precision = 1;
        return Math.Round(position1.x, precision).Equals(Math.Round(position2.x, precision))
            && Math.Round(position1.y, precision).Equals(Math.Round(position2.y, precision))
            && Math.Round(position1.z, precision).Equals(Math.Round(position2.z, precision));
    }

    private static void PrintBuffer(ServerWorldState serverState)
    {
        Debug.Log($"buffer size at server tick {serverState.tick}: {stateBuffer.Count}");
        stateBuffer.ForEach((el) => { Debug.Log($"\t {el.tick}"); });
    }

    private static Vector3 GetDeltaPosition(Vector3 serverAuthorizedPosition, Transform playerTransform)
    {
        // Get difference beteween serverAuthorizedPosition and it's current position and let the delta be handled by the character controller
        return serverAuthorizedPosition - playerTransform.position;
    }

    private void SendInputToServer()
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button2),
        };
        // Send to user these inputs for the tick, let server decide
        ClientSend.PlayerMovement(GameManager.tick, inputs);

        // Client side prediction
        Vector3 predictedDeltaPos = GetMovementVector(inputs);
        ApplyMovementDelta(predictedDeltaPos);
        stateBuffer.Add(new ClientPlayerState(GameManager.tick, inputs, transform.position, yVelocity));
    }

    public Vector3 GetMovementVector(bool[] inputs)
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

        return ConvertToVector3(inputdirection, inputs);
    }

    private Vector3 ConvertToVector3(Vector2 inputdirection, bool[] inputs)
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

        return moveDirection;
    }

    public void ApplyMovementDelta(Vector3 movementDelta)
    {
        //controller.Move(Vector3.Lerp(Vector3.zero, movementDelta, REMOVE_JITTER_BY_LERP));
        controller.Move(movementDelta);
    }

    public void MassiveJump(float jumpForce)
    {
        isMassiveJumpTriggered = true;
        massiveJumpSpeed = jumpForce;
    }

}
