using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image fill;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 4f, 0f);
    [SerializeField] private Camera cam;
    
    private BossController boss;

    private void Start()
    {
        if (cam == null) cam = Camera.main;

        boss = GetComponentInParent<BossController>();
        
        if (boss != null)
        {
            if (target == null) target = boss.transform;
            
            // Subscribe to event
            boss.OnHealthChanged += UpdateBar;
            // Initial update
            UpdateBar(boss.CurrentHealth, boss.MaxHealth);
        }
    }

    private void LateUpdate()
    {
        if (target != null) transform.position = target.position + worldOffset;

        if (cam != null)
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
        }
    }

    private void OnDestroy()
    {
        if (boss != null) boss.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        float t = (max <= 0) ? 0f : Mathf.Clamp01((float)current / max);
        if (fill != null) fill.fillAmount = t;
    }
}