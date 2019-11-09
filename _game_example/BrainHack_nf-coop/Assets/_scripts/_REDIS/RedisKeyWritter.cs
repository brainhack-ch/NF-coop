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
            redis.Configuration.Host = "192.168.99.100";
            redis.Configuration.Port = 6379;
            redis.Connect();
        }
        catch (Exception e)
        {

        }
    }

    public void _redis_send_command(string s_value)
    {
        redis.SendCommand(RedisCommand.SET, Key, s_value);
    }

}