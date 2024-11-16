using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CBGameManager : MonoBehaviour
{

    public static CBGameManager instance;

    public List<GameObject> players;

    public static int gameRound;
    public static GameObject currentPlayerTurn;
    public static int currentPlayerIndex;
    public int randomlyChoosedPlayer;
    public static int firstPlayerToThrowCard;
    public static Card.CardType firstPlayerThrowedCardType;

    public static GameObject highThrowedCard;

    public List<GameObject> throwedCardList;

    public static bool isUserPlacedBid, RoundCompleted;

    Vector2 topBottomPlayerCardStartPosition = new Vector2(-1.853f, 0f);
    Vector2 leftRightPlayerCardStartPosition = new Vector2(0f, 1.27f);

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        throwedCardList = new List<GameObject>();
        RoundCompleted = false;
    }

    public void DistributeCardsToPlayer()
    {
        gameRound++;
        randomlyChoosedPlayer = Random.Range(0, 4);
        firstPlayerToThrowCard = randomlyChoosedPlayer;
        currentPlayerIndex = firstPlayerToThrowCard;
        currentPlayerTurn = players[currentPlayerIndex];

        int temp = randomlyChoosedPlayer;

        List<GameObject> copyOfAllCards = new List<GameObject>(CardGenerator.instance.totalCards);

        for (int card = 0; card < 13; card++)
        {
            for (int player = 0; player < 4; player++)
            {

                GameObject RandomCard = copyOfAllCards[Random.Range(0, copyOfAllCards.Count)];
                players[temp].GetComponent<PlayerManager>().SetMyCards(RandomCard);

                copyOfAllCards.Remove(RandomCard);

                temp++;

                if (temp == 4)
                {
                    temp = 0;
                }

            }
        }

        //Sort All Player Card in List
        for (int playerIndex = 0; playerIndex <= 3; playerIndex++)
        {
            players[playerIndex].GetComponent<PlayerManager>().SortMyCards();
        }

        StartCoroutine(SetCardToTable());
    }

    public IEnumerator SetCardToTable()
    {
        Debug.Log("setCard ");
        if (UIManager.isSoundEffectOn)
        {
            UIManager.instance.sound.clip = UIManager.instance.distributeCards;
            UIManager.instance.sound.Play();
        }
        int temp = randomlyChoosedPlayer;
        topBottomPlayerCardStartPosition = new Vector2(-1.853f, 0f);
        leftRightPlayerCardStartPosition = new Vector2(0f, 1.27f);

        Vector2 topBottomPlayerPosition = topBottomPlayerCardStartPosition;
        Vector2 leftRightPlayerPosition = leftRightPlayerCardStartPosition;

        float incrementValue = 0.3f;
        float decrementValue = 0.2f;

        for (int cardIndex = 0; cardIndex <= 12; cardIndex++)
        {

            for (int playerIndex = 0; playerIndex <= 3; playerIndex++)
            {

                Vector2 currentPlayerPosition;

                if (temp % 2 == 0)
                {
                    currentPlayerPosition = topBottomPlayerPosition;
                }
                else
                {
                    currentPlayerPosition = leftRightPlayerPosition;
                }

                players[temp].GetComponent<PlayerManager>().myCards[cardIndex].transform.SetParent(players[temp].transform);
                players[temp].GetComponent<PlayerManager>().myCards[cardIndex].transform.localPosition = currentPlayerPosition;
               // players[temp].GetComponent<PlayerManager>().myCards[cardIndex].transform.localRotation = Quaternion.Euler((-1)*15*cardIndex,0, 0);
                players[temp].GetComponent<PlayerManager>().myCards[cardIndex].GetComponent<SpriteRenderer>().sprite = CardGenerator.instance.backCardSprite;
                
                players[temp].GetComponent<PlayerManager>().myCards[cardIndex].GetComponent<SpriteRenderer>().sortingOrder = cardIndex + 1;

                temp++;
                if (temp == 4)
                {
                    temp = 0;
                }

                yield return new WaitForSeconds(0.01f);
            }

            topBottomPlayerPosition = new Vector2(topBottomPlayerPosition.x + incrementValue, 0f);
            leftRightPlayerPosition = new Vector2(0, leftRightPlayerPosition.y - decrementValue);

        }
        UIManager.instance.sound.Stop();

        StartCoroutine(ShowUserCards());
    }

    public IEnumerator ShowUserCards()
    {
        if (UIManager.isSoundEffectOn)
        {
            UIManager.instance.sound.clip = UIManager.instance.showUserCards;
            UIManager.instance.sound.Play();
        }

        for (int cardIndex = 0; cardIndex <= 12; cardIndex++)
        {
            players[0].GetComponent<PlayerManager>().myCards[cardIndex].GetComponent<SpriteRenderer>().sprite =
                players[0].GetComponent<PlayerManager>().myCards[cardIndex].GetComponent<Card>().cardSprite;

            yield return new WaitForSeconds(0.02f);
        }

        StartCoroutine(StartChoosingBid());
    }


    public IEnumerator StartChoosingBid()
    {
        int temp = randomlyChoosedPlayer;
        for (int player = 0; player < 4; player++)
        {

            if (temp == 0)
            {

                if (UIManager.currentlyActivePanel != null)
                    UIManager.instance.CloseCurrentlyActivedPanel();

                CanvasUIScript.instance.bidPanel.SetActive(true);
                UIManager.currentlyActivePanel = CanvasUIScript.instance.bidPanel;
                CanvasUIScript.instance.bidPanel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().enabled = false;
                yield return new WaitUntil(() => isUserPlacedBid == true);

            }
            else
            {
                players[temp].GetComponent<PlayerManager>().PlaceBoatBid();
            }

            temp++;

            if (temp == 4)
            {
                temp = 0;
            }

            yield return new WaitForSeconds(0.5f);
        }

        StartThrowingCardToTable();

    }

    public void IncrementDecrementUserBid(Button clickedButton)
    {
        if (clickedButton.name == "Button_BidPlus")
        {
            CanvasUIScript.instance.userBidPanelBidText.text = "" + (int.Parse(CanvasUIScript.instance.userBidPanelBidText.text) + 1);
        }
        else
        {

            CanvasUIScript.instance.userBidPanelBidText.text = "" + (int.Parse(CanvasUIScript.instance.userBidPanelBidText.text) - 1);

        }

        //bid plus button enable and disable
        if (CanvasUIScript.instance.userBidPanelBidText.text == "8")
        {
            CanvasUIScript.instance.bidPanel.transform.Find("Panel_BidChild/Panel_Content/Button_BidPlus").GetComponent<Button>().enabled = false;
        }
        else
        {
            CanvasUIScript.instance.bidPanel.transform.Find("Panel_BidChild/Panel_Content/Button_BidPlus").GetComponent<Button>().enabled = true;
        }

        //bid minus button enable and disable.
        if (CanvasUIScript.instance.userBidPanelBidText.text == "1")
        {
            CanvasUIScript.instance.bidPanel.transform.Find("Panel_BidChild/Panel_Content/Button_BidMinus").GetComponent<Button>().enabled = false;
        }
        else
        {
            CanvasUIScript.instance.bidPanel.transform.Find("Panel_BidChild/Panel_Content/Button_BidMinus").GetComponent<Button>().enabled = true;
        }
    }


    public void StartThrowingCardToTable()
    {
        if (firstPlayerToThrowCard == 0)
            UserTurnToThrowCard();
        else
            BoatChooseCardToThrow();
    }

    public void BoatChooseCardToThrow()
    {
        GameObject card;
        if (throwedCardList.Count == 0)
        {
            card = currentPlayerTurn.GetComponent<PlayerManager>().myCards[Random.Range(0, currentPlayerTurn.GetComponent<PlayerManager>().myCards.Count)];
            firstPlayerThrowedCardType = card.GetComponent<Card>().cardType;
            highThrowedCard = card;
        }
        else
        {
            if (currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType].Count > 0)
            {
                if (highThrowedCard.GetComponent<Card>().cardType == firstPlayerThrowedCardType)
                {
                    if (highThrowedCard.GetComponent<Card>().cardValue < currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType][0].GetComponent<Card>().cardValue)
                    {
                        card = currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType][0];
                    }
                    else
                    {
                        card = currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType][currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType].Count - 1];
                    }
                }
                else
                {
                    card = currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType][currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[(int)firstPlayerThrowedCardType].Count - 1];
                }
            }
            else if (currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[0].Count > 0)
            {
                if (highThrowedCard.GetComponent<Card>().cardValue < currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[0][0].GetComponent<Card>().cardValue)
                {
                    card = currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[0][0];
                }
                else
                {
                    card = currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[0][currentPlayerTurn.GetComponent<PlayerManager>().AllCardLists[0].Count - 1];
                }
            }
            else
            {
                card = currentPlayerTurn.GetComponent<PlayerManager>().myCards[Random.Range(0, currentPlayerTurn.GetComponent<PlayerManager>().myCards.Count)];
            }

            CheckIsItHighCard(card);
        }

        StartCoroutine(ThrowCard(card));
    }


    public void UserTurnToThrowCard()
    {
        UIManager.instance.userTurnIndicatorText.SetActive(true);

        LeanTween.value(UIManager.instance.userTurnIndicatorText, 1F, 0.1F, 1f).setOnUpdate((float val) =>
        {

            Color color = UIManager.instance.userTurnIndicatorText.GetComponent<Text>().color;
            color.a = val;
            UIManager.instance.userTurnIndicatorText.GetComponent<Text>().color = color;

        }).setLoopCount(-1);

        PlayerManager userPlayerManagerObj = currentPlayerTurn.GetComponent<PlayerManager>();
        if (firstPlayerToThrowCard == 0)
        {
            EnableOrDisableUserCards(userPlayerManagerObj.myCards, true);
        }
        else
        {
            if (userPlayerManagerObj.AllCardLists[(int)firstPlayerThrowedCardType].Count > 0)
                EnableOrDisableUserCards(userPlayerManagerObj.AllCardLists[(int)firstPlayerThrowedCardType], true);
            else if (userPlayerManagerObj.spadeCards.Count > 0)
                EnableOrDisableUserCards(userPlayerManagerObj.spadeCards, true);
            else
                EnableOrDisableUserCards(userPlayerManagerObj.myCards, true);
        }
    }



    public void EnableOrDisableUserCards(List<GameObject> cardList, bool enableOrDisable)
    {
        for (int cardIndex = 0; cardIndex < cardList.Count; cardIndex++)
        {
            cardList[cardIndex].GetComponent<BoxCollider2D>().enabled = enableOrDisable;
        }

        PlayerManager userObj = currentPlayerTurn.GetComponent<PlayerManager>();

        float colorValue = 0.5F;

        if (!enableOrDisable)
        {
            colorValue = 1F;
        }

        for (int cardIndex = 0; cardIndex < userObj.myCards.Count; cardIndex++)
        {

            if (!cardList.Contains(userObj.myCards[cardIndex]) || enableOrDisable == false)
            {
                Vector2 position = userObj.myCards[cardIndex].transform.localPosition;

                LeanTween.moveLocal(userObj.myCards[cardIndex], new Vector2(position.x, position.y - 0.15F), 0.1F);//0.2F

                userObj.myCards[cardIndex].GetComponent<SpriteRenderer>().color = new Color(colorValue, colorValue, colorValue);

            }
        }
    }

    public IEnumerator ThrowCard(GameObject card)
    {
        if (UIManager.isSoundEffectOn)
        {
            UIManager.instance.sound.clip = UIManager.instance.throwcard;
            UIManager.instance.sound.Play();
        }

        card.GetComponent<SpriteRenderer>().sprite = card.GetComponent<Card>().cardSprite;
        LeanTween.moveLocal(card, currentPlayerTurn.GetComponent<PlayerManager>().myThrowedCardPosition, 0.1F);//0.3F
        throwedCardList.Add(card);

        card.GetComponent<Card>().player.GetComponent<PlayerManager>().myCards.Remove(card);
        card.GetComponent<Card>().player.GetComponent<PlayerManager>().AllCardLists[(int)card.GetComponent<Card>().cardType].Remove(card);

        if (card.GetComponent<Card>().player.GetComponent<PlayerManager>().myCards.Count > 0)
        {
            ReArrangeCard();
            
        }

        yield return new WaitForSeconds(1F);

        ChangeTurn();
    }

    void ReArrangeCard()
    {
        Debug.Log("rearrange ");
        PlayerManager currentPlayerObj = currentPlayerTurn.GetComponent<PlayerManager>();
        Vector2 startPosition;

        if (currentPlayerIndex % 2 == 0)
        {
            
            topBottomPlayerCardStartPosition = new Vector2(topBottomPlayerCardStartPosition.x + 0.15F, topBottomPlayerCardStartPosition.y);
            startPosition = topBottomPlayerCardStartPosition;
        }
        else
        {
            
            leftRightPlayerCardStartPosition = new Vector2(leftRightPlayerCardStartPosition.x, leftRightPlayerCardStartPosition.y - 0.1F);
            startPosition = leftRightPlayerCardStartPosition;

        }



        for (int i = 0; i < currentPlayerObj.myCards.Count; i++)
        {

            if (currentPlayerIndex % 2 == 0 && i != 0)
            {
                
                startPosition = new Vector2(startPosition.x + 0.3F, startPosition.y);
            }
            else if (currentPlayerIndex % 2 != 0 && i != 0)
            {
               
                startPosition = new Vector2(startPosition.x, startPosition.y - 0.2F);
            }

            LeanTween.moveLocal(currentPlayerObj.myCards[i], startPosition, 0.1F);
        }

        if (currentPlayerIndex % 2 == 0)
            topBottomPlayerCardStartPosition = new Vector2(topBottomPlayerCardStartPosition.x - 0.15F, topBottomPlayerCardStartPosition.y);
        else
            leftRightPlayerCardStartPosition = new Vector2(leftRightPlayerCardStartPosition.x, leftRightPlayerCardStartPosition.y + 0.1F);
        
    }

   
   
    public float radius = 1f; 
    public float startAngle = 0f;
    void ArrangeCards()
    {
        PlayerManager currentPlayerObj = currentPlayerTurn.GetComponent<PlayerManager>();
        float angleStep = 180f / currentPlayerObj.myCards.Count;
        float angle = startAngle;

        for (int i = 0; i < currentPlayerObj.myCards.Count; i++)
        {
            float angleInRadians = angle * Mathf.Deg2Rad;
            float xOffset = Mathf.Cos(angleInRadians) * radius;
            float yOffset = Mathf.Sin(angleInRadians) * radius;
            Vector3 cardPosition = new Vector3(0,0,0) + new Vector3(xOffset, yOffset, 0f);

            currentPlayerObj.myCards[i].transform.position = cardPosition;
            angle += angleStep;
        }
    }

    public void CheckIsItHighCard(GameObject card)
    {
        Card cardObj = card.GetComponent<Card>();

        Card highCardObj = highThrowedCard.GetComponent<Card>();

        if (highCardObj.cardType == Card.CardType.Spade && cardObj.cardType == Card.CardType.Spade && cardObj.cardValue > highCardObj.cardValue)
        {
            highThrowedCard = card;
        }
        else if (highCardObj.cardType != Card.CardType.Spade && cardObj.cardType == highCardObj.cardType && cardObj.cardValue > highCardObj.cardValue)
        {
            highThrowedCard = card;
        }
        else if (highCardObj.cardType != Card.CardType.Spade && cardObj.cardType == Card.CardType.Spade)
        {
            highThrowedCard = card;
        }
    }

    public void ChangeTurn()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex == 4)
            currentPlayerIndex = 0;

        currentPlayerTurn = players[currentPlayerIndex];

        if (throwedCardList.Count == 4)
        {
            StartCoroutine(GiveHandsToPlayer());
        }
        else
        {
            if (currentPlayerTurn == players[0])
            {
                UserTurnToThrowCard();
            }
            else
            {
                BoatChooseCardToThrow();
            }

        }
    }

    public IEnumerator GiveHandsToPlayer()
    {

        yield return new WaitForSeconds(0.5F);
        Card highCardObj = highThrowedCard.GetComponent<Card>();

        topBottomPlayerCardStartPosition = new Vector2(topBottomPlayerCardStartPosition.x + 0.15F, topBottomPlayerCardStartPosition.y);
        leftRightPlayerCardStartPosition = new Vector2(leftRightPlayerCardStartPosition.x, leftRightPlayerCardStartPosition.y - 0.1F);

        if (UIManager.isSoundEffectOn)
        {
            UIManager.instance.sound.clip = UIManager.instance.handsToPlayer;
            UIManager.instance.sound.Play();
        }

        ScoreManager.instance.IncrementBidPoint(highCardObj.player.GetComponent<PlayerManager>());

        for (int i = 0; i < throwedCardList.Count; i++)
        {
            GameObject card = throwedCardList[i];

            LeanTween.move(card, highCardObj.player.transform.position, 0.5F).setOnComplete(() =>
            {
                card.GetComponent<SpriteRenderer>().sprite = null;
                card.transform.SetParent(GameObject.Find("AllCards").transform);

                highCardObj.player.GetComponent<PlayerManager>().playerBidText.text = highCardObj.player.GetComponent<PlayerManager>().myBidPoint + "/" + highCardObj.player.GetComponent<PlayerManager>().myBid;

            });
        }

        if (highCardObj.player.GetComponent<PlayerManager>().myCards.Count == 0)
        {
            yield return new WaitForSeconds(0.6F);

            if (gameRound != 5)
            {
                RoundCompleted = true;
                UIManager.instance.ShowUserScorePanel();
            }

            ScoreManager.instance.CountPlayerPoints();

            if (gameRound == 5)
            {
                UIManager.instance.ShowWinnerPanel();
            }

        }
        else
        {
            ResetLocally();
        }
    }



    void ResetLocally()
    {
        firstPlayerToThrowCard = highThrowedCard.GetComponent<Card>().player.GetComponent<PlayerManager>().playerId;

        currentPlayerIndex = firstPlayerToThrowCard;
        currentPlayerTurn = players[currentPlayerIndex];
        throwedCardList.Clear();

        highThrowedCard = null;
        Invoke("StartThrowingCardToTable", 1F);

    }

    public void ResetEverything()
    {
        topBottomPlayerCardStartPosition = new Vector2(-1.853f, 0f);
        leftRightPlayerCardStartPosition = new Vector2(0f, 1.27f);

        gameRound = 0;
        randomlyChoosedPlayer = 0;
        firstPlayerToThrowCard = 0;
        isUserPlacedBid = false;
        RoundCompleted = false;
        currentPlayerTurn = null;
        highThrowedCard = null;
        throwedCardList.Clear();

        Destroy(GameObject.FindWithTag("CardGeneratorParent").transform.GetChild(0).gameObject);
        GameObject AllCard = new GameObject("AllCards");
        AllCard.transform.SetParent(GameObject.FindWithTag("CardGeneratorParent").transform);

        for (int i = 0; i < 5; i++)
        {
            if (i != 0)
            {
                ScoreManager.instance.scoreListArray[i].SetActive(false);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            ScoreManager.instance.scoreListArray[0].transform.GetChild(i + 1).GetChild(0).GetComponent<Text>().text = "0";
            ScoreManager.instance.scoreTotalArray[i].text = "0";
        }
    }
}
