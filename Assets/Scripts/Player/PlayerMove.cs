using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public event Action<bool> OnRunStarting;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerRotation _rotation;
    public Player Player { get; private set; }
    private bool _isRun = false;
    private bool _oldState = false;

    private Vector2 _movement;

    public void Init(Player player)
    {
        Player = player;
    }

    private void Update()
    {
        _movement = _joystick.Value;

        if (_movement != Vector2.zero)
        {
            _rotation.SetDirection(new Vector3(_movement.x, 0f, _movement.y));
        }
    }

    private void FixedUpdate()
    {
        Vector3 speedVector = _speed * (1 + Player.Stats.MovementSpeed) * new Vector3(_movement.x, 0f, _movement.y);
        _rigidbody.linearVelocity = speedVector;

        _isRun = _rigidbody.linearVelocity != Vector3.zero;

        if (_isRun != _oldState)
        {
            _oldState = _isRun;
            OnRunStarting?.Invoke(_isRun);
        }
    }

}
