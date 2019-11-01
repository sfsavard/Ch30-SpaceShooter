using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;      // The speed in m/s
    public float fireRate = 0.3f;  // Seconds/shot (Unused)
    public float health = 10;
    public int score = 100;      // Points earned for destroying this

    protected BoundsCheck bndCheck;                                           // a

    void Awake()
    {                                                          // b
        bndCheck = GetComponent<BoundsCheck>();
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos
    {                                                     // a
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();
        if (bndCheck != null && !bndCheck.isOnScreen)
        {                    // c
            // Check to make sure it's gone off the bottom of the screen
            if (pos.y < - bndCheck.camHeight - bndCheck.radius)
            {            // d
                // We're off the bottom, so destroy this GameObject
                Destroy(gameObject);
            }
        }
    }

    public virtual void Move() // virtual allows you to override a method that already exists
    {                                             // b
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {                                // a
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":                                           // b If the GameObject that hit this Enemy has the ProjectileHero tag, it should damage this Enemy. If it has any other tag, it will be handled by the default case (// f)
                Projectile p = otherGO.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                {                                // c If this Enemy is not on screen, the Projectile GameObject that hit it is destroyed, and break; is called, which exits the switch statement without completing any of the remaining code in the case "ProjectileHero"
                    Destroy(otherGO);
                    break;
                }

                // Hurt this Enemy
                // Get the damage amount from the Main WEAP_DICT.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {// d If this Enemy's health is decreased to below 0, then this Enemy is destroyed. With a default Enemy health of 10 and blaster damageOnHit of 1, this will take 10 shots
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);                                          // e The Projectile GameObject is destroyed
                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name); // f If somehow a GameObject tagged something other than a ProjectileHero hits this Enemy, a message about it posts to the Console pane
                break;

        }
    }
}
