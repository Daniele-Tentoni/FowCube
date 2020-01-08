// Scaricare una carta
fetch('https://www.fowdb.altervista.org/cards', {
    method: 'get'
}).then(res => {
    console.log(res.text());
    return res.text();
}).then(data => {
    console.log(data);
    return;
}).catch(error => {
    console.log('There has been a problem with your fetch operation: ' + error.message);
});
// Trovare all'interno della pagina l'id ingleseid

// Estrapolare dal suo interno le informazioni che ci servono XD.

/*
Quello che ti volevo chiedere è se riuscissi a fornirmi, in formato json o csv, una lista di tutte le carte uscite composte da nome della carta e codice.
*/