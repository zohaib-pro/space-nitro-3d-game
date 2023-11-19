using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static long Millis()
    {
        DateTime currentTime = DateTime.Now;
        return  (long)(currentTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public static int Hours()
    {
        return (int) (Millis() / 3600000);
    }
}
