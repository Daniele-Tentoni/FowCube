package it.danieletentoni.fowcube.repositories

import androidx.lifecycle.LiveData
import it.danieletentoni.fowcube.dao.CardDao
import it.danieletentoni.fowcube.models.card.CardModel

// We have to access only to cards, so we pass in only the CardDao.
class CardRepository(private val cardDao: CardDao) {

    // Room executes all queries on a separate thread.
    // Observed LiveData will notify the observer when the data has changed.
    val allCards: LiveData<List<CardModel>> = cardDao.getAllCardsLiveData()

    suspend fun insert(card: CardModel) {
        cardDao.insertCard(card)
    }

    suspend fun update(card: CardModel) {
        cardDao.updateCard(card)
    }

    suspend fun delete(card: CardModel) {
        cardDao.deleteCard(card)
    }
}