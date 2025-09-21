using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveControls : MonoBehaviour
{
    public float speed;
    private GatherInput gI;
    private Rigidbody2D rb;
    private Animator anim;
    public float jumpForce;

    public int additionalJumps=2;
    private int resetJumpsNumber;
    private int direction=1;//向右
    private bool doubleJump = true;
    public float rayLength;
    public LayerMask groundLayer;
    public Transform leftPoint;
    public Transform rightPoint;
    private bool grounded=true;
    private void Start(){
        gI=GetComponent<GatherInput>();
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        resetJumpsNumber=additionalJumps;
    }

    private void Update(){
        SetAnimatorValues();
    }


    //固定时间刷新，使每个玩家体验一样
    private void FixedUpdate(){
        CheckStatus();
        Move();
        JumpPlayer();
    }
    private void CheckStatus(){
        RaycastHit2D leftCheckHit=Physics2D.Raycast(leftPoint.position,Vector2.down,rayLength,groundLayer);
        RaycastHit2D rightCheckHit=Physics2D.Raycast(rightPoint.position,Vector2.down,rayLength,groundLayer);
        if (leftCheckHit||rightCheckHit) {
            grounded=true;
            // doubleJump=false;
            additionalJumps=resetJumpsNumber;
        }
        else {
            grounded=false;
        }
        SeeRays(leftCheckHit);
        SeeRays(rightCheckHit);
    }

    private void SeeRays(RaycastHit2D leftCheckHit){
        Color color1=leftCheckHit?Color.red :Color.green; 
        Debug.DrawRay(leftPoint.position,Vector2.down*rayLength,color1);
    }
    

    private void Move(){
        Flip();
        rb.velocity = new Vector2(speed*gI.valueX,rb.velocity.y);
        
    }

    private void JumpPlayer(){
        if (gI.jumpInput) {
            if (grounded) {
                rb.velocity = new Vector2(gI.valueX * speed, jumpForce);
                // doubleJump = true;
            }else if (additionalJumps > 0) {
                rb.velocity = new Vector2(gI.valueX * speed, jumpForce);
                // doubleJump = false;
                additionalJumps--;
            }
        }
        gI.jumpInput = false;
    }
    private void Flip(){
        if (gI.valueX * direction < 0) {
            transform.localScale = new Vector3(-transform.localScale.x,1,1);
            direction *= -1;
        }
    }

    private void SetAnimatorValues(){
        anim.SetFloat("Speed",Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vSpeed",rb.velocity.y); 
        anim.SetBool("Grounded",grounded); 
    }
}
