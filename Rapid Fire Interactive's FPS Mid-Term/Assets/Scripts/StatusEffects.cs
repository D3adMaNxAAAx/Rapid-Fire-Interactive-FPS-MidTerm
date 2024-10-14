using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    public float duration;  // How long the effect lasts
    protected float timer;  // Tracks time since the effect was applied
    public abstract void ApplyEffect(GameObject target);  // Apply effect logic
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void UpdateEffect()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            EndEffect();  // End the effect when the timer is up
        }
    }

    public virtual void EndEffect()
    {
        Destroy(this);  // Remove effect from the object
    }

    public class SlownessEffect : StatusEffects
    {
        public float slowAmount = 0.5f;  // How much to slow the target

        public override void ApplyEffect(GameObject target)
        {
            if (target.TryGetComponent(out playerMovement player))
            {
                player.setSpeed(player.getSpeed() * slowAmount);
            }
            else if (target.TryGetComponent(out enemyAI enemy))
            {
                enemy.agent.speed *= slowAmount;
            }
        }

        public override void EndEffect()
        {
            // Reset speed back to normal
            ApplyEffect(gameObject);  // Reapply to undo the slow effect
            base.EndEffect();
        }
    }

    public class BurningEffect : StatusEffects
    {
        public float damagePerSecond = 5f;

        public override void ApplyEffect(GameObject target)
        {
            StartCoroutine(InflictBurn(target));
        }

        private IEnumerator InflictBurn(GameObject target)
        {
            while (timer < duration)
            {
                if (target.TryGetComponent(out IDamage damageable))
                {
                    damageable.takeDamage(damagePerSecond);
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public class ToxicEffect : StatusEffects
    {
        public float damagePerSecond = 2f;

        public override void ApplyEffect(GameObject target)
        {
            StartCoroutine(InflictToxicDamage(target));
        }

        private IEnumerator InflictToxicDamage(GameObject target)
        {
            while (timer < duration)
            {
                if (target.TryGetComponent(out IDamage damageable))
                {
                    damageable.takeDamage(damagePerSecond);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public class BleedingEffect : StatusEffects
    {
        public float damagePerTick = 3f;

        public override void ApplyEffect(GameObject target)
        {
            StartCoroutine(InflictBleed(target));
        }

        private IEnumerator InflictBleed(GameObject target)
        {
            while (timer < duration)
            {
                if (target.TryGetComponent(out IDamage damageable))
                {
                    damageable.takeDamage(damagePerTick);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }

    }

}
