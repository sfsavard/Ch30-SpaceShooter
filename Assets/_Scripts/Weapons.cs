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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
