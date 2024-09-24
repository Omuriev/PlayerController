using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private PlayerInput _playerInput;
    private CharacterController _characterController;

    public PlayerInput PlayerInput => _playerInput;
    public CharacterController CharacterController => _characterController;
    public Animator Animator => _animator;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable() => _playerInput.Enable();

    private void OnDisable() => _playerInput.Disable();

}
