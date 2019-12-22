using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
   public Transform weaponHold;
   Gun equippedGun;  
   public Gun startingGun;
   void Start()
   {
       if(startingGun != null)
       {
           EquipGun(startingGun);
       }
   }
   public void EquipGun(Gun gunToEquip)
   {
      if(equippedGun != null)
      {
          Destroy(equippedGun.gameObject);
      }
      equippedGun=Instantiate(gunToEquip,weaponHold.position+new Vector3(-1,2,-1),weaponHold.rotation) as Gun; //changed as some pos error check it out
      equippedGun.transform.parent=weaponHold; //see how this works
   }
   public void Shoot()
   {
       if(equippedGun != null)
       {
           equippedGun.Shoot();
       }
   }
}
