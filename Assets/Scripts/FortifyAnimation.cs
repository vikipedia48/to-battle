using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortifyAnimation : MonoBehaviour
{

    public static float AnimationTime = 0.8f;
    public Grid gameLogic;
    public Color spriteColor;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<Grid>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
    }

    public IEnumerator PikeAnimation(bool spawn)
    {
        if (spawn) gameObject.SetActive(false);
        while (gameLogic.animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameLogic.animationPlaying = true;
        gameObject.SetActive(true);
        var position = transform.position;
        gameLogic.MainCamera.transform.position =
            new Vector3(position.x, position.y, -10);
        var elapsedTime = 0f;
        while (elapsedTime < AnimationTime)
        {
            spriteColor.a = spawn ? elapsedTime / AnimationTime : (1-elapsedTime/AnimationTime);
            spriteRenderer.color = spriteColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        gameLogic.animationPlaying = false;
        //Debug.Log(Convert.ToString(spawn));
        if (!spawn) Destroy(this.gameObject);
    }
}
