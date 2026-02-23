using System;
using UnityEngine;

public class OpponentUI : DuelistUI {

    public void DrawCreatureCard(CreatureCard card) {
        CreatureHandCardUI cardUI = Instantiate(creatureCard, handOrigin);
        cardUI.Init(card);
        cardUI.transform.Rotate(-90f, 0, 0);
        cardsInHand.Add(cardUI);
        SpaceCards();
    }

    public void DrawSpellCard(SpellCard card) {
        SpellHandCardUI cardUI = Instantiate(spellCard, handOrigin);
        cardUI.Init(card);
        cardUI.transform.Rotate(-90f, 0, 0);
        cardsInHand.Add(cardUI);
        SpaceCards();
    }

    public void SetManaCount(int value) {
        manaCount.text = value.ToString();
    }

    private void SpaceCards() {
        float cardSpacing = 0.34f;
        float cardVerticalOffset = 0.005f;
        float cardRotation = -1f;

        int cardCount = cardsInHand.Count;
        float handOffsetX = (cardCount - 1) * cardSpacing / 2;
        float centerCardPoint = (cardCount - 1) / 2f;
        float handRotation = (cardCount - 1) * cardRotation / 2;
        for (int i = 0; i < cardCount; i++) {
            cardsInHand[i].transform.position = handOrigin.position;
            float xOffset = (i * cardSpacing - handOffsetX);
            float zOffset = (float)Math.Floor(Math.Abs(i - centerCardPoint)) * cardVerticalOffset * 3f;
            Vector3 cardPosition = new Vector3(xOffset, i * 0.005f, zOffset);
            cardsInHand[i].transform.Translate(cardPosition, Space.World);
            cardsInHand[i].transform.eulerAngles = new Vector3(cardsInHand[i].transform.eulerAngles.x, 0f, cardsInHand[i].transform.eulerAngles.z);
            cardsInHand[i].transform.Rotate(new Vector3(0, i * cardRotation - handRotation, 0), Space.World);
        }
    }
}
