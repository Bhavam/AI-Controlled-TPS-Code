using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed=10;  
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
      Ray ray=new Ray (transform.position,transform.forward);
      RaycastHit hit;
      if(Physics.Raycast(ray,out hit,moveDistance,collisionMask,QueryTriggerInteraction.Collide))
      {
          OnHitObject(hit);
      } 
    }
    void OnHitObject(RaycastHit hit)
    {
        Debug.Log(hit.collider.gameObject.name);
        Destroy(gameObject); //changes from SebLague used Destroy instead of Gameobject.Destroy
    }
}
