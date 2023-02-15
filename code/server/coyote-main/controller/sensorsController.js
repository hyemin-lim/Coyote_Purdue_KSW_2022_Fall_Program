//get access to the sensor model
const Sensors = require("../models/sensors");

//get sound1 sensor coordinates
const getSensor1 = async (req, res) => {
    try{
        Sensors.find({id: 'sound1'}, (err, docs)=>{
            if(err){
                console.log(err);
            }
            else{
                if(docs.length > 0){
                    res.status(200).json({id: "sound1", x: docs[0].x, y: docs[0].y});
                }
                else{
                    var sensor1 = new Sensors({
                        id: 'sound1',
                        x: "40.4203008430482",
                        y: "-86.90254211425781",
                    });
                    sensor1.save();
                    res.status(200).json({msg: 'sound1 coordinate added.'});
                }
            }
        });
    } catch (err) {
        console.log(err);
        res.send(err);
    }
};

//get sound2 sensor coordinates
const getSensor2 = (req, res) => {
    Sensors.find({id: 'sound2'}, (err, docs)=>{
        if(err){
            console.log(err);
        }
        else{
            if(docs.length > 0){
                res.status(200).json({id: "sound2",x: docs[0].x, y: docs[0].y});
            }
            else{
                console.log(req.body);
                var sensor2 = new Sensors({
                    id: 'sound2',
                    x: "40.4203008530482",
                    y: "-86.90354211425781",
                });
                sensor2.save();
                res.status(200).json({msg: 'sound2 coordinate added.'});
            }
        }
    })
};
//get sound3 sensor coordinates
const getSensor3 = (req, res) => {
    Sensors.find({id: 'sound3'}, (err, docs)=>{
        if(err){
            console.log(err);
        }
        else{
            if(docs.length > 0){
                res.status(200).json({id: "sound3",x: docs[0].x, y: docs[0].y});
            }
            else{
                var sensor3 = new Sensors({
                    id: 'sound3',
                    x: "40.4203018430482",
                    y: "-86.90274211425781",
                });
                sensor3.save();
                res.status(200).json({msg: 'sound3 coordinate added.'});
            }
        }
    })
};

module.exports = {
    getSensor1,
    getSensor2,
    getSensor3
}