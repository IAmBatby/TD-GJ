using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody rigidBody;
    [field: SerializeField] public Camera ActiveCamera { get; private set; }
    [SerializeField] private KeyCode interactKey;
    [SerializeField] private Transform heldItemPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask lookMask;
    [SerializeField] private LayerMask dropMask;
    [SerializeField] private Animator animator;

    [SerializeField] private Vector3 translationAtRestCameraPos;
    [SerializeField] private Vector3 translationOffsetCameraPos;

    private PositionConstraint positionConstraint;

    private Vector2 moveInput;

    [SerializeField] private ItemBehaviour mostRecentItemInRange;
    [field: SerializeField] public ItemBehaviour ActiveItem { get; private set; }

    private void Awake()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        ActiveCamera.enabled = true;
        ConstraintSource source = new ConstraintSource();
        source.weight = 1;
        source.sourceTransform = transform;
        positionConstraint = ActiveCamera.GetComponent<PositionConstraint>();
        positionConstraint.SetSource(0, source);
        ActiveCamera.transform.position = new Vector3(1.1f, translationAtRestCameraPos.y, -16.4f);
        ActiveCamera.transform.parent = null;
    }

    public void ForwardedTriggerEnter(Collider other)
    {
        Debug.Log("Forwarded Enter: " + other);
        if (ActiveItem == null && other.TryGetComponent(out ItemBehaviour item))
        {
            mostRecentItemInRange = item;
        }
    }

    public void ForwardedTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ItemBehaviour item))
            if (item == mostRecentItemInRange)
                mostRecentItemInRange = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0))
        {
            if (ActiveItem == null && mostRecentItemInRange != null)
                PickupItem(mostRecentItemInRange);
            else if (ActiveItem != null)
                if (Physics.Raycast(heldItemPosition.transform.position, new Vector3(heldItemPosition.position.x,-5000, heldItemPosition.position.z), out RaycastHit hit, Mathf.Infinity, dropMask))
                    DropItem(hit.point);
        }
    }

    private void PickupItem(ItemBehaviour item)
    {
        Debug.Log("Picking Up: " + item.name);
        ActiveItem = item;
        item.IsBeingHeld = true;
        item.transform.SetParent(heldItemPosition, true);
        item.transform.localPosition = Vector3.zero;
    }

    public void DropItem(Vector3 position)
    {
        if (ActiveItem == null) return;
        Debug.Log("Dropping: " + ActiveItem.name);

        ActiveItem.transform.SetParent(null);
        ActiveItem.transform.position = position;
        ActiveItem.transform.rotation = Quaternion.identity;
        ActiveItem.IsBeingHeld = false;
        ActiveItem = null;
    }




    private void LateUpdate()
    {
        SetRotation();

        animator.SetBool("isWalking", rigidBody.velocity.magnitude > 1f);
    }
    private void FixedUpdate()
    {
        positionConstraint.translationAtRest = translationAtRestCameraPos;
        positionConstraint.translationOffset = translationOffsetCameraPos;

        SetPosition();

        if (ActiveItem != null)
        {
            ActiveItem.transform.localPosition = Vector3.zero;
        }
    }

    private void SetRotation()
    {
        Vector3 returnValue = Vector3.zero;

        if (Physics.Raycast(ActiveCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, layerMask: lookMask, maxDistance: Mathf.Infinity, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
            returnValue = hit.point;
        transform.LookAt(returnValue);
        Vector3 fixedRotation = transform.rotation.eulerAngles;
        fixedRotation.x = 0;
        fixedRotation.z = 0;
        transform.eulerAngles = fixedRotation;
    }

    private void SetPosition()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        rigidBody.velocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
    }
}
