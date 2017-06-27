API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "markAtm") {
        var x = args[0];
        var y = args[1];
        API.setWaypoint(x, y);
    }
});
