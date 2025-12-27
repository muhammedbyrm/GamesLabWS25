using UnityEngine;
using System.Collections.Generic;

public class LaserEmitter : MonoBehaviour
{
    [Header("Laser Settings")]
    public int maxBounces = 10;
    public float maxDistance = 100f;
    public Material laserMaterial;
    public float laserWidth = 0.02f;
    public Color laserColor = Color.red;

    private List<LineRenderer> laserPool = new List<LineRenderer>();
    private int activeLines = 0;

    void Update()
    {
        // Reset the pool count for this frame
        activeLines = 0;
        
        // Start the recursive laser casting
        CastLaser(transform.position, transform.forward, maxBounces);
        

        // Turn off any unused line renderers from the pool
        for (int i = activeLines; i < laserPool.Count; i++)
        {
            laserPool[i].enabled = false;
        }
    }

    void CastLaser(Vector3 startPos, Vector3 direction, int bouncesLeft)
    {
        if (bouncesLeft <= 0) return;

        LineRenderer line = GetOrCreateLine();
        line.enabled = true;
    
        // Create the ray
        Ray ray = new Ray(startPos, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            
            //Debug.Log("Laser hit: " + hit.collider.name + " tagged: " + hit.collider.tag);
            line.positionCount = 2;
            line.SetPosition(0, startPos);
            line.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                Vector3 mirrorForward = hit.collider.transform.forward;
                Vector3 nextStart = hit.point + mirrorForward * 0.1f; 
                CastLaser(nextStart, mirrorForward, bouncesLeft - 1);
                
            }
            else if (hit.collider.CompareTag("Splitter"))
            {
                Vector3 rightDir = hit.collider.transform.right;
                Vector3 leftDir = -hit.collider.transform.right;

                CastLaser(hit.point + rightDir * 0.1f, rightDir, bouncesLeft - 1);
                CastLaser(hit.point + leftDir * 0.1f, leftDir, bouncesLeft - 1);
            }
            else if (hit.collider.CompareTag("Receiver"))
            {
                hit.collider.GetComponent<LaserReceiver>()?.NotifyHit();
            }
        }
        else
        {
            line.positionCount = 2;
            line.SetPosition(0, startPos);
            line.SetPosition(1, startPos + direction * maxDistance);
        }
    }
    LineRenderer GetOrCreateLine()
    {
        if (activeLines < laserPool.Count)
        {
            return laserPool[activeLines++];
        }

        // Create a new LineRenderer if the pool is too small
        GameObject lineObj = new GameObject("LaserLine_" + laserPool.Count);
        lineObj.transform.SetParent(this.transform);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        
        // Setup visual properties
        lr.material = laserMaterial;
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;
        lr.startColor = laserColor;
        lr.endColor = laserColor;
        lr.positionCount = 0;

        laserPool.Add(lr);
        activeLines++;
        return lr;
    }
}