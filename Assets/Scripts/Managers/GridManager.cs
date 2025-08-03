using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : SingletonClass<GridManager>
{
    public int width, height;
    public Vector2 offset;

    [SerializeField] List<Vector2Int> routeCoordinates = new List<Vector2Int>();
    private HashSet<Vector2Int> invalidPositions = new HashSet<Vector2Int>();

    private Dictionary<Vector2Int,GameObject> placedTurrets = new Dictionary<Vector2Int, GameObject>();

    const int heightInPixels = 540;

    public GameObject selectedTurret;
    private SpriteRenderer gridSelectionObjectSR;
    public GameObject gridSelectionObject;

    public GameObject locktilePrefab;

    public GameObject LoadUpgradePopup;
    public Vector2Int upgradePos;
    public float upgradeTime;


    public Color placableTurretColor, notPlacableTurretColor;

    public void Start()
    {
        invalidPositions.Clear();
        placedTurrets.Clear();
        LoadHistoryReservations();
        foreach (Vector2Int coordinate in routeCoordinates)
        {
            invalidPositions.Add(coordinate);
        }
        gridSelectionObjectSR = gridSelectionObject.gameObject.GetComponent<SpriteRenderer>();
    }

    private void LoadHistoryReservations()
    {
        foreach (var historyPlayer in GameManager.Instance.GetHistoryPlayers())
        {
            if (historyPlayer == null || historyPlayer.currentEntryIndex < 0)
                continue;
            foreach (var entry in historyPlayer.historyEntries)
                if (!entry.upgrade)
                {
                    PlaceReservetion(entry, historyPlayer.color);
                }
        }
    }

    public Vector2Int GetMouseOnGrid()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int(width > 0 ? Mathf.FloorToInt(mousePosition.x - offset.x + 0.5f) : 0,
                               height > 0 ? Mathf.FloorToInt(mousePosition.y - offset.y + 0.5f) : 0);

    }

    private void Update()
    {
        ProcessUpgrade();
        UpdatePositionSelectedTurret();
        UpdateColorSelectedTurret();
        PlaceSelectedTurret();
        UpdateHoverUpgradeSelection();
    }

    private void UpdateHoverUpgradeSelection()
    {
        // get cursor square
        Vector2Int cursorPos = GetMouseOnGrid();

        // check if there is a turret at the cursorPos of current player color
        GameObject turret;
        if (placedTurrets.TryGetValue(cursorPos, out turret) && turret != null)
        {
            TurretScript turretScript;
            if (turret.TryGetComponent<TurretScript>(out turretScript) && turretScript != null &&
                turretScript.GetComponent<SpriteRenderer>().color == GameManager.Instance.currentPlayerColor)
            {
                GameManager.Instance.UpdateSelectedTurretUpgradeCost(turretScript.level);
                return;
            }
        }
        GameManager.Instance.UpdateSelectedTurretUpgradeCost(-1);

    }

    private void ProcessUpgrade()
    {
        if(GameManager.Instance.gameRunning == false)
        {
            LoadUpgradePopup.SetActive(false);
            return;
        }
        if (upgradePos.x == -1 && !Input.GetMouseButtonDown(0))
        {
            LoadUpgradePopup.SetActive(false);
            return;
        }
        if (upgradePos.x == -1 && Input.GetMouseButtonDown(0))
        {
            //start upgrading
            //check if a turret is selected
            Vector2Int cursorPos = GetMouseOnGrid();
            GameObject turret;
            if (!placedTurrets.TryGetValue(cursorPos, out turret))
                return;
            if (turret == null)
                return;
            TurretScript turretScript;
            if(!turret.TryGetComponent<TurretScript>(out turretScript))
                return;
            if (turretScript == null)
                return;
            if (turretScript.GetComponent<SpriteRenderer>().color != GameManager.Instance.currentPlayerColor)
                return;
            if (turretScript.level >= 3)
                return; // Cannot upgrade beyond level 3

            //check money
            if (GameManager.Instance.Money < GameManager.Instance.BaseUpgradeCost + turretScript.level*GameManager.Instance.TurretUpgradeIncrement)
            {
                Debug.Log("Not enough money to upgrade turret.");
                AudioManager.Instance.PlayNotAllowed();
                return;
            }

            upgradePos = cursorPos;
            LoadUpgradePopup.SetActive(true);
            upgradeTime = 0f;
            //put at mouse position
            Vector3 poss = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            poss.z = 0f; // Set z to 0 to avoid depth issues
            LoadUpgradePopup.transform.position = poss;
            return;
        }
        Vector2Int currentPos = GetMouseOnGrid();
        if (Input.GetMouseButtonUp(0) || upgradePos != currentPos)
        {
            upgradePos = new Vector2Int(-1, -1);
            LoadUpgradePopup.SetActive(false);
            return;
        }
        
        if (upgradePos.x == -1)
            return;

        upgradeTime += Time.deltaTime;
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f; // Set z to 0 to avoid depth issues
        LoadUpgradePopup.transform.position = pos;
        if (upgradeTime >= 1f)
        {
            // Upgrade turret
            UpgradeTurret(upgradePos);
            LoadUpgradePopup.SetActive(false);
            upgradePos = new Vector2Int(-1, -1);
            upgradeTime = 0f;
            
        }
    }

    private void UpdateColorSelectedTurret()
    {
        Vector2Int gridPosition = GetMouseOnGrid();
        bool validPos = !invalidPositions.Contains(gridPosition) && gridSelectionObject.activeSelf;
        bool validMoney = GameManager.Instance.Money >= GameManager.Instance.NextTurretCost;
        if(gridSelectionObjectSR == null)
            gridSelectionObjectSR = gridSelectionObject.GetComponent<SpriteRenderer>();

        gridSelectionObjectSR.color = validPos && validMoney ? placableTurretColor : notPlacableTurretColor;


    }

    public void UpdatePositionSelectedTurret()
    {
        if(gridSelectionObject == null || selectedTurret == null)
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
            gridSelectionObject.SetActive(false);
            return;
        }
        gridSelectionObject.SetActive(true);
        gridSelectionObject.transform.position = gridPosition;
    }

    public void SelectTurret(GameObject turret)
    {
        selectedTurret = turret;
        if(turret == null)
            return;

        gridSelectionObject.GetComponent<SpriteRenderer>().sprite = turret.GetComponent<SpriteRenderer>().sprite;


    }

    public void PlaceHistoryTurret(TurretType type, Color color, Vector2Int position)
    {
        GameObject prefab = CollectionManager.Instance.GetTurretPrefab(type);

        GameObject instance = Instantiate(prefab, new Vector2(position.x + offset.x, position.y + offset.y), Quaternion.identity);
        instance.GetComponent<SpriteRenderer>().color = color;
        Destroy(placedTurrets[position].gameObject);
        placedTurrets[position] = instance;

        AudioManager.Instance.PlayPlaceDown();


    }

    public void UpgradeHistoryTurret(Vector2Int posiition)
    {
        Debug.Log($"Upgrade turret at {posiition} from History");
        GameObject turret;
        if (!placedTurrets.TryGetValue(posiition, out turret) || turret == null)
        {
            Debug.LogError($"No turret found at position {posiition}");
            return;
        }

        TurretScript turretScript = turret.GetComponent<TurretScript>();
        turretScript.UpgradeLevel();

        AudioManager.Instance.PlayUpgrade();
    }

    public void PlaceReservetion(GameManager.HistoryEntry entry, Color color)
    {
        var l = Instantiate(locktilePrefab, new Vector2(entry.position.x + offset.x, entry.position.y + offset.y), Quaternion.identity);
        l.GetComponent<SpriteRenderer>().color = color;
        placedTurrets[entry.position] = l;
        invalidPositions.Add(entry.position);
    }

    public void PlaceSelectedTurret()
    {
        if(selectedTurret == null || !gridSelectionObject.activeSelf)
            return;
        // check if left click was pressed
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int position = GetMouseOnGrid();
            if (invalidPositions.Contains(position))
            {
                Debug.Log("Cannot place turret here, position is invalid.");
                AudioManager.Instance.PlayNotAllowed();
                return;
            }
            if (GameManager.Instance.Money < GameManager.Instance.NextTurretCost)
            {
                Debug.Log("Not enough money to place turret.");
                AudioManager.Instance.PlayNotAllowed();
                return;
            }
            if (GameManager.Instance.gameRunning == false)
                GameManager.Instance.gameRunning = true;
            // Place the turret

            Vector3 gridPos = gridSelectionObject.transform.position;
            GameObject turretInstance = Instantiate(selectedTurret, gridPos, Quaternion.identity);
            turretInstance.SetActive(true);
            turretInstance.GetComponent<SpriteRenderer>().color = GameManager.Instance.currentPlayerColor;
            // Add the position to invalid positions
            
            invalidPositions.Add(position);
            GameManager.Instance.MarkTurretBuild(position, turretInstance.GetComponent<TurretScript>().turretType);
            placedTurrets[position] = turretInstance;

            GameManager.Instance.Money -= GameManager.Instance.NextTurretCost;
            GameManager.Instance.NextTurretCost += GameManager.Instance.TurretCostIncrement;

            AudioManager.Instance.PlayPlaceDown();
            selectedTurret = null;
            gridSelectionObject.SetActive(false);
        }
    }

    public void UpgradeTurret(Vector2Int at)
    {
        Debug.Log($"Upgrade turret at {at}");
        GameObject turret;
        if (!placedTurrets.TryGetValue(at, out turret) || turret == null)
        {
            Debug.LogError($"No turret found at position {at}");
            return;
        }

        TurretScript turretScript = turret.GetComponent<TurretScript>();
        GameManager.Instance.MarkTurretUpgrade(at);

        GameManager.Instance.Money -= GameManager.Instance.BaseUpgradeCost + turretScript.level * GameManager.Instance.TurretUpgradeIncrement;
        turretScript.UpgradeLevel();

        AudioManager.Instance.PlayUpgrade();

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
