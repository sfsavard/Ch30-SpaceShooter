using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //============================ Materials Functions ===========================\\

    // Returns a list of all Materials on this GameObject and its children
    static public Material[] GetAllMaterials(GameObject go )
    {               // a As a static public method, GetAllMaterials() can be called anywhere in this project via Utils.GetAllMaterials().
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();           // b GetComponentsInChildren<>() is a GameObject method that iterates over the GameObject itself and all of its children, and returns an array of whatever component type is passed into the generic <> parameter of the method

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)
        {                                   // c This foreach loop iterates over the Renderer components in the rends array and extracts the material field from each. This Material is then added to the mats List
            mats.Add(rend.material);
        }

        return (mats.ToArray());                                            // d Finally, the mats List is converted into an array and returned
    }
}
