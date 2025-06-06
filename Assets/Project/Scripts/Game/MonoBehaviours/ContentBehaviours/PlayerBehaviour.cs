using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEditor.Progress;
using Logger = IterationToolkit.Logger;

public class PlayerBehaviour : HurtableBehaviour
{
    [field: SerializeField] public Camera ActiveCamera { get; private set; }
    [SerializeField] private KeyCode interactKey;
    [SerializeField] private Transform heldItemPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask lookMask;
    [SerializeField] private LayerMask dropMask;
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private Transform skinParent;
    private Animator activeAnimator;
    [SerializeField] private Texture2D defaultCursorSprite;

    [Space(20), Header("Camera")]
    [SerializeField] private Vector3 translationAtRestCameraPos;
    [SerializeField] private Vector3 translationOffsetCameraPos;
    [SerializeField] private Vector3 cameraRotation;

    [Space(20), Header("Audio")]
    [SerializeField] private AudioPreset movementPreset;
    [SerializeField] private AudioPreset damagePreset;
    [SerializeField] private AudioPreset pickupItemPreset;
    [SerializeField] private AudioPreset validDropItemPreset;
    [SerializeField] private AudioPreset invalidDropItemPreset;
    [SerializeField] private AudioPreset failedInteractionPreset;

    public Logger Log => GameManager.Logs.Player;


    private PositionConstraint positionConstraint;

    private Vector2 moveInput;

    private Texture2D newCursor;

    [SerializeField] private ItemBehaviour mostRecentItemInRange;
    private IInteractable mostRecentInteractableInRange;
    [field: SerializeField] public ItemBehaviour ActiveItem { get; private set; }

    public ExtendedEvent<ItemBehaviour> OnItemPickup = new ExtendedEvent<ItemBehaviour>();
    public ExtendedEvent<ItemBehaviour> OnItemDropped = new ExtendedEvent<ItemBehaviour>();

    [SerializeField] private bool canMove;
    [SerializeField] private float lockoutDuration;
    [SerializeField] private Timer movementLockTimer;
    protected override void OnSpawn()
    {
        base.OnSpawn();
        InitializeCamera();

        if (ContentData is ScriptablePlayer player)
            SetSkin(player.DefaultSkin);

        GameManager.OnNewWave.AddListener(SwitchRandomSkin);
        OnHealthModified.AddListener(FlashHealth);
        OnHealthModified.AddListener(LogDamage);
        GameManager.OnNewWave.AddListener(FlashOnNewWave);
        CheatManager.RegisterCheat(ResetHealth, "Player");

        canMove = true;
    }

    private void LogDamage((int, int) health)
    {
        if (health.Item2 < health.Item1)
            Log.LogInfo("Took " + (health.Item2 - health.Item2) + " Damage!", Color.red);
        else
            Log.LogInfo("Healed " + (health.Item2 - health.Item2) + " Damage!", Color.green);
    }

    private void FlashOnNewWave()
    {
        //MaterialController.ApplyMaterial(new Material(GlobalData.Instance.PreviewMaterial), Color.magenta, 1f);
    }

    private void FlashHealth((int oldHealth, int newHealth) health)
    {
        if (health.oldHealth >= health.newHealth) return;

        //MaterialController.ApplyMaterial(GlobalData.Instance.PreviewMaterial, Color.red, 0.25f);
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
        //Debug.Log("Forwarded Enter: " + other);
        if (ActiveItem == null)
        {
            ItemBehaviour item = null;
            if (other.TryGetComponent(out ItemBehaviour directItem))
                item = directItem;
            else if (other.transform.root.TryGetComponent(out ItemBehaviour rootItem))
                item = rootItem;
            if (item != null)
            if (item.ItemData != null && item.ItemData.CanBePickedUp)
            {
                mostRecentItemInRange = item;
                Log.LogInfo("Item In Range: " + item.ContentData.GetDisplayName(), item.ItemData.GetDisplayColor());
            }
        }

        if (other.TryGetComponent(out IInteractable interactable))
        {
            Log.LogInfo("Item In Range: " + interactable.ToString(), Color.gray);
            mostRecentInteractableInRange = interactable;
        }
    }

    public void ForwardedTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ItemBehaviour item))
            if (item == mostRecentItemInRange)
                mostRecentItemInRange = null;

        if (other.TryGetComponent(out IInteractable interactable))
            if (interactable == mostRecentInteractableInRange)
                mostRecentInteractableInRange = null;
    }

    private void Update()
    {
        if (GameManager.IsLevelActive == false) return;
        if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0))
        {
            if (mostRecentInteractableInRange != null)
                if (mostRecentInteractableInRange.TryInteract())
                    return;
            if (ActiveItem == null && mostRecentItemInRange != null)
                PickupItem(mostRecentItemInRange);
            else if (ActiveItem != null)
            {
                if (Physics.Raycast(heldItemPosition.transform.position, new Vector3(heldItemPosition.position.x, -5000, heldItemPosition.position.z), out RaycastHit hit, Mathf.Infinity, dropMask))
                    DropItem(hit.point);
                else
                    ReactionPlayer.Play(invalidDropItemPreset);
            }
            else
                ReactionPlayer.Play(failedInteractionPreset);

        }
    }

    public void PickupItem(ItemBehaviour item)
    {
        Log.LogInfo("Picked Up: " + item.ContentData.GetDisplayName(), item.ContentData.GetDisplayColor());
        ActiveItem = item;
        item.transform.SetParent(heldItemPosition, true);
        item.transform.localPosition = Vector3.zero;

        ReactionPlayer.Play(pickupItemPreset);

        item.Pickup();
        OnItemPickup.Invoke(item);
    }

    public void DropItem(Vector3 position)
    {
        if (ActiveItem == null) return;
        if (ActiveItem.ContentData == null)
        {
            Debug.LogError(ActiveItem.name + " has missing contentdata!", ActiveItem);
            return;
        }
        Log.LogInfo("Dropped: " + ActiveItem.ContentData.GetDisplayName(), ActiveItem.ContentData.GetDisplayColor());

        ActiveItem.transform.SetParent(null);
        ActiveItem.transform.position = position;
        ActiveItem.transform.rotation = Quaternion.identity;

        ReactionPlayer.Play(validDropItemPreset);

        ActiveItem.Drop();
        OnItemDropped.Invoke(ActiveItem);
        ActiveItem = null;
    }
    protected override void OnDeath()
    {
        if (activeAnimator != null)
            activeAnimator.SetTrigger("Die");
        GameManager.Instance.EndGame();
    }

    private void LateUpdate()
    {
        if (GameManager.IsLevelActive == false) return;
        SetRotation();

        if (activeAnimator != null && canMove)
            activeAnimator.SetBool("isWalking", Rigidbody.velocity.magnitude > 1f);
    }
    private void FixedUpdate()
    {
        if (GameManager.IsLevelActive == false)
        {
            Rigidbody.velocity = Vector3.zero;
            return;
        }
        positionConstraint.translationAtRest = translationAtRestCameraPos;
        positionConstraint.translationOffset = translationOffsetCameraPos;

        if(canMove)
            SetPosition();

        if (ActiveItem != null)
            ActiveItem.transform.localPosition = Vector3.zero;
    }

    private void SwitchRandomSkin()
    {
        //ScriptableSkin randomSkin = GlobalData.Skins.Skins[Random.Range(0, GlobalData.Skins.Skins.Count)];
        //SetSkin(randomSkin);
    }

    private void SetSkin(ScriptableSkin skin)
    {
        if (activeAnimator != null)
        {
            GameObject.Destroy(activeAnimator.gameObject);
            activeAnimator = null;
        }

        GameObject newSkin = GameObject.Instantiate(skin.SkinPrefab, skinParent.transform);
        activeAnimator = newSkin.gameObject.AddComponent<Animator>();
        activeAnimator.runtimeAnimatorController = animatorController as RuntimeAnimatorController;
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

        Rigidbody.velocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
    }

    protected override Vector3 OnBeforeHitForceApplied(Vector3 forceToBeHitWith)
    {
        return (new Vector3(forceToBeHitWith.x, 0, forceToBeHitWith.z));
    }

    protected override void OnReceivedHit()
    {
        activeAnimator.SetTrigger("Hit");
        canMove = false;
        movementLockTimer = new Timer(this, lockoutDuration, ResetMovement);
    }

    public void ResetMovement()
    {
        canMove = true;
    }

    public override void RegisterBehaviour()
    {
        ContentManager.RegisterBehaviour(this);
        base.RegisterBehaviour();
    }
    public override void UnregisterBehaviour(bool destroyOnUnregistration)
    {
        ContentManager.UnregisterBehaviour(this, destroyOnUnregistration);
        base.UnregisterBehaviour(destroyOnUnregistration);
    }
}
