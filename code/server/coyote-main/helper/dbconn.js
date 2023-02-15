var mongoose = require("mongoose");
var dotenv = require("dotenv");
dotenv.config();
// const mongoAtlasUri = `mongodb://localhost:27017/basic`; -> localhost db
const uri = "mongodb+srv://"+process.env.USER_ID+":"+process.env.USER_PW+"@cluster0.ptguqyi.mongodb.net/test"; //cloud db
function mongooseConnection(){
    try {
// Connect to the MongoDB cluster
        mongoose.connect(
            uri,
            { useNewUrlParser: true, useUnifiedTopology: true },
            () => console.log("Mongoose is connected"),
        );
    } catch (e) {
        console.log("could not connect");
    }
    const dbConnection = mongoose.connection;
    dbConnection.on("error", (err) => console.log(`Connection error ${err}`));
    dbConnection.once("open", () => console.log("Connected to DB!"));
}
module.exports = mongooseConnection;
