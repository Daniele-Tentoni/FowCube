const functions = require('firebase-functions');
const admin = require('firebase-admin');
const express = require('express');
const cors = require('cors');
const app = express();

var serviceAccount = require("./permissions.json");

admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    databaseURL: "https://fowcube.firebaseio.com"
});

const db = admin.firestore();
const cards = db.collection("cards");
const collections = db.collection("collections");

app.use(cors({ origin: true }));

// Hello World. Used for test.
app.get('/hello-world', (req, res) => {
    return res.status(200).send('Hello World!');
});

// Create a card.
app.post('/card', (req, res) => {
    (async() => {
        try {
            let l_index = 0; // Search the last index.
            await db.collection('cards')
                .orderBy('card_id', 'desc')
                .limit(1).get().then(snapshot => {
                    let docs = snapshot.docs;
                    for (let doc of docs) {
                        if (doc.data().card_id > l_index) {
                            l_index = doc.data().card_id;
                        }
                    }
                    return;
                });

            console.log(req.body.id !== undefined ?
                "In this case I have to assign " + req.body.id + " as id." :
                "Not assigned id. Set as " + l_index + ".");
            let n_index = req.body.id !== undefined ? req.body.id : l_index + 1;
            await db.collection('cards').add({
                card_id: n_index,
                description: req.body.description,
                name: req.body.name
            }).then((docRef) => {
                console.log("Added a card with id:" + docRef.id);
                return res.status(200).send(docRef.id);
            });
            return res.status(500).send(n_index);
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
            const document = db.collection('cards').doc(req.params.id);
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
            let query = db.collection('cards');
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
            const document = db.collection('cards').doc(req.params.item_id);
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
            const document = db.collection('cards').doc(req.params.item_id);
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
app.post('/collection', (req, res) => {
    (async() => {
        try {
            var docRef = await db.collection('collections').add({
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
app.get('/collection/:uid', (req, res) => {
    (async() => {
        try {
            let query = db.collection('collections').where("uid", "==", req.params.uid);
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
                return;
            });
            return res.status(200).send(response);
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Read only the selected collection.
app.get('/collection/:uid/:name', (req, res) => {
    (async() => {
        try {
            let query = db.collection("collections")
                .where("name", "==", req.params.name)
                .where("uid", "==", req.params.uid).limit(1);
            await query.get().then(querySnapshot => {
                let docs = querySnapshot.docs;
                for (let doc of docs) {
                    const selectedItem = {
                        id: doc.id,
                        cards_in: doc.data().cards_in,
                        name: doc.data().name
                    };
                    return res.status(200).send(selectedItem);
                }
                return res.status(500).send("Didn't find any collection.");
            }).catch((error) => {
                console.log(error);
                return res.status(500).send("Didn't find any collection.");
            });
            return res.status(500).send("Body: " + req.body);
        } catch (error) {
            console.log(error);
            return res.status(500).send(error);
        }
    })();
});

// Remove a card from a collection.
app.put('/collection/removecard/:coll_id', (req, res) => {
    (async() => {
        try {
            var docRef = db.collection("collections").doc(req.params.coll_id);
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
app.post('/collection/addcard/:coll_id', (req, res) => {
    (async() => {
        try {
            var docRef = db.collection("collections").doc(req.params.coll_id);
            await docRef.get()
                .then((doc) => {
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
app.delete('/collection/:item_id', () => {});

exports.app = functions.https.onRequest(app);

// Adds a message that welcomes new users into the chat.
exports.addWelcomeMessages = functions.auth.user().onCreate(async(user) => {
    console.log('A new user signed in for the first time.');
    const fullName = user.displayName || 'Anonymous';

    // Saves the new welcome message into the database
    // which then displays it in the FriendlyChat clients.
    // await admin.firestore().collection('cards').add({
    //     name: 'Firebase Bot',
    //     description: `${fullName} signed in for the first time! Welcome!`
    // });
    console.log('Welcome message written to database.');
});

// Sends a notifications to all users when a new message is posted.
exports.sendNotifications = functions.firestore.document('cards/{cardId}').onCreate(async(snapshot) => {
    // Notification details.
    const description = snapshot.data().description;
    const payload = {
        notification: {
            title: `${snapshot.data().name} posted a new card.`,
            body: description ? (description.length <= 100 ? description : description.substring(0, 97) + '...') : '',
            icon: snapshot.data().profilePicUrl || '/images/profile_placeholder.png' //,
                //click_action: `https://${process.env.GCLOUD_PROJECT}.firebaseapp.com`,
        }
    };

    // Get the list of device tokens.
    const allTokens = await admin.firestore().collection('fcmTokens').get();
    const tokens = [];
    allTokens.forEach((tokenDoc) => {
        tokens.push(tokenDoc.id);
    });

    if (tokens.length > 0) {
        // Send notifications to all tokens.
        const response = await admin.messaging().sendToDevice(tokens, payload);
        await cleanupTokens(response, tokens);
        console.log('Notifications have been sent and tokens cleaned up.');
    }
});

// Cleans up the tokens that are no longer valid.
function cleanupTokens(response, tokens) {
    // For each notification we check if there was an error.
    const tokensDelete = [];
    response.results.forEach((result, index) => {
        const error = result.error;
        if (error) {
            console.error('Failure sending notification to', tokens[index], error);
            // Cleanup the tokens who are not registered anymore.
            if (error.code === 'messaging/invalid-registration-token' ||
                error.code === 'messaging/registration-token-not-registered') {
                const deleteTask = admin.firestore().collection('messages').doc(tokens[index]).delete();
                tokensDelete.push(deleteTask);
            }
        }
    });
    return Promise.all(tokensDelete);
}