using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class BubbleSortPuzzle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private int revealDigit = 4;
    [SerializeField] private RenderTexture videoRenderTexture;

    private VisualElement root;
    private VisualElement codeArea, pool, demoArea, barsContainer;
    private Label statusLabel, digitLabel;
    private VisualElement[] slots = new VisualElement[4];
    private VisualElement[] blocks = new VisualElement[4];
    private string[] correctOrder = { "block-0", "block-1", "block-2", "block-3" };
    
    private VisualElement draggingBlock;
    private Vector2 dragOffset;
    private bool isSolved = false;

// Add this to your BubbleSortPuzzle.cs
    void Start()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // FIND THE BACKGROUND ELEMENT AND APPLY THE VIDEO TEXTURE
        VisualElement bg = root.Q<VisualElement>("BubbleRoot"); // Based on your UXML name
        if (bg != null && videoRenderTexture != null)
        {
            bg.style.backgroundImage = Background.FromRenderTexture(videoRenderTexture);
        }

        root.style.display = DisplayStyle.None; 
        CacheElements();
        ShuffleBlocks();
        SetupDragAndDrop();
    
        if (digitLabel != null) digitLabel.text = revealDigit.ToString();
    }

    public void OpenPuzzle()
    {
        root.style.display = DisplayStyle.Flex; // Show the UI [cite: 1]
        UnityEngine.Cursor.lockState = CursorLockMode.None; // Unlock the mouse for dragging [cite: 18]
        UnityEngine.Cursor.visible = true;
    }

    public void ClosePuzzle()
    {
        root.style.display = DisplayStyle.None; // Hide the UI
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; // Lock the mouse back for gameplay
        UnityEngine.Cursor.visible = false;
    }

    void CacheElements()
    {
        codeArea = root.Q<VisualElement>("CodeArea");
        pool = root.Q<VisualElement>("Pool");
        demoArea = root.Q<VisualElement>("DemoArea");
        barsContainer = root.Q<VisualElement>("BarsContainer");
        statusLabel = root.Q<Label>("Status");
        digitLabel = root.Q<Label>("Digit");

        for (int i = 0; i < 4; i++)
        {
            slots[i] = root.Q<VisualElement>($"slot-{i}");
            
            // Create code blocks dynamically
            var block = new VisualElement { name = $"block-{i}" };
            block.AddToClassList("code-block");
            block.userData = i; // Store index for validation
            
            var label = new Label(GetCodeForBlock(i));
            block.Add(label);
            
            blocks[i] = block;
        }
    }

    string GetCodeForBlock(int i) {
        string[] code = { 
            "for (int i=0; i < n-1; i++)", 
            "  for (int j=0; j < n-i-1; j++)", 
            "    if (arr[j] > arr[j+1])", 
            "      Swap(arr[j], arr[j+1]);" 
        };
        return code[i];
    }

    void ShuffleBlocks()
    {
        List<VisualElement> blockList = blocks.ToList();
        for (int i = blockList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = blockList[i];
            blockList[i] = blockList[j];
            blockList[j] = temp;
        }

        pool.Clear();
        foreach (var block in blockList)
        {
            pool.Add(block);
        }
    }

    void SetupDragAndDrop()
    {
        foreach (var block in blocks)
        {
            block.RegisterCallback<PointerDownEvent>(evt => {
                if (isSolved) return;
                draggingBlock = block;
                
                // FIXED: Explicitly use Vector2 for offset calculation
                dragOffset = (Vector2)evt.localPosition;
                
                block.AddToClassList("dragging");
                block.BringToFront();
                block.style.position = Position.Absolute;
                block.CapturePointer(evt.pointerId);
            });
            /*
            block.RegisterCallback<PointerMoveEvent>(evt => {
                if (draggingBlock != block || !block.HasPointerCapture(evt.pointerId)) return;
                
                // FIXED: Cast evt.position to Vector2 to avoid ambiguity error
                Vector2 currentMousePos = (Vector2)evt.position;
                Vector2 newPos = currentMousePos - dragOffset;
                
                block.style.left = newPos.x;
                block.style.top = newPos.y;
                UpdateDropFeedback(currentMousePos);
            });*/
            block.RegisterCallback<PointerMoveEvent>(evt => {
                if (draggingBlock != block || !block.HasPointerCapture(evt.pointerId)) return;

                // Get the change in mouse position relative to where we started dragging
                // This ensures the block stays attached to the cursor correctly
                Vector2 currentMousePos = evt.localPosition;
                Vector2 delta = currentMousePos - dragOffset;

                block.style.left = block.resolvedStyle.left + delta.x;
                block.style.top = block.resolvedStyle.top + delta.y;
            });

            block.RegisterCallback<PointerUpEvent>(evt => {
                if (draggingBlock != block) return;
                block.ReleasePointer(evt.pointerId);
                DropBlock((Vector2)evt.position);
                draggingBlock = null;
                block.RemoveFromClassList("dragging");
                ClearDropFeedback();
            });
        }
    }

    void UpdateDropFeedback(Vector2 pointerPos)
    {
        ClearDropFeedback();
        foreach (var slot in slots)
        {
            // Use worldBound.Contains for precise UI detection
            if (slot.worldBound.Contains(pointerPos))
            {
                if (IsValidDrop(slot, draggingBlock))
                    slot.AddToClassList("valid-hover");
                else
                    slot.AddToClassList("invalid-hover");
            }
        }
    }

    void ClearDropFeedback()
    {
        foreach (var slot in slots)
        {
            slot.RemoveFromClassList("valid-hover");
            slot.RemoveFromClassList("invalid-hover");
        }
    }

    bool IsValidDrop(VisualElement slot, VisualElement block)
    {
        if (slot.childCount > 0) return false;
        int slotIdx = System.Array.IndexOf(slots, slot);
        int blockIdx = (int)block.userData;
        return slotIdx == blockIdx;
    }

    void DropBlock(Vector2 pointerPos)
    {
        VisualElement targetSlot = null;
        foreach (var slot in slots)
        {
            if (slot.worldBound.Contains(pointerPos))
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot != null && IsValidDrop(targetSlot, draggingBlock))
        {
            // Successfully placed
            draggingBlock.style.position = Position.Relative;
            draggingBlock.style.left = StyleKeyword.Null;
            draggingBlock.style.top = StyleKeyword.Null;
            targetSlot.Add(draggingBlock);
            draggingBlock.AddToClassList("placed");
            CheckComplete();
        }
        else
        {
            // Return to pool
            draggingBlock.style.position = Position.Relative;
            draggingBlock.style.left = StyleKeyword.Null;
            draggingBlock.style.top = StyleKeyword.Null;
            pool.Add(draggingBlock);
        }
    }

    void CheckComplete()
    {
        bool complete = true;
        for (int i = 0; i < 4; i++)
        {
            if (slots[i].childCount == 0) { complete = false; break; }
        }

        if (complete && !isSolved)
        {
            isSolved = true;
            StartCoroutine(SolveSequence());
        }
    }

    IEnumerator SolveSequence()
    {
        if (statusLabel != null)
        {
            statusLabel.style.opacity = 1;
            yield return Typewriter(statusLabel, "COMPILING...", 0.05f);
        }
        
        if (demoArea != null) demoArea.RemoveFromClassList("demo-hidden");
        CreateBars();
        yield return AnimateBubbleSort();

        yield return new WaitForSeconds(1f);
        if (statusLabel != null) yield return Typewriter(statusLabel, $"ACCESS GRANTED: {revealDigit}", 0.05f);
        if (digitLabel != null) digitLabel.RemoveFromClassList("digit-hidden");
    }

    void CreateBars()
    {
        if (barsContainer == null) return;
        barsContainer.Clear();
        int[] array = { 4, 2, 1, 3 };
        for (int i = 0; i < array.Length; i++)
        {
            var bar = new VisualElement { name = $"bar-{i}" };
            bar.AddToClassList("bar");
            bar.style.width = 30;
            bar.style.height = array[i] * 20;
            barsContainer.Add(bar);
        }
    }

    IEnumerator AnimateBubbleSort()
    {
        int[] array = { 4, 2, 1, 3 };
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                    
                    var bar1 = barsContainer.Q<VisualElement>($"bar-{j}");
                    var bar2 = barsContainer.Q<VisualElement>($"bar-{j + 1}");
                    
                    if (bar1 != null && bar2 != null)
                    {
                        float h1 = bar1.resolvedStyle.height;
                        bar1.style.height = bar2.resolvedStyle.height;
                        bar2.style.height = h1;
                    }
                    
                    yield return new WaitForSeconds(0.4f);
                }
            }
        }
    }

    IEnumerator Typewriter(Label label, string text, float delay)
    {
        label.text = "";
        foreach (char c in text)
        {
            label.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}