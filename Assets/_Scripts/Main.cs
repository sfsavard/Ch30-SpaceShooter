using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // For loading & reloading of scenes

public class Main : MonoBehaviour
{
    static public Main S;                                // A singleton for Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;               // a Dictionaries are declared and defined with both a key type and value type
    //Making WEAP_DICT static but protected means that any instance of Main can access it and any static method of Main can access it

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;              // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # Enemies/second
    public float enemyDefaultPadding = 1.5f; // Padding for position
    public WeaponDefinition[] weaponDefinitions;

    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);                      // a

        // A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();         // a 
        foreach (WeaponDefinition def in weaponDefinitions)
        {              // b This loop iterates through each element of the weaponDefinitions array and creates an entry in the WEAP_DICT dictionary that matches it.
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);                     // b
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);     // c

        // Position the Enemy above the screen with a random x position
        float enemyPadding = enemyDefaultPadding;                            // d
        if (go.GetComponent<BoundsCheck>() != null)
        {                        // e
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // Set the initial position for the spawned Enemy                    // f
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);                      // g
    }

    public void DelayedRestart(float delay)
    {
        // Invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // Reload _Scene_0 to restart the game
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary> 
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static
    /// protected field of the Main class. 
    /// </summary> 
    /// <returns>The WeaponDefinition or, if there is no WeaponDefinition with 
    /// the WeaponType passed in, returns a new WeaponDefinition with a 
    /// WeaponType of none..</returns> 
    /// <param name="wt">The WeaponType of the desired WeaponDefinition</param> 
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {          // a 
               // Check to make sure that the key exists in the Dictionary 
               // Attempting to retrieve a key that didn't exist, would throw an error, 
               // so the following if statement is important. 
        if (WEAP_DICT.ContainsKey(wt))
        {                                           // b This if statement checks to make sure that WEAP_DICT has an entry with the key that was passed in as wt.
            return (WEAP_DICT[wt]);
        }
        // This returns a new WeaponDefinition with a type of WeaponType.none, 
        //   which means it has failed to find the right WeaponDefinition 
        return (new WeaponDefinition());                                          // c If there is no entry in WEAP_DICT with the proper WeaponType key, a new WeaponDefinition with a type of WeaponType.none is returned.
    }
                                                                                  // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
