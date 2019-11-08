// Working on Unity 5.6.1f1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamDev.Redis;

public class RedisKeyWritter : MonoBehaviour
{
    private RedisDataAccessProvider redis;
    public string Key = "KeyfromUnity";
    public string Value = "ValueFromUnity"; 
  

    void Start()
    {
        redis = new RedisDataAccessProvider();
        redis.Configuration.Host = "192.168.99.100";
        redis.Configuration.Port = 6379;
        redis.Connect();

        redis.SendCommand(RedisCommand.SET, Key, Value);
    }

}