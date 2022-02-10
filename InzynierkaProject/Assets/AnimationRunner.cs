using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRunner : MonoBehaviour
{
    public Animator anim;
    public int speed;
    public GameObject legLeft, legRight;
    public bool attackCheck;

    public void Start()
    {
        attackCheck = true;
    }

    void Update()
    {
        if (anim.GetInteger("AnimationState") == 0)
        {
            legLeft.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            legRight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        if (anim.GetInteger("AnimationState") == 1)
        {
            legLeft.transform.Rotate(new Vector3(speed, 0, 0));
            legRight.transform.Rotate(new Vector3(speed, 0, 0));
        }

        if (anim.GetInteger("AnimationState") == 2)
        {
            if(attackCheck == true)
            {
                attackCheck = false;
                StartCoroutine(Attack());
            }
        }

        if (anim.GetInteger("AnimationState") == 3)
        {
            if (attackCheck == true)
            {
                attackCheck = false;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        legLeft.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        legRight.transform.rotation = Quaternion.Euler(360f, 0f, 0f);
        yield return new WaitForSeconds(0.2f);
        legLeft.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        legRight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        attackCheck = true;
    }
}
