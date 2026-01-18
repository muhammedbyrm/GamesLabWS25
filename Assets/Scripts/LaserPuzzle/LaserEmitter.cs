using UnityEngine;
using System.Collections.Generic;

public class LaserEmitter : MonoBehaviour
{
    [Header("Laser Settings")]
    public bool isEnabled = false; 
    public int maxBounces = 10;
    public float maxDistance = 100f;
    public Material laserMaterial;
    public float laserWidth = 0.02f;
    public Color laserColor = Color.red;

    private List<LineRenderer> laserPool = new List<LineRenderer>();
    private int activeLines = 0;

    void Update()
    {
        activeLines = 0;
        
        if (!isEnabled)
        {
            foreach (var line in laserPool) line.enabled = false;
            return;
        }

        // Start the laser slightly in front of the emitter to avoid self-collision
        CastLaser(transform.position + (transform.forward * 0.01f), transform.forward, maxBounces);

        // Clean up unused lines in the pool
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
        
        Ray ray = new Ray(startPos, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            line.SetPosition(0, startPos);
            line.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                // Reflection: Mirrors use their BLUE axis (Forward) for the bounce
                CastLaser(hit.point, hit.collider.transform.forward, bouncesLeft - 1);
            }
            else if (hit.collider.CompareTag("Splitter"))
            {
                // Splitting: Splitters use their RED axis (Right/-Right)
                CastLaser(hit.point, hit.collider.transform.right, bouncesLeft - 1);
                CastLaser(hit.point, -hit.collider.transform.right, bouncesLeft - 1);
            }
            else if (hit.collider.CompareTag("Receiver"))
            {
                hit.collider.GetComponent<LaserReceiver>()?.NotifyHit();
            }
        }
        else
        {
            line.SetPosition(0, startPos);
            line.SetPosition(1, startPos + direction * maxDistance);
        }
    }

    LineRenderer GetOrCreateLine()
    {
        if (activeLines < laserPool.Count)
        {
            LineRenderer existingLine = laserPool[activeLines++];
            existingLine.positionCount = 2; // Ensure it has points
            return existingLine;
        }
        
        GameObject lineObj = new GameObject("LaserLine_" + laserPool.Count);
        lineObj.transform.SetParent(this.transform);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        
        lr.material = laserMaterial;
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;
        lr.startColor = laserColor;
        lr.endColor = laserColor;
        lr.positionCount = 2;
        
        laserPool.Add(lr);
        activeLines++;
        return lr;
    }
}