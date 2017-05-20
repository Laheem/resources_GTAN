using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using System.Diagnostics;

public class Test
    {
    const int NUMBER_OF_POINTS = 10;

    public static readonly Vector3 POINT_ONE = new Vector3(422.2121, -648.8938, 25.92004);
    public static readonly Vector3 POINT_TWO = new Vector3(413.1408, -678.5109, 26.73291);
    public static readonly Vector3 POINT_THREE = new Vector3(405.3297, -768.432, 26.72458);
    public static readonly Vector3 POINT_FOUR = new Vector3(404.9759, -807.2639, 26.71573);
    public static readonly Vector3 POINT_FIVE = new Vector3(440.7471, -829.3017, 25.73711);
    public static readonly Vector3 POINT_SIX = new Vector3(489.1374, -826.1282, 22.34144);
    public static readonly Vector3 POINT_SEVEN = new Vector3(507.2491, -758.4941, 22.24066);
    public static readonly Vector3 POINT_EIGHT = new Vector3(503.7536, -701.137, 22.42259);
    public static readonly Vector3 POINT_NINE = new Vector3(457.5133, -677.2026, 25.22766);
    public static readonly Vector3 POINT_TEN = new Vector3(426.331, -666.5215, 26.39154);

    public static readonly String[] ALL_TEST_TYPES = new String[3] { "Bike", "Car", "Trucker" };
    public static readonly Vector3[] DRIVER_POINTS = new Vector3[NUMBER_OF_POINTS] { POINT_ONE, POINT_TWO, POINT_THREE, POINT_FOUR, POINT_FIVE, POINT_SIX, POINT_SEVEN, POINT_EIGHT, POINT_NINE, POINT_TEN };
    private Client player;
    private String typeOfTest;
    private int currentPoint = 0;
    private Stopwatch timeStarted;

    public Test(Client player, String typeOfTest,Stopwatch timeStarted)
    {
        this.player = player;
        this.typeOfTest = typeOfTest;
        this.timeStarted = timeStarted;
    }

    public bool passedLastPoint()
    {
        if (currentPoint == NUMBER_OF_POINTS - 1)
        {
            return true;
        }
        else
            currentPoint = currentPoint + 1;
        return false;
    }

    public int getCurrentPoint()
    {
        return currentPoint;
    }

    public Stopwatch getStopWatch()
    {
        return timeStarted;
    }

    public String getTypeOfTest()
    {
        return typeOfTest;
    }
}

