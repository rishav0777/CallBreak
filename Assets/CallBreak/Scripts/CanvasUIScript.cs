using UnityEngine;
using UnityEngine.UI;

public class CanvasUIScript : MonoBehaviour
{

    public static CanvasUIScript instance;

    public GameObject mainMenuPanel, settingPanel, escapePanel, bidPanel, scorePanel, winPanel, GameUIPanel;
    public GameObject statisticsInformationPanel;

    public GameObject gameParentPrefab, gameManager;

    public Text userBidPanelBidText;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (UIManager.currentlyActivePanel != null && UIManager.currentlyActivePanel != mainMenuPanel &&
                UIManager.currentlyActivePanel != bidPanel &&
                UIManager.currentlyActivePanel != winPanel &&
                UIManager.currentlyActivePanel != scorePanel)
            {
                UIManager.instance.CloseCurrentlyActivedPanel();
            }
            else if (!bidPanel.activeSelf && !scorePanel.activeSelf && UIManager.currentlyActivePanel == null)
            {
                escapePanel.SetActive(true);
                UIManager.currentlyActivePanel = escapePanel;
            }

        }
    }

    public void PlayGame()
    {
        mainMenuPanel.SetActive(false);
        GameUIPanel.SetActive(true);

        CardGenerator.instance.GenerateAllCards();

        GameObject gameTableParent = Instantiate(gameParentPrefab);
        UIManager.instance.gameTableParent = gameTableParent;

        AdaptScreenResolution();

        GameObject gameManagerObj = Instantiate(gameManager);
        UIManager.instance.gameManager = gameManagerObj;
        for (int i = 0; i < 4; i++)
        {
            CBGameManager.instance.players.Add(gameTableParent.transform.GetChild(1).GetChild(i).gameObject);
        }

        UIManager.instance.userTurnIndicatorText = gameTableParent.transform.Find("Canvas_Table/Text_UserTurnIndicator").gameObject;

        CBGameManager.instance.DistributeCardsToPlayer();
    }

    private void AdaptScreenResolution()
    {
        float refHeight = 854F;
        float refWidth = 480F;
        float refRatio = refWidth / refHeight;
        float refScale = 1F;

        float currentRatio = (float)Screen.width / (float)Screen.height;

        float currentScale = (currentRatio * refScale) / refRatio;

        if (UIManager.instance.gameTableParent.transform.localScale.x != currentScale)
        {
            UIManager.instance.gameTableParent.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
    }

    public void OpenStatisticsInformationPanel()
    {
        if (UIManager.currentlyActivePanel == null)
        {
            statisticsInformationPanel.SetActive(true);
            UIManager.currentlyActivePanel = statisticsInformationPanel;
        }
    }


    public void GameSetting()
    {
        if (UIManager.currentlyActivePanel == null)
        {
            settingPanel.SetActive(true);
            UIManager.currentlyActivePanel = settingPanel;
        }
    }

    public void EscapeGame()
    {
        if (UIManager.currentlyActivePanel == null)
        {
            escapePanel.SetActive(true);
            UIManager.currentlyActivePanel = escapePanel;
        }
    }

}
