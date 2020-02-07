const functions = require('firebase-functions');
const admin = require('firebase-admin');
const express = require('express');
const cors = require('cors');
const app = express(); // Represent all other cloud functions.
const func_coll = express(); // Represent the cloud function "Collection".

var serviceAccount = require("./permissions.json");

admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    databaseURL: "https://fowcube.firebaseio.com"
});

const db = admin.firestore();
const cards_coll = db.collection("cards");
const db_coll = db.collection("collections"); // Represent the collection "Collection".

app.use(cors({ origin: true }));
func_coll.use(cors({ origin: true }));

// Hello World. Used for test.
app.get('/hello-world', (req, res) => {
    return res.status(200).send('Hello World!');
});

/*
 Ho lasciato la possibilità di specificare l'id nel body della chiamata
 perché in un futuro si aggiungeranno le carte tramite una chiamata API
 da repository ufficiali per poter gestire in modo automatico tali ope-
 razioni. Chiedere con @Alain d'Ettore di mettere a disposizione un 
 endpoint al suo portale per scaricare i dati di cui ho bisogno.
 !! Lui ha solamente la lingua inglese, valutare di creare delle traduzioni.
 */
app.post('/card', (req, res) => {
    (async() => {
        try {
            let l_index = 0; // Search the last index.
            await cards_coll.orderBy('card_id', 'desc')
                .limit(1).get().then(snapshot => {
                    let docs = snapshot.docs;
                    for (let doc of docs) {
                        if (doc.data().card_id > l_index) {
                            l_index = doc.data().card_id;
                        }
                    }
                    return;
                });
            let n_index = req.body.id !== undefined ? req.body.id : l_index + 1;
            await cards_coll.add({
                card_id: n_index,
                description: req.body.description,
                name: req.body.name
            }).then((docRef) => {
                console.log("Added a card with id:" + docRef.id);
                return res.status(200).send(docRef.id);
            }).catch((error) => {
                console.log(error);
                return res.status(500).send(error);
            });
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Read a card.
app.get('/card/:id', (req, res) => {
    (async() => {
        try {
            const document = cards_coll.doc(req.params.id);
            let item = await document.get();
            let response = item.data();
            return res.status(200).send(response);
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Read all cards.
app.get('/card', (req, res) => {
    (async() => {
        try {
            let query = cards_coll;
            let response = [];
            await query.get().then(querySnapshot => {
                let docs = querySnapshot.docs;
                for (let doc of docs) {
                    const selectedItem = {
                        id: doc.id,
                        card_id: doc.data().card_id,
                        description: doc.data().description,
                        name: doc.data().name
                    };
                    response.push(selectedItem);
                }
                return;
            });
            return res.status(200).send(response);
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Update a card.
app.put('/card/:item_id', (req, res) => {
    (async() => {
        try {
            const document = cards_coll.doc(req.params.item_id);
            await document.update({
                name: req.body.name,
                description: req.body.description
            });
            return res.status(200).send();
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Delete a card.
app.delete('/card/:item_id', (req, res) => {
    (async() => {
        try {
            const document = cards_coll.doc(req.params.item_id);
            await document.delete();
            return res.status(200).send();
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// GONG Tutorial.
app.get('/api', (req, res) => {
    const date = new Date();
    const hours = (date.getHours() % 12) + 1; // London is UTC + 1hr;
    res.json({ bongs: 'BONG '.repeat(hours) });
});

// Create a collection.
func_coll.post('/collection', (req, res) => {
    (async() => {
        try {
            var docRef = await db_coll.add({
                name: req.body.name,
                uid: req.body.uid,
                cards_in: Array()
            }).then((docRef) => {
                console.log("Added a collection with id:" + docRef.id);
                return res.status(200).send(docRef.id);
            });
            return res.status(200).send(docRef.id);
        } catch (error) {
            console.log("Name: " + req.body.name + "Uid: " + req.body.uid + " Error: " + error);
            return res.status(500).send(error);
        }
    })();
});

// Read all the collection of the user.
func_coll.get('/collections/:uid', (req, res) => {
    (async() => {
        try {
            let query = db_coll.where("uid", "==", req.params.uid);
            let response = [];
            await query.get().then(querySnapshot => {
                let docs = querySnapshot.docs;
                for (let doc of docs) {
                    const selectedItem = {
                        id: doc.id,
                        name: doc.data().name
                    };
                    response.push(selectedItem);
                }
                const result = {
                    uid: req.params.uid,
                    collections: response
                };
                return res.status(200).send(result);
            });
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        } finally {
            console.log("get all collections");
        }
    })();
});

// Read only the selected collection.
func_coll.get('/collection/:coll_id', (req, res) => {
    (async() => {
        try {
            let docRef = db_coll.doc(req.params.coll_id);
            await docRef.get().then((doc) => {
                if (doc.exists) {
                    const result = {
                        id: doc.id,
                        name: doc.data().name,
                        cards_in: doc.data().cards_in
                    };
                    return res.status(200).send(result);
                } else {
                    return res.status(500).send("Didn't find any collection.");
                }
            }).catch((error) => {
                console.log(error);
                return res.status(500).send("Didn't find any collection.");
            });
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Remove a card from a collection.
func_coll.put('/collection/removecard/:coll_id', (req, res) => {
    (async() => {
        try {
            var docRef = db_coll.doc(req.params.coll_id);
            await docRef.get().then((doc) => {
                if (doc.exists) {
                    docRef.update({
                        cards_in: firebase.firestore.FieldValue.arrayRemove(req.body.id)
                    });
                }
                return res.status(200).send("Correctly removed.");
            });
            return res.status(500).send("Body: " + req.body);
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })
});

// Add a card in a collection.
func_coll.put('/collection/addcard/:coll_id', (req, res) => {
    (async() => {
        // I don't need to see the above error, the function return ever a value.
        try {
            var docRef = db_coll.doc(req.params.coll_id);
            await docRef.get().then((doc) => {
                if (doc.exists) {
                    docRef.update({
                        cards_in: admin.firestore.FieldValue.arrayUnion(req.body.id)
                    });
                }
                return res.status(200).send("Correctly updated.");
            }).catch((error) => {
                console.error("Error adding document: ", error);
                return res.status(500).send("Error adding document. Body: " + req.body);
            });
        } catch (error) {
            console.log("Add a card error: " + error);
            return res.status(500).send(error);
        }
    })();
});

// Delete a collection.
func_coll.delete('/collection/:item_id', () => {});

exports.app = functions.https.onRequest(app);
exports.func_coll = functions.https.onRequest(func_coll);