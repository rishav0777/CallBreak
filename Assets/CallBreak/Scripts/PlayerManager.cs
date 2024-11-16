using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerManager : MonoBehaviour
{

    public List<GameObject> spadeCards;
    public List<GameObject> heartCards;
    public List<GameObject> clubCards;
    public List<GameObject> diamondCards;

    public Vector3 myThrowedCardPosition;

    public List<GameObject> myCards;

    public List<List<GameObject>> AllCardLists;

    public int playerId;

    public Text playerBidText;

    public float myBid;
    public float myBidPoint;
    public float myTotalBidPoint;

    void Awake()
    {
        AllCardLists = new List<List<GameObject>>();
    }

    // Use this for initialization
    void Start()
    {
        myBid = myBidPoint = myTotalBidPoint = 0;
    }

    public void SetMyCards(GameObject card)
    {
        switch ((int)card.GetComponent<Card>().cardType)
        {
            case 0:
                spadeCards.Add(card);
                card.GetComponent<Card>().player = this.gameObject;
                break;
            case 1:
                heartCards.Add(card);
                card.GetComponent<Card>().player = this.gameObject;
                break;
            case 2:
                clubCards.Add(card);
                card.GetComponent<Card>().player = this.gameObject;
                break;
            case 3:
                diamondCards.Add(card);
                card.GetComponent<Card>().player = this.gameObject;
                break;
            default:
                break;
        }
    }

    public void SortMyCards()
    {
        spadeCards = spadeCards.OrderBy(x => x.GetComponent<Card>().cardValue).ToList();
        spadeCards.Reverse();

        heartCards = heartCards.OrderBy(x => x.GetComponent<Card>().cardValue).ToList();
        heartCards.Reverse();

        clubCards = clubCards.OrderBy(x => x.GetComponent<Card>().cardValue).ToList();
        clubCards.Reverse();

        diamondCards = diamondCards.OrderBy(x => x.GetComponent<Card>().cardValue).ToList();
        diamondCards.Reverse();

        myCards.AddRange(spadeCards);
        myCards.AddRange(heartCards);
        myCards.AddRange(clubCards);
        myCards.AddRange(diamondCards);

        AllCardLists = new List<List<GameObject>>();
        AllCardLists.Clear();
        AllCardLists.Add(spadeCards);
        AllCardLists.Add(heartCards);
        AllCardLists.Add(clubCards);
        AllCardLists.Add(diamondCards);

       // Invoke("ArrangeCards",2);

    }


    public void PlaceBoatBid()
    {
        for (int i = 0; i < 4; i++)
        {

            if (AllCardLists[i].Count > 0)
            {
                if (AllCardLists[i][0].GetComponent<Card>().cardValue == 14)
                {
                    myBid++;
                }
                if (AllCardLists[i].Count <= 2 && spadeCards.Count > AllCardLists[i].Count)
                {
                    myBid++;
                }

                if (AllCardLists[i].Count == 0)
                {
                    myBid++;
                }

            }
        }

        if (myBid == 0)
            myBid++;

        playerBidText.text = myBidPoint + "/" + myBid;
    }

    public void PlaceUserBid()
    {
        CBGameManager.instance.players[0].GetComponent<PlayerManager>().myBid = int.Parse(CanvasUIScript.instance.userBidPanelBidText.text);

        Debug.Log("user Bid = " + CBGameManager.instance.players[0].GetComponent<PlayerManager>().myBid);
        CBGameManager.instance.players[0].GetComponent<PlayerManager>().playerBidText.text = myBidPoint + "/" + CBGameManager.instance.players[0].GetComponent<PlayerManager>().myBid;
        CanvasUIScript.instance.bidPanel.SetActive(false);
        UIManager.currentlyActivePanel = null;
        CBGameManager.isUserPlacedBid = true;

        CanvasUIScript.instance.userBidPanelBidText.text = "" + 1;
    }
}
