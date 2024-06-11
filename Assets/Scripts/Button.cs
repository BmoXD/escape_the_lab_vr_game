using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class VRButton : XRBaseInteractable
{
    [Tooltip("The Transform of the button.")]
    [SerializeField] private Transform buttonTransform;
    
    [Tooltip("The maximum depth the button can be pressed in.")]
    [SerializeField] private float maxPressDepth = 0.1f;  
    
    [Tooltip("Option to stay pressed once fully pressed.")]
    [SerializeField] private bool stayPressed = false;    
    
    [Tooltip("Option to start in the pressed state.")]
    [SerializeField] private bool startPressed = false;   
    
    [Tooltip("Option to run OnButtonReleased when externally released.")]
    [SerializeField] private bool runReleaseOnExternalRelease = false; 
    
    [Tooltip("List of other buttons to release when this button is pressed.")]
    [SerializeField] private List<VRButton> buttonsToRelease;  
    
    [Tooltip("Duration of the release animation in seconds.")]
    [SerializeField] private float releaseAnimationDuration = 0.5f; 
    
    [Tooltip("Sound played when the button is pressed.")]
    [SerializeField] private AudioClip pressSound;
    
    [Tooltip("Sound played when the button is released.")]
    [SerializeField] private AudioClip releaseSound;

    public UnityEvent onPressed;  // Event for button press
    public UnityEvent onReleased; // Event for button release

    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    private bool isHovering = false;
    private bool isStuckPressed = false;
    private bool hasBeenPressed = false;  // Flag to track if button was pressed
    private XRBaseInteractor hoverInteractor;
    private Vector3 localInteractorStartPosition;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (buttonTransform == null)
        {
            buttonTransform = transform;
        }
        initialPosition = buttonTransform.localPosition;
        pressedPosition = initialPosition + new Vector3(0, -maxPressDepth, 0);

        if (startPressed)
        {
            buttonTransform.localPosition = pressedPosition;
            isStuckPressed = true;
            hasBeenPressed = true;
            OnButtonPressed();
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (isStuckPressed) return;

        isHovering = true;
        hoverInteractor = args.interactor;

        localInteractorStartPosition = buttonTransform.parent.InverseTransformPoint(hoverInteractor.transform.position);
        Debug.Log("OnHoverEntered");
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (isStuckPressed) return;

        isHovering = false;
        hoverInteractor = null;
        StartCoroutine(ReleaseButton());
    }

    private void Update()
    {
        if (isHovering && hoverInteractor != null && !isStuckPressed)
        {
            UpdateButtonPosition();
        }
    }

    private void UpdateButtonPosition()
    {
        // Calculate the new button position based on the interactor's position
        Vector3 localInteractorPosition = buttonTransform.parent.InverseTransformPoint(hoverInteractor.transform.position) - localInteractorStartPosition;
        Debug.Log(localInteractorPosition);
        float pressAmount = Mathf.Clamp(localInteractorPosition.y - initialPosition.y, -maxPressDepth, 0);
        buttonTransform.localPosition = initialPosition + new Vector3(0, pressAmount, 0);

        if (buttonTransform.localPosition.y <= pressedPosition.y && !hasBeenPressed)
        {
            hasBeenPressed = true;
            OnButtonPressed();
            if (stayPressed)
            {
                isStuckPressed = true;
            }
        }
    }

    private System.Collections.IEnumerator ReleaseButton()
    {
        float timer = 0f;
        Vector3 startPos = buttonTransform.localPosition;
        while (timer < releaseAnimationDuration)
        {
            buttonTransform.localPosition = Vector3.Lerp(startPos, initialPosition, timer / releaseAnimationDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localPosition = initialPosition;

        if (hasBeenPressed)
        {
            hasBeenPressed = false;
            OnButtonReleased();
        }
    }

    private void OnButtonPressed()
    {
        // Handle the button press logic here
        Debug.Log("Button Pressed!");
        onPressed?.Invoke();
        audioSource.PlayOneShot(pressSound); // Play press sound

        // Release other buttons in the list
        foreach (var button in buttonsToRelease)
        {
            if (button.isStuckPressed)
            {
                button.ExternalRelease();
            }
        }
    }

    private void OnButtonReleased()
    {
        // Handle the button release logic here
        Debug.Log("Button Released!");
        onReleased?.Invoke();
        if (runReleaseOnExternalRelease && !isStuckPressed)
        {
            audioSource.PlayOneShot(releaseSound); // Play release sound only if externally released
        }
    }

    public void ExternalRelease()
    {
        if (isStuckPressed)
        {
            isStuckPressed = false;
            isHovering = false;          // Ensure the hovering state is reset
            hoverInteractor = null;      // Ensure the interactor reference is reset
            StartCoroutine(ReleaseButton());

            if (runReleaseOnExternalRelease)
            {
                OnButtonReleased();
            }
        }
    }
}