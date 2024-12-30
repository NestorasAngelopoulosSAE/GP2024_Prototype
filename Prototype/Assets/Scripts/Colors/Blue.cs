/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Moves the object in the direction specified by the player.
/// When it hits a wall (or a series of obstacles that lead to a wall), it sends the object back.
/// </summary>
using UnityEngine;

public class Blue : MonoBehaviour
{
    ColorManager colorManager;
    GameObject player;
    Collider playerCollider;

    Collider thisCollider;
    Collider triggerCollider;
    Rigidbody rigidBody;
    GameObject RideTriggerGameObject;
    GameObject SweepTestObject;
    Vector3 moveDirection;

    float speed = 2f;

    void Start()
    {
        //Debug.Log($"{gameObject.name} is Blue!");

        colorManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<ColorManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider>();

        // Lift object so it doesn't overlap with the floor. (Necessary in order to detect the floor as an obstacle when moving downwards.)
        transform.position += Vector3.up * 0.01f;

        GetMoveDirection();

        thisCollider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;

        // Create a trigger on top of the platform, so that objects can ride it.
        RideTriggerGameObject = new GameObject("Ride Trigger");
        RideTriggerGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        RideTriggerGameObject.transform.position = transform.position;
        RideTriggerGameObject.transform.rotation = transform.rotation;
        RideTriggerGameObject.transform.localScale = transform.localScale * 0.95f; // Make trigger slightly smaller so that it doesn't trigger from the sides.

        // Make new object follow this object.
        RideTriggerGameObject.transform.parent = transform;

        RideTriggerGameObject.AddComponent<PlatformRideTrigger>();

        // Copy Collider over to trigger object;
        triggerCollider = CopyCollider(thisCollider, RideTriggerGameObject);
        triggerCollider.isTrigger = true;
    }

    void GetMoveDirection()
    {
        // Get direction of movement. (the direction opposite that to the player, snapped to the nearset axis in local or world space, based on the selected option)
        if (colorManager.allowBlueVertical) moveDirection = transform.TransformDirection(NearestAxis(transform.InverseTransformDirection(transform.position - Camera.main.transform.position), true)).normalized;
        else moveDirection = NearestAxis(Camera.main.transform.TransformDirection(Vector3.forward), false).normalized;
    }

    private void OnDisable()
    {
        // Detach all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).SetParent(null, true);
        }

        Destroy(RideTriggerGameObject);

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }

    private void Update()
    {
        // Re-calculate the moveDirection every time the player clicks the platform.
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform == transform) GetMoveDirection();
            }
        }
    }

    void FixedUpdate()
    {
        // SweepTest will include the colliders of child objects even when they are triggers.
        // To only test for the platform collider and not any child collider, we create a new GameObject with the same collider, set it to trigger so that it doesn't interact with anything, and do the SweepTest from there.
        SweepTestObject = new GameObject("Sweep Test Object");
        SweepTestObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        Collider sweepCollider = CopyCollider(thisCollider, SweepTestObject);
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

        transform.position += moveDirection * Time.deltaTime * speed;
    }

    private void LateUpdate()
    {
        // Make sure the ride trigger is always on top of the object, regardless of orientation.
        RideTriggerGameObject.transform.position = transform.position + Vector3.up * 0.3f;
    }

    public void Bounce()
    {
        moveDirection *= -1f;
    }
    
    public void CheckForObstacle(Rigidbody rb)
    {
        if (rb.SweepTest(moveDirection, out RaycastHit hit, speed * Time.deltaTime, QueryTriggerInteraction.Ignore))
        {
            // Check if object is to collide with something on the next frame.

            // Known issue:
            // When the platform is already colliding with something, SweepTest ignores that collision.
            // Therefore, if the platform has already been pushed against a wall or another obstacle, the SweepTest will fail.
            // This will cause the platform to clip through the obstacle.

            if (hit.collider.transform.parent != gameObject && hit.collider.tag == "Colorable" && hit.collider.gameObject.GetComponent<Rigidbody>() &! hit.collider.gameObject.GetComponent<Green>())
            {
                // Add an Obstacle script to the movable obstacle and let it create a chain of objects that are being pushed.
                // Once one of them hits something, a bounce command will propagate back through the chain, unlinking all obstacles in the process.

                if (hit.collider.gameObject.GetComponent<Obstacle>()) return; // Don't add another Obstacle script if it already has one so as not to lag the whole engine.
                Obstacle obstacle = hit.collider.gameObject.AddComponent<Obstacle>();
                obstacle.first = true;
                obstacle.Platform = this;
                obstacle.moveDirection = moveDirection;
                obstacle.speed = speed;
            }
            else if (hit.collider.tag != "Player") Bounce();
        }
        else if (transform.childCount > 1 && Vector3.Dot(moveDirection, Vector3.up) > 0)
        {
            // If we're going upwards, Raycast from all the vertices above each child's origin, and if you hit something, bounce.

            // Known issue:
            // When two objects are stacked on top of each other while riding the platform, the top object isn't calculated as part of the platform.
            // Therefore, the top object will clip though either the ceiling, or the object below it.
            
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject == RideTriggerGameObject) continue; // Skip the Ride Trigger

                Vector3 tallestPoint = transform.position;

                Mesh mesh;
                Vector3 scalingFactor = Vector3.one;
                Vector3 offset = Vector3.zero;
                // Get mesh from child collider
                if (child.GetComponent<MeshFilter>()) mesh = child.GetComponent<MeshFilter>().mesh;
                else // If object doesn't have a mesh filter, assume a bounding box.
                {
                    mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                    scalingFactor = child.localScale;
                }

                if (child.gameObject == player) // The player's collision does not fit the general approach.
                {
                    CharacterController controller = player.GetComponent<CharacterController>();
                    scalingFactor = new Vector3(controller.radius, controller.height, controller.radius);
                    offset = controller.center;
                }

                foreach (Vector3 vertex in mesh.vertices)
                {
                    Vector3 testpoint = vertex;

                    // Adjust bounding box to match scale and offset of object.
                    testpoint.x *= scalingFactor.x;
                    testpoint.y *= scalingFactor.y;
                    testpoint.z *= scalingFactor.z;
                    testpoint = child.TransformPoint(testpoint + offset);

                    if (Physics.Raycast(testpoint, moveDirection, out hit, speed * Time.deltaTime))
                    {
                        if (hit.transform != transform && hit.transform.parent != transform) Bounce();
                    }
                }
            }
        }
        else if (MovingIntongCollider(thisCollider, playerCollider, moveDirection))
        {
            Bounce(); // If the player is in front of the object, bounce. (handling this in the sweep test gave inconsistent results)
        }
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

        Vector3 thisClosest = Physics.ClosestPoint(otherTransform.position, thisCollider, thisTransform.position, thisTransform.rotation);
        Vector3 playerClosest = Physics.ClosestPoint(thisClosest, otherCollider, otherTransform.transform.position, otherTransform.transform.rotation);

        float thresshold = 0.5f; // How close thisCollider can get to otherCollider before it is recognized.
        if (Vector3.Distance(thisClosest, playerClosest) <= thresshold)
        {
            // If thisCollider is moving in the general direction of otherCollider, return true.
            if (Vector3.Dot(playerClosest - thisClosest, moveDirection) > 0f) movingIntoCollider = true;
        }

        return movingIntoCollider;
    }
}
