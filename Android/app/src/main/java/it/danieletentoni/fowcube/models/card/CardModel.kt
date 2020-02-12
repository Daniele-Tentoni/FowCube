package it.danieletentoni.fowcube.models.card

import androidx.annotation.Nullable
import androidx.room.ColumnInfo
import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "card")
class CardModel {
    @PrimaryKey(autoGenerate = true)
    @ColumnInfo(name = "id")
    var id: Int = 0

    @ColumnInfo(name = "name")
    var name: String = ""

    @ColumnInfo(name = "description")
    @Nullable
    var description: String = ""

    @ColumnInfo(name = "type", defaultValue = "1")
    var type: Int = 1
}