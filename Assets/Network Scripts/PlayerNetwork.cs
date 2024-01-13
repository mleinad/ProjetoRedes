using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour 
{
    // Start is called before the first frame update
    private CharacterController characterController;
    public PlayerController playerController;

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData
        {
        _int = 10,
        _bool = true,
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    [SerializeField]
    private Transform spawnedObjectPrefab;

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + " -> " + newValue._int +", "+ newValue._bool);
        };
    }

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = new PlayerController();

        playerController.PlayerCombat.Enable();
        playerController.PlayerCombat.Jump.performed += NewRandomInt;
    }

    // Update is called once per frame
    void Update()
    {

      

        if (!IsOwner)
        {
            return;
        }




        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * 9.8f * Time.deltaTime);
        }

        PlayerMove();

    }

    private void PlayerMove()
    {

        Vector2 playerInput = playerController.PlayerCombat.Move.ReadValue<Vector2>();

        characterController.Move(playerInput*Time.deltaTime* 4f);

    }


    void NewRandomInt(InputAction.CallbackContext context)
    {
        if (!IsOwner)
        {
            return;
        }

        Transform spawnedObjectTransform = Instantiate(spawnedObjectPrefab);

        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        //randomNumber.Value = new MyCustomData
        //{
        //    _int = Random.RandomRange(1, 100),
        //    _bool = false,

        //};


    }

    [ServerRpc]
    void TestServerRpc()
    {
        Debug.Log("test server rpc " + OwnerClientId);
    }



    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("test client rpc");
    }
}
