// Working on Unity 5.6.1f1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamDev.Redis;
using System;

public class RedisKeyWritter : MonoBehaviour
{
    private RedisDataAccessProvider redis;
    public string Key = "KeyfromUnity";
    public string Value = "ValueFromUnity"; 
  

    void Start()
    {
        try
        {
            redis = new RedisDataAccessProvider();
            redis.Configuration.Host = "192.168.36.92";
            redis.Configuration.Port = 6379;
            redis.Connect();

            _game_manager.Instance.GetComponent<_game_manager>().b_REDIS_connected = true;

            _game_manager.Instance.GO_RedisKeyValueWritter.GetComponent<RedisKeyWritter>()._redis_send_EOF_command();
        }
        catch (Exception e)
        {

        }
    }

    public void _redis_send_command(string s_value)
    {
        redis.SendCommand(RedisCommand.SET, Key, s_value);
    }

    public void _redis_send_EOF_command()
    {
        redis.SendCommand(RedisCommand.DEL, Key);
    }

}