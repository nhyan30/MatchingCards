using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public int cardId;
    public Sprite backImage;
    public Image cardImage;

    private Sprite frontImage;
    private bool isFlipped = false;


    public void Init(int id, Sprite front)
    {
        cardId = id;
        frontImage = front;
        cardImage.sprite = frontImage;
    }

    public void FlipUp()
    {
        isFlipped = true;
        cardImage.sprite = frontImage;
        transform.DORotateQuaternion(Quaternion.Euler(0, 180, 0), 0.5f);
    }

    public void FlipDown()
    {
        isFlipped = false;
        cardImage.sprite = backImage;
        transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);
    }

    public void OnClick()
    {
        if (!isFlipped)
            GameManager.Instance.SelectCard(this);
    }
    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.4f);
    }
}

