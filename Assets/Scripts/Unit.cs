using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class Unit : MonoBehaviour
{

    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<Grid>();
        unitSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public Grid gameLogic;
    public Vector3 destination;
    public SpriteRenderer unitSprite;
    public static float OrderGiven_DarkerColor = 0.2f;
    public static float UnitMoveTime = 0.3f;
    public static float WaitTimeAfterMoveHasBeenFinished = 0.35f;
    public static float DieTime = 0.25f;
    private Transform[] _children;

    public IEnumerator MoveToPath(List<Point> path, bool destroyAfterwards)
    {
        GameObject chargeChild = null;
        foreach (var v in gameObject.GetComponentsInChildren<Transform>())
        {
            if (v.gameObject.tag.Equals("charge"))
            {
                chargeChild = v.gameObject;
                chargeChild.SetActive(false);
                break;
            }
        }
        while (gameLogic.animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameLogic.animationPlaying = true;
        if (chargeChild) chargeChild.SetActive(true);
        var transform1 = transform.position;
        gameLogic.MainCamera.transform.position = new Vector3(transform1.x, transform1.y, -10);
        for (var i = 1; i < path.Count; ++i)
        {
            var startingPos = transform.position;
            destination = Grid.GetPositionFromTile(path[i]);
            var elapsedTime = 0f;
            while (elapsedTime < UnitMoveTime)
            {
                transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / UnitMoveTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        if (chargeChild) Destroy(chargeChild);
        yield return new WaitForSeconds(WaitTimeAfterMoveHasBeenFinished);
        gameLogic.animationPlaying = false;
        if (destroyAfterwards) Destroy(gameObject);
    }

    public IEnumerator MoveToTile(Point tile)
    {
        var startingPos = transform.position;
        destination = Grid.GetPositionFromTile(tile);
        var elapsedTime = 0f;
        while (elapsedTime < UnitMoveTime)
        {
            transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / UnitMoveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Die()
    {
        while (gameLogic.animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        gameLogic.animationPlaying = true;
        var transform1 = transform.position;
        gameLogic.MainCamera.transform.position = new Vector3((0.5f + transform1.x), -0.5f - transform1.y, -10);
        var elapsedTime = 0f;
        while (elapsedTime < DieTime)
        {
            var currentColor = unitSprite.color;
            currentColor.a = 1-(elapsedTime / DieTime);
            unitSprite.color = currentColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        gameLogic.animationPlaying = false;
        Destroy(gameObject);
    }

    public IEnumerator ChangeWaveringAnimation(bool wavering)
    {
        while (gameLogic.animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameLogic.animationPlaying = true;
        GetComponent<Animator>().Play(wavering ? "UnitMorale" : "UnitNormal");
        gameLogic.animationPlaying = false;
    }
}


