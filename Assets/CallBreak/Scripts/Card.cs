using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    public enum CardType
    {
        Spade = 0,
        Heart = 1,
        Club = 2,
        Diamond = 3
    }

    public CardType cardType;
    public int cardValue;
    public Sprite cardSprite;

    public GameObject player;

    void OnMouseUp()
    {
        CBGameManager.instance.EnableOrDisableUserCards(player.GetComponent<PlayerManager>().myCards, false);

        if (CBGameManager.firstPlayerToThrowCard == 0)
        {
            CBGameManager.firstPlayerThrowedCardType = this.cardType;
            CBGameManager.highThrowedCard = this.gameObject;
        }
        else
        {
            CBGameManager.instance.CheckIsItHighCard(this.gameObject);
        }

        StartCoroutine(CBGameManager.instance.ThrowCard(this.gameObject));

        LeanTween.cancel(UIManager.instance.userTurnIndicatorText);
        UIManager.instance.userTurnIndicatorText.GetComponent<Text>().color = new Color(0F, 0F, 0F, 1F);
        UIManager.instance.userTurnIndicatorText.SetActive(false);
    }
}
