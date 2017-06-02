API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "markAtm") {
        var dist = args[0];
        var blip = API.createBlip(dist);
        API.setBlipName(blip, "Your nearest ATM.");
        API.setBlipSprite(blip, 76);
        API.setBlipColor(blip, 49);
    }
});
