using IterationToolkit;
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
    [SerializeField] private Texture2D defaultCursorSprite;

    [Space(20), Header("Camera")]
    [SerializeField] private Vector3 translationAtRestCameraPos;
    [SerializeField] private Vector3 translationOffsetCameraPos;
    [SerializeField] private Vector3 cameraRotation;

    [Space(20), Header("Health")]
    [SerializeField] private int maxHealth;
    [field: SerializeField] public HealthController HealthController { get; private set; }

    [Space(20), Header("Audio")]
    [SerializeField] private AudioSource movementAudioSource;
    [SerializeField] private AudioSource interactAudioSource;
    [SerializeField] private AudioPreset movementPreset;
    [SerializeField] private AudioPreset damagePreset;
    [SerializeField] private AudioPreset pickupItemPreset;
    [SerializeField] private AudioPreset validDropItemPreset;
    [SerializeField] private AudioPreset invalidDropItemPreset;
    [SerializeField] private AudioPreset failedInteractionPreset;


    private PositionConstraint positionConstraint;

    private Vector2 moveInput;

    private Texture2D newCursor;

    [SerializeField] private ItemBehaviour mostRecentItemInRange;
    [field: SerializeField] public ItemBehaviour ActiveItem { get; private set; }

    public ExtendedEvent<ItemBehaviour> OnItemPickup = new ExtendedEvent<ItemBehaviour>();
    public ExtendedEvent<ItemBehaviour> OnItemDropped = new ExtendedEvent<ItemBehaviour>();

    private void Awake()
    {
        InitializeCamera();

        HealthController.LinkBehaviour(this, damagePreset);
        HealthController.SetMaxHealth(maxHealth);
        HealthController.ResetHealth();
        HealthController.OnDeath.AddListener(Die);
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
        ActiveCamera.transform.rotation = Quaternion.Euler(cameraRotation);

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
        if (GameManager.Instance.HasGameEnded) return;
        if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0))
        {
            if (ActiveItem == null && mostRecentItemInRange != null)
                PickupItem(mostRecentItemInRange);
            else if (ActiveItem != null)
            {
                if (Physics.Raycast(heldItemPosition.transform.position, new Vector3(heldItemPosition.position.x, -5000, heldItemPosition.position.z), out RaycastHit hit, Mathf.Infinity, dropMask))
                    DropItem(hit.point);
                else
                    AudioManager.PlayAudio(invalidDropItemPreset, interactAudioSource);
            }
            else
                AudioManager.PlayAudio(failedInteractionPreset, interactAudioSource);

        }
    }

    private void PickupItem(ItemBehaviour item)
    {
        Debug.Log("Picking Up: " + item.name);
        ActiveItem = item;
        item.IsBeingHeld = true;
        item.transform.SetParent(heldItemPosition, true);
        item.transform.localPosition = Vector3.zero;

        AudioManager.PlayAudio(pickupItemPreset, interactAudioSource);

        item.Pickup();
        OnItemPickup.Invoke(item);
    }

    public void DropItem(Vector3 position)
    {
        if (ActiveItem == null) return;
        Debug.Log("Dropping: " + ActiveItem.name);

        ActiveItem.transform.SetParent(null);
        ActiveItem.transform.position = position;
        ActiveItem.transform.rotation = Quaternion.identity;
        ActiveItem.IsBeingHeld = false;

        AudioManager.PlayAudio(validDropItemPreset, interactAudioSource);

        ActiveItem.Drop();
        OnItemDropped.Invoke(ActiveItem);
        ActiveItem = null;
    }

    private void Die()
    {

    }




    private void LateUpdate()
    {
        if (GameManager.Instance.HasGameEnded) return;
        SetRotation();

        animator.SetBool("isWalking", rigidBody.velocity.magnitude > 1f);
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.HasGameEnded)
        {
            rigidBody.velocity = Vector3.zero;
            return;
        }
        positionConstraint.translationAtRest = translationAtRestCameraPos;
        positionConstraint.translationOffset = translationOffsetCameraPos;

        SetPosition();

        if (ActiveItem != null)
        {
            ActiveItem.transform.localPosition = Vector3.zero;
        }

        RefreshCursor();
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

    public void RequestNewCursor(Texture2D newNewCursor)
    {
        newCursor = newNewCursor;
    }

    private void RefreshCursor()
    {
        if (newCursor != null)
        {
            Cursor.SetCursor(newCursor, Vector2.zero, CursorMode.Auto);
            newCursor = null;
        }
        else if (defaultCursorSprite != null)
            Cursor.SetCursor(defaultCursorSprite, Vector2.zero, CursorMode.Auto);
    }

    private void SetPosition()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        rigidBody.velocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
    }
}
