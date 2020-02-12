package it.danieletentoni.fowcube.dao

import androidx.lifecycle.LiveData
import androidx.room.*
import it.danieletentoni.fowcube.models.card.CardModel

@Dao
interface CardDao {
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertCard(vararg card: CardModel)

    @Update
    suspend fun updateCard(vararg card: CardModel)

    @Delete
    suspend fun deleteCard(vararg card: CardModel)

    @Query(value = "SELECT * FROM card")
    fun getAllCardsLiveData(): LiveData<List<CardModel>>

    @Query(value = "SELECT * FROM card WHERE id = :cardId")
    fun getCardByIdLiveData(cardId: String): LiveData<CardModel>

    @Query(value = "SELECT * FROM card WHERE type = :cardType")
    fun getCardsByTypeLiveData(cardType: Int): LiveData<List<CardModel>>
}