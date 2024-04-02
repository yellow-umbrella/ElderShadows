using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private GameObject target;
    private Vector3 startPos;
    private RangedAttackSO attack;
    private float dmg;
    private GameObject attacker;
    private const string OBSTACLE_TAG = "Tree";

    public void Setup(GameObject target, GameObject attacker, RangedAttackSO attack, float dmg)
    {
        if (target == null) { return; }
        startPos = transform.position;
        transform.up = target.transform.position - transform.position;
        this.target = target;
        this.attack = attack;
        this.dmg = dmg;
        this.attacker = attacker;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, startPos) >= attack.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            target.GetComponent<IAttackable>().TakeDamage(dmg, attack.dmgType, attacker);
            Destroy(gameObject);
        }
        else if (collision.CompareTag(OBSTACLE_TAG))
        {
            Destroy(gameObject);
        }
    }
}
