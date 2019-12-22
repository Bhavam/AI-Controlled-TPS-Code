using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamagable
{
  protected float health;
  protected bool dead;
  public float startingHealth;

  public event System.Action OnDeath;  // use of event look up documentation
  public virtual void Start() // use of virtual keyword look up use
  {
      health=startingHealth;
  }
  public void TakeHit(float damage , RaycastHit hit) // health system and also check how does RaycastHit datatype work 
  {
      health-=damage;
      if(health <= 0 && !(dead))
      Die();
  }
  public void Die()
  {
    dead=true;
    if(OnDeath != null)  // event and delegation system use further research req
    {
        OnDeath();
    }
    Destroy(gameObject); // again changed the GameObject method call to direct call 
  }
}
