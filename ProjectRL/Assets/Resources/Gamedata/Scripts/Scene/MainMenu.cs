using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLClient;
[RequireComponent(typeof(Decryptor))]
public class MainMenu : MonoBehaviour, IStrLoader
{
    private Decryptor _decryptor;
    void Start()
    {
        _decryptor = GetComponent<Decryptor>();
    }

   
    public void LoadStoryline( string fileName)
    {
        Debug.Log("loaded: " + fileName);
    }

    void Update()
    {
        
    }
}
