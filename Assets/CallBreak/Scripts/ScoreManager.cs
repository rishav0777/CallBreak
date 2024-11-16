using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    public GameObject[] scoreListArray;
    public GameObject[] winnerListArray;
    public Text[] scoreTotalArray;

    public static string PREFS_TOTALGAMES = "prefs_totalGames";
    public static string PREFS_WINGAMES = "prefs_winGames";
    public static string PREFS_HIGHESTBID = "prefs_highestBid";

    void Start()
    {
        instance = this;

        if (!PlayerPrefs.HasKey(PREFS_TOTALGAMES))
        {
            PlayerPrefs.SetInt(PREFS_TOTALGAMES, 0);
        }

        if (!PlayerPrefs.HasKey(PREFS_WINGAMES))
        {
            PlayerPrefs.SetInt(PREFS_WINGAMES, 0);
        }

        if (!PlayerPrefs.HasKey(PREFS_HIGHESTBID))
        {
            PlayerPrefs.SetInt(PREFS_HIGHESTBID, 0);
        }

        UIManager.instance.SetStatisticsInformationDataOnPanel();
    }

    public void IncrementBidPoint(PlayerManager playerObj)
    {
        playerObj.myBidPoint++;
    }

    public void CountPlayerPoints()
    {
        PlayerManager playerObjRef = CBGameManager.instance.players[0].GetComponent<PlayerManager>();
        if (playerObjRef.myBidPoint >= playerObjRef.myBid)
        {
            int highestBidRef = PlayerPrefs.GetInt(PREFS_HIGHESTBID);
            int myBidRef = (int)playerObjRef.myBid;
            if (playerObjRef.myBid > highestBidRef)
            {
                PlayerPrefs.SetInt(PREFS_HIGHESTBID, myBidRef);
                UIManager.instance.SetStatisticsInformationDataOnPanel();
            }
        }

        for (int playerIndex = 0; playerIndex < CBGameManager.instance.players.Count; playerIndex++)
        {

            PlayerManager playerObj = CBGameManager.instance.players[playerIndex].GetComponent<PlayerManager>();

            if (playerObj.myBidPoint < playerObj.myBid)
            {
                playerObj.myBidPoint = -playerObj.myBid;
            }
            else if (playerObj.myBidPoint > playerObj.myBid)
            {

                float tempPointDiff = playerObj.myBidPoint - playerObj.myBid;

                string tempPoint = playerObj.myBid + "." + tempPointDiff;
                playerObj.myBidPoint = float.Parse(tempPoint);
            }
            else
            {
                string tempPoint = playerObj.myBidPoint + "." + 0F;
                playerObj.myBidPoint = float.Parse(tempPoint);
            }

            playerObj.myTotalBidPoint = playerObj.myTotalBidPoint + playerObj.myBidPoint;

            if (CBGameManager.gameRound != 5)
            {
                scoreListArray[CBGameManager.gameRound - 1].SetActive(true);
                scoreListArray[CBGameManager.gameRound - 1].transform.GetChild(playerIndex + 1).GetChild(0).GetComponent<Text>().text = "" + playerObj.myBidPoint;

                scoreTotalArray[playerIndex].text = "" + playerObj.myTotalBidPoint.ToString("F1");
            }

            playerObj.playerBidText.text = "0/0";
        }

    }
}
