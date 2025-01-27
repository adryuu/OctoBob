using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //Instancia
    private static T _instance;

    //Propiedad de la instancia
    public static T Instance => _instance;

    //Lanza la instancia
    public void Awake()
    {
        _instance = (T)(object)this;
    }
}
