using UnityEngine;

public class Pill : MonoBehaviour
{
    public BrainComponent brainComponent;
    new public Rigidbody rigidbody;

    public Genes genes { get; private set; } = new Genes();

    public LineRenderer[] sights;

    void Start()
    {
        genes.speed = 2f;
    }

    void FixedUpdate()
    {
        brainComponent.brain.Think();
        rigidbody.velocity = transform.rotation * Vector3.forward * genes.speed * brainComponent.brain.outputValues[0];
        rigidbody.velocity += transform.rotation * Vector3.right * genes.speed * brainComponent.brain.outputValues[1];
        rigidbody.angularVelocity = new Vector3(0f, brainComponent.brain.outputValues[2], 0f);
    }
}
