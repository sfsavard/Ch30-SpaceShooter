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

    public float showDamageDuration = 0.1f; // # seconds to show damage // a
    public float powerUpDropChance = 1f;  // Chance to drop a power-up     // a powerUpDropChance determines how likely this Enemy is to drop a PowerUp when it is destroyed. A value of 0 will never drop a PowerUp, and a 1 will always drop one

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// All the Materials of this & its children
    public bool showingDamage = false;
    public float damageDoneTime; // Time to stop showing damage
    public bool notifiedOfDestruction = false; // Will be used later

    protected BoundsCheck bndCheck;                                           // a

    void Awake()
    {                                                          // b
        bndCheck = GetComponent<BoundsCheck>();

        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject);                     // b The materials array is filled using the new Utils.GetAllMaterials() method. Then, code here iterates through all the materials and stores their original color
        // Though all of the Enemy GameObjects are currently white, this method allows you to set whatever color you want on them, colors each one red when the Enemy is damaged, and then returns them to their original color.
        // Importantly, this call to Utils.GetAllMaterials() is made in the Awake() method, and the result is cached in materials. This ensures that it only happens once for each Enemy
        // Utils.GetAllMaterials() makes use of GetComponentsInChildren<>(), which is a somewhat slow function that can take processing time and decrease performance. As such, it is generally better to call it once and cache the result rather than calling it every frame
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
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

        if (showingDamage && Time.time > damageDoneTime)
        {                 // c If the Enemy is currently showing damage (i.e., it's red) and the current time is later than damageDoneTime, UnShowDamage() is called
            UnShowDamage();
        }

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
                ShowDamage();                                                // d A call to ShowDamage() is added to the section of OnCollisionEnter() that damages the Enem

                // Get the damage amount from the Main WEAP_DICT.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    // Tell the Main singleton that this ship was destroyed     // b Immediately before this Enemy is destroyed, it notifies the Main singleton by calling ShipDestroyed(). This only happens once for each ship, which is enforced by the notifiedOfDestruction bool
                    if (!notifiedOfDestruction)
                    {
                        Main.S.shipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    // Destroy this Enemy 
                    Destroy(this.gameObject);
                }
                // d If this Enemy's health is decreased to below 0, then this Enemy is destroyed. With a default Enemy health of 10 and blaster damageOnHit of 1, this will take 10 shots
                // Destroy this Enemy
                //Destroy(this.gameObject);
                //}
                Destroy(otherGO);                                          // e The Projectile GameObject is destroyed
                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name); // f If somehow a GameObject tagged something other than a ProjectileHero hits this Enemy, a message about it posts to the Console pane
                break;

        }
    }

    void ShowDamage()
    {                                                      // e ShowDamage() turns all materials in the materials array red, sets showingDamage to true, and sets the time at which it should stop showing damage.
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {                                                    // f UnShowDamage() turns all materials in the materials array back to their original color and sets showingDamage to false
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
