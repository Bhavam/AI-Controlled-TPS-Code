using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed=10;  
    float damage=1;
    float lifetime=3;
    float skinWidth=0.1f;
    public LayerMask collisionMask;
     // Update is called once per frame
    void Start()
     {
        Destroy(gameObject,lifetime);
        Collider [] initialCollisions=Physics.OverlapSphere(transform.position,0.1f,collisionMask);//how does this work and also collision mask
        if(initialCollisions.Length > 0 )
        {
          OnHitObject(initialCollisions[0]);
        }
     }
    public void SetSpeed(float newSpeed)
    {
        speed=newSpeed;
    }
    void Update()
    {
      float moveDistance=speed*Time.deltaTime;
      CheckCollisions(moveDistance);
      transform.Translate(Vector3.forward*moveDistance);  
      
    }
    void CheckCollisions(float moveDistance)
    {
      Ray ray=new Ray (transform.position,transform.forward); // create a vector from pos of projectile thru its forward vector
      RaycastHit hit; // check out how this works
      if(Physics.Raycast(ray,out hit,moveDistance+skinWidth,collisionMask,QueryTriggerInteraction.Collide)) // check out how does QueryTriggerInteraction work
      {
          OnHitObject(hit);
      } 
    }
    void OnHitObject(RaycastHit hit) // check out use of RaycastHit
    {
        IDamageable damageableObject=hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeHit(damage,hit); // check out the inheritance tree of use of this object
        }
        GameObject.Destroy(gameObject); //changes from SebLague used Destroy instead of Gameobject.Destroy
    }
	void OnHitObject(Collider c) {
		IDamageable damageableObject = c.GetComponent<IDamageable> ();
		if (damageableObject != null) {
			damageableObject.TakeDamage(damage);
		}
		GameObject.Destroy (gameObject);
	}
}
