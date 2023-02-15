const express = require("express");
const coyotesController = require("../controller/coyotesController");
const coyotesRouter = express.Router();

coyotesRouter.route("/getInitialCoyotes").post(coyotesController.getInitialCoyotes);

module.exports = coyotesRouter;