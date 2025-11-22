using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image fill;   // Drag Bar_FG here
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.9f, 0f);
    [SerializeField] private Camera cam;
    
    // TURN THIS OFF IN INSPECTOR IF YOU WANT TO SEE THE BAR ALWAYS
    [SerializeField] private bool hideWhenFull = true;

    private EnemyManager health;
    private const float Epsilon = 1e-6f;

    private void Start()
    {
        if (cam == null) cam = Camera.main;

        health = GetComponentInParent<EnemyManager>();
        if (health == null)
        {
            enabled = false;
            return;
        }

        if (target == null) target = health.transform;

        health.OnHealthChanged += OnHealthChanged;
        // Force initial update
        OnHealthChanged(health.CurrentHealth, health.MaxHealth);
    }

    private void LateUpdate()
    {
        // 1. Follow position
        if (target != null) transform.position = target.position + worldOffset;

        // 2. Billboard (Face Camera)
        if (cam != null)
        {
            Vector3 toCam = transform.position - cam.transform.position;
            if (toCam.sqrMagnitude > Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(toCam, Vector3.up);
            }
        }
    }

    private void OnDestroy()
    {
        if (health != null) health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int current, int max)
    {
        float t = (max <= 0) ? 0f : Mathf.Clamp01((float)current / max);
        
        if (fill != null) fill.fillAmount = t;

        // Hide logic
        if (hideWhenFull && fill != null && fill.transform.parent != null)
        {
            fill.transform.parent.gameObject.SetActive(t < 0.999f);
        }
    }
}