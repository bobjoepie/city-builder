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

    //public static bool HasTag(this BG_EntityTags ownerTag, BG_EntityTags tagToCheck)
    //{
    //    var comboTag = ownerTag & tagToCheck;
    //    var result = ownerTag != BG_EntityTags.None && ownerTag.HasFlag(comboTag);
    //    return comboTag != BG_EntityTags.None && result;
    //}

    //public static bool HasTag(this BG_EntityController owner, BG_EntityTags tagToCheck)
    //{
    //    return HasTag(owner.attributes.tags, tagToCheck);
    //}

    //public static bool HasTag(this BG_EntityTags ownerTag, BG_EntityController entity)
    //{
    //    return HasTag(ownerTag, entity.attributes.tags);
    //}

    //public static bool MatchParentTag(this BG_EntityTags parentTag, BG_EntityTags entity1, BG_EntityTags entity2)
    //{
    //    return parentTag.HasTag(entity1) && parentTag.HasTag(entity2);
    //}

    //public static BG_EntityTags GetTagUnderParent(this BG_EntityTags child, BG_EntityTags parent)
    //{
    //    var comboTag = child & parent;
    //    return comboTag;
    //}

    //public static bool IsOpposingTag(this BG_EntityTags parentTag, BG_EntityTags tag1, BG_EntityTags tag2)
    //{
    //    var matchParents = MatchParentTag(parentTag, tag1, tag2);
    //    var childTag1 = tag1.GetTagUnderParent(parentTag);
    //    var childTag2 = tag2.GetTagUnderParent(parentTag);
    //    var isSameTag = childTag1.HasTag(childTag2);
    //    return matchParents && !isSameTag;
    //}

    //public static bool IsOpposingTag(this BG_EntityTags parentTag, BG_EntityController entity1, BG_EntityController entity2)
    //{
    //    return IsOpposingTag(parentTag, entity1.attributes.tags, entity2.attributes.tags);
    //}

    //public static bool IsSameTag(this BG_EntityTags parentTag, BG_EntityTags tag1, BG_EntityTags tag2)
    //{
    //    var matchParents = MatchParentTag(parentTag, tag1, tag2);
    //    var childTag1 = tag1.GetTagUnderParent(parentTag);
    //    var childTag2 = tag2.GetTagUnderParent(parentTag);
    //    var isSameTag = childTag1.HasTag(childTag2);
    //    return matchParents && isSameTag;
    //}

    //public static bool IsSameTag(this BG_EntityTags parentTag, BG_EntityController entity1, BG_EntityController entity2)
    //{
    //    return IsSameTag(parentTag, entity1.attributes.tags, entity2.attributes.tags);
    //}
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