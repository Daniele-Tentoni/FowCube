package it.danieletentoni.fowcube

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import it.danieletentoni.fowcube.dao.CardDao
import it.danieletentoni.fowcube.dao.CollectionDao
import it.danieletentoni.fowcube.models.card.CardModel

@Database(entities = arrayOf(CardModel::class), version = 1, exportSchema = false)
public abstract class FowCubeDatabase: RoomDatabase() {
    abstract fun CardDao(): CardDao

    abstract fun CollectionDao(): CollectionDao

    companion object {
        // Singleton instance
        @Volatile
        private var INSTANCE: FowCubeDatabase? = null

        fun getDatabase(context: Context): FowCubeDatabase {
            val tempInstance = INSTANCE
            if (tempInstance != null) {
                return tempInstance
            }

            synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    FowCubeDatabase::class.java,
                    "word_database"
                ).build()
                INSTANCE = instance
                return instance
            }
        }
    }
}