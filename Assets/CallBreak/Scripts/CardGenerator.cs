using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{

    public static CardGenerator instance;

    public GameObject cardPrefab;
    public Sprite backCardSprite;
    public List<Sprite> allCardSprites;
    public List<GameObject> totalCards;

    void Awake()
    {
        instance = this;
    }

    public void GenerateAllCards()
    {
        totalCards.Clear();
        int indexValue = 0;
        for (int type = 0; type < 4; type++)
        {

            for (int value = 2; value <= 14; value++)
            {

                GameObject cardObject = Instantiate(cardPrefab, this.transform.GetChild(0).transform);
                //Debug.Log ("CardObject = " + cardObject.name +" -- Parent -- "+cardObject.transform.parent.name);
                cardObject.name = allCardSprites[indexValue].name;

                cardObject.GetComponent<Card>().cardType = (Card.CardType)type;
                cardObject.GetComponent<Card>().cardValue = value;
                cardObject.GetComponent<Card>().cardSprite = allCardSprites[indexValue];

                totalCards.Add(cardObject);

                indexValue++;
            }
        }
    }
}
