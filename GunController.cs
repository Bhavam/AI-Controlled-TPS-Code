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
      equippedGun=Instantiate(gunToEquip) as Gun; //changed as some pos error
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
