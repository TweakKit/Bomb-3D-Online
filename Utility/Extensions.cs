using System.Collections.Generic;
using UnityEngine;
using ZB.Gameplay;

public static class Extensions
{
    #region Members

    public static readonly System.Random systemRandom = new System.Random();

    #endregion Members

    #region UI

    public static void SetActive(this CanvasGroup canvas, bool isActive)
    {
        canvas.alpha = isActive ? 1 : 0;
        canvas.interactable = isActive;
        canvas.blocksRaycasts = isActive;
    }

    #endregion UI

    #region Transform

    public static GameObject GetChild(this GameObject gameObject, string childName)
    {
        Transform childTransform = gameObject.transform.FindChildByName(childName);
        return childTransform.gameObject;
    }

    public static Transform FindChildByName(this Transform transform, string childName)
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == childName)
                return child;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i).FindChildByName(childName);
            if (child != null)
                return child;
        }

        return null;
    }

    #endregion Transform

    #region Game Object

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
            return gameObject.AddComponent<T>();
        return component;
    }

    #endregion Game Object

    #region List/Array

    public static void Shuffle<T>(this T[] array)
    {
        System.Random prng = new System.Random(systemRandom.Next(-10000, 10000));

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
    }

    public static T[] ShuffleAndReturn<T>(this T[] array)
    {
        array.Shuffle();
        return array;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random prng = new System.Random(systemRandom.Next(-10000, 10000));

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, list.Count);
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }
    }

    public static List<T> ShuffleAndReturn<T>(this List<T> list)
    {
        list.Shuffle();
        return list;
    }

    public static T GetRandomFromArray<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            return default(T);

        var length = array.Length;
        return array[systemRandom.Next(0, length)];
    }

    public static T GetRandomFromList<T>(this List<T> list)
    {
        return list[systemRandom.Next(0, list.Count)];
    }

    #endregion List/Array

    #region Game Helper

    public static bool HasPath(this List<Vector2> pathPositions)
    {
        return pathPositions.Count > 1;
    }

    public static Vector3 ChangeDirection(this Vector3 offset, Vector3 direction)
    {
        float rotationInRadians = Vector3.Angle(direction, Vector3.forward) * Mathf.Deg2Rad;

        if (Vector3.Dot(Vector3.right, direction) >= 0)
        {
            float offsetX = offset.z * Mathf.Sin(rotationInRadians) + offset.x * Mathf.Cos((rotationInRadians));
            float offsetZ = offset.z * Mathf.Cos(rotationInRadians) - offset.x * Mathf.Sin((rotationInRadians));
            float offsetY = offset.y;
            return new Vector3(offsetX, offsetY, offsetZ);
        }
        else
        {
            float offsetX = -offset.z * Mathf.Sin(rotationInRadians) + offset.x * Mathf.Cos((rotationInRadians));
            float offsetZ = offset.z * Mathf.Cos(rotationInRadians) + offset.x * Mathf.Sin((rotationInRadians));
            float offsetY = offset.y;
            return new Vector3(offsetX, offsetY, offsetZ);
        }
    }

    public static string GetElement(this CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Aries:
            case CharacterType.Leo:
            case CharacterType.Sagittarius:
                return "FIRE";

            case CharacterType.Taurus:
            case CharacterType.Virgo:
            case CharacterType.Capricorn:
                return "EARTH";

            case CharacterType.Cancer:
            case CharacterType.Scorpio:
            case CharacterType.Pisces:
                return "WATER";

            case CharacterType.Gemini:
            case CharacterType.Libra:
            case CharacterType.Aquarius:
                return "AIR";

            default:
                return "NONE";
        }
    }

    public static BombExplosionEffectType GetBombExplosionEffectType(this BombType bombType)
    {
        switch (bombType)
        {
            case BombType.BombAries:
            case BombType.BombLeo:
            case BombType.BombSagittarius:
                return BombExplosionEffectType.BombExplosionEffectFire;

            case BombType.BombTaurus:
            case BombType.BombVirgo:
            case BombType.BombCapricorn:
                return BombExplosionEffectType.BombExplosionEffectEarth;

            case BombType.BombCancer:
            case BombType.BombScorpio:
            case BombType.BombPisces:
                return BombExplosionEffectType.BombExplosionEffectWater;

            case BombType.BombGemini:
            case BombType.BombLibra:
            case BombType.BombAquarius:
                return BombExplosionEffectType.BombExplosionEffectAir;

            case BombType.BombDefault:
            case BombType.BombBigSecret:
            default:
                return BombExplosionEffectType.BombExplosionEffectDefault;
        }
    }

    #endregion Game Helper
}