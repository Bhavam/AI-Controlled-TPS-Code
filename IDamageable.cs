using UnityEngine;

public interface IDamagable // use of interface class improves use of method by objects look up more use and also docs
{
    void TakeHit(float damage,RaycastHit hit);
}