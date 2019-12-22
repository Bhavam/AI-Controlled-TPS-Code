using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity

{
    Camera viewCamera;
    public float moveSpeed;
    private PlayerController controller;
    private GunController gunController;
    public override void Start()
    {
        controller=GetComponent<PlayerController>();
        gunController=GetComponent<GunController>();
        viewCamera=Camera.main;
    }

   
    void Update()
    {
        //Movement input
        Vector3 moveInput=new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")); // GetAxisRaw gets input vals more smoothly
        Vector3 moveVelocity=moveInput.normalized*moveSpeed; // normalised to get dir otherwise speed increases with apart dist
        controller.Move(moveVelocity);

        //Look input
        Ray ray=viewCamera.ScreenPointToRay(Input.mousePosition); //code to make a ray (vector line) from camera through the func input  
        Plane groundPlane=new Plane(Vector3.up,Vector3.zero); // create a vector component for the ground
        float rayDistance;
        if(groundPlane.Raycast(ray,out rayDistance))  // outs ray dist from view camera to intersection point of view cam ray and ground plane 
        {
            Vector3 point=ray.GetPoint(rayDistance); //check out the GetPoint method from ray class also research ray class
           //Debug.DrawLine(ray.origin,point,Color.red);
           controller.LookAt(point);
        }

        //Weapon Input
        if(Input.GetMouseButton(0)) // gets left mouse click status
        {
            gunController.Shoot();
        }
    }
}
