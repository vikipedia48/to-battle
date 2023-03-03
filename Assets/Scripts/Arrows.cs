using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Arrows : MonoBehaviour
{
    public Vector3 destination;
    public Point pos1, pos2;
    public SpriteRenderer spriteRenderer;
    public Grid gameLogic;
    public static float AnimationTime = 1f;

    private void Awake()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<Grid>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public IEnumerator FireAtPoint()
    {
        gameObject.SetActive(false);
        while (gameLogic.animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameLogic.animationPlaying = true;
        gameObject.SetActive(true);
        destination = Grid.GetPositionFromTile(pos2);
        /*{
            Vector3 targ = new Vector3(destination.x, destination.y, 0);
            var position = transform.position;
            targ.x -= position.x;
            targ.y -= position.y;
            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            Debug.Log("angle je valda " + angle);
            var rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            rotation.z -= 45;
            transform.rotation = rotation;
        }*/
        if (pos1.X > pos2.X && pos1.Y >= pos2.Y)
        {
            spriteRenderer.flipY = true;
        }
        else if (pos1.X < pos2.Y && pos1.Y <= pos2.Y)
        {
            spriteRenderer.flipX = true;
        }
        else if (pos1.X < pos2.Y && pos1.Y >= pos2.Y)
        {
            spriteRenderer.flipY = true;
            spriteRenderer.flipX = true;
        }
        
        var startingPos = Grid.GetPositionFromTile(pos1);
        var elapsedTime = 0f;
        while (elapsedTime < AnimationTime)
        {
            gameObject.transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / AnimationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameLogic.animationPlaying = false;
        Destroy(gameObject);
    }
    
}