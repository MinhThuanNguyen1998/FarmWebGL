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
 
    private Dictionary<AnimalType, Collider> m_ZoneCache;
 
    private void Awake()
    {
        m_ZoneCache = new Dictionary<AnimalType, Collider>(movementZones.Count);
        foreach (var zone in movementZones)
        {
            if (zone.movementCollider == null)
            {
                Debug.LogWarning($"[MovementArea] Zone for {zone.animalType} has no collider assigned.");
                continue;
            }
 
            if (!m_ZoneCache.TryAdd(zone.animalType, zone.movementCollider))
                Debug.LogWarning($"[MovementArea] Duplicate zone for {zone.animalType} — skipping.");
        }
    }
 
    public Collider GetMovementAreaFor(AnimalType type)
    {
        if (m_ZoneCache.TryGetValue(type, out var collider)) return collider;
 
        Debug.LogWarning($"[MovementArea] Cannot find movement area for: {type}");
        return null;
    }
 
    public List<AnimalMovementZone> GetAllMovementZones()
    {
        return movementZones;
    }

}
