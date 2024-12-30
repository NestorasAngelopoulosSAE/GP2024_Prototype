/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Creates a chain of obstacles that propagate collisions so that the blue platfom doesn't clip through objects when approaching a wall.
/// </summary>
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Obstacle previous;
    public bool first;
    public Obstacle next;
    public Blue Platform;

    public Vector3 moveDirection;
    public float speed;

    Collider thisCollider;
    Collider playerCollider;

    private void Start()
    {
        thisCollider = GetComponent<Collider>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
    }

    public void PropagateBounce()
    {
        if (first) Platform.Bounce();
        else previous.PropagateBounce();
        Destroy(this);
    }

    public void PropagateUnlink()
    {
        if (next == null) Destroy(this);
        else next.PropagateUnlink();
        Destroy(this);
    }

    private void FixedUpdate()
    {
        // Break chain of obstacles if one is removed.
        if ((first &! Platform) || (!first && !previous) || (GetComponent<Red>() && GetComponent<Red>().isHeld) || GetComponent<Green>()) PropagateUnlink();

        // SweepTest will include the colliders of child objects even when they are triggers.
        // To only test for the platform collider, we create a new GameObject with the same collider, set it to trigger so that it doesn't interact with anything, and do the SweepTest from there.
        GameObject SweepTestObject = new GameObject("Sweep Test Object");
        SweepTestObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        Collider sweepCollider = Blue.CopyCollider(GetComponent<Collider>(), SweepTestObject);
        sweepCollider.isTrigger = true;
        Rigidbody sweepRigidBody = SweepTestObject.AddComponent<Rigidbody>();
        sweepRigidBody.isKinematic = true;
        // Copy its position to this object's position
        SweepTestObject.transform.position = transform.position;
        SweepTestObject.transform.rotation = transform.rotation;
        SweepTestObject.transform.localScale = transform.localScale;

        // Check for obstacle using the new rigidbody
        CheckForObstacle(sweepRigidBody);
        Destroy(SweepTestObject);
    }

    public void CheckForObstacle(Rigidbody rb)
    {
        // Known issue:
        // When the object is already colliding with something, SweepTest ignores that collision.
        // Therefore, if this obstacle has already been pushed against a wall, the SweepTest will fail.
        // This will cause the platform and any previous obstacles to clip through this obstacle.

        RaycastHit hit;
        // Check if object is to collide with something on the next frame.
        if (rb.SweepTest(moveDirection, out hit, speed * Time.deltaTime, QueryTriggerInteraction.Ignore))
        {            
            // When platform collides with something that is movable, add it to the chain.
            if (hit.collider.tag == "Colorable" && hit.collider.gameObject.GetComponent<Rigidbody>() & !hit.collider.gameObject.GetComponent<Green>())
            {
                if (hit.collider.gameObject.GetComponent<Obstacle>()) return; // Don't add another Obstacle script if it already has one so as not to lag the whole engine.
                Obstacle obstacle = hit.collider.gameObject.AddComponent<Obstacle>();
                obstacle.previous = this;
                obstacle.moveDirection = moveDirection;
                obstacle.speed = speed;
            }
            else if (hit.collider.tag != "Player") PropagateBounce(); // When it collides with something immovable, sent the platform back, while breaking the chain.
        }
        else if (GetComponent<Green>() || Blue.MovingIntongCollider(thisCollider, playerCollider, moveDirection)) PropagateBounce(); // If an object in the chain becomes green, or if you're about to hit the player, bounce off.
    }

    private void OnCollisionExit(Collision collision)
    {
        // Break chain of obstacles if one is removed.
        if (collision.collider.tag == "Colorable" || (first && collision.collider.gameObject == Platform.gameObject)) PropagateUnlink();
    }
}
