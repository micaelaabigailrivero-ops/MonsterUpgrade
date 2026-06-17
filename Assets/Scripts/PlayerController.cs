using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private CharacterController _characterController;
    private PlayerInput _playerInput; 
    private Vector2 _Input;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    private float _yvelocity;
    private InputAction moveAction;
    private bool listoParaMoverse = false;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>(); 

        if (_playerInput != null)
        {
            moveAction = _playerInput.actions["Move"];
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            
            if (_playerInput != null) _playerInput.enabled = true;

            StartCoroutine(SpawnSeguroDueño());
        }
        else
        {
            
            if (_characterController != null) _characterController.enabled = false;
            if (_playerInput != null) _playerInput.enabled = false; 

            listoParaMoverse = true;
        }
    }

    private System.Collections.IEnumerator SpawnSeguroDueño()
    {
        yield return new WaitForSeconds(0.3f);

        if (_characterController != null)
        {
            _characterController.enabled = false;

            GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position + Vector3.up * 2f;
            }
            else
            {
                transform.position = new Vector3(0f, 6f, 0f);
            }

            _characterController.enabled = true;
        }

        listoParaMoverse = true;
        Debug.Log("[NETCODE] ¡Dueño listo para moverse con controles independientes!");
    }

    private void Update()
    {
        if (!listoParaMoverse) return;

        if (IsOwner)
        {
            
            if (moveAction != null)
            {
                _Input = moveAction.ReadValue<Vector2>();
            }

            Vector3 move = new Vector3(_Input.x, 0, _Input.y);

            if (_characterController.isGrounded && _yvelocity < 0)
            {
                _yvelocity = -2f;
            }
            else
            {
                _yvelocity += _gravity * Time.deltaTime;
            }

            move.y = _yvelocity;
            _characterController.Move(move * _speed * Time.deltaTime);


          
           
            }
        }
    }
