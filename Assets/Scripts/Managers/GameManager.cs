using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonClass<GameManager>
{
    public class HistoryPlayer
    {
        public Color color;
        public List<HistoryEntry> historyEntries = new List<HistoryEntry>();
        public int currentEntryIndex = 0;

    }
    public class HistoryEntry
    {
        public Vector2Int position;
        public bool upgrade;
        public float timeStamp;
        public TurretType turretType;
        public HistoryEntry(Vector2Int position,  float timeStamp, TurretType turretType, bool upgrade = false)
        {
            this.position = position;
            this.upgrade = upgrade;
            this.timeStamp = timeStamp;
            this.turretType = turretType;
        }
    }



    [Header("Active Player UI")]
    [SerializeField] private List<Image> playerProfiles;
    [SerializeField] private List<Image> playerActiveIndicators;

    private int currentPlayerIndex = -1;
    [SerializeField] List<Color> playerColors;
    HistoryPlayer[] historyPlayers = new HistoryPlayer[4];

    public AnimationCurve dmgSFXCurve;
    public float dmgSFXDuration = 0.5f;

    public Color currentPlayerColor;
    public float gameTime = 0f;
    public bool gameRunning = false;

    public int HP = 3;

    public GameObject pointers;

    [Header("UI texts")]
    public TMPro.TextMeshProUGUI moneyText;
    public TMPro.TextMeshProUGUI hpText;
    public TMPro.TextMeshProUGUI nextTurretCostText;
    public TMPro.TextMeshProUGUI nextUpgradeCostText;
    public TMPro.TextMeshProUGUI helpText;
    public TMPro.TextMeshProUGUI flavorText;
    public TMPro.TextMeshProUGUI timerText;
    public TMPro.TextMeshProUGUI timeScaleText;
    public List<string> flavorTexts;

    [Header("Economy")]
    public int Money = 10;
    [HideInInspector]
    public int NextTurretCost = 10;
    public int BaseUpgradeCost = 10;
    public int TurretUpgradeIncrement = 10;
    public int TurretCostIncrement = 10;
    public int _startingMoney = 10;
    public int _startingTurretCost = 10;

    private HashSet<RuntimeGO> runtimeGOs = new HashSet<RuntimeGO>();

    public void AddRuntimeGO(RuntimeGO go)
    {
        if (!gameRunning)
            return;
        runtimeGOs.Add(go);
    }

    public void RemoveRuntimeGO(RuntimeGO go)
    {
        runtimeGOs.Remove(go);
    }

    public HistoryPlayer[] GetHistoryPlayers() => historyPlayers;

    public void CleanRuntimeGO()
    {
        var enumerator = runtimeGOs.GetEnumerator();
        while (enumerator.MoveNext())
        {
            RuntimeGO go = enumerator.Current;
            if (go != null && go.gameObject != null)
            {
                Destroy(go.gameObject);
            }
        }
        runtimeGOs.Clear();
    }

    private void Start()
    {
        CleanRuntimeGO();
        currentPlayerIndex = (currentPlayerIndex+1)% playerColors.Count;
        currentPlayerColor = playerColors[currentPlayerIndex];
        historyPlayers[currentPlayerIndex] = new HistoryPlayer();
        historyPlayers[currentPlayerIndex].color = currentPlayerColor;
        for (int i = 0; i < historyPlayers.Length; i++)
        {
            if (i==currentPlayerIndex || historyPlayers[i] == null)
                continue;
            historyPlayers[i].currentEntryIndex = 0;
        }

        foreach(Image indicator in playerActiveIndicators)
        {
            indicator.gameObject.SetActive(false);
        }
        playerActiveIndicators[currentPlayerIndex].gameObject.SetActive(true);
        playerProfiles[currentPlayerIndex].color = currentPlayerColor;
        gameTime = 0f;

        HP = 3;

        Money = _startingMoney;
        NextTurretCost = _startingTurretCost;
        flavorText.text = flavorTexts[Random.Range(0, flavorTexts.Count)];

    }

    public void ToggleTimeSpeed()
    {
        if(Time.timeScale == 2f)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 2f;
        }
        timeScaleText.text = "Time: " + ((int)Time.timeScale).ToString() + "x";
    }

    private void UpdateTexts()
    {
        hpText.text = "Queen HP: " + HP.ToString();
        moneyText.text = "$$$: " + Money.ToString();
        nextTurretCostText.text = "Next Turret Cost:\n" + NextTurretCost.ToString() + "$";
        if(!gameRunning)
            helpText.text = "Start by choosing a turret and placing it with mouse left click on a valid position.";
        else
            helpText.text = "You can upgrade turrets by holding mouse left click on them.";
        float remainingTime = 5*60 - gameTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        int hundredths = Mathf.FloorToInt((remainingTime - Mathf.Floor(remainingTime)) * 100);
        timerText.text = string.Format("{0:D1}:{1:D2}:{2:D2}", minutes, seconds, hundredths);
    }

    public void UpdateSelectedTurretUpgradeCost(int level)
    {
        string cost = "";
        if (level == -1 || level == 3)
            cost = "---";
        else
            cost = (BaseUpgradeCost + TurretUpgradeIncrement * level).ToString() + "$";
        nextUpgradeCostText.text = "Next Upgrade Cost:\n" + cost;

    }

    private void Update()
    {
        UpdateTexts();
        pointers.SetActive(!gameRunning);
        if (!gameRunning)
            return;
        gameTime += Time.deltaTime;
        PlayHistoryPlayers();
        if(gameTime >= 5 * 60)
        {
            SceneManager.LoadScene("WinScreen");
        }
    }

    public void MarkTurretBuild(Vector2Int pos, TurretType tt)
    {
        HistoryEntry entry = new HistoryEntry(pos, gameTime, tt);
        historyPlayers[currentPlayerIndex].historyEntries.Add(entry);
    }

    public void MarkTurretUpgrade(Vector2Int pos)
    {
        HistoryEntry entry = new HistoryEntry(pos, gameTime, TurretType.Aoe, true);
        historyPlayers[currentPlayerIndex].historyEntries.Add(entry);
    }

    public void TakePlayerDamage()
    {
        HP--;
        if(HP <= 0)
        {
            gameRunning = false;
            Debug.Log("Game Over");
            // Handle game over logic here
            Start();
            GridManager.Instance.Start();
            AudioManager.Instance.PlayLose();
        }
    }

    private void PlayHistoryPlayers()
    {
        for (int i = 0; i < historyPlayers.Length; i++)
        {
            if (i == currentPlayerIndex || historyPlayers[i] == null)
                continue;

            HistoryPlayer player = historyPlayers[i];
            if (player.currentEntryIndex >= player.historyEntries.Count)
                continue;

            HistoryEntry entry = player.historyEntries[player.currentEntryIndex];
            if(gameTime >= entry.timeStamp)
            {
                if(entry.upgrade)
                {
                    GridManager.Instance.UpgradeHistoryTurret(entry.position);
                }
                else
                {
                    GridManager.Instance.PlaceHistoryTurret(entry.turretType, player.color, entry.position);
                }
                player.currentEntryIndex++;
            }
        }
    }

}
