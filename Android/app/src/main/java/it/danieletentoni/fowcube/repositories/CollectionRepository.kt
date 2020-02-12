package it.danieletentoni.fowcube.repositories

import androidx.lifecycle.LiveData
import it.danieletentoni.fowcube.dao.CollectionDao
import it.danieletentoni.fowcube.models.collection.CollectionModel

class CollectionRepository(private val collectionDao: CollectionDao) {

    val allCollection:LiveData<List<CollectionModel>> = collectionDao.getAllCollectionLiveData()

    suspend fun insert(collection: CollectionModel) {
        collectionDao.insertCollection(collection)
    }

    suspend fun update(collection: CollectionModel) {
        collectionDao.updateCollection(collection)
    }
}