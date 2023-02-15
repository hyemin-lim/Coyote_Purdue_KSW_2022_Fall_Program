const mongoose = require("mongoose");
const {Schema} = mongoose;
//make a new schema for sensors
const sensorSchema = new mongoose.Schema({
    id: {
        type: String,
        unique: true,
        required: true
    },
    x: {
        type: String,
        required: true
    },
    y: {
        type: String,
        required: true
    }
});
module.exports = mongoose.model('Sensors', sensorSchema);