using UnityEngine;
using SojaExiles;
using TMPro;

public class LaserPuzzleController : MonoBehaviour
{
    public enum PlacementType { Mirror, Splitter }

    [Header("Camera Settings")]
    public Camera puzzleCamera;
    public float transitionSpeed = 5f;

    [Header("Placement & Prefabs")]
    public GameObject mirrorPrefab;
    public GameObject splitterPrefab;
    public LayerMask placementLayer; 
    
    [Header("Placement Settings")]
    public Transform placementPlane;
    public float yOffset = 0.05f; 

    [Header("Laser Setup")]
    public LaserEmitter emitter; // Drag your LaserEmitter object here

    [Header("Win Condition")]
    public LaserReceiver[] goalReceivers;
    public GameObject digitRevealObject;
    
    [Header("Puzzle Description UI")]
    public GameObject instructionPanel; 
    public TextMeshProUGUI instructionText;
    
    private PlacementType currentSelection = PlacementType.Mirror;
    private Camera mainCamera;
    private bool isInteracting = false;
    private bool puzzleSolved = false;
    private bool canPlace = false; // Prevents the immediate click-spawn

    void Start()
    {
        mainCamera = Camera.main;
        if (puzzleCamera != null) puzzleCamera.enabled = false;
        
        AudioListener puzzleListener = puzzleCamera.GetComponent<AudioListener>();
        if (puzzleListener != null) puzzleListener.enabled = false;

        if (digitRevealObject != null) digitRevealObject.SetActive(false);

        // Ensure laser starts OFF
        if (emitter != null) emitter.isEnabled = false;
        if (instructionPanel != null) 
            instructionPanel.SetActive(false);
    }

    void Update()
    {
        if (!isInteracting) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzle();
            GameManager.Instance.escConsumedThisFrame = true;
        }

        // Selection logic
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentSelection = PlacementType.Mirror;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentSelection = PlacementType.Splitter;

        // PREVENTION LOGIC: Don't allow placement until the user releases the first click
        if (!canPlace)
        {
            if (Input.GetMouseButtonUp(0)) canPlace = true;
            return; 
        }

        if (Input.GetMouseButtonDown(0)) HandlePlacementInput(true); 
        if (Input.GetMouseButtonDown(1)) HandlePlacementInput(false);

        if (!puzzleSolved) CheckWinCondition();
    }

    public void ActivatePuzzle()
    {
        if (isInteracting) return;
        
        isInteracting = true;

        //stops past timer from incrementing
        GameManager.Instance.SetIncrementPastTimer(false);
        GameManager.Instance.SetPastStateTimerVisible(false);

        // set UIOpen flag
        GameManager.Instance.isUIOpen = true;

        // Disable player movement (using your project's component)
        canPlace = false; // Reset placement block

        PlayerMovement pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainCamera == null) mainCamera = Camera.main;
        mainCamera.enabled = false;
        puzzleCamera.enabled = true;
        
        mainCamera.GetComponent<AudioListener>().enabled = false;
        puzzleCamera.GetComponent<AudioListener>().enabled = true;

        // Turn ON the laser
        if (emitter != null) emitter.isEnabled = true;
        
        // Display Instruction pannel
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
            instructionText.text = "<b>MISSION:</b> Direct Laser Beam to power both Receivers\n" +
                                   "<b>[1 / 2]</b> Select Mirror / Splitter Block\n" +
                                   "<b>[L-Click]</b> Place or Rotate Block\n" +
                                   "<b>[R-Click]</b> Remove Block\n" +
                                   "<b>[ESC]</b> Exit Terminal";
            instructionText.alignment = TextAlignmentOptions.Center;
        }
        
    }

    void HandlePlacementInput(bool isLeftClick)
    {
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            bool hitObject = hit.collider.CompareTag("Mirror") || hit.collider.CompareTag("Splitter");

            if (isLeftClick)
            {
                if (hitObject)
                {
                    // Simple rotation since parent is now unscaled (NewLaserPuzzle)
                    hit.transform.Rotate(0, 90, 0);
                }
                else if (((1 << hit.collider.gameObject.layer) & placementLayer) != 0)
                {
                    PlaceObject(hit.point);
                }
            }
            else if (hitObject)
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void PlaceObject(Vector3 point)
    {
        GameObject toSpawn = (currentSelection == PlacementType.Mirror) ? mirrorPrefab : splitterPrefab;
        if (toSpawn != null && placementPlane != null)
        {
            float targetY = placementPlane.position.y + yOffset;
            Vector3 spawnPosition = new Vector3(point.x, targetY, point.z);

            // Parent to 'this.transform' (NewLaserPuzzle @ Scale 1,1,1)
            GameObject newItem = Instantiate(toSpawn, spawnPosition, Quaternion.identity, this.transform);
            newItem.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
    }

    public void ExitPuzzle()
    {
        isInteracting = false;
        if (emitter != null) emitter.isEnabled = false; // Turn OFF laser


        //starts past timer again
        GameManager.Instance.SetIncrementPastTimer(true);
        GameManager.Instance.SetPastStateTimerVisible(true);

        // set UIOpen flag
        GameManager.Instance.isUIOpen = false;

        puzzleCamera.enabled = false;
        mainCamera.enabled = true;
        puzzleCamera.GetComponent<AudioListener>().enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerMovement pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;
        
        // Hide instructions
        if (instructionPanel != null)
            instructionPanel.SetActive(false);
    }

    void CheckWinCondition()
    {
        if (goalReceivers.Length == 0) return;
        bool allHit = true;
        foreach (LaserReceiver receiver in goalReceivers)
        {
            if (!receiver.isHit) { allHit = false; break; }
        }
        if (allHit) { puzzleSolved = true; OnPuzzleSolved(); }
    }

    void OnPuzzleSolved()
    {
        if (digitRevealObject != null) digitRevealObject.SetActive(true);
        
        if (instructionText != null)
        {
            
            instructionText.text = "<color=green>PUZZLE SOLVED</color>\n\nDigit Reveal: <size=120%>1</size>";
        
            instructionText.alignment = TextAlignmentOptions.Center;
        }
        
    }
}