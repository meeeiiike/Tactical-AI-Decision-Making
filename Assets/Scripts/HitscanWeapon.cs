using UnityEngine;

public class HitscanWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float baseDamage = 20f;
    public float maxRange = 50f;
    public float fireRate = 0.2f; 
    [Range(0f, 0.2f)] public float accuracySpread = 0.02f;

    [Header("Damage Falloff")]
    public float falloffStartRange = 15f;
    public float minDamageModifier = 0.5f;

    [Header("Configuration")]
    public Transform firePoint;
    public LayerMask hitMask;
    public bool isHeadlessTraining = false;

    [Header("Visuals")]
    public LineRenderer bulletTracer;

    private float nextFireTime;

    public void Fire(Vector3 targetPosition)
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        // 1. Calculate accuracy spread
        Vector3 baseDirection = (targetPosition - firePoint.position).normalized;
        Vector3 spread = new Vector3(
            Random.Range(-accuracySpread, accuracySpread),
            Random.Range(-accuracySpread, accuracySpread),
            Random.Range(-accuracySpread, accuracySpread)
        );
        Vector3 fireDirection = (baseDirection + spread).normalized;

        // 2. Physics Raycast
        if (Physics.Raycast(firePoint.position, fireDirection, out RaycastHit hit, maxRange, hitMask))
        {
            float damage = CalculateDamage(hit.distance);

            // Avoids garbage allocation overhead of GetComponent
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
                Debug.Log($"<color=green>{gameObject.name} hit {hit.collider.name} at Time: {Time.time}</color>");
            }

            if (!isHeadlessTraining) SpawnVisuals(hit.point);
        }
        else
        {
            if (!isHeadlessTraining) SpawnVisuals(firePoint.position + fireDirection * maxRange);
        }
    }

    private float CalculateDamage(float distance)
    {
        if (distance <= falloffStartRange) return baseDamage;
        float falloffRatio = Mathf.Clamp01((distance - falloffStartRange) / (maxRange - falloffStartRange));
        return Mathf.Lerp(baseDamage, baseDamage * minDamageModifier, falloffRatio);
    }

    private void SpawnVisuals(Vector3 targetPoint)
    {
        // Enterprise constraint: Use Object Pooling here in production builds.
        if (bulletTracer == null) return;
        LineRenderer trail = Instantiate(bulletTracer, firePoint.position, Quaternion.identity);
        trail.SetPosition(0, firePoint.position);
        trail.SetPosition(1, targetPoint);
        Destroy(trail.gameObject, 0.05f);
    }
}