using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _game_manager : _singleton<_game_manager>
{
    protected _game_manager() { }

    [Header("Parameters")]
    public int i_resting_state_duration = 30;

    [Header("Useless")]
    [HideInInspector]
    public List<GameObject> list_GO_levels = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> list_GO_players = new List<GameObject>();

    [HideInInspector]
    public GameObject GO_RedisKeyValueWritter;
    [HideInInspector]
    public GameObject GO_RedisListenerList1;
    [HideInInspector]
    public GameObject GO_RedisListenerList2;

    string s_path_GO_levels = "_levels";
    string s_path_GO_level_x = "_level_";
    string s_path_GO_players = "_players";
    string s_path_GO_player_x = "_player_";
    string s_path_GO_spawn_player_x = "_spawn_player_";
    public int i_current_level;


    GameObject GO_obj_level_0;
    GameObject GO_obj_level_1;
    GameObject GO_obj_level_2;
    [HideInInspector]
    public GameObject GO_text;

    public void Start()
    {
        GO_text = GameObject.Find("Canvas/Text");
        GO_text.GetComponent<Text>().text = "";

        GO_RedisKeyValueWritter = GameObject.Find("_REDIS_communication/RedisKeyValueWritter");
        GO_RedisListenerList1 = GameObject.Find("_REDIS_communication/GO_RedisListenerList1");
        GO_RedisListenerList2 = GameObject.Find("_REDIS_communication/GO_RedisListenerList2");

        GO_obj_level_0 = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + "0" + "/_objects").transform.GetChild(0).gameObject;
        GO_obj_level_1 = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + "0" + "/_objects").transform.GetChild(1).gameObject;
        GO_obj_level_2 = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + "0" + "/_objects").transform.GetChild(2).gameObject;

        // level management
        for (int i = 0; i < GameObject.Find(s_path_GO_levels).transform.childCount; i++)
        {
            list_GO_levels.Add(GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i.ToString()));
            list_GO_levels[i].SetActive(false);
        }

        // characters management
        for (int i = 0; i < GameObject.Find(s_path_GO_players).transform.childCount; i++)
        {
            list_GO_players.Add(GameObject.Find(s_path_GO_players + "/" + s_path_GO_player_x + i.ToString()));
            list_GO_players[i].SetActive(false);
            list_GO_players[i].GetComponent<_player_controller>().b_can_control = false;
        }

        _load_level_(0);
    }


    public void _load_level_(int i_level)
    {
        i_current_level = i_level;

        for (int i = 0; i < list_GO_levels.Count; i++)
        {
            list_GO_levels[i].SetActive(false);
        }
        list_GO_levels[i_level].SetActive(true);

        for (int i = 0; i < list_GO_players.Count; i++)
        {
            GameObject GO_spawn_player_x = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/" + s_path_GO_spawn_player_x + i.ToString());
            list_GO_players[i].SetActive(true);

            if (i_level == 0)
            {
                list_GO_players[i].transform.SetPositionAndRotation(GO_spawn_player_x.transform.position, GO_spawn_player_x.transform.rotation);

                list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints .FreezePosition;

                GO_obj_level_0.SetActive(true);
                GO_obj_level_1.SetActive(true);
                GO_obj_level_2.SetActive(true);

                load_level_0_a();
            }
            else if (i_level == 1)
            {
                GO_current_exit = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/_exit");
            }
            else if (i_level > 1)
            {
                //list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                StartCoroutine(coroutine_start_level(i_level));
            }
        }
    }



    [HideInInspector]
    public bool b_start_record_resting_state;
    [HideInInspector]
    public bool b_data_computed;
    [HideInInspector]
    public bool b_start_raising_hands;
    bool b_waiting_reaching_exit;
    void load_level_0_a()
    {
        b_start_record_resting_state = true;
        GO_text.GetComponent<Text>().text = "Press Space to start recording resting state";
        GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
    }

    public IEnumerator coroutine_level_0_advanced(int i_level)
    {
        GO_text.GetComponent<Text>().color = Color.green;
        GO_text.GetComponent<Text>().text = "Congratulation!";
        yield return new WaitForSeconds(1);
        GO_text.GetComponent<Text>().text = "";

        yield return new WaitForSeconds(1);
        GO_obj_level_0.SetActive(false);
        GO_text.GetComponent<Text>().text = "3";
        GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
        yield return new WaitForSeconds(0.5f);
        GO_obj_level_1.SetActive(false);
        GO_text.GetComponent<Text>().text = "2";
        GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
        yield return new WaitForSeconds(0.5f);
        GO_obj_level_2.SetActive(false);
        GO_text.GetComponent<Text>().text = "1";
        GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
        yield return new WaitForSeconds(1);
        GO_text.GetComponent<Text>().text = "GO!";
        GO_text.GetComponent<Text>().color = Color.green;

        for (int i = 0; i < list_GO_players.Count; i++)
        {
            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            _load_level_(i_level);

            GameObject GO_spawn_player_x = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/" + s_path_GO_spawn_player_x + i.ToString());
            list_GO_players[i].GetComponent<Rigidbody>().AddForce((GO_spawn_player_x.transform.position - list_GO_players[i].transform.position) * 40);
        }

        yield return new WaitForSeconds(1);
        GO_text.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3);

        for (int i = 0; i < list_GO_players.Count; i++)
        {
            GameObject GO_spawn_player_x = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/" + s_path_GO_spawn_player_x + i.ToString());
            list_GO_players[i].transform.SetPositionAndRotation(list_GO_players[i].transform.position, GO_spawn_player_x.transform.rotation);
            list_GO_players[i].GetComponent<_player_controller>().reset_hands();
        }

        yield return new WaitForSeconds(1);
        for (int i = 0; i < list_GO_players.Count; i++)
        {
            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            list_GO_players[i].GetComponent<_player_controller>().b_can_control = true;
            b_waiting_reaching_exit = true;
        }

    }

    [HideInInspector]
    public GameObject GO_current_exit;
    public IEnumerator coroutine_start_level(int i_level)
    {
        for (int i = 0; i < list_GO_players.Count; i++)
        {
            GO_current_exit = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/_exit");            

            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

            GameObject GO_spawn_player_x = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/" + s_path_GO_spawn_player_x + i.ToString());
            list_GO_players[i].GetComponent<Rigidbody>().AddForce((GO_spawn_player_x.transform.position - list_GO_players[i].transform.position + new Vector3(0,8,0)) * 40);
        }

        yield return new WaitForSeconds(4);

        for (int i = 0; i < list_GO_players.Count; i++)
        {
            GameObject GO_spawn_player_x = GameObject.Find(s_path_GO_levels + "/" + s_path_GO_level_x + i_level.ToString() + "/" + s_path_GO_spawn_player_x + i.ToString());
            list_GO_players[i].transform.SetPositionAndRotation(list_GO_players[i].transform.position, GO_spawn_player_x.transform.rotation);
            list_GO_players[i].GetComponent<_player_controller>().reset_hands();
        }

        yield return new WaitForSeconds(1);
        for (int i = 0; i < list_GO_players.Count; i++)
        {
            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            list_GO_players[i].GetComponent<_player_controller>().b_can_control = true;
            b_waiting_reaching_exit = true;
        }
    }


    public void _end_level(int i_level)
    {
        for (int i = 0; i < list_GO_players.Count; i++)
        {
            list_GO_players[i].GetComponent<_player_controller>().b_can_control = false;
            list_GO_players[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            list_GO_players[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            list_GO_players[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }





    [Header("DEBUG")]
    [HideInInspector]
    public int i_debug_level;
    [HideInInspector]
    public bool b_debug_load_level;
    private void Update()
    {
        if (b_debug_load_level)
        {
            b_debug_load_level = false;
            _load_level_(i_debug_level);
        }

        if (b_start_record_resting_state)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                b_start_record_resting_state = false;
                StartCoroutine(coroutine_record_resting_state(i_resting_state_duration)); // TO DO CHANGE
                // TODO
                // Send message to start recording of resting state
                GO_RedisKeyValueWritter.GetComponent<RedisKeyWritter>()._redis_send_command("start_resting_state_" + i_resting_state_duration);
            }
        }

        if (GO_text.GetComponent<Text>().text == "Resting state - computing data")
        {
            // TODO
            // If receive message resting state computation ended - start broadcasting rt alpha computed value
            if (GO_RedisListenerList1.GetComponent<ThrQueueRedisListen>().State == 1 && GO_RedisListenerList2.GetComponent<ThrQueueRedisListen>().State == 1)
            {
                b_data_computed = true;
            }

            if (b_data_computed)
            {
                b_data_computed = false;
                GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
                GO_text.GetComponent<Text>().text = "Raise Pat arms to start";
                b_start_raising_hands = true;
            }
        }
        if (b_start_raising_hands)
        {
            for (int i = 0; i < list_GO_players.Count; i++)
            {
                b_start_raising_hands = false;
                list_GO_players[i].GetComponent<_player_controller>().b_can_control_hands = true;
            }
        }

        if (b_waiting_reaching_exit)
        {
            for (int i = 0; i < list_GO_players.Count; i++)
            {
                if (GO_current_exit != null)
                {
                    if (Vector2.Distance(new Vector2(list_GO_players[i].transform.position.x, list_GO_players[i].transform.position.z), new Vector2(GO_current_exit.transform.position.x, GO_current_exit.transform.position.z)) < 0.5f)
                    {
                        b_waiting_reaching_exit = false;

                        StartCoroutine(coroutine_exit_reached(i_current_level));
                    }
                }
            }
        }
    }

    IEnumerator coroutine_exit_reached(int i_level)
    {
        _end_level(i_level);
        GO_text.GetComponent<Text>().color = Color.green;
        GO_text.GetComponent<Text>().text = "Congratulation!";
        yield return new WaitForSeconds(1);
        GO_text.GetComponent<Text>().text = "";
        if (i_level < list_GO_levels.Count)
        {
            _load_level_(i_level + 1);
        }
        else
        {
            GO_text.GetComponent<Text>().color = Color.green;
            GO_text.GetComponent<Text>().text = "TO BE CONTINUED";

            // TODO
            // Send message to stop sending data
            GO_RedisKeyValueWritter.GetComponent<RedisKeyWritter>()._redis_send_command("game_ended_stop");
        }
    }




    IEnumerator coroutine_record_resting_state(float f)
    {
        GO_text.GetComponent<Text>().text = "Resting state - " + f + " seconds remaining";
        GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
        yield return new WaitForSeconds(1);

        if (f > 1) {
            StartCoroutine(coroutine_record_resting_state(f - 1));
        }
        else
        {
            GO_text.GetComponent<Text>().text = "Resting state - computing data";
            GO_text.GetComponent<Text>().color = new Color(255, 140, 0);
        }
    }
}
