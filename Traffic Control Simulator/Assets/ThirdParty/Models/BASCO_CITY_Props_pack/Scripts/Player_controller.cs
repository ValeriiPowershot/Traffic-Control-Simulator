using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BascoCity
{


public class Player_controller : MonoBehaviour
{

    public Animator anim;
    public Rigidbody rb;
    public Transform trs;
    public int velocity;
    public int rotate_velocity;
    public ParticleSystem ps;

    void Update()
    {

        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
       // rb.velocity = new Vector3(move.x * velocity, 0, move.z * velocity);
        Quaternion torotation = Quaternion.LookRotation(new Vector3(rb.velocity.x, 0, rb.velocity.z), Vector3.up);

        if (rb.velocity.x == 0 && rb.velocity.z == 0)
        {
            anim.SetBool("run", false);
            ps.loop = false;
        }
        else
        {

            ps.loop = true;
            if (!ps.isPlaying)
            {
                ps.Play();
            }

            trs.rotation = Quaternion.RotateTowards(trs.rotation, torotation, Time.deltaTime * rotate_velocity);

            anim.SetBool("run", true);
        }
    }
}
}