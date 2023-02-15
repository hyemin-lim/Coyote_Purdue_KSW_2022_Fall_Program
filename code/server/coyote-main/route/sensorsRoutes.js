const express = require("express");
const sensorController = require("../controller/sensorsController");
const sensorsRouter = express.Router();

sensorsRouter.route("/getSound1Coord").post(sensorController.getSensor1);
sensorsRouter.route("/getSound2Coord").post(sensorController.getSensor2);
sensorsRouter.route("/getSound3Coord").post(sensorController.getSensor3);

module.exports = sensorsRouter;