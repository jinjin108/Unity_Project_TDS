using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    Vector3 TargetPosition;
    float speed = 2f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    public void StartMove(Vector3 startPos)
    {
        transform.position = startPos;
        TargetPosition = ObjectManager.Instance.TargetPosition.position;
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, TargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
