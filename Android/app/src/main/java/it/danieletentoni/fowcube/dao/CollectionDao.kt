package it.danieletentoni.fowcube.dao

import androidx.lifecycle.LiveData
import androidx.room.*
import it.danieletentoni.fowcube.models.collection.CollectionModel

@Dao
interface CollectionDao {
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertCollection(vararg collection:CollectionModel)

    @Update
    suspend fun updateCollection(vararg collection: CollectionModel)

    @Query(value = "SELECT * FROM collection")
    fun getAllCollectionLiveData(vararg uid:String): LiveData<List<CollectionModel>>

    @Query(value = "SELECT * FROM collection WHERE uid = :uid")
    fun getAllCollectionByUidLiveData(vararg uid:String): LiveData<List<CollectionModel>>

    @Query(value = "SELECT * FROM collection WHERE id = :id")
    fun getCollectionByIdLiveData(vararg id:String): LiveData<CollectionModel>
}