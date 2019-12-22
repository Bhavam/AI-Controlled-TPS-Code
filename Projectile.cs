using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed=10;  
    float damage=1;
    public LayerMask collisionMask;
     // Update is called once per frame
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
      if(Physics.Raycast(ray,out hit,moveDistance,collisionMask,QueryTriggerInteraction.Collide)) // check out how does QueryTriggerInteraction work
      {
          OnHitObject(hit);
      } 
    }
    void OnHitObject(RaycastHit hit) // check out use of RaycastHit
    {
        IDamagable damagableObject=hit.collider.GetComponent<IDamagable>();
        if(damagableObject != null)
        {
            damagableObject.TakeHit(damage,hit); // check out the inheritance tree of use of this object
        }
        Destroy(gameObject); //changes from SebLague used Destroy instead of Gameobject.Destroy
    }
}
