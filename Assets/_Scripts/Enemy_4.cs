using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Enemy_4 will start offscreen and then pick a random point on screen to 
///   move to. Once it has arrived, it will pick another random point and 
///   continue until the player has shot it down. 
/// </summary> 
public class Enemy_4 : Enemy
{

    private Vector3 p0, p1;        // The two points to interpolate 
    private float timeStart;     // Birth time for this Enemy_4 
    private float duration = 4;  // Duration of movement 

    void Start()
    {
        // There is already an initial position chosen by Main.SpawnEnemy() 
        //   so add it to points as the initial p0 & p1 
        p0 = p1 = pos;                                                            // a Enemy_4 interpolates from p0 to p1 (i.e., moves smoothly from p0 to p1). The Main.SpawnEnemy() script gives this instance a position just above the top of the screen, which is assigned here to both p0 and p1. InitMovement() is then called

        InitMovement();
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
}