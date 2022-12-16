#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
namespace Assets.Scripts.Multiplayer.API.Helper
{
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Net.NetworkInformation; NetworkInformation.Ping & UnityEngine.Ping have a conflict and I need Debug.Log so I commented this line
using System.Net;
using System;
using System.IO;
 
public static class TestPing
{
    public static bool status = false;
    public static bool isDone = false;
    public static string ipAdd; //The IP addres for the ping call
 
    public static bool PingThis()
    {
        try
        {
            //I strongly recommend to check Ping, Ping.Send & PingOptions on microsoft C# docu or other C# info source
            //in this block you configure the ping call to your host or server in order to check if there is network connection.
         
            //from https://stackoverflow.com/questions/55461884/how-to-ping-for-ipv4-only
            //from https://stackoverflow.com/questions/49069381/why-ping-timeout-is-not-working-correctly
            //and from https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net
         
         
            System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();
         
            byte[] buffer = new byte[32]; //array that contains data to be sent with the ICMP echo
            int timeout = 10000; //in milliseconds
            System.Net.NetworkInformation.PingOptions pingOptions = new System.Net.NetworkInformation.PingOptions(64, true);
            System.Net.NetworkInformation.PingReply reply = myPing.Send(ipAdd, timeout, buffer, pingOptions); //the same method can be used without the timeout, data buffer & pingOptions overloadd but this works for me
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                return true;
            }
            else if(reply.Status == System.Net.NetworkInformation.IPStatus.TimedOut) //to handle the timeout scenario
            {
                return status;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e) //To catch any exception of the method
        {
            Debug.Log(e);
            return false;
        }
        finally { } //To not get stuck in an error or exception, see "Try, Catch, Finally" docs.
    }
 
    public static string GetIPAddress() //Get the actual IP addres of your host/server
    {
        //Yes, I could use the "host name" or the "host IP address" direct on the ping.send method BUT!!
        //I find out and "Situation" in which due to my network setting in my PC any ping call (from script or cmd console)
        //returned the IPv6 instead of IPv4 which couse the Ping.Send thrown an exception
        //that could be the scenario for many of your users so you have to ensure this run for everyone.
     
     
        //from https://stackoverflow.com/questions/1059526/get-ipv4-addresses-from-dns-gethostentry
     
        IPHostEntry host;
        host = Dns.GetHostEntry("google.com"); //I use google.com as an example but it can be any host name (preferably yours)
 
        try
        {
            host = Dns.GetHostEntry("google.com"); //Get the IP host entry from your host/server
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally { }
 
 
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //filter just the IPv4 IPs
            {                                                                      //you can play around with this and get all the IP arrays (if any)
                return ip.ToString();                                              //and check the connection with all of then if needed
            }
        }
        return string.Empty;
    }
 
    public static void DoPing()
    {
        ipAdd = GetIPAddress(); //call to get the IP address from your host/server
 
        if (PingThis()) //call to check if you can make ping to that host IP
        {
            status = true;
            isDone = true;
        }
        else
        {
            status = false;
            isDone = true;
        }
    }
}
}