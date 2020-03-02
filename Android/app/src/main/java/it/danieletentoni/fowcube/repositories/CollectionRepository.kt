package it.danieletentoni.fowcube.repositories

import android.os.AsyncTask
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

    fun clear() {
        DeleteAllWordsAsyncTask(collectionDao).execute()
    }

    class DeleteAllWordsAsyncTask(private val dao: CollectionDao) : AsyncTask<Void?, Void?, Void?>() {
        override fun doInBackground(vararg params: Void?): Void? {
            dao.clearCollections()
            return null
        }
    }
}