using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public float punchDamage = 5f;
    public LayerMask enemyLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            if (health != null && !health.IsBlocking)
            {
                health.TakeDamage(punchDamage);
                Debug.Log("✅ Enemy hit via trigger!");
            }
            else
            {
                Debug.Log("🛡️ Enemy blocked or no HealthSystem.");
            }
        }
    }
}
