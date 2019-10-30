using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shields : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float rotationsPerSecond = 0.1f;

    [Header("Set Dynamically")]
    public int levelShown = 0;

    // This non-public variable will not appear in the Inspector
    Material mat;                                                   // a

    void Start()
    {
        mat = GetComponent<Renderer>().material;                           // b gets the material and sets it to a variable so you can manipulate it
    }

    void Update()
    {
        // Read the current shield level from the Hero Singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);            // c get the current shield level and drop it to the lowest integer
        // If this is different from levelShown...
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            // Adjust the texture offset to show different shield level
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);      // d every 20% is the pixel of our material we want, so you take current shield level and times it by 20%
        }
        // Rotate the shield a bit every frame in a time-based way
        float rZ = -(rotationsPerSecond * Time.time * 3600) % 360f;            // e
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
