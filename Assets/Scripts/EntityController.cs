using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public Texture2D thumbnail;
    public string displayName;
    public string description;

    private Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        material.SetFloat("_Selected", 1);
    }

    public void Deselect()
    {
        material.SetFloat("_Selected", 0);
    }
}
