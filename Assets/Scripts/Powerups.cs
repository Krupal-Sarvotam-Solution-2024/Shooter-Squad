using UnityEngine;
using System.Collections;

public class Powerups : MonoBehaviour
{
    // 1. Typo in enum name - should be "AllPowerups" instead of "allpowerusp"
    public enum AllPowerups  // Consider using PascalCase for better readability
    {
        Shield,     // Fixed typo "shild" -> "Shield"
        Bomb,
        Speed,
        Invisible,  // Changed from "invisble" to match later usage
        Passthrough,// Fixed typo "passthrough" -> "Passthrough"
        BigBomb     // Fixed typo "bigbomb" -> "BigBomb"
    }

    // 2. Add SerializeField for inspector visibility where appropriate
    [SerializeField] private GameManager manager;  // Made private with SerializeField
    [SerializeField] private AllPowerups powerups;
    [SerializeField] private float health;
    [SerializeField] private float dangerRadius;   // Fixed typo "damgeradious" -> "dangerRadius"
    [SerializeField] private GameObject textshower;
    // 3. Consider making these configurable in Inspector
    [SerializeField] private GameObject effectObject;  // Fixed typo "effectobject"

    private void OnTriggerEnter(Collider other)  // Changed to private as it's a Unity callback
    {
        // 4. Null check for entity could be extracted
        Entity entity = other.GetComponent<Entity>();
        if (entity == null && powerups != AllPowerups.Bomb)  // Bomb handles its own collision
        {
            return;
        }
        if(textshower)
        textshower.SetActive(true);

        // 5. Consider using switch statement for better readability
        switch (powerups)
        {
            case AllPowerups.Shield:
                entity.StartCoroutine(entity.ShieldActivate());  // Fixed typo "shildAcitavte"
                gameObject.SetActive(false);
                break;

            case AllPowerups.Bomb:
                Bullet bullet = other.GetComponent<Bullet>();
                if (bullet != null)  // 6. Null check instead of GetComponent directly in if
                {
                    health--;
                    if (health <= 0)
                    {
                        StartCoroutine(Blast(entity));
                    }
                }
                break;

            case AllPowerups.Speed:
                entity.StartCoroutine(entity.SpeedBoost());  // Fixed typo "SpeedBost"
                gameObject.SetActive(false);
                break;

            case AllPowerups.Passthrough:
                entity.StartCoroutine(entity.Invisible());  // Inconsistent method name?
                gameObject.SetActive(false);
                // TODO: Implement passthrough functionality
                break;

            case AllPowerups.Invisible:
                entity.StartCoroutine(entity.Hide());
                gameObject.SetActive(false);
                break;

            // 7. Missing BigBomb implementation
            case AllPowerups.BigBomb:
                // TODO: Add implementation
                break;
        }
    }

    private IEnumerator Blast(Entity entity)  // Changed to private
    {
        GetComponent<MeshRenderer>().enabled = false;
        effectObject.SetActive(true);

        // 8. Null check for manager and botAll
        if (manager?.botAll != null)
        {
            for (int i = 0; i < manager.botAll.Count; i++)
            {
                if (manager.botAll[i] == null) continue;

                if (Vector3.Distance(manager.botAll[i].transform.position, transform.position) < dangerRadius)
                {
                    manager.botAll[i].ReduceHeath(100, null);  // Fixed typo "ReduceHeath" -> "ReduceHealth"?
                }
            }
        }

        yield return new WaitForSeconds(0.5f);  // Added lowercase 'f' for consistency

        // 9. Null check for entity
        if (entity != null)
        {
            entity.StartCoroutine(entity.Invisible());
        }

        gameObject.SetActive(false);
    }
}