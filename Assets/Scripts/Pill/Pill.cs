using System;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    public BrainComponent brainComponent;
    public MeshRenderer meshRenderer;
    new public Rigidbody rigidbody;

    public Genes genes { get; private set; } = new Genes();

    public float health { get; private set; }
    public float energy { get; private set; }

    void Start()
    {
        genes.Randomize();

        meshRenderer.material.color = genes.color;
        health = genes.maxHealth;
        energy = genes.maxEnergy;
    }

    #region angles
    private static readonly Vector3 angle1 = new Vector3(0f, -72f, 0f);
    private static readonly Vector3 angle2 = new Vector3(0f, -36f, 0f);
    private static readonly Vector3 angle3 = Vector3.forward;
    private static readonly Vector3 angle4 = new Vector3(0f, 36f, 0f);
    private static readonly Vector3 angle5 = new Vector3(0f, 72f, 0f);
    #endregion angles

    private RaycastHit[] CastSights()
    {
        RaycastHit[] hits = new RaycastHit[5];

        Physics.Raycast(
            transform.position,
            transform.rotation * angle1,
            out hits[0],
            genes.eyeSight
        );
        Physics.Raycast(
            transform.position,
            transform.rotation * angle2,
            out hits[1],
            genes.eyeSight
        );
        Physics.Raycast(
            transform.position,
            transform.rotation * angle3,
            out hits[2],
            genes.eyeSight
        );
        Physics.Raycast(
            transform.position,
            transform.rotation * angle4,
            out hits[3],
            genes.eyeSight
        );
        Physics.Raycast(
            transform.position,
            transform.rotation * angle5,
            out hits[4],
            genes.eyeSight
        );

        return hits;
    }

    private int ColorToInt(Color color)
    {
        Color32 byteColor = color;
        return BitConverter.ToInt32(new Byte[] { 0x00, byteColor[0], byteColor[1], byteColor[2] }, 0);
    }

    void FixedUpdate()
    {
        // gather input info
        RaycastHit[] sightHits = CastSights();

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
            sightHits[0].transform ? sightHits[0].distance : 0f,
            sightHits[0].transform && sightHits[0].transform.TryGetComponent<Renderer>(out Renderer renderer1) ? ColorToInt(renderer1.material.color) : 0f,
            sightHits[1].transform ? sightHits[1].distance : 0f,
            sightHits[1].transform && sightHits[1].transform.TryGetComponent<Renderer>(out Renderer renderer2) ? ColorToInt(renderer2.material.color) : 0f,
            sightHits[2].transform ? sightHits[2].distance : 0f,
            sightHits[2].transform && sightHits[2].transform.TryGetComponent<Renderer>(out Renderer renderer3) ? ColorToInt(renderer3.material.color) : 0f,
            sightHits[3].transform ? sightHits[3].distance : 0f,
            sightHits[3].transform && sightHits[3].transform.TryGetComponent<Renderer>(out Renderer renderer4) ? ColorToInt(renderer4.material.color) : 0f,
            sightHits[4].transform ? sightHits[4].distance : 0f,
            sightHits[4].transform && sightHits[4].transform.TryGetComponent<Renderer>(out Renderer renderer5) ? ColorToInt(renderer5.material.color) : 0f,
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
