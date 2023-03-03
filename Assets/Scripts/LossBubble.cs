using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class LossBubble : MonoBehaviour
{
    public Grid gameLogic;
    private Vector3 destination;
    public static float AnimationTime = 1.5f;
    public static float AnimationFirstPeriod = 0.5f;
    public static float AnimationDistanceY = 1;
    public SpriteRenderer bubbleSprite;
    public Color bubbleSpriteColor;
    public TextMeshPro lossNumberText;
    public Color lossNumberTextColor;

    private void Awake()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<Grid>();
        bubbleSprite = gameObject.GetComponent<SpriteRenderer>();
        lossNumberText = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
    }

    public void SetColor(Color color)
    {
        bubbleSpriteColor = color;
        bubbleSprite.color = bubbleSpriteColor;
        lossNumberTextColor = (color.r + color.g + color.b > 2.7f) ? Color.black : Color.white;
        lossNumberText.color = lossNumberTextColor;
    }

    public void SetText(string text)
    {
        lossNumberText.text = text;
    }

    public IEnumerator DoAnimation()
    {
        gameObject.SetActive(false);
        while (gameLogic.animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameLogic.animationPlaying = true;
        gameObject.SetActive(true);
        var startingPos = transform.position;
        destination = new Vector3(startingPos.x,startingPos.y+AnimationDistanceY);
        var elapsedTime = 0f;
        while (elapsedTime < AnimationFirstPeriod)
        {
            transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / AnimationTime));
            bubbleSpriteColor.a = (float)Math.Min(1, Math.Pow(AnimationTime, 2) - Math.Pow(elapsedTime,2));
            lossNumberTextColor.a = bubbleSpriteColor.a;
            bubbleSprite.color = bubbleSpriteColor;
            lossNumberText.color = lossNumberTextColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameLogic.animationPlaying = false;
        while (elapsedTime < AnimationTime)
        {
            transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / AnimationTime));
            bubbleSpriteColor.a = (float)Math.Min(1, Math.Pow(AnimationTime, 2) - Math.Pow(elapsedTime,2));
            lossNumberTextColor.a = bubbleSpriteColor.a;
            bubbleSprite.color = bubbleSpriteColor;
            lossNumberText.color = lossNumberTextColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
