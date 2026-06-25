using System.Collections;
using System.Collections.Generic;
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

    private AnimalAnimation m_AnimalAnimation;
    private Collider m_TargetCollider;
    private Vector3 m_TargetPosition;

    // Flag to prevent OnEnable from triggering movement before Start() finishes
    private bool m_IsInitialized = false;

    private Coroutine m_MovementCoroutine;

    [Inject]
    public void Construct(MovementArea movementArea) => m_MovementArea = movementArea;

    protected virtual IEnumerator Start()
    {
        // Wait 1 frame to ensure Zenject dependency injection and environment are fully ready
        yield return null;
        m_AnimalAnimation = GetComponent<AnimalAnimation>();
        m_TargetCollider = m_MovementArea?.GetMovementAreaFor(AnimalType);

        if (m_TargetCollider == null)
        {
            Debug.LogError($"[{name}] Cannot find movement area for {AnimalType}!");
            yield break;
        }

        m_IsInitialized = true;
        TriggerMovementLoop();
    }
    private void OnEnable()
    {
        // Guard: only restart movement if Start() has already completed initialization
        if (m_IsInitialized && m_TargetCollider != null)
            TriggerMovementLoop();
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
            m_AnimalAnimation?.SetAnimTarget(1f);
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
                m_AnimalAnimation?.Tick();

                // Wait for the next frame
                yield return null;
            }
            // 3. Idle/Wait before picking the next target destination
            // Manual loop instead of WaitForSeconds to keep ticking animation blend each frame
            m_AnimalAnimation?.SetAnimTarget(0f);
            float waitTime = Random.Range(m_MinWaitTime, m_MaxWaitTime);
            float elapsed = 0f;
            while (elapsed < waitTime)
            {
                m_AnimalAnimation?.Tick();
                elapsed += Time.deltaTime;
                yield return null;
            }
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
            // ClosestPoint check works correctly for convex colliders (Box, Sphere, Capsule, convex MeshCollider).
            if (m_TargetCollider.ClosestPoint(randomPoint) == randomPoint)
            {
                m_TargetPosition = randomPoint;
                return;
            }
        }
        // Fallback: clamp to collider center if no valid point found after 10 attempts
        m_TargetPosition = bounds.center;
        m_TargetPosition.y = transform.position.y;
    }

}
