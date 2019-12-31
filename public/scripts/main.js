/**
 * Copyright 2018 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
'use strict';

// Signs-in Friendly Chat.
function signIn() {
    // Sign into Firebase using popup auth & Google as the identity provider.
    var provider = new firebase.auth.GoogleAuthProvider();
    firebase.auth().signInWithPopup(provider);
}

// Signs-out of Friendly Chat.
function signOut() {
    // Sign out of Firebase.
    firebase.auth().signOut();
}

// Initiate Firebase Auth.
function initFirebaseAuth() {
    // Listen to auth state changes.
    firebase.auth().onAuthStateChanged(authStateObserver);
}

// Returns the signed-in user's profile pic URL.
function getProfilePicUrl() {
    return firebase.auth().currentUser.photoURL || '/images/profile_placeholder.png';
}

// Returns the signed-in user's display name.
function getUserName() {
    return firebase.auth().currentUser.displayName;
}

// Returns true if a user is signed-in.
function isUserSignedIn() {
    return !!firebase.auth().currentUser;
}

// Saves a new message to your Cloud Firestore database.
function saveCard(cardName) {
    console.log("Saving " + cardName);
    // Add a new message entry to the database.
    return firebase.firestore().collection('cards').add({
        name: cardName,
        description: "This is a automatic description from the web."
    }).catch(function(error) {
        console.error('Error writing new message to database', error);
    });
}

// Loads cards and listens for upcoming ones.
function loadCards() {
    // Create the query to load the last 12 cards and listen for new ones.
    var query = firebase.firestore()
        .collection("cards")
        .orderBy("name", "asc")
        .limit(12);

    // Start listening to the query.
    query.onSnapshot(function(snapshot) {
        snapshot.docChanges().forEach(function(change) {
            if (change.type === 'removed') {
                deleteCard(change.doc.id);
            } else {
                var card = change.doc.data();
                displayCard(change.doc.id, card.name, card.description, card.imageUrl);
            }
        });
    });
}

// Saves a new message containing an image in Firebase.
// This first saves the image in Firebase storage.
function saveImageMessage(file) {
    // TODO 9: Posts a new image as a message.
}

// Saves the messaging device token to the datastore.
function saveMessagingDeviceToken() {
    // TODO 10: Save the device token in the realtime datastore
}

// Requests permissions to show notifications.
function requestNotificationsPermissions() {
    // TODO 11: Request permissions to send notifications.
}

// Triggered when a file is selected via the media picker.
function onMediaFileSelected(event) {
    event.preventDefault();
    var file = event.target.files[0];

    // Clear the selection in the file picker input.
    imageFormElement.reset();

    // Check if the file is an image.
    if (!file.type.match('image.*')) {
        var data = {
            message: 'You can only share images',
            timeout: 2000
        };
        signInSnackbarElement.MaterialSnackbar.showSnackbar(data);
        return;
    }
    // Check if the user is signed-in
    if (checkSignedInWithMessage()) {
        saveImageMessage(file);
    }
}

// Triggered when the send new message form is submitted.
function onCardFormSubmit(e) {
    e.preventDefault();
    // Check that the user entered a message and is signed in.
    if (cardInputElement.value && checkSignedInWithMessage()) {
        saveCard(cardInputElement.value).then(function() {
            // Clear message text field and re-enable the SEND button.
            resetMaterialTextfield(cardInputElement);
            toggleButton();
        });
    }
}

// Triggers when the auth state change for instance when the user signs-in or signs-out.
function authStateObserver(user) {
    if (user) { // User is signed in!
        // Get the signed-in user's profile pic and name.
        var profilePicUrl = getProfilePicUrl();
        var userName = getUserName();

        // Set the user's profile pic and name.
        userPicElement.style.backgroundImage = 'url(' + addSizeToGoogleProfilePic(profilePicUrl) + ')';
        userNameElement.textContent = userName;

        // Show user's profile and sign-out button.
        userNameElement.removeAttribute('hidden');
        userPicElement.removeAttribute('hidden');
        signOutButtonElement.removeAttribute('hidden');

        // Hide sign-in button.
        signInButtonElement.setAttribute('hidden', 'true');

        // We save the Firebase Messaging Device token and enable notifications.
        saveMessagingDeviceToken();
    } else { // User is signed out!
        // Hide user's profile and sign-out button.
        userNameElement.setAttribute('hidden', 'true');
        userPicElement.setAttribute('hidden', 'true');
        signOutButtonElement.setAttribute('hidden', 'true');

        // Show sign-in button.
        signInButtonElement.removeAttribute('hidden');
    }
}

// Returns true if user is signed-in. Otherwise false and displays a message.
function checkSignedInWithMessage() {
    // Return true if the user is signed in Firebase
    if (isUserSignedIn()) {
        return true;
    }

    // Display a message to the user using a Toast.
    var data = {
        message: 'You must sign-in first',
        timeout: 2000
    };
    signInSnackbarElement.MaterialSnackbar.showSnackbar(data);
    return false;
}

// Resets the given MaterialTextField.
function resetMaterialTextfield(element) {
    element.value = '';
    element.parentNode.MaterialTextfield.boundUpdateClassesHandler();
}

// Template for cards.
var CARD_TEMPLATE =
    '<div class="card-container">' +
    '<div class="spacing"><div class="pic"></div></div>' +
    '<div class="description"></div>' +
    '<div class="name"></div>' +
    '</div>';

// Adds a size to Google Profile pics URLs.
function addSizeToGoogleProfilePic(url) {
    if (url.indexOf('googleusercontent.com') !== -1 && url.indexOf('?') === -1) {
        return url + '?sz=150';
    }
    return url;
}

// A loading image URL.
var LOADING_IMAGE_URL = 'https://www.google.com/images/spin-32.gif?a';

// Delete a Message from the UI.
function deleteCard(id) {
    var div = document.getElementById(id);
    // If an element for that message exists we delete it.
    if (div) {
        div.parentNode.removeChild(div);
    }
}

function createAndInsertCard(id, name) {
    debugger;
    const container = document.createElement('div');
    container.innerHTML = CARD_TEMPLATE;
    const div = container.firstChild;
    div.setAttribute('id', id);

    // If name is null, assume we've gotten a brand new message.
    // https://stackoverflow.com/a/47781432/4816918
    name = name ? name : "New card.";
    div.setAttribute('name', name);

    // figure out where to insert new message
    const existingCards = cardListElement.children;
    if (existingCards.length === 0) {
        cardListElement.appendChild(div);
    } else {
        let cardListNode = existingCards[0];

        while (cardListNode) {
            const cardListNodeName = cardListNode.getAttribute('name');

            if (!cardListNodeName) {
                throw new Error(
                    `Child ${cardListNode.id} has no 'name' attribute`
                );
            }

            if (cardListNodeName > name) {
                break;
            }

            cardListNode = cardListNode.nextSibling;
        }

        cardListElement.insertBefore(div, cardListNode);
    }

    return div;
}

// Displays a Message in the UI. Missing picUrl.
function displayCard(id, name, description, imageUrl) {
    debugger;
    var div = document.getElementById(id) || createAndInsertCard(id, name);

    // Profile picture. Disabled before card image.
    // if (picUrl) {
    //     div.querySelector('.pic').style.backgroundImage = 'url(' + addSizeToGoogleProfilePic(picUrl) + ')';
    // }

    div.querySelector('.name').textContent = name;
    var cardElement = div.querySelector('.description');

    if (description) { // If the card have description.
        cardElement.textContent = description;
        // Replace all line breaks by <br>.
        cardElement.innerHTML = cardElement.innerHTML.replace(/\n/g, '<br>');
    } else if (imageUrl) { // If the card is an image.
        var image = document.createElement('img');
        image.addEventListener('load', function() {
            cardListElement.scrollTop = cardListElement.scrollHeight;
        });
        image.src = imageUrl + '&' + new Date().getTime();
        cardElement.innerHTML = '';
        cardElement.appendChild(image);
    }

    // Show the card fading-in and scroll to view the new card.
    setTimeout(function() { div.classList.add('visible') }, 1);
    cardListElement.scrollTop = cardListElement.scrollHeight;
    cardInputElement.focus();
}

// Enables or disables the submit button depending on the values of the input
// fields.
function toggleButton() {
    if (cardInputElement.value) {
        submitButtonElement.removeAttribute('disabled');
    } else {
        submitButtonElement.setAttribute('disabled', 'true');
    }
}

// Checks that the Firebase SDK has been correctly setup and configured.
function checkSetup() {
    if (!window.firebase || !(firebase.app instanceof Function) || !firebase.app().options) {
        window.alert('You have not configured and imported the Firebase SDK. ' +
            'Make sure you go through the codelab setup instructions and make ' +
            'sure you are running the codelab using `firebase serve`');
    }
}

// Checks that Firebase has been imported.
checkSetup();

// Shortcuts to DOM Elements.
var cardListElement = document.getElementById('cards');
var cardFormElement = document.getElementById('card-form');
var cardInputElement = document.getElementById('card');
var submitButtonElement = document.getElementById('submit');
var imageButtonElement = document.getElementById('submitImage');
var imageFormElement = document.getElementById('image-form');
var mediaCaptureElement = document.getElementById('mediaCapture');
var userPicElement = document.getElementById('user-pic');
var userNameElement = document.getElementById('user-name');
var signInButtonElement = document.getElementById('sign-in');
var signOutButtonElement = document.getElementById('sign-out');
var signInSnackbarElement = document.getElementById('must-signin-snackbar');

// Saves card on form submit.
cardFormElement.addEventListener('submit', onCardFormSubmit);
signOutButtonElement.addEventListener('click', signOut);
signInButtonElement.addEventListener('click', signIn);

// Toggle for the button.
cardInputElement.addEventListener('keyup', toggleButton);
cardInputElement.addEventListener('change', toggleButton);

// Events for image upload.
// imageButtonElement.addEventListener('click', function(e) {
//     e.preventDefault();
//     mediaCaptureElement.click();
// });
// mediaCaptureElement.addEventListener('change', onMediaFileSelected);

// initialize Firebase
initFirebaseAuth();

var firestore = firebase.firestore();

// TODO: Enable Firebase Performance Monitoring.

// We load currently existing chat cards and listen to new ones.
loadCards();