using UnityEngine;
using Fusion;

public class Player: NetworkBehaviour
{
    private NetworkCharacterController _cc;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject ballPrefab;

    private void Awake() {
        _cc = GetComponent<NetworkCharacterController>();
    }

    private Vector3 moveDirection;
    public override void FixedUpdateNetwork() {
        if (GetInput(out NetworkInputData inputData)) {

            if (inputData.moveActionValue.magnitude > 0) {
                inputData.moveActionValue.Normalize();   //  Ensure that the vector magnitude is 1, to prevent cheating.
                moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
                Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
                //Debug.Log($"{speed} * {moveDirection} * {Runner.DeltaTime} = {DeltaX}");
                _cc.Move(DeltaX);
            }

            if (inputData.shootActionValue) {
                Debug.Log("SHOOT!");
                Runner.Spawn(ballPrefab,
                    transform.position + moveDirection, Quaternion.LookRotation(moveDirection),
                    Object.InputAuthority);
            }
        }
    }
}
