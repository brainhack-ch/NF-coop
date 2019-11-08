using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamDev.Redis;
using System;
using System.Threading;

public class ThrQueueRedisListen : MonoBehaviour
{
    private RedisDataAccessProvider redis;
    public string LastValueReceived;
    public int UNIXTimestamp;
    public int State;
    public float valueRead;
    public string REDISListName= "mylist";

    bool valueReceived=false;
    public ReaderWriterLock rwl = new ReaderWriterLock();
    public Queue queue = new Queue();
    // receiving Thread
    Thread receiveThread;


    void Start()
    {
        try
        {
        redis = new RedisDataAccessProvider();
        redis.Configuration.Host = "192.168.99.100";
        redis.Configuration.Port = 6379;
        redis.Connect();
      
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("Not possible to connect with REDIS   " + e);
        }
    }


    private void ReceiveData()
    {
        while (true)
        {

            try
            {
                redis.SendCommand(RedisCommand.LPOP, REDISListName);
                string text = redis.ReadString();

                if (text != null) { 
                    ///Debug.Log("  REDIS   LPOP ");

                    rwl.AcquireWriterLock(1000);
                    queue.Enqueue(text);
                    ///Debug.Log("Enqueued  " + text);

                    rwl.ReleaseWriterLock();
                }

            }
            catch
            {

            }

        }
    }


    private void Update()
    {
        rwl.AcquireReaderLock(5);
        if (queue.Count > 0)
        {
             LastValueReceived = (string)queue.Dequeue();
            valueReceived = true;
            ///Debug.Log("valueReceived  " + valueReceived);



        }
        rwl.ReleaseReaderLock();

        if (valueReceived) { 
            try
            {
                //Debug.Log("value" + value);
                if (LastValueReceived != null)
                {
                    ///Debug.Log("value" + LastValueReceived);
                    ParseCSV(LastValueReceived);

                }
            }
            catch { }
        }



    }


    void OnApplicationQuit()
    {
        try
        {
             receiveThread.Abort();

        }
        catch (Exception e)
        {
            Debug.Log("Not possible to kill thread " + e.Message);
        }
    }


    //Format  (timestamp,STATE(0 or 1), value[0,1])
    // (1573243916,0, 0.543)
    private void ParseCSV(string csvStrRaw)
    {

        try
        {
            //Remove first parenthesis
            string csvStrP= csvStrRaw.Remove(0,1);

            //Remove second parenthesis
            string csvStr = csvStrP.Remove( csvStrP.Length-1 ,1);

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