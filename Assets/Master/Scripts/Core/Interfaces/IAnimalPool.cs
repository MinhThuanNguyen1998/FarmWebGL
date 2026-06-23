using UnityEngine;

public interface IAnimalPool
{
    GameObject Spawn(Vector3 position, Quaternion rotation);
    void Despawn(GameObject animal);
    void Clear();
}
