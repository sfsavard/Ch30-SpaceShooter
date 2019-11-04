using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Part is another serializable data storage class just like WeaponDefinition 
/// </summary> 
[System.Serializable]
public class Part
{
    // These three fields need to be defined in the Inspector pane 
    public string name;         // The name of this part 
    public float health;       // The amount of health this part has 
    public string[] protectedBy;  // The other parts that protect this 

    // These two fields are set automatically in Start(). 
    // Caching like this makes it faster and easier to find these later 
    [HideInInspector]  // Makes field on the next line not appear in the Inspector 
    public GameObject go;           // The GameObject of this part 
    [HideInInspector]
    public Material mat;         // The Material to show damage 
}

/// <summary> 
/// Enemy_4 will start offscreen and then pick a random point on screen to 
///   move to. Once it has arrived, it will pick another random point and 
///   continue until the player has shot it down. 
/// </summary> 
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]                                    // a In the Inspector, all the public fields from Enemy are listed above those from Enemy_4. Adding the ": Enemy_4" to the end of the header here makes it more clear in the Inspector which script is tied to which field
    public Part[] parts;  // The array of ship Parts

    private Vector3 p0, p1;        // The two points to interpolate 
    private float timeStart;     // Birth time for this Enemy_4 
    private float duration = 4;  // Duration of movement 

    void Start()
    {
        // There is already an initial position chosen by Main.SpawnEnemy() 
        //   so add it to points as the initial p0 & p1 
        p0 = p1 = pos;                                                            // a Enemy_4 interpolates from p0 to p1 (i.e., moves smoothly from p0 to p1). The Main.SpawnEnemy() script gives this instance a position just above the top of the screen, which is assigned here to both p0 and p1. InitMovement() is then called

        InitMovement();

        // Cache GameObject & Material of each Part in parts 
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {                                                         // b InitMovement() first stores the current p1 location in p0 (because Enemy_4 should be at location p1 any time InitMovement() is called). Next, a new p1 location is chosen that uses information from the BoundsCheck component to guarantee it is on screen.
        p0 = p1;   // Set p0 to the old p1 
        // Assign a new on-screen location to p1 
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Reset the time 
        timeStart = Time.time;
    }

    public override void Move()
    {                                                // c This Move() method completely overrides the inherited Enemy.Move() method. It interpolates from p0 to p1 in duration seconds (4 seconds by default). The float u increases from 0 to 1 with time as this interpolation happens, and when u is >= 1, InitMovement() is called to set up a new interpolation
        // This completely overrides Enemy.Move() with a linear interpolation 
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);    // Apply Ease Out easing to u        // d This line applies easing to the u value, causing the ship to move in a non-linear fashion. With this "Ease Out" easing, the ship begins its movement quickly and then slows as it approaches p1
        pos = (1 - u) * p0 + u * p1;          // Simple linear interpolation       // e This line performs a simple linear interpolation from p0 to p1
    }

    // These two functions find a Part in parts based on name or GameObject 
    Part FindPart(string n)
    {                                                // a The FindPart() methods at // a and // b are overloads of each other, meaning that they are two methods with the same name but different parameters (one takes a string, and the other takes a GameObject). Based on what type of variable is passed in, the correct overload of the FindPart() function is executed. In either case, FindPart() searches through the parts array to find which part the string or GameObject is associated with
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go)
    {                                           // b A GameObject overload of FindPart(). Another overloaded function that you've used before is Random.range(), which has different behavior based on whether floats or ints are passed into it.
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    // These functions return true if the Part has been destroyed 
    bool Destroyed(GameObject go)
    {                                          // c Three overloads of the Destroyed() method that checks to see whether a certain part has been destroyed or still has health
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt)
    {
        if (prt == null)
        {  // If no real ph was passed in 
            return (true);   // Return true (meaning yes, it was destroyed) 
        }
        // Returns the result of the comparison: prt.health <= 0 
        // If prt.health is 0 or less, returns true (yes, it was destroyed) 
        return (prt.health <= 0);
    }

    // This changes the color of just one Part to red instead of the whole ship. 
    void ShowLocalizedDamage(Material m)
    {                                   // d ShowLocalizedDamage() is a more specialized version of the inherited Enemy.ShowDamage() method. This only turns one part red, not the whole ship
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    // This will override the OnCollisionEnter that is part of Enemy.cs. 
    void OnCollisionEnter(Collision coll)
    {                                // e This OnCollisionEnter() method completely overrides the inherited Enemy.OnCollisionEnter() method. Because of the way that MonoBehaviour declares common Unity functions like OnCollisionEnter(), the override keyword is not necessary
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it. 
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                // Hurt this Enemy 
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // f This line finds the GameObject that was hit. The Collision coll includes a field contacts[], which is an array of ContactPoints. Because there was a collision, you're guaranteed that at least one ContactPoint (i.e., contacts[0]) exists, and each ContactPoint has a field named thisCollider, which is the collider for the part of the Enemy_4 that was hit
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                { // If prtHit wasn't found…         // g If the prtHit you searched for wasn't found (and therefore prtHit == null), then it's usually because—very rarely—thisCollider on contacts[0] will refer to the ProjectileHero that hit the ship instead of the ship part that was hit. In that case, just look at contacts[0].otherCollider instead
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // Check whether this part is still protected 
                if (prtHit.protectedBy != null)
                {                           // h If this part is still protected by another part that has not yet been destroyed, apply damage to the protecting part instead
                    foreach (string s in prtHit.protectedBy)
                    {
                        // If one of the protecting parts hasn't been destroyed... 
                        if (!Destroyed(s))
                        {
                            // ...then don't damage this part yet 
                            Destroy(other);  // Destroy the ProjectileHero 
                            return;          // return before damaging Enemy_4 
                        }
                    }
                }
                // It's not protected, so make it take damage 
                // Get the damage amount from the Projectile.type and Main.W_DEFS $$$$$$$$$$$$$$$$ 
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // Show damage on the part 
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {                                    // i If a single part's health reaches 0, then set it to inactive, which makes it disappear and stop colliding with things
                    // Instead of destroying this enemy, disable the damaged part 
                    prtHit.go.SetActive(false);
                }
                // Check to see if the whole ship is destroyed 
                bool allDestroyed = true; // Assume it is destroyed 
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {  // If a part still exists... 
                        allDestroyed = false;  // ...change allDestroyed to false
                        break;                 // & break out of the foreach loop 
                    }
                }
                if (allDestroyed)
                { // If it IS completely destroyed...  // j If the whole ship has been destroyed, notify Main.S.ShipDestroyed() just like the Enemy script would have (if you hadn't overridden OnCollisionEnter())
                    // ...tell the Main singleton that this ship was destroyed 
                    Main.S.shipDestroyed(this);
                    // Destroy this Enemy 
                    Destroy(this.gameObject);
                }
                Destroy(other);  // Destroy the ProjectileHero 
                break;
        }
    }
}