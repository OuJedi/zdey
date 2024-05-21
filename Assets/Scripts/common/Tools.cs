using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tools : MonoBehaviour
{
    private static Tools _instance;
    public static Tools Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("The Singlton is NULL.");
            }

            return _instance;
        }
    }



    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        _instance = this;


    }



    public void StopAllDelay()
    {

        StopAllCoroutines();

    }


    public void KillDelay(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }



    public void KillInterval(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }


    public Coroutine Delay(float time, Action callback)
    {
        return StartCoroutine(delayFunction(time, callback));

    }



    IEnumerator delayFunction(float time, Action callBack)
    {
        yield return new WaitForSeconds(time);
        if (callBack != null)
        {
            callBack();
        }

    }


    public Coroutine Interval(float time, Action callback)
    {
        return StartCoroutine(RepeatingFunction(time, callback));

    }


    IEnumerator RepeatingFunction(float repeatTime, Action callback)
    {
        while (true)
        {
            callback();
            yield return new WaitForSeconds(repeatTime);
        }
    }



    public Vector3 Angle2pos(Vector3 centerPoint, float rayon, float angle)
    {
        float posX = centerPoint.x + (rayon * Mathf.Cos((Mathf.PI) * (angle / 180f)));
        float posZ = centerPoint.z + (rayon * Mathf.Sin((Mathf.PI) * (angle / 180f)));
        return new Vector3(posX, 0.0f, posZ);
    }



    public float Pos2angle(Vector3 centerPoint, Vector3 position)
    {
        return Mathf.Atan2(centerPoint.z - position.z, centerPoint.x - position.x) * (180f / Mathf.PI) + 180f;
    }



    public Vector2 PolygoneCenter(GameObject[] arr)
    {

        float minX = 0f, maxX = 0f, minY = 0f, maxY = 0f;

        for (int i = 0; i < arr.Length; i++)
        {
            minX = (arr[i].transform.position.x < minX) ? arr[i].transform.position.x : minX;
            maxX = (arr[i].transform.position.x > maxX) ? arr[i].transform.position.x : maxX;
            minY = (arr[i].transform.position.y < minY) ? arr[i].transform.position.y : minY;
            maxY = (arr[i].transform.position.y > maxY) ? arr[i].transform.position.y : maxY;
        }
        return new Vector2((minX + maxX) / 2, (minY + maxY) / 2);


    }



    IEnumerator CheckAnimationCompleted(Animator animator, string clipName, float delay, Action onComplete)
    {

        yield return new WaitForSeconds(delay);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            yield return new WaitForSeconds(.001f);
        }

        onComplete?.Invoke();
    }

    public Coroutine OnAnimationCompleted(Animator animator, string clipName, float delay = .5f, Action onComplete = null)
    {
        return StartCoroutine(CheckAnimationCompleted(animator, clipName, delay, onComplete));
    }




    public bool IsFacingTarget(GameObject source, GameObject target, float targetSize = -1f)
    {
        float direction = target.transform.position.x - source.transform.position.x;
        return (Math.Sign(direction * source.transform.localScale.x) > 0 || (targetSize != -1 && MathF.Abs(direction) <= targetSize / 2));
    }




}
