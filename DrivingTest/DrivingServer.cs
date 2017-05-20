using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using System.Diagnostics;

class DrivingServer : Script
    {
    public static readonly Vector3 DRIVING_TEST_LOCATION = new Vector3(431.8165, -621.7194, 26.81234);
    public static readonly Vector3 CAR_SPAWN = new Vector3(422.1202, -647.7612, 28.50027);

    private Dictionary<Client, Test> currTest = new Dictionary<Client, Test>();

    const int ALLOWED_MINUTES_CAR = 1;
    const int ALLOWED_SECONDS_CAR = 30;
    const int ALLOWED_MINUTES_BIKE = 1;
    const int ALLOWED_SECONDS_BIKE = 30;
    const int ALLOWED_MINUTES_TRUCK = 2;
    const int ALLOWED_SECONDS_TRUCK = 0;

    public DrivingServer()
    {
        API.onResourceStart += drivingTestStart;
        API.onClientEventTrigger += testSelected;
        API.onEntityEnterColShape += checkDriverPoint;
        API.onPlayerEnterVehicle += startTest;
        API.onPlayerDisconnected += checkTest;

    }

    private void checkTest(Client player, string reason)
    {
        Test testExist;
        if(currTest.TryGetValue(player, out testExist))
        {
            currTest.Remove(player);
        }
    }

    private void startTest(Client player, NetHandle vehicle)
    {
        Test playerTest;
        if (currTest.TryGetValue(player, out playerTest) &&  playerIsDriver(player)){
            if(playerTest.getCurrentPoint() > 0)
            {
                return;
            }
            Stopwatch playerTime = new Stopwatch();
            playerTime.Start();
            Test playerTestWithTimer = new Test(player, playerTest.getTypeOfTest(),playerTime);
            currTest.Remove(player);
            currTest.Add(player, playerTestWithTimer);
            API.triggerClientEvent(player, "placeFirstPoint", Test.POINT_ONE);
            CylinderColShape firstPoint = API.createCylinderColShape(Test.POINT_ONE, 5.0f, 6.0f);
        }
        API.triggerClientEvent(player, "cleanUpVehMarker");
    }

    private void checkDriverPoint(ColShape colShape, NetHandle entity)  
    {
        API.deleteColShape(colShape);
        var player = API.getPlayerFromHandle(entity);
        Test testCheck;
        if (player == null || !currTest.TryGetValue(player, out testCheck))
        {
            return;
        }
        else
            if (testCheck.passedLastPoint())
        {
            completeTest(player);
            return;
        }
        else
            API.triggerClientEvent(player, "newCheckPoint", Test.DRIVER_POINTS[testCheck.getCurrentPoint()]);
        CylinderColShape newPoint = API.createCylinderColShape(Test.DRIVER_POINTS[testCheck.getCurrentPoint()], 5.0f, 6.0f);
    }

    private void testSelected(Client sender, string eventName, params object[] arguments)
    {
        switch (eventName)
        {
            case "Car":
                beginTest(sender, "Car");
                break;
            case "Bike":
                beginTest(sender, "Bike");
                break;
            case "Trucker":
                beginTest(sender, "Trucker");
                break;
            default:
                break;
        }
    }

    private void drivingTestStart()
    {
        Blip testBlip = API.createBlip(DRIVING_TEST_LOCATION);
        API.setBlipName(testBlip,"Driving Test Location");
        API.setBlipSprite(testBlip, 315);
        API.createMarker(1, DRIVING_TEST_LOCATION, new Vector3(), new Vector3(), new Vector3(4, 4, 4), 255, 255, 0, 0);
        API.consoleOutput("Hands at 10 and 2. Driving test is ready!");
    }



    [Command("test")]
    public void startDriversTest(Client sender)
    {
        if (currTest.Count > 0)
        {
            API.sendChatMessageToPlayer(sender, "Sorry, someone is already on a test. You'll need to wait.");
            return;
        }
        if (sender.position.DistanceTo(DRIVING_TEST_LOCATION) < 10)
        {
            API.triggerClientEvent(sender, "chooseTest");
        }
        else
            API.sendChatMessageToPlayer(sender, "You need to be at the DMV to do your test!");
    }

    public void beginTest(Client sender,String type)
    {
        Vehicle learnerCar;
        switch (type)
        {
            case "Car":
                learnerCar = API.createVehicle(VehicleHash.Premier,CAR_SPAWN,new Vector3(),0,0,0);
                API.sendChatMessageToPlayer(sender, "Hop in the car, get past all the checkpoints as fast as you can.");
                break;
            case "Bike":
                learnerCar = API.createVehicle(VehicleHash.Fcr, CAR_SPAWN, new Vector3(), 0, 0, 0);
                API.sendChatMessageToPlayer(sender, "Hop on the bike, get past all the checkpoints as fast as you can.");
                break;
            case "Trucker":
                learnerCar = API.createVehicle(VehicleHash.Flatbed, CAR_SPAWN, new Vector3(), 0, 0, 0);
                API.sendChatMessageToPlayer(sender, "Hop in the cab, get past all the checkpoints as fast as you can.");
                break;
            default:
                break;

        }
        Test currentTest = new Test(sender, type,null);
        currTest.Add(sender, currentTest);
        API.triggerClientEvent(sender, "placeFirstPoint",Test.POINT_ONE);
        CylinderColShape firstPoint = API.createCylinderColShape(Test.POINT_ONE, 5.0f, 6.0f);
    }

    public void completeTest(Client player)
    {
        Test finishedTest = currTest.Get(player);
        TimeSpan totalTime = finishedTest.getStopWatch().Elapsed;

        int mins = totalTime.Minutes;
        int seconds = totalTime.Seconds;

        switch(finishedTest.getTypeOfTest()){

            case "Bike":
                if ((mins == 1 && seconds > 30) || mins > 1 || !vehicleHealthCorrect(player))
                {
                    API.sendChatMessageToPlayer(player, "You failed the test.");
                }
                else
                    API.sendChatMessageToPlayer(player, "You passed, well done!");
                break;
            case "Car":
                if ((mins == 1 && seconds > 30) || mins > 1 || !vehicleHealthCorrect(player))
                {
                    API.sendChatMessageToPlayer(player, "You failed the test.");
                }
                else 
                    API.sendChatMessageToPlayer(player, "You passed, well done!");
                break;
            case "Trucker":
                if (mins > 2 || (mins == 2 && seconds > 0) || !vehicleHealthCorrect(player))
                {
                    API.sendChatMessageToPlayer(player, "You failed the test.");
                }
                else
                    API.sendChatMessageToPlayer(player, "You passed, well done.");
                break;
        }
        currTest.Remove(player);
        API.triggerClientEvent(player, "cleanUp");
        API.sendChatMessageToPlayer(player, "Your stats : " + API.getVehicleHealth(player.vehicle).ToString() + " Time : " + mins.ToString() + ":" + seconds.ToString("D2"));
    }

    public bool playerIsDriver(Client player)
    {
        return (API.isPlayerInAnyVehicle(player) && API.getPlayerVehicleSeat(player) == -1);
    }

    public bool vehicleHealthCorrect(Client player)
    {
        Vehicle car = player.vehicle;
        if (API.getVehicleHealth(car) < 750) {
            return false;
        }
        return true;
    }
}

