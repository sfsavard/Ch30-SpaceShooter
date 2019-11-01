using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// This is an enum of the various possible weapon types. 
/// It also includes a "shield" type to allow a shield power-up. 
/// Items marked [NI] below are Not Implemented in the IGDPD book. 
/// </summary> 
public enum WeaponType       // As a public enum outside of the Weapon class, WeaponType can be seen by and used by any other script in the project.
{
    none,       // The default / no weapon 
    blaster,    // A simple blaster 
    spread,     // Two shots simultaneously 
    phaser,     // [NI] Shots that move in waves
    missile,    // [NI] Homing missiles 
    laser,      // [NI]Damage over time 
    shield      // Raise shieldLevel 
}

/// <summary> 
/// The WeaponDefinition class allows you to set the properties 
///   of a specific weapon in the Inspector. The Main class has 
///   an array of WeaponDefinitions that makes this possible. 
/// </summary> 
[System.Serializable]                                                         // a  The [System.Serializable] attribute causes the class defined immediately after it to be serializable and editable within the Unity Inspector
public class WeaponDefinition
{                                                // b  You can alter each of the fields of WeaponDefinition to change an aspect of the bullets fired by your ship
    public WeaponType type = WeaponType.none;
    public string letter;                // Letter to show on the power-up 
    public Color color = Color.white;       // Color of Collar & power-up 
    public GameObject projectilePrefab;          // Prefab for projectiles 
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;           // Amount of damage caused 
    public float continuousDamage = 0;      // Damage per second (Laser) 
    public float delayBetweenShots = 0;
    public float velocity = 20;             // Speed of projectiles 
}

public class Weapons : MonoBehaviour
{

    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired

    private Renderer collarRend;

    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Call SetType() for the default _type of WeaponType.none
        SetType(_type);                                                    // a When the Weapon GameObject starts, it calls SetType() with whatever WeaponType _type is set to. This ensures that either the Weapon disappears (if _type is WeaponType.none) or shows the correct collar color

        // Dynamically create an anchor for all Projectiles
        if (PROJECTILE_ANCHOR == null)
        {                                     // b PROJECTILE_ANCHOR is a static Transform created to act as a parent in the Hierarchy to all the Projectiles created by Weapon scripts
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;                       // c Weapons are always attached to other GameObjects (like _Hero). This finds the root GameObject of which this Weapon is a child
        if (rootGO.GetComponent<Hero>() != null)
        {                         // d If this root GameObject has a Hero script attached to it, then the Fire() method of this Weapon is added to the fireDelegate delegate of that Hero class instance
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {                                       // e If type is WeaponType.none, this GameObject is disabled. When a GameObject is not active, it doesn't receive any MonoBehaviour method calls
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);                               // f Not only does SetType() set whether or not the GameObject is active, it also pulls the proper WeaponDefinition from Main, sets the color of the Collar, and resets lastShotTime
        collarRend.material.color = def.color;
        lastShotTime = 0; // You can fire immediately after _type is set.    // g Resetting lastShotTime to 0 allows this Weapon to be fired immediately
    }

    public void Fire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;                           // h In any case where gameObject.activeInHierarchy is false, this function will return, and the Weapon will not fire
        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {              // i If the difference between the current time and the last time this Weapon was fired is less than the delayBetweenShots defined in the WeaponDefinition, this Weapon will not fire
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;                             // j An initial velocity in the up direction is set, but if transform.up.y is < 0 (which would be true for Enemy Weapons that are facing downward), the y component of vel is set to face downward as well.
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {                                                      // k The WeaponType.blaster generates a single Projectile by calling MakeProjectile() and then assigns a velocity to its Rigidbody in the direction of vel
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:                                          // l If the _type is WeaponType.spread, then three different Projectiles are created. Two of them have their direction rotated 10 degrees around the Vector3.back axis (i.e., the -z axis that extends out of the screen toward you). Then, their Rigidbody.velocity is set to the multiplication of that rotation by vel.
                p = MakeProjectile();     // Make middle Projectile
                p.rigid.velocity = vel;
                p = MakeProjectile();     // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();     // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

        }
    }

    public Projectile MakeProjectile()
    {                                    // m The MakeProjectile() method instantiates a clone of the prefab stored in the WeaponDefinition and returns a reference to the attached Projectile class instance
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {                  // n Based on whether this was fired by the _Hero or an Enemy, the Projectile is given the proper tag and physics layer
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);                  // o The Projectile GameObject's parent is set to be PROJECTILE_ANCHOR. This places it under _ProjectileAnchor in the Hierarchy pane
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;                                           // p lastShotTime is set to the current time, preventing this Weapon from shooting for def.delayBetweenShots seconds
        return (p);

    }
    
}
