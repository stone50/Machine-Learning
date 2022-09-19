using UnityEngine;

public class FoodContainer : MonoBehaviour
{
    public static GameObject instance;

    void Start()
    {
        instance = gameObject;
    }
}