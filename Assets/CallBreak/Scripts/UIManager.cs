using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public GameObject gameTableParent;
    public GameObject mainMenuParent;
    public GameObject gameManager;

    public GameObject scorePanel;
    public GameObject winnerPanel;
    public GameObject userTurnIndicatorText;

    public AudioSource sound;

    public AudioClip throwcard;
    public AudioClip distributeCards;
    public AudioClip showUserCards;
    public AudioClip handsToPlayer;

    public static GameObject currentlyActivePanel;
    public static bool isSoundEffectOn, isSoundMusicOn, isRestartGame;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Debug.Log("UIManager Start");
        isSoundEffectOn = true;
    }


    public void ShowUserScorePanel()
    {
        Debug.Log("ShowUserScorePanel");
        if (currentlyActivePanel != null)
            CloseCurrentlyActivedPanel();

        scorePanel.SetActive(true);
        currentlyActivePanel = scorePanel;
        GoogleAdMobController.Instance.ShowInterstitialAd();
    }

    public void ShowWinnerPanel()
    {
        if (currentlyActivePanel != null)
            CloseCurrentlyActivedPanel();

        winnerPanel.SetActive(true);
        currentlyActivePanel = winnerPanel;
        List<GameObject> winnerList = CBGameManager.instance.players;

        winnerList = winnerList.OrderBy(x => x.GetComponent<PlayerManager>().myTotalBidPoint).ToList();
        winnerList.Reverse();

        //Set total games prefs for statistics information
        int totalGamesRef = PlayerPrefs.GetInt(ScoreManager.PREFS_TOTALGAMES);
        totalGamesRef++;
        PlayerPrefs.SetInt(ScoreManager.PREFS_TOTALGAMES, totalGamesRef);

        //Set win games prefs for statistics information
        int winGamesRef = PlayerPrefs.GetInt(ScoreManager.PREFS_WINGAMES);
        if (winnerList[0].GetComponent<PlayerManager>().playerId == 0)
        {
            winGamesRef++;
            PlayerPrefs.SetInt(ScoreManager.PREFS_WINGAMES, winGamesRef);

        }



        for (int i = 0; i < 4; i++)
        {
            string name = "";
            if (winnerList[i].GetComponent<PlayerManager>().playerId == 0)
            {
                name = "You";
            }
            else
            {

                if (winnerList[i].GetComponent<PlayerManager>().playerId == 1)
                {
                    name = "Right";
                }
                else if (winnerList[i].GetComponent<PlayerManager>().playerId == 2)
                {
                    name = "Top";
                }
                else if (winnerList[i].GetComponent<PlayerManager>().playerId == 3)
                {
                    name = "Left";
                }

            }

            ScoreManager.instance.winnerListArray[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = name;
            ScoreManager.instance.winnerListArray[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = winnerList[i].GetComponent<PlayerManager>().myTotalBidPoint.ToString("F1");

        }

        SetStatisticsInformationDataOnPanel();

        GoogleAdMobController.Instance.ShowInterstitialAd();
    }

    public void SetStatisticsInformationDataOnPanel()
    {
        Debug.Log("Into SetStatisticsInformationDataOnPanel");

        CanvasUIScript.instance.statisticsInformationPanel.transform.Find("Panel_InformationListChild/Panel_Content/Image_TotalGames/Text").GetComponent<Text>().text = "" + PlayerPrefs.GetInt(ScoreManager.PREFS_TOTALGAMES);
        CanvasUIScript.instance.statisticsInformationPanel.transform.Find("Panel_InformationListChild/Panel_Content/Image_WinGames/Text").GetComponent<Text>().text = "" + PlayerPrefs.GetInt(ScoreManager.PREFS_WINGAMES);
        CanvasUIScript.instance.statisticsInformationPanel.transform.Find("Panel_InformationListChild/Panel_Content/Image_HighestBid/Text").GetComponent<Text>().text = "" + PlayerPrefs.GetInt(ScoreManager.PREFS_HIGHESTBID);

    }

    public void CloseUserScorePanel()
    {
        if (CBGameManager.RoundCompleted)
        {
            StartOtherRound();
        }
        else
        {
            CloseCurrentlyActivedPanel();
        }
        scorePanel.SetActive(false);


    }

    public void StartOtherRound()
    {
        CBGameManager.isUserPlacedBid = false;
        CBGameManager.RoundCompleted = false;
        CBGameManager.currentPlayerTurn = null;
        CBGameManager.highThrowedCard = null;
        CBGameManager.instance.throwedCardList.Clear();

        for (int i = 0; i < 4; i++)
        {
            CBGameManager.instance.players[i].GetComponent<PlayerManager>().myBid = 0;
            CBGameManager.instance.players[i].GetComponent<PlayerManager>().myBidPoint = 0;
        }

        CBGameManager.instance.DistributeCardsToPlayer();
    }


    public void CloseCurrentlyActivedPanel()
    {
        Debug.Log("CloseCurrentlyActivePanel Called");

        if (currentlyActivePanel != null)
        {
            currentlyActivePanel.SetActive(false);
            currentlyActivePanel = null;

            if (isRestartGame)
                isRestartGame = false;
        }
    }

    public void EscapeConfirm()
    {
        if (gameTableParent != null && gameTableParent.activeSelf)
        {
            CBGameManager.instance.ResetEverything();
            CBGameManager.instance.players.Clear();
            Destroy(gameTableParent);
            Destroy(gameManager);

            if (!isRestartGame)
            {
                mainMenuParent.SetActive(true);
                CanvasUIScript.instance.GameUIPanel.SetActive(false);
                GoogleAdMobController.Instance.ShowInterstitialAd();
            }

            if (isRestartGame)
            {
                StartCoroutine(RestartGameAndPlayAgain());
            }


        }
        else if (mainMenuParent.activeSelf)
        {
            QuitGame();
        }

        CloseCurrentlyActivedPanel();

    }

    public void SoundEffectOnOff()
    {
        isSoundEffectOn = !isSoundEffectOn;

    }

    public void SoundMusicOnOff()
    {
        isSoundMusicOn = !isSoundMusicOn;
    }

    public void SetVolume(float value)
    {
        sound.volume = value / 100F;
    }

    public void RestartGame()
    {
        if (currentlyActivePanel == null)
        {
            isRestartGame = true;
            CanvasUIScript.instance.escapePanel.SetActive(true);
            UIManager.currentlyActivePanel = CanvasUIScript.instance.escapePanel;
        }
    }

    public IEnumerator RestartGameAndPlayAgain()
    {
        yield return new WaitForSeconds(0F);
        CanvasUIScript.instance.PlayGame();
        isRestartGame = false;
    }

    public void RestartGameAftareGameComplete()
    {
        isRestartGame = true;
        EscapeConfirm();
        CloseCurrentlyActivedPanel();
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame Called");
        Application.Quit();
    }
}
