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

// Adds a message that welcomes new users into the chat.
exports.addWelcomeMessages = functions.auth.user().onCreate(async(user) => {
    console.log('A new user signed in for the first time.');
    const fullName = user.displayName || 'Anonymous';

    // Saves the new welcome message into the database
    // which then displays it in the FriendlyChat clients.
    await admin.firestore().collection('cards').add({
        name: 'Firebase Bot',
        description: `${fullName} signed in for the first time! Welcome!`
    });
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