using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class PlayerFighting : MonoBehaviour
{
    public List<AttackSO> combo;
    
    
    float lastClickedTime=0f;
    float lastComboEnd;

    bool isCombo= false;


    int comboCounter=0;
    Animator animator;
    public float transitionTime=10f;
    private bool comboSwitch = true;

    public float comboTime = 1f;

    [SerializeField] GameObject comboCamera;

    public CombatMovement movement;


    private void Awake()
    {
        movement = GetComponent<CombatMovement>();
        
        animator = GetComponent<Animator>();

    }

    void Start()
    {
       // comboCamera.SetActive(false);
    }

    // Update is called once per frame

    void Update()
    {
        if (AnimationFinished("Attack"))
        {
            isCombo = false;

        }else isCombo = true;

        if (movement != null)
        {
            movement.MoveEnabDesab(isCombo);
        } //   Debug.Log(isCombo);

    }

    public void Attack()
    {

        if (!isCombo)
        {
            animator.SetTrigger("attack");
        }
        else
        {

            animator.SetBool("combo_switch", comboSwitch);
            comboSwitch =! comboSwitch;
        }

        if(!isCombo) animator.SetTrigger("end_combo");
       
    }


    void NormalAttack()
    {
        Debug.Log("start attack");
        animator.SetTrigger("attack");
        animator.SetTrigger("end_combo");
    
        if(comboSwitch) { }
    
    }

    void ComboAttack()
    {
        if (AnimationFinished("Attack"))
        {
            if (comboCounter < combo.Count)
            {
                Debug.Log(comboCounter);
                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;

                if (comboCounter == 0)
                {
                    animator.SetTrigger("attack");

                    Invoke("EndCombo", GetAnimationLength("Attack") + transitionTime);
                }
                else
                {
                    //comboCamera.SetActive(true);
                    //activate combo camera
                }

                animator.SetBool("combo_switch", comboSwitch);

                comboSwitch = !comboSwitch;

                comboCounter++;


                //   transitionTime -= 3;

            }
            else comboCounter = 0;
        }
        else
        {
            //ruins combo 
            EndCombo();

        }
    }



   public void EndCombo()
   { 
        animator.SetTrigger("end_combo"); 
   }

    float GetAnimationLength(string tag)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag(tag))
        {
            AnimatorClipInfo[] clipInfoArray = animator.GetCurrentAnimatorClipInfo(0);
           
            if (clipInfoArray.Length > 0)
            {
                AnimatorClipInfo clipInfo = clipInfoArray[0];

                return clipInfo.clip.length;
            }
        }
        return 0f;
    }


    bool AnimationFinished(string Tag)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && animator.GetCurrentAnimatorStateInfo(0).IsTag(Tag)) 
        {
            return false;
        }
        else return true;
    }

}

