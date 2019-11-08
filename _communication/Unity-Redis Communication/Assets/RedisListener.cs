// Working on Unity 5.6.1f1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamDev.Redis;

public class RedisListener : MonoBehaviour
{
    private RedisDataAccessProvider redis;
    private string[] keys;

    void Start()
    {
        redis = new RedisDataAccessProvider();
        redis.Configuration.Host = "192.168.99.100";
        redis.Configuration.Port = 6379;
        redis.Connect();


        redis.SendCommand(RedisCommand.KEYS, "*");
        keys = redis.ReadMultiString();
        for (int i = 0; i < keys.Length; i++)
        {
            redis.SendCommand(RedisCommand.GET, keys[i]);
            string value = redis.ReadString();
            Debug.Log(i.ToString() + " " + keys[i] + ":" + value);
        }
    }




}