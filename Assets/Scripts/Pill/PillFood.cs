using UnityEngine;

public class PillFood : MonoBehaviour
{
    public static readonly Color defaultColor = new Color(0.4f, 0.2f, 0.1f);

    public MeshRenderer meshRenderer;

    public static readonly float energyMin = 1f;
    public static readonly float energyMax = 50f;
    public static readonly float energyDecayMin = 0.1f;
    public static readonly float energyDecayMax = 0.5f;

    public float energy { get; private set; }
    public float energyDecay { get; private set; }

    public bool isDead = false;

    void Start()
    {
        meshRenderer.material.color = defaultColor;

        energy = Utils.RandomRange(energyMin, energyMax);
        energyDecay = Utils.RandomRange(energyDecayMin, energyDecayMax);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        energy -= energyDecay;

        if (energy < 0f)
        {
            Destroy(gameObject);
            isDead = true;
            return;
        }

        float energyRatio = energy / energyMax;
        meshRenderer.material.color = new Color(
            defaultColor.r * energyRatio,
            defaultColor.g * energyRatio,
            defaultColor.b * energyRatio
        );
    }
}