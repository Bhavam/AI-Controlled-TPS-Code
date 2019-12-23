using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
   public enum State {Idle,Chasing,Attacking}; // creates a list of keywords which can be accessed by state datatype
    State currentState;
    NavMeshAgent pathfinder;
    LivingEntity targetEntity;
    Transform target;
    float attackDistanceThreshold=.5f;
    float nextAttackTime;
    float timeBetweenAttacks=1;
    float myCollisionRadius;
    float targetCollisionRadius;
    Color originalColour;
    Material skinMaterial;
    bool hasTarget;
    protected override void Start()
    { 
       base.Start();
       pathfinder=GetComponent<NavMeshAgent>(); 
       target=GameObject.FindGameObjectWithTag("Player").transform; // assigning the transfrom characteristics of the player
       skinMaterial=GetComponent<Renderer>().material;
       originalColour=skinMaterial.color;      
       if(GameObject.FindGameObjectWithTag("Player") != null)
       {
       StartCoroutine(UpdatePath()); // calling the update path coroutine continuously 
       currentState=State.Chasing;    // default state
       hasTarget=true;
       myCollisionRadius=GetComponent<CapsuleCollider>().radius;
       targetCollisionRadius=target.GetComponent<CapsuleCollider>().radius;
       targetEntity=target.GetComponent<LivingEntity>();
       //Debug.Log("player health"+targetEntity.health);
       targetEntity.OnDeath+=OnTargetDeath;
       }

    }
    void OnTargetDeath()
    {
       hasTarget=false;
       currentState=State.Idle;
    }
    void Update()
    {

        if(Time.time > nextAttackTime && hasTarget) 
        {
          float sqDstToTarget=(target.position-transform.position).sqrMagnitude; // implemnting this as sqaure value to avoid recalc overhead
          if(sqDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius,2))
          {
              nextAttackTime=Time.time+timeBetweenAttacks; // recalc time value for attack consistency
              StartCoroutine(Attack());
          }
        }
    }
    IEnumerator Attack()
    {
        currentState=State.Attacking;
        pathfinder.enabled=false;  //stop pathfinding for the attack duration
        Vector3 dirToTarget=(target.position-transform.position).normalized;
        Vector3 attackPosition=target.position-dirToTarget*(myCollisionRadius+targetCollisionRadius);
        Vector3 originalPosition=transform.position;
        
        float percent=0;  // how further the enemy obj is into the leap trajectory
        float attackSpeed=3;
        skinMaterial.color=Color.red;
        bool hasAppliedDamage=false;
        float damage=1;
        while(percent <= 1)
         {
          if(percent >= 0.5f && !(hasAppliedDamage))
          {
            hasAppliedDamage=true;
            targetEntity.TakeDamage(damage);
          }
           percent+=Time.deltaTime*attackSpeed; // increasing percent of enemy into its leap trajectory 
           float interpolation=(-Mathf.Pow(percent,2)+percent)*4; // calc interpolated value to simulate position going from orig to attack to orig
           transform.position=Vector3.Lerp(originalPosition,attackPosition,interpolation); //check out lerp most prob does smoother vector translations
            yield return null;
         }
        currentState=State.Chasing;
        skinMaterial.color=originalColour;
        pathfinder.enabled=true; // recontinue the pathfinder

    }
    IEnumerator UpdatePath()
    {
     float refreshRate=.25f; // so that path is not readjusted every frame
     while(target != null)
     { 
         if (currentState == State.Chasing)
         {
           Vector3 dirToTarget=(target.position-transform.position).normalized;
           Vector3 targetPosition=target.position-dirToTarget*(myCollisionRadius+targetCollisionRadius+attackDistanceThreshold/2);
           if(!(dead))
             { 
             pathfinder.SetDestination(targetPosition); // sets the movement dir of enemy towards the player acc to path calc
             }
            
         }
        yield return new WaitForSeconds(refreshRate); // delay to avoid recalc of path every frame
     }

    }
}
