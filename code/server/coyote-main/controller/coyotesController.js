//get access to the coyote model
const Coyotes = require("../models/coyotes");

//get 5 of the coyote coordinates stored in the database
const getInitialCoyotes = (req, res) => {
    try{
        Coyotes.find({}, (err, docs)=>{ //get all documents from the coyote collection
            if(err){
                console.log(err);
            }
            else{
                if(docs.length < 5){ //if number of detected coyote coordinates is under 5
                    var msg = [];
                    docs.forEach((doc)=>{
                        var coyote = doc.x.toString() + "/" + doc.y.toString();
                        msg.push(coyote);
                    })
                    res.status(200).json({msg: msg});
                }
                else{//if number of detected coyote coordinates is over 5
                    //descending order sort
                    docs.sort((a, b) => {
                        if(a.time < b.time){
                            return 1;
                        }
                        if(a.time > b.time){
                            return -1;
                        }
                        return 0;
                    });

                    var msg = [];
                    for(var i = 0; i < 5; i++){
                        var coyote = docs[i].x.toString() + "/" + docs[i].y.toString();
                        msg.push(coyote);
                    }
                    res.status(200).json({msg: msg});
                }
            }
        });
    } catch (err) {
        console.log(err);
        res.send(err);
    }
}

module.exports = {
    getInitialCoyotes
}