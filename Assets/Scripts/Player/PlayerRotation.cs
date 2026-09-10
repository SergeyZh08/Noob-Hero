using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Player Player { get; private set; }
    [SerializeField] private float _rotationSpeed = 10f;
    public Vector3 Forward => transform.forward;
    public Quaternion Rotation => transform.rotation;

    public void Init(Player player)
    {
        Player = player;
    }

    private Vector3 _direction;

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        if (_direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(_direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
