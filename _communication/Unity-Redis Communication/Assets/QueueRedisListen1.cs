using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamDev.Redis;
using System;

public class QueueRedisListen : MonoBehaviour
{
    private RedisDataAccessProvider redis;
    public string LastValueReceived;
    public int UNIXTimestamp;
    public int State;
    public float valueRead;
    public string REDISListName= "mylist"; 


    void Start()
    {
        redis = new RedisDataAccessProvider();
        redis.Configuration.Host = "192.168.99.100";
        redis.Configuration.Port = 6379;
        redis.Connect();

    }

    private void Update()
    {
        try
        {

                redis.SendCommand(RedisCommand.LPOP, REDISListName);
                 LastValueReceived = redis.ReadString();
                //Debug.Log("value" + value);
                if (LastValueReceived != null)
                {
                    Debug.Log("value" + LastValueReceived);
                    ParseCSV(LastValueReceived);

                }



        }
        catch { }


    }


    //Format  (timestamp,STATE(0 or 1), value[0,1])
    // (1573243916,0, 0.543)
    private void ParseCSV(string csvStrRaw)
    {

        try
        {
            //Remove first parenthesis
            string csvStrP= csvStrRaw.Remove(0,1);
            Debug.Log("csvStrP "+ csvStrP);

            //Remove second parenthesis
            string csvStr = csvStrP.Remove( csvStrP.Length-1 ,1);
            Debug.Log("csvStr " + csvStr);

            string[] lineData = (csvStr.Trim()).Split(","[0]);
            int.TryParse(lineData[0], out UNIXTimestamp);
            int.TryParse(lineData[1], out State);
            float.TryParse(lineData[2], out valueRead);
        }
        catch
        {
            Debug.Log("Error parsing CSV data") ;

        }

    }
}