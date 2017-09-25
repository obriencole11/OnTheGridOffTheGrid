using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("General")]
    public int turnCount = 1;

    [Header("Animation")]
    public float turnDuration;
    public AnimationCurve basicMovement;
    public AnimationCurve collision;

    [Header("Movement")]
    public float offGridSpeed = 50.0f;

    [Header("Character")]
    public Material blackMaterial;
    public Material whiteMaterial;
    public GameObject rookPrefab;

    [Header("Levels")]
    public List<GameObject> levels;
}
