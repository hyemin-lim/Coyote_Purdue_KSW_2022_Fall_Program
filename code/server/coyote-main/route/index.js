const {Router: expressRouter} = require("express");
const router = expressRouter();

//sensor coords
const sensorRouter = require("./sensorsRoutes");
router.use("/sensors", sensorRouter);

//coyote coords
const coyoteRouter = require("./coyotesRoutes");
router.use("/coyotes", coyoteRouter);

module.exports = router;