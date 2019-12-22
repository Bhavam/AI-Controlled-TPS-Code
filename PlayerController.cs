using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
   private Rigidbody myRigidbody;
   private Vector3 velocity;
    void Start()
    {
        myRigidbody=GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
      velocity=_velocity;     // assigning new velocity
    }  
    public void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position+velocity*Time.deltaTime); //check this out prob like translation for rigidbody
    }
    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint=new Vector3(lookPoint.x,transform.position.y,lookPoint.z); // the capsule was stooping to look adjusted ht corrects that 
        transform.LookAt(heightCorrectedPoint); // check out how does LookAt work
    }
}   
