const mongoose = require("mongoose");
const {Schema} = mongoose;
//make a new schema for coyotes
const coyoteSchema = new mongoose.Schema({
    x: {
        type: String,
        required: true
    },
    y: {
        type: String,
        required: true
    },
    time: {
        type: String,
        required: true
    }
});
module.exports = mongoose.model('Coyotes', coyoteSchema);