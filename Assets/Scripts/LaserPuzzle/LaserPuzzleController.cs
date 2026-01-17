using UnityEngine;
using SojaExiles; // Including your specific namespace for PlayerMovement

public class LaserPuzzleController : MonoBehaviour
{
    public enum PlacementType { Mirror, Splitter }

    [Header("Camera Settings")]
    public Camera puzzleCamera; // The Top-Down camera
    public float transitionSpeed = 5f;

    [Header("Placement & Prefabs")]
    public GameObject mirrorPrefab;
    public GameObject splitterPrefab;
    public LayerMask placementLayer; // HAS TO BE SET TO "Interactable" (the Table's layer)
    
    [Header("Placement Settings")]
    public Transform placementPlane;
    
    
    [Header("Win Condition")]
    public LaserReceiver[] goalReceivers;
    public GameObject digitRevealObject; // The object/decal that shows the number

    private PlacementType currentSelection = PlacementType.Mirror;
    private Camera mainCamera;
    private bool isInteracting = false;
    private bool puzzleSolved = false;
    public float yOffset = 0.05f; 

    void Start()
    {
        mainCamera = Camera.main;
        
        if (puzzleCamera != null) 
            puzzleCamera.enabled = false;
        
        AudioListener puzzleListener = puzzleCamera.GetComponent<AudioListener>();
        if (puzzleListener != null) puzzleListener.enabled = false;

        if (digitRevealObject != null)
            digitRevealObject.SetActive(false);
    }

    void Update()
    {
        if (!isInteracting) return;

        if (Input.GetKeyDown(KeyCode.Escape)) ExitPuzzle();

        // Selection logic
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentSelection = PlacementType.Mirror;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentSelection = PlacementType.Splitter;

        // LEFT CLICK to Place or Rotate
        if (Input.GetMouseButtonDown(0)) 
        {
            HandlePlacementInput(true); 
        }
    
        // RIGHT CLICK to Delete
        if (Input.GetMouseButtonDown(1))
        {
            HandlePlacementInput(false);
        }

        if (!puzzleSolved) CheckWinCondition();
    }

    public void ActivatePuzzle()
    {
        if (isInteracting) return;
        
        isInteracting = true;

        //stops past timer from incrementing
        GameManager.Instance.SetIncrementPastTimer(false);
        GameManager.Instance.SetPastStateTimerVisible(false);

        // Disable player movement (using your project's component)
        PlayerMovement pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Unlock cursor so we can click on the table
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Swap Cameras
        if (mainCamera == null) mainCamera = Camera.main;
        mainCamera.enabled = false;
        puzzleCamera.enabled = true;
        
        // Swap Listeners
        mainCamera.GetComponent<AudioListener>().enabled = false;
        puzzleCamera.GetComponent<AudioListener>().enabled = true;
        
        Debug.Log("Puzzle View Activated. 1: Mirror, 2: Splitter");
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
                    // Rotate existing
                    Transform originalParent = hit.transform.parent;
                    hit.transform.SetParent(null);

                    // 2. Rotate in world space
                    hit.transform.Rotate(0, 90, 0);

                    // 3. Set the absolute world scale (lossyScale) to exactly what you want
                    // We do this by setting localScale while it has no parent
                    hit.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                    // 4. Put it back under the table
                    hit.transform.SetParent(originalParent);
        
                    Debug.Log("Rotated and Rescaled " + hit.collider.name);
                }
                else if (((1 << hit.collider.gameObject.layer) & placementLayer) != 0)
                {
                    // Place new
                    PlaceObject(hit.point);
                }
            }
            else // Right Click Logic
            {
                if (hitObject)
                {
                    // Remove the object
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Object Removed");
                }
            }
        }
    }

    void PlaceObject(Vector3 point)
    {
        GameObject toSpawn = (currentSelection == PlacementType.Mirror) ? mirrorPrefab : splitterPrefab;
    
        if (toSpawn != null && placementPlane != null)
        {
            // Use the Plane's Y position plus a tiny offset
            float targetY = placementPlane.position.y + yOffset;
            Vector3 spawnPosition = new Vector3(point.x, targetY, point.z);

            GameObject newItem = Instantiate(toSpawn, spawnPosition, Quaternion.identity, this.transform);
            Vector3 parentScale = transform.lossyScale;
            newItem.transform.localScale = new Vector3(
                0.4f / parentScale.x, 
                0.4f / parentScale.y, 
                0.4f / parentScale.z
            );
        }
    }

    void CheckWinCondition()
    {
        if (goalReceivers.Length == 0) return;

        bool allHit = true;
        foreach (LaserReceiver receiver in goalReceivers)
        {
            if (!receiver.isHit)
            {
                allHit = false;
                break;
            }
        }

        if (allHit)
        {
            puzzleSolved = true;
            OnPuzzleSolved();
        }
    }

    void OnPuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
        if (digitRevealObject != null)
        {
            digitRevealObject.SetActive(true);
        }
    }

    public void ExitPuzzle()
    {
        isInteracting = false;

        //starts past timer again
        GameManager.Instance.SetIncrementPastTimer(true);
        GameManager.Instance.SetPastStateTimerVisible(true);

        puzzleCamera.enabled = false;
        mainCamera.enabled = true;
        
        puzzleCamera.GetComponent<AudioListener>().enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;

        // Lock cursor back for FPS mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerMovement pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;
    }
}