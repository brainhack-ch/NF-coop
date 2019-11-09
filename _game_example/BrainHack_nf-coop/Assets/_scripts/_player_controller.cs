using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _player_controller : MonoBehaviour
{
    public float f_speed_X; // player 1 control
    public float f_speed_Z; // player 2 control

    public float f_speed_multiplicator = 0.5f;

    Rigidbody RB_rigidBody; //Reference to the rigidbody component

    public bool b_can_control;
    public bool b_can_control_hands;

    private void Start()
    {
        RB_rigidBody = GetComponent<Rigidbody>();
    }

    //Move with physics so the movement code goes in FixedUpdate()
    void FixedUpdate()
    {
        if (b_can_control)
        {
            Vector3 v3_move_direction = new Vector3(f_speed_X, 0, f_speed_Z);

            v3_move_direction.Set(v3_move_direction.x, 0, v3_move_direction.z);
            RB_rigidBody.MovePosition(transform.position + v3_move_direction.normalized * ((Mathf.Abs(f_speed_X) + Mathf.Abs(f_speed_Z)) * f_speed_multiplicator) * Time.deltaTime);

            if (v3_move_direction != new Vector3(0, 0, 0))
            {
                RB_rigidBody.MoveRotation(Quaternion.LookRotation(v3_move_direction));
            }
        }

        if (b_can_control_hands)
        {
            transform.GetChild(0).GetChild(0).transform.localEulerAngles = new Vector3(Mathf.Lerp(0, -90, f_speed_X), 0,0);
            transform.GetChild(0).GetChild(1).transform.localEulerAngles = new Vector3(Mathf.Lerp(0, -90, f_speed_Z),0,0);

            if (f_speed_X > 0.65f && f_speed_Z > 0.65f)
            {
                b_can_control_hands = false;
                StartCoroutine(_game_manager.Instance.coroutine_level_0_advanced(1));                
            }
        }
    }

    public void reset_hands()
    {
        b_can_control_hands = false;

        transform.GetChild(0).GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
        transform.GetChild(0).GetChild(1).transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        debugUpdate();
    }


    [Header("DEBUG")]
    float b_debug_increase_speed = 0.01f;
    void debugUpdate()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            f_speed_X += b_debug_increase_speed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            f_speed_X -= b_debug_increase_speed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            f_speed_Z += b_debug_increase_speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            f_speed_Z -= b_debug_increase_speed;
        }
    }
}
