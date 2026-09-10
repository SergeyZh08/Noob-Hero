using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    public float MaxHealth = 100;
    public float CurrentHealth;

    public float Damage = 10f;

    void Start()
    {
        CurrentHealth = MaxHealth;
        _materialPropertyBlock = new MaterialPropertyBlock();
        SetNewValue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetDamage();
        }
    }

    public void SetDamage()
    {
        MaxHealth -= Damage;

        if (MaxHealth <= 0)
        {
            MaxHealth = 0;
        }

        SetNewValue();
    }

    private void SetNewValue()
    {
        _materialPropertyBlock.SetFloat("_CurrentHealth", CurrentHealth / MaxHealth);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

}
