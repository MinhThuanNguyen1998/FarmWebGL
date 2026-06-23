using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArea : MonoBehaviour
{
    [System.Serializable]
    public class AnimalMovementZone
    {
        public AnimalType animalType;
        public Collider movementCollider;
    }

    [SerializeField]
    private List<AnimalMovementZone> movementZones = new List<AnimalMovementZone>();

    public Collider GetMovementAreaFor(AnimalType type)
    {
        foreach (var zone in movementZones)
        {
            if (zone.animalType == type)
            {
                return zone.movementCollider;
            }
        }

        Debug.LogWarning($"Can not find a movement area for: {type}");
        return null;
    }

    public List<AnimalMovementZone> GetAllMovementZones()
    {
        return movementZones;
    }
}
