using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Pill : MonoBehaviour
{
    public BrainComponent brainComponent;
    public GameObject foodPrefab;
    public MeshRenderer meshRenderer;
    new public Rigidbody rigidbody;

    public Genes genes { get; private set; } = new Genes();

    public float health { get; private set; }
    public float energy { get; private set; }

    private bool isDead = false;

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

    private void OnDeath()
    {
        isDead = true;

        for (int i = 0; i < 5; i++)
        {
            GameObject food = PrefabUtility.InstantiatePrefab(foodPrefab) as GameObject;
            food.name = $"{gameObject.name} Food {i + 1}";
            food.transform.parent = FoodContainer.instance.transform;
            food.transform.position = transform.position + new Vector3(Utils.RandomRange(-0.01f, 0.01f), 0f, Utils.RandomRange(-0.01f, 0.01f));
        }

        Destroy(gameObject);
    }

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
        else if (energy > genes.maxEnergy / 2f)
        {
            health = Mathf.Min(health + 0.1f, genes.maxHealth);
        }
        energy = Mathf.Max(energy - (rigidbody.velocity.magnitude * genes.energyDrain / genes.speed), 0f);

        // check for death
        if (health < 0f)
        {
            OnDeath();
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

    private void OnFoodCollision(PillFood food)
    {
        if (energy + food.energy < genes.maxEnergy)
        {
            energy += food.energy;
            Destroy(food.gameObject);
            food.isDead = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent<PillFood>(out PillFood food))
        {
            OnFoodCollision(food);
        }
    }
}
