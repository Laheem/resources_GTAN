var scrWidth = API.getScreenResolutionMantainRatio().Width;
var scrHeight = API.getScreenResolutionMantainRatio().Height;
var player = API.getLocalPlayer();
var pool = null;
var enemy;
API.onUpdate.connect(function () {
    var inFight = API.getEntitySyncedData(player, "inFight");
    if (inFight) {
        API.disableAllControlsThisFrame();
        API.enableControlThisFrame(24 /* Attack */);
        API.enableControlThisFrame(25 /* Aim */);
        API.enableControlThisFrame(21 /* Sprint */);
        API.enableControlThisFrame(31 /* MoveUpDown */);
        API.enableControlThisFrame(32 /* MoveUpOnly */);
        API.enableControlThisFrame(140 /* MeleeAttackLight */);
        API.enableControlThisFrame(141 /* MeleeAttackHeavy */);
        API.enableControlThisFrame(142 /* MeleeAttackAlternate */);
        API.enableControlThisFrame(143 /* MeleeBlock */);
        API.enableControlThisFrame(263 /* MeleeAttack1 */);
        API.enableControlThisFrame(264 /* MeleeAttack2 */);
        var playerHealth = API.getPlayerHealth(player);
        var enemyHealth = API.getPlayerHealth(enemy);
        API.drawRectangle(scrWidth, scrHeight, 100, 50, 255, 0, 0, 255);
        API.drawRectangle(scrWidth, scrHeight, playerHealth, 50, 0, 255, 0, 255);
        API.drawRectangle(scrWidth - 80, scrHeight, 100, 50, 255, 0, 0, 255);
        API.drawRectangle(scrWidth - 80, scrHeight, enemyHealth, 50, 0, 255, 0, 255);
    }
});
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "preFight") {
        var camVec = args[0];
        enemy = args[1];
        var cam = API.createCamera(camVec, camVec);
        API.setActiveCamera(cam);
        API.showColorShard("Street Fight", "Fight until there's a winner.", 0, 0, 3000);
        API.sleep(3000);
    }
    else if (eventName === "fightRequest") {
        pool = API.getMenuPool();
        var menu = API.createMenu("Fight Request!", 0, 0, 6);
        API.setMenuTitle(menu, "Fight Request!");
        API.setMenuBannerRectangle(menu, 100, 255, 0, 0);
        var buttonAcc = API.createMenuItem("Accept", "Accept the fight request, and go into the brawl!");
        var buttonNo = API.createMenuItem("Decline", "Decline the fight request, no brawl.");
        buttonAcc.Activated.connect(function (menu, item) {
            API.triggerServerEvent("acceptFight", args[0]);
            menu.Visible = false;
        });
        buttonNo.Activated.connect(function (menu, item) {
            menu.Visible = false;
        });
        menu.AddItem(buttonAcc);
        menu.AddItem(buttonNo);
    }
});
//Moving this outside of the draw text loop for clarity purpose.
API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});
