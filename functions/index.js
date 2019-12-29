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

app.use(cors({ origin: true }));

// Hello World. Used for test.
app.get('/hello-world', (req, res) => {
  return res.status(200).send('Hello World!');
});

// Create a card.
app.post('/api/card', (req, res) => {
    (async () => {
        try {
			let l_index = 0;
			await db.collection('cards')
				.orderBy('card_id', 'desc')
				.limit(1).get().then(snapshot => {
					let docs = snapshot.docs;
					for (let doc of docs) {
						l_index = doc.data().card_id;
					}
					return;
				});
			let n_index = l_index + 1;
			await db.collection('cards').doc('/' + n_index + '/')
				.create({
					card_id: n_index,
					description: req.body.description,
					name: req.body.name
				});
			return res.status(200).send();
        } catch (error) {
			console.log(error);
			return res.status(500).send(error);
		}
	})();
});
  
// Read a card.
app.get('/api/card/:id', (req, res) => {
    (async () => {
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
app.get('/api/card', (req, res) => {
    (async () => {
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
app.put('/api/card/:item_id', (req, res) => {
(async () => {
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
app.delete('/api/card/:item_id', (req, res) => {
(async () => {
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

exports.app = functions.https.onRequest(app);

