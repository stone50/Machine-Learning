using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    public BrainComponent brainComponent;
    public MeshRenderer meshRenderer;
    new public Rigidbody rigidbody;

    public Genes genes { get; private set; } = new Genes();

    //public LineRenderer[] sights;

    public float health { get; private set; }
    public float energy { get; private set; }

    void Start()
    {
        genes.Randomize();

        meshRenderer.material.color = genes.color;
        health = genes.maxHealth;
        energy = genes.maxEnergy;
    }

    void FixedUpdate()
    {
        // gather input info


        if (Mathf.Approximately(energy, 0f))
        {
            health -= genes.energyDrain;
        }
        energy = Mathf.Max(energy - (rigidbody.velocity.magnitude * genes.energyDrain / genes.speed), 0f);

        // check for death
        if (health < 0f)
        {
            Destroy(gameObject);
        }

        // brain think
        brainComponent.brain.SetInputs(new List<float>()
        {
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            health,
            energy
        });
        brainComponent.brain.Think();

        // take action based on outputs
        rigidbody.velocity = transform.rotation * Vector3.forward * genes.speed * brainComponent.brain.outputValues[0];
        rigidbody.velocity += transform.rotation * Vector3.right * genes.speed * brainComponent.brain.outputValues[1];
        rigidbody.angularVelocity = new Vector3(0f, brainComponent.brain.outputValues[2], 0f);
    }
}
