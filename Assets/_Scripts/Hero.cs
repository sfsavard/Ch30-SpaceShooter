﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; // Singleton                                // a
    [Header("Set in Inspector")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab; //asks for the prefab in inspector
    public float projectileSpeed = 40;
    public Weapons[] weapons;                                        // a weapons array

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1; // Remember the underscore
    //public float shieldLevel = 1;

    // This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGo = null;
    // Declare a new delegate type WeaponFireDelegate 
    public delegate void WeaponFireDelegate();                               // a Though both are public, neither the WeaponFireDelegate() delegate type nor the fireDelegate field will appear in the Unity Inspector
    // Create a WeaponFireDelegate field named fireDelegate. 
    public WeaponFireDelegate fireDelegate;                 // allows you to save functions as a variable

    void Awake()
    {
        if (S == null)
        {
            S = this; // Set the Singleton                                    // a
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
       // fireDelegate += TempFire;                                            // b Adding TempFire to the fireDelegate causes TempFire to be called any time fireDelegate is called like a function
    }

    void Update()
    {
        // Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");                            // b
        float yAxis = Input.GetAxis("Vertical");                              // b

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic                      // c
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //        if ( Input.GetKeyDown(KeyCode.Space) ) {                           // c 
        //            TempFire();                                                    // c 
        //        }                                                                  // c 

        // Use the fireDelegate to fire Weapons 
        // First, make sure the button is pressed: Axis("Jump") 
        // Then ensure that fireDelegate isn't null to avoid an error 
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {            // d Input.GetAxis("Jump") is equal to 1 when the space bar or jump button on a controller is pressed
            fireDelegate();                                                 // e fireDelegate is called here as if it were a function. This, in turn, calls all the functions that have been added to the fireDelegate delegate


        }
    }

    //void TempFire()
    //{                                                        // b
    //    GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //    projGO.transform.position = transform.position;
    //    Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    //    //rigidB.velocity = Vector3.up * projectileSpeed;
    //                            // g 

    //        Projectile proj = projGO.GetComponent<Projectile>();                 // h 
    //        proj.type = WeaponType.blaster;
    //        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
    //        rigidB.velocity = Vector3.up * tSpeed;
    //    }

    void OnTriggerEnter(Collider other)
    {
        // print("Triggered: " + other.gameObject.name);
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: "+go.name);                                      // b

        // Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo)
        {                                           // c
            return;
        }
        lastTriggerGo = go;                                                  // d

        if (go.tag == "Enemy")
        {  // If the shield was triggered by an enemy
            shieldLevel--;        // Decrease the level of the shield by 1
            Destroy(go);          // … and Destroy the enemy                 // e
        }
        else if (go.tag == "PowerUp")
        {
            // If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);                      // f
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {

            // Leave this switch block empty for now.
            case WeaponType.shield:                                          // a If the PowerUp has the WeaponType shield, it increases the shield level by 1
                shieldLevel++;
                break;

            default:                                                         // b Any other PowerUp WeaponType will be a weapon, so that is the default state
                if (pu.type == weapons[0].type)
                { // If it is the same type  // c If the PowerUp is the same WeaponType as the existing weapons, a search occurs for an unused weapon slot and an attempt is made to set that empty slot to the same weapon type. If all five slots are already in use, nothing happens
                    Weapons w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        // Set it to pu.type
                        w.SetType(pu.type);
                    }
                }
                else
                { // If this is a different weapon type               // d If the PowerUp is a different WeaponType, then all weapon slots are cleared, and Weapon_0 is set to the new WeaponType that was picked up
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);                                         // a The get clause just returns the value of _shieldLevel
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);                             // b ensures that _shieldLevel is never set to a number higher than 4.
            // If the shield is going to be set to less than zero
            if (value < 0)
            {                                                 // c If the value passed into the set clause is less than 0, _Hero is destroyed
                Destroy(this.gameObject);
                // Tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);                 // a
            }
        }
    }

    Weapons GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapons w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
