using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]                                                        // a The [SerializeField] attribute above the _type declaration forces _type to be visible and settable in the Unity Inspector even though it is private
    private WeaponType      _type;                                        // b 

    // This public property masks the field _type and takes action when it is set 
    public WeaponType type
    {                                           // c 
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);                                                // c he set clause calls the SetType() method, allowing you to do more than just set _type.
        }
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();                                     // d 
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    /// <summary> 
    /// Sets the _type private field and colors this projectile to match the 
    ///   WeaponDefinition. 
    /// </summary> 
    /// <param name="eType">The WeaponType to use.</param> 
    public void SetType(WeaponType eType)
    {                               // e SetType() not only sets the _type private field but also colors the projectile to match the color based on the weaponDefinitions in Main.
        // Set the _type 
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
