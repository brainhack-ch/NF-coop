using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _player_controller : MonoBehaviour
{
    public float f_speed_X; // player 1 control // horizontal
    public float f_speed_Z; // player 2 control // vertical

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
        f_speed_X = _game_manager.Instance.GO_RedisListenerList1.GetComponent<ThrQueueRedisListen>().valueRead;
        f_speed_Z = _game_manager.Instance.GO_RedisListenerList2.GetComponent<ThrQueueRedisListen>().valueRead;

        if (b_can_control)
        {
            Vector3 v3_move_direction = new Vector3(f_speed_X, 0, f_speed_Z);

            v3_move_direction.Set(v3_move_direction.x, 0, v3_move_direction.z);
            RB_rigidBody.MovePosition(transform.position + v3_move_direction.normalized * ((Mathf.Abs(f_speed_X) + Mathf.Abs(f_speed_Z)) * f_speed_multiplicator) * Time.deltaTime);

            if (v3_move_direction != new Vector3(0, 0, 0))
            {
                RB_rigidBody.MoveRotation(Quaternion.LookRotation(v3_move_direction));
            }

            _game_manager.Instance.GO_feedback_player_1.GetComponent<Slider>().value = (1 + f_speed_X) / 2;
            _game_manager.Instance.GO_feedback_player_2.GetComponent<Slider>().value = (1 + f_speed_Z) / 2;

            this.GetComponent<AudioSource>().volume = (Mathf.Abs(f_speed_X) + Mathf.Abs(f_speed_Z))/2;
        }
        else
        {
            this.GetComponent<AudioSource>().volume = 0;
        }

        if (b_can_control_hands)
        {
            transform.GetChild(0).GetChild(0).transform.localEulerAngles = new Vector3(Mathf.Lerp(0, -90, (1 + f_speed_X) / 2), 0,0);
            transform.GetChild(0).GetChild(1).transform.localEulerAngles = new Vector3(Mathf.Lerp(0, -90, (1 + f_speed_Z) / 2),0,0);

            if (f_speed_X > 0.65f && f_speed_Z > 0.65f)
            {
                b_can_control_hands = false;

                _game_manager.Instance.GO_feedback_player_1.SetActive(true);
                _game_manager.Instance.GO_feedback_player_2.SetActive(true);

                StartCoroutine(_game_manager.Instance.coroutine_level_0_advanced(1));           
            }

            _game_manager.Instance.GO_feedback_player_1.GetComponent<Slider>().value = (1 + f_speed_X) / 2;
            _game_manager.Instance.GO_feedback_player_2.GetComponent<Slider>().value = (1 + f_speed_Z) / 2;

            this.transform.GetChild(0).GetChild(0).GetComponent<AudioSource>().volume = Mathf.Abs(f_speed_X);
            this.transform.GetChild(0).GetChild(1).GetComponent<AudioSource>().volume = Mathf.Abs(f_speed_Z);
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
