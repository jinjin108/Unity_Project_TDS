using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] float m_Speed = 2;

    [SerializeField] float m_JumpForce;

    [SerializeField] float m_DetectDistance;

    [SerializeField] LayerMask m_EnemyLayerMask;

    private bool m_IsJumping = false;
    private float m_JumpCooldown = 3f;
    private float m_LastJumpTime = -3f;
    CapsuleCollider2D m_Cl;
    private Rigidbody2D m_Rb;

    Vector3 m_TargetPosition;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Cl = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        m_EnemyLayerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
    }

    public void StartMove(Vector3 startPos)
    {
        transform.position = startPos;
        m_TargetPosition = ObjectManager.Instance.TargetPosition.position;
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, m_TargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, m_Speed * Time.deltaTime);
            DetectAndJump();
            yield return null;
        }
    }

    void DetectAndJump()
    {
        if (Time.time - m_LastJumpTime < m_JumpCooldown || m_IsJumping) return;

        Vector2 rayOrigin = new Vector2(m_Cl.bounds.min.x - 0.1f, transform.position.y + 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, m_DetectDistance, m_EnemyLayerMask);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            Jump();
        }
    }

    void Jump()
    {
        m_Rb.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
        m_IsJumping = true;
        m_LastJumpTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == gameObject.layer)
        {
            m_IsJumping = false;
        }
    }
}
