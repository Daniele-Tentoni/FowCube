package it.danieletentoni.fowcube.viewModels

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.viewModelScope
import it.danieletentoni.fowcube.FowCubeDatabase
import it.danieletentoni.fowcube.models.card.CardModel
import it.danieletentoni.fowcube.repositories.CardRepository
import kotlinx.coroutines.launch

class CardViewModel(application: Application): AndroidViewModel(application) {

    private val repository: CardRepository

    val allCards: LiveData<List<CardModel>>

    init {
        val cardDao = FowCubeDatabase.getDatabase(application).CardDao()
        repository = CardRepository(cardDao)
        allCards = repository.allCards
    }

    /**
     * The implementation of insert() in the database is completely hidden from the UI.
     * Room ensures that you're not doing any long running operations on
     * the main thread, blocking the UI, so we don't need to handle changing Dispatchers.
     * ViewModels have a coroutine scope based on their lifecycle called
     * viewModelScope which we can use here.
     */
    fun insert(card: CardModel) = viewModelScope.launch {
        repository.insert(card)
    }
}