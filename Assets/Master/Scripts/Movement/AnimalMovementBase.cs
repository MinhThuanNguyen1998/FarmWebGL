using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;

public abstract class AnimalMovementBase : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float m_MoveSpeed = 2f;
    [SerializeField] protected float m_TurnSpeed = 8f;

    [Header("Wait Time Settings")]
    [SerializeField] protected float m_MinWaitTime = 2f;
    [SerializeField] protected float m_MaxWaitTime = 5f;

    protected abstract AnimalType AnimalType { get; }
    protected MovementArea m_MovementArea;

    private Collider m_TargetCollider;
    private Vector3 m_TargetPosition;

    // Store the coroutine reference to safely manage its lifecycle
    private Coroutine m_MovementCoroutine;

    [Inject]
    public void Construct(MovementArea movementArea) => m_MovementArea = movementArea;

    protected virtual IEnumerator Start()
    {
        // Wait 1 frame to ensure Zenject dependency injection and environment are fully ready
        yield return null;
        m_TargetCollider = m_MovementArea?.GetMovementAreaFor(AnimalType);

        if (m_TargetCollider == null)
        {
            Debug.LogError($"[{name}] Cannot find movement area for {AnimalType}!");
            yield break;
        }
        TriggerMovementLoop();
    }

    private void OnEnable()
    {
        if (m_TargetCollider != null) TriggerMovementLoop();
    }

    private void OnDisable()
    {
        if (m_MovementCoroutine != null)
        {
            StopCoroutine(m_MovementCoroutine);
            m_MovementCoroutine = null;
        }
    }

    private void TriggerMovementLoop()
    {
        if (m_MovementCoroutine != null) StopCoroutine(m_MovementCoroutine);
        m_MovementCoroutine = StartCoroutine(MovementLoop());
    }

    private IEnumerator MovementLoop()
    {
        // Use 'while (enabled)' to safely break the loop if the component is disabled
        while (enabled)
        {
            // 1. Generate a new destination
            SetNextTarget();

            // 2. Move towards the destination until close enough
            // Using sqrMagnitude instead of Vector3.Distance to bypass expensive square root calculations (0.05f * 0.05f = 0.0025f)
            while ((m_TargetPosition - transform.position).sqrMagnitude > 0.0025f)
            {
                // Rotate towards target
                Vector3 direction = m_TargetPosition - transform.position;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
                }
                // Move position
                transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, m_MoveSpeed * Time.deltaTime);

                // Wait for the next frame
                yield return null;
            }

            // 3. Idle/Wait before picking the next target destination
            yield return new WaitForSeconds(Random.Range(m_MinWaitTime, m_MaxWaitTime));
        }
    }

    private void SetNextTarget()
    {
        Bounds bounds = m_TargetCollider.bounds;
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                transform.position.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // Accept point if it's inside the collider, or use the last attempt as fallback
            if (m_TargetCollider.ClosestPoint(randomPoint) == randomPoint || i == 9)
            {
                m_TargetPosition = randomPoint;
                break;
            }
        }
    }
}
