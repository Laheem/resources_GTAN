var scrWidth = API.getScreenResolutionMantainRatio().Width;
var scrHeight = API.getScreenResolutionMantainRatio().Height;
var LEVEL_ONE_BOUNDARY = 40;
var LEVEL_TWO_BOUNDARY = 20;
var LEVEL_THREE_BOUNDARY = 10;
var player = API.getLocalPlayer();
API.onUpdate.connect(function () {
    var visable = API.getEntitySyncedData(player, "hungerVisable");
    if (visable) {
        var hunger = API.getEntitySyncedData(player, "hungerVal");
        API.drawText("Hunger:", scrWidth - 210, scrHeight - 225, 1, 255, 255, 255, 255, 4, 4, false, true, 0);
        if (hunger > LEVEL_ONE_BOUNDARY) {
            API.drawText("" + hunger, scrWidth - 80, scrHeight - 225, 1, 255, 255, 255, 255, 4, 4, false, true, 0); // White
        }
        else if (hunger <= LEVEL_ONE_BOUNDARY && hunger > LEVEL_TWO_BOUNDARY) {
            API.drawText("" + hunger, scrWidth - 80, scrHeight - 225, 1, 237, 249, 67, 255, 4, 4, false, true, 0); // Yellow
        }
        else if (hunger <= LEVEL_TWO_BOUNDARY && hunger > LEVEL_THREE_BOUNDARY) {
            API.drawText("" + hunger, scrWidth - 80, scrHeight - 225, 1, 236, 138, 42, 255, 4, 4, false, true, 0); // Orange
        }
        else
            API.drawText("" + hunger, scrWidth - 80, scrHeight - 225, 1, 255, 45, 33, 255, 4, 4, false, true, 0); // Red
    }
});
