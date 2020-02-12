package it.danieletentoni.fowcube.models.collection

import androidx.room.ColumnInfo
import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "collection")
class CollectionModel {
    @PrimaryKey(autoGenerate = true)
    @ColumnInfo(name = "id")
    var id:Int = 0

    @ColumnInfo(name = "fid")
    var fid:String = ""

    @ColumnInfo(name = "name")
    var name:String = ""

    @ColumnInfo(name = "uid")
    var uid:String = ""
}