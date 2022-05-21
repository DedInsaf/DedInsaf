using UnityEngine;
using System.Collections;

public class Monster : Unit
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();

        if (bullet)
        {
            ReceiveDamage();
        }

        Player player = collider.GetComponent<Player>();

        if (player)
        {
            player.ReceiveDamage();
        }
    }
}
