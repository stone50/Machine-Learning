using System;
using UnityEngine;

[Serializable]
public class Genes
{
    #region mins and maxs
    public static readonly float eyeSightMin = 0.5f;
    public static readonly float eyeSightMax = 5f;
    public static readonly float speedMin = 0f;
    public static readonly float speedMax = 5f;
    public static readonly float maxHealthMin = 0f;
    public static readonly float maxHealthMax = 100f;
    public static readonly float damageMin = 0f;
    public static readonly float damageMax = 5f;
    public static readonly float damageResistMin = 0f;
    public static readonly float damageResistMax = 5f;
    public static readonly float maxEnergyMin = 0f;
    public static readonly float maxEnergyMax = 100f;
    public static readonly float energyDrainMin = 0.01f;
    public static readonly float energyDrainMax = 0.1f;
    #endregion mins and maxs

    #region stats
    public Color color;
    public float eyeSight;
    public float speed;
    public float maxHealth;
    public float damage;
    public float damageResist;
    public float maxEnergy;
    public float energyDrain;
    #endregion stats

    public void Randomize()
    {
        color = new Color(
            Utils.RandomRange(0f, 1f),
            Utils.RandomRange(0f, 1f),
            Utils.RandomRange(0f, 1f)
        );

        eyeSight = Utils.RandomRange(eyeSightMin, eyeSightMax);
        speed = Utils.RandomRange(speedMin, speedMax);
        maxHealth = Utils.RandomRange(maxHealthMin, maxHealthMax);
        damage = Utils.RandomRange(damageMin, damageMax);
        damageResist = Utils.RandomRange(damageResistMin, damageResistMax);
        maxEnergy = Utils.RandomRange(maxEnergyMin, maxEnergyMax);
        energyDrain = Utils.RandomRange(energyDrainMin, energyDrainMax);
    }
}