package it.danieletentoni.fowcube.viewModels

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.viewModelScope
import it.danieletentoni.fowcube.FowCubeDatabase
import it.danieletentoni.fowcube.models.collection.CollectionModel
import it.danieletentoni.fowcube.repositories.CollectionRepository
import kotlinx.coroutines.launch

class CollectionViewModel(application: Application): AndroidViewModel(application) {

    private val repository: CollectionRepository

    // LiveData gives us updated words when they change.
    val allCollections: LiveData<List<CollectionModel>>

    init {
        val collectionDao = FowCubeDatabase.getDatabase(application, viewModelScope).collectionDao()
        repository = CollectionRepository(collectionDao)
        allCollections = repository.allCollection
    }

    /**
     * The implementation of insert() in the database is completely hidden from the UI.
     * Room ensures that you're not doing any long running operations on
     * the main thread, blocking the UI, so we don't need to handle changing Dispatchers.
     * ViewModels have a coroutine scope based on their lifecycle called
     * viewModelScope which we can use here.
     */
    fun insert(collection: CollectionModel) = viewModelScope.launch {
        repository.insert(collection)
    }
}