using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public Texture2D thumbnail;
    public string displayName;
    public string description;
    public int level;

    private Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    
    void Start()
    {
        EconomyManager.Instance.Register(this);
    }

    public void Select()
    {
        material.SetFloat("_Selected", 1);
    }

    public void Deselect()
    {
        material.SetFloat("_Selected", 0);
    }

    public void Upgrade()
    {
        var color = material.GetColor("_Color");
        color.r += 0.35f;
        color.r %= 1.0f;
        color.g += 0.15f;
        color.g %= 1.0f;
        color.b += 0.25f;
        color.b %= 1.0f;
        material.SetColor("_Color", color);
        level += 1;
        UIDocManager.Instance.SetEntityHUD(this);
    }

    public void DestroySelf()
    {
        EconomyManager.Instance.Unregister(this);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        EconomyManager.Instance.Unregister(this);
    }
}
