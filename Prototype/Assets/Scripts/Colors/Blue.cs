/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Moves the object in the direction specified by the player.
/// Does muutiple checks to determine when to flip the direction of movement.
/// Contains helper functions used by PlatformRideTrigger and Obstacle.
/// 
/// When it hits a wall (or a series of obstacles that lead to a wall), it sends the object back.
/// 
/// Unfortunately, no single test covers all cases, so a combination of rigidbody sweep tests, 
/// raycasts from each vertex of each object riding the platfomr, and a and closest point distance check is performed each FixedUpdate.
/// 
/// The Obstacle script aids the functionality of the platform.
/// It is used to form a chain of objects that the platform is pushing in front of it.
/// They each perform a sweep test to find immovable level geometry, and a closest point distance check to find the player.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class Blue : MonoBehaviour
{
    UIManager uiManager;
    ColorManager colorManager;
    GameObject gameplayManager;

    GameObject player;
    Collider playerCollider;

    Rigidbody rigidBody;
    Collider thisCollider;

    public Vector3 moveDirection;
    bool bouncedThisFrame;
    
    float speed = 2f;

    void Start()
    {
        //Debug.Log($"{gameObject.name} is Blue!");

        gameplayManager = GameObject.FindGameObjectWithTag("Gameplay Manager");
        colorManager = gameplayManager.GetComponent<ColorManager>();
        uiManager = gameplayManager.gameObject.GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        thisCollider = GetComponent<Collider>();
        playerCollider = player.GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();

        // Lift object so it doesn't overlap with the floor. (Helps with sweep test on top of weird geometries)
        transform.position += Vector3.up * 0.02f;

        // If a green object was on top of the platform when it became blue,
        Rigidbody sweepRigidBody = SweepTestRigidBody(thisCollider, Vector3.up, speed);
        RaycastHit[] hits = sweepRigidBody.SweepTestAll(Vector3.up, speed * Time.deltaTime * 2, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Colorable" && hit.collider.gameObject.GetComponent<Green>())
            {
                // Lift it slightly higher than the platform, so that when the platform returns, it can pass under the green object.
                hit.transform.position += Vector3.up * 0.04f;
            }
        }
        Destroy(sweepRigidBody.gameObject);

        // Get direction of movement (a unit vector in local or world space, based on the selected option)
        GetMoveDirection(); 

        // Stop the physics engine from applying forces to the object.
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
    }

    private void OnDisable()
    {
        //Debug.Log($"{gameObject.name} is no longer Green!");

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }

    void GetMoveDirection()
    {
        if (colorManager.allowBlueVertical) // Local-space-aligned axess based on the position of the camera.
        {
            moveDirection = transform.TransformDirection(NearestAxis(transform.InverseTransformDirection(transform.position - Camera.main.transform.position), true)).normalized;
        }
        else // World-space-aligned axes based on the rotation of the camera.
        {
            moveDirection = NearestAxis(Camera.main.transform.TransformDirection(Vector3.forward), false).normalized;
        }
    }

    void FixedUpdate()
    {
        // SweepTest will include the colliders of child objects even when they are triggers.
        // To only test for the platform collider and not any child collider, we create a new GameObject with the same collider, set it to trigger so that it doesn't interact with anything, and do the SweepTest from there.

        // Create the test object and place it one step before the actual position of the platform, to ensure that the sweep test will recognize the collision.
        Rigidbody sweepRigidBody = SweepTestRigidBody(thisCollider, moveDirection, speed);
        CheckForObstacle(sweepRigidBody); // Check for obstacle using the new rigidbody for the sweep test. If any of the tests passes, flip moveDirection.
        Destroy(sweepRigidBody.gameObject); // Destroy the test object.

        transform.position += moveDirection * Time.deltaTime * speed; // Move the platform.
    }

    private void Update()
    {
        if (uiManager.isPaused) return; // Don't take input if the game is paused.

        // Re-calculate the moveDirection if the player clicks the platform.
        if (Input.GetMouseButtonDown(0))
        {
            LayerMask raycastLayers = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Ignore Raycast")) & ~(1 << LayerMask.NameToLayer("Splat"));
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, raycastLayers))
            {
                if (hit.transform == transform) GetMoveDirection();
            }
        }
    }

    void CheckForObstacle(Rigidbody rigidbody)
    {
        // |Sweep Test| Check if object is to collide with something on the next frame. (*2 because we moved the collider back a frame)
        RaycastHit[] hits = rigidbody.SweepTestAll(moveDirection, speed * Time.deltaTime * 2, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            // When platform collides with something that is movable, add it to the chain.
            if (hit.collider.tag == "Colorable" &! hit.collider.gameObject.GetComponent<Green>())
            {
                // Add an Obstacle script to the object and let it create a chain of objects that are being pushed.
                // Once one of them hits something immovable, a bounce command will propagate back through the chain, unlinking all obstacles in the process.

                if (!hit.collider.gameObject.GetComponent<Obstacle>()) // Don't add another Obstacle script if it already has one.
                {
                    Obstacle obstacle = hit.collider.gameObject.AddComponent<Obstacle>();
                    obstacle.first = true;
                    obstacle.Platform = this;
                    obstacle.moveDirection = moveDirection;
                    obstacle.speed = speed;
                }
            }
            else if (hit.collider.tag != "Player") Bounce(); // When it collides with something immovable, send the platform back.
        }

        // |Raycast Test| If we're going upwards, Raycast from all the vertices of each child, and if you hit something immovable, bounce.
        if (transform.childCount > 1 && Vector3.Dot(moveDirection, Vector3.up) >= 0.01f)
        {
            foreach (Collider childCollider in transform.GetComponentsInChildren<Collider>())
            {
                Transform child = childCollider.transform;
                if (child.GetComponent<RideTrigger>()) continue; // For each child that isn't a trigger object,

                Mesh mesh;
                Vector3 scalingFactor = Vector3.one;
                Vector3 meshOffset = Vector3.zero;

                // Get mesh from child's mesh filter.
                if (child.GetComponent<MeshFilter>() && child.GetComponent<MeshFilter>().mesh.isReadable)
                {
                    mesh = child.GetComponent<MeshFilter>().mesh;
                }
                else // If object doesn't have a mesh filter, assume a bounding box and pray that its collider doesn't extend out of it.
                {
                    mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                    scalingFactor = child.localScale;
                }

                if (child.gameObject == player) // The player's collider extends out of the assumed bounding box. A specialized approach is needed. :(
                {
                    CharacterController controller = player.GetComponent<CharacterController>();
                    scalingFactor = new Vector3(controller.radius, controller.height, controller.radius);
                    meshOffset = controller.center;
                }

                foreach (Vector3 vertex in mesh.vertices)
                {
                    Vector3 testpoint = vertex;

                    // Apply necessary transformations to each vertex to match scale and offset of the object.
                    testpoint.x *= scalingFactor.x; testpoint.y *= scalingFactor.y; testpoint.z *= scalingFactor.z;
                    testpoint = child.TransformPoint(testpoint + meshOffset);

                    if (Physics.Raycast(testpoint, moveDirection, out RaycastHit hit, speed * Time.deltaTime))
                    {
                        // If what you hit isn't an object already riding the platform or an object connected to the player,
                        if (hit.collider.transform.root != transform && hit.collider.tag != "Player" && hit.collider.gameObject.layer != LayerMask.NameToLayer("Held Object"))
                        {
                            Bounce();
                        }
                    }
                }
            }
        }

        // |Closest Point Distance Test| If the player is in front of the object, bounce. (handling this in the sweep test gave inconsistent results)
        if (player.transform.parent != transform && MovingIntongCollider(thisCollider, playerCollider, moveDirection)) Bounce();
    }

    public void Bounce()
    {
        if (bouncedThisFrame) return; // If multiple checks pass, don't toggle multiple times.

        bouncedThisFrame = true;
        moveDirection *= -1f;
    }

    private void LateUpdate()
    {
        bouncedThisFrame = false;
    }
    
    /// <summary>
    /// Snaps Vector3 to its nearest axis.
    /// </summary>
    /// <param name="input">The Vector3 you want to convert.</param>
    /// <param name="allowY">Whether or not to consider the Y axis.</param>
    /// <returns>The unit Vector3 that is closest to the input Vector3.</returns>
    Vector3 NearestAxis(Vector3 input, bool allowY)
    {
        // Find component with laregest magnitude.
        float xMag = Mathf.Abs(input.x); float yMag = Mathf.Abs(input.y); float zMag = Mathf.Abs(input.z);
        float max = Mathf.Max(xMag, allowY? yMag : 0f, zMag);

        float sign;
        Vector3 output;

        if (max == xMag)
        {
            sign = input.x >= 0 ? 1f : -1f;
            output = Vector3.right * sign;
        }
        else if (max == yMag && allowY)
        {
            sign = input.y >= 0 ? 1f : -1f;
            output = Vector3.up * sign;
        }
        else
        {
            sign = input.z >= 0 ? 1f : -1f;
            output = Vector3.forward * sign;
        }

        return output;
    }

    /// <summary>
    /// Copies an arbitrary collider to a new gameobject.
    /// </summary>
    /// <param name="original">The collider you want to copy.</param>
    /// <param name="target">The GameObject you want to paste to.</param>
    /// <returns>The newly created collider.</returns>
    public static Collider CopyCollider(Collider original, GameObject target)
    {
        Collider copy = (Collider)target.AddComponent(original.GetType());
        System.Reflection.FieldInfo[] fields = original.GetType().GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }

    /// <summary>
    /// Creates a new GameObject with a copy of the collider provided, positioned one frame before its current position (assuming it's moving towards moveDirection at speed m/s. Be sure to Dispose of the GameObject after your SweepTest, and to do your SweepTest for twice the move distance.)
    /// </summary>
    /// <param name="thisCollider">The collider to copy.</param>
    /// <param name="moveDirection">The direction that you plan to sweeptest towards.</param>
    /// <param name="speed">The speed with which your object is moving.</param>
    /// <returns>The Rigidbody with which to do your SweepTest.</returns>
    public static Rigidbody SweepTestRigidBody(Collider thisCollider, Vector3 moveDirection, float speed)
    {
        GameObject SweepTestObject = new GameObject("Sweep Test Object");
        SweepTestObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Collider sweepCollider = CopyCollider(thisCollider, SweepTestObject);
        sweepCollider.isTrigger = true;

        Rigidbody sweepRigidBody = SweepTestObject.AddComponent<Rigidbody>();
        sweepRigidBody.isKinematic = true;

        // Copy its position to this object's position, but one step before, so that SweepTest can detect the collision.
        SweepTestObject.transform.position = thisCollider.transform.position - moveDirection * speed * Time.deltaTime;
        SweepTestObject.transform.rotation = thisCollider.transform.rotation;
        SweepTestObject.transform.localScale = thisCollider.transform.localScale;

        return sweepRigidBody;
    }

    /// <summary>
    /// Checks if thisCollider, when moving towards moveDirection, is approaching otherCollider.
    /// </summary>
    /// <param name="thisCollider">The collider of the moving object.</param>
    /// <param name="otherCollider">The collider of the object it's approaching.</param>
    /// <param name="moveDirection">The direction thisCollider is moving towards.</param>
    public static bool MovingIntongCollider(Collider thisCollider, Collider otherCollider, Vector3 moveDirection)
    {
        bool movingIntoCollider = false;

        Transform thisTransform = thisCollider.transform;
        Transform otherTransform = otherCollider.transform;

        // Approximate the points on the surface of each collider that minimize the distance from each other.
        Vector3 thisClosest = Physics.ClosestPoint(otherTransform.position, thisCollider, thisTransform.position, thisTransform.rotation);
        Vector3 otherClosest = Physics.ClosestPoint(thisClosest, otherCollider, otherTransform.transform.position, otherTransform.transform.rotation);

        float threshold = 0.5f; // How close thisCollider has to get to otherCollider for the test to pass.
        if (Vector3.Distance(thisClosest, otherClosest) <= threshold)
        {
            // If thisCollider is moving in the general direction of otherCollider (within 45 degrees), return true.
            if (Vector3.Dot(otherClosest - thisClosest, moveDirection) > 0.25f) movingIntoCollider = true;
        }

        return movingIntoCollider;
    }
}
