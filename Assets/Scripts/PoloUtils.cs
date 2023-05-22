using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class PoloUtils
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return go.GetComponent<T>() ? go.GetComponent<T>() : go.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour go) where T : Component
    {
        return go.GetComponent<T>() ? go.GetComponent<T>() : go.gameObject.AddComponent<T>();
    }

    public static Quaternion AngleTowards2D(this Vector2 position, Vector2 target)
    {
        var dir = position - target;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rotToTarget = Quaternion.Euler(0f, 0f, angle);
        return rotToTarget;
    }

    public static Quaternion AngleTowards2D(this Vector3 position, Vector3 target)
    {
        var dir = (Vector2)position - (Vector2)target;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rotToTarget = Quaternion.Euler(0f, 0f, angle);
        return rotToTarget;
    }

    public static Quaternion AngleTowards2D(this Vector2 position, Vector3 target)
    {
        var dir = (Vector2)position - (Vector2)target;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rotToTarget = Quaternion.Euler(0f, 0f, angle);
        return rotToTarget;
    }

    public static Quaternion AngleTowards2D(this Vector3 position, Vector2 target)
    {
        var dir = (Vector2)position - (Vector2)target;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rotToTarget = Quaternion.Euler(0f, 0f, angle);
        return rotToTarget;
    }

    public static int ToLayer(this LayerMask mask)
    {
        var layer = (int)Mathf.Log(mask.value, 2);
        return layer;
    }

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static void AllowUIButtonKeyModifiers(this Button button)
    {
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Shift });
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Alt });
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Control });
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Shift | EventModifiers.Alt });
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Shift | EventModifiers.Control });
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Alt | EventModifiers.Control });
        button.clickable.activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse, modifiers = EventModifiers.Shift | EventModifiers.Alt | EventModifiers.Control });
    }
}

public static class LayerUtility
{
    public static LayerMask Only(params string[] ids)
    {
        LayerMask mask = new LayerMask();

        for (int i = 0; i < ids.Length; i++)
        {
            mask = Add(mask, Shift(Get(ids[i])));
        }

        return mask;
    }

    public static LayerMask AllBut(params string[] ids)
    {
        return Invert(Only(ids));
    }

    public static LayerMask Invert(LayerMask mask)
    {
        return ~mask;
    }

    public static LayerMask Add(LayerMask mask1, LayerMask mask2)
    {
        return mask1 | mask2;
    }

    public static LayerMask Shift(LayerMask mask)
    {
        return 1 << mask;
    }

    public static LayerMask Get(string id)
    {
        return LayerMask.NameToLayer(id);
    }
}