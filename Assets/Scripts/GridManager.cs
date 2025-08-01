using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : SingletonClass<GridManager>
{
    public int width, height;
    public Vector2 offset;

    [SerializeField] List<Vector2Int> routeCoordinates = new List<Vector2Int>();
    private HashSet<Vector2Int> invalidPositions = new HashSet<Vector2Int>();

    const int heightInPixels = 540;

    public GameObject selectedTurret;

    private void Start()
    {
        foreach (Vector2Int coordinate in routeCoordinates)
        {
            invalidPositions.Add(coordinate);
        }
    }

    private void Update()
    {
        UpdatePositionSelectedTurret();
        PlaceSelectedTurret();
    }
    public void UpdatePositionSelectedTurret()
    {
        if(selectedTurret == null)
            return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPosition = new Vector2(
            Mathf.Floor((mousePosition.x - offset.x + 0.5f)) + offset.x,
            Mathf.Floor((mousePosition.y - offset.y + 0.5f)) + offset.y
        );
        // check if out of bounds
        if (gridPosition.x < offset.x || gridPosition.x >= offset.x + width  ||
            gridPosition.y < offset.y || gridPosition.y >= offset.y + height)
        {
            selectedTurret.SetActive(false);
            return;
        }
        selectedTurret.SetActive(true);
        selectedTurret.transform.position = gridPosition;
    }

    public void PlaceSelectedTurret()
    {
        if(selectedTurret == null || !selectedTurret.activeSelf)
            return;
        Vector2 gridPosition = selectedTurret.transform.position;
        // check if left click was pressed
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int position = new Vector2Int((int)(gridPosition.x - offset.x), (int)(gridPosition.y - offset.y));
            if (invalidPositions.Contains(position))
            {
                Debug.Log("Cannot place turret here, position is invalid.");
                //TODO X sound
                return;
            }
            // Place the turret
            GameObject turretInstance = Instantiate(selectedTurret, gridPosition, Quaternion.identity);
            turretInstance.SetActive(true);
            // Add the position to invalid positions
            
            invalidPositions.Add(position);
        }
    }

    private void OnDrawGizmos()
    {
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = invalidPositions.Contains(new Vector2Int(x, y)) ? Color.red : Color.green;
                float gridUnitSize = 1;
                Vector2 position = new Vector2(x * gridUnitSize, y * gridUnitSize) + offset;
                Gizmos.DrawWireCube(position, new Vector2(gridUnitSize, gridUnitSize));
            }
        }
    }
}
