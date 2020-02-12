package it.danieletentoni.fowcube

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.sqlite.db.SupportSQLiteDatabase
import it.danieletentoni.fowcube.dao.CardDao
import it.danieletentoni.fowcube.dao.CollectionDao
import it.danieletentoni.fowcube.models.card.CardModel
import it.danieletentoni.fowcube.models.collection.CollectionModel
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch

@Database(entities = [CardModel::class, CollectionModel::class], version = 2, exportSchema = false)
abstract class FowCubeDatabase : RoomDatabase() {
    abstract fun cardDao(): CardDao

    abstract fun collectionDao(): CollectionDao

    private class FowCubeDatabaseCallback(
        private val scope: CoroutineScope
    ) : RoomDatabase.Callback() {

        override fun onOpen(db: SupportSQLiteDatabase) {
            // Use this to populate the database whenever the app is started.
            super.onOpen(db)
            INSTANCE?.let { database ->
                scope.launch {
                    // Update cards from remote database.
                    val cardDao = database.cardDao()

                    // Add sample cards.
                    var card = CardModel()
                    card.name = "Alice Zeus"
                    card.description = "Fa cose"
                    card.type = 1
                    cardDao.insertCard(card)
                    card = CardModel()
                    card.name = "Alice Oscura Coniglio"
                    card.description = "Fa troppe cose"
                    card.type = 1
                    cardDao.insertCard(card)

                    // TODO: Add your own cards!
                    card = CardModel()
                    card.name = "Bastet"
                    card.description = "Fa poche cose"
                    card.type = 1
                    cardDao.insertCard(card)

                    val collectionDao = database.collectionDao()

                    var collection = CollectionModel()
                    collection.name = "First"
                    collectionDao.insertCollection(collection)
                    collection = CollectionModel()
                    collection.name = "Second"
                    collectionDao.insertCollection(collection)
                    collection = CollectionModel()
                    collection.name = "Third"
                    collectionDao.insertCollection(collection)
                }
            }
        }
    }

    companion object {
        // Singleton instance
        @Volatile
        private var INSTANCE: FowCubeDatabase? = null

        fun getDatabase(context: Context, scope: CoroutineScope): FowCubeDatabase {
            val tempInstance = INSTANCE
            if (tempInstance != null) {
                return tempInstance
            }

            synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    FowCubeDatabase::class.java,
                    "fow_cube_database"
                )
                    .addCallback(FowCubeDatabaseCallback(scope))
                    .build()
                INSTANCE = instance
                return instance
            }
        }
    }
}