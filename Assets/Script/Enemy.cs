using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector3 TargetPosition;
    float speed = 2f;

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

    public void ReturnToPool()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
