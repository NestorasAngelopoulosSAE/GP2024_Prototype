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

    GameObject player;

    private void Start()
    {
        thisCollider = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider>();
    }

    // Send a collision signal back to the platform and disassemble the chain moving from the end to start.
    public void PropagateBounce()
    {
        if (first) Platform.Bounce();
        else previous.PropagateBounce();
        Destroy(this);
    }

    // Disassemble the chain, moving from from start to end.
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
        // Sync all moveDirections with the Platform.
        if (first && Platform.moveDirection != moveDirection) moveDirection = Platform.moveDirection;
        else if (!first && previous.moveDirection != moveDirection) moveDirection = previous.moveDirection;

        // SweepTest will include the colliders of child objects even when they are triggers.
        // To only test for the platform collider and not any child collider, we create a new GameObject with the same collider, set it to trigger so that it doesn't interact with anything, and do the SweepTest from there.
        Rigidbody sweepRigidBody = Blue.SweepTestRigidBody(thisCollider, moveDirection, speed);

        // Check for obstacle using the new rigidbody
        CheckForObstacle(sweepRigidBody);
        Destroy(sweepRigidBody.gameObject);
    }

    void CheckForObstacle(Rigidbody rigidbody)
    {
        // Check if object is to collide with something on the next frame. (*2 because we moved the collider back a frame)
        if (rigidbody.SweepTest(moveDirection, out RaycastHit hit, speed * Time.deltaTime * 2, QueryTriggerInteraction.Ignore))
        {            
            // When platform collides with something that is movable, add it to the chain.
            if (hit.collider.tag == "Colorable" &! hit.collider.gameObject.GetComponent<Green>())
            {
                // Add an Obstacle script to the movable obstacle and let it create a chain of objects that are being pushed.
                // Once one of them hits something, a bounce command will propagate back through the chain, unlinking all obstacles in the process.

                if (hit.collider.gameObject.GetComponent<Obstacle>()) return; // Don't add another Obstacle script if it already has one.
                Obstacle obstacle = hit.collider.gameObject.AddComponent<Obstacle>();
                obstacle.previous = this;
                obstacle.moveDirection = moveDirection;
                obstacle.speed = speed;
            }
            else if (hit.collider.tag != "Player") PropagateBounce(); // When it collides with something immovable, sent the platform back, while breaking the chain.
        }
        
        if (Blue.MovingIntongCollider(thisCollider, playerCollider, moveDirection) && player.transform.root != transform.root) PropagateBounce(); // If you're about to hit the player, bounce off. (don't, if you're moving towards )
    }

    private void OnCollisionExit(Collision collision)
    {
        // Break chain of obstacles if one is removed.
        if (collision.collider.tag == "Colorable" || (first && collision.collider.gameObject == Platform.gameObject)) PropagateUnlink();
    }
}
