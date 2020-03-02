package it.danieletentoni.fowcube.ui.collections

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import it.danieletentoni.fowcube.R
import it.danieletentoni.fowcube.models.collection.CollectionModel
import kotlinx.android.synthetic.main.fragment_card_list_item.view.*
import kotlinx.android.synthetic.main.fragment_collection.view.*

class MyCollectionRecyclerViewAdapter(
    context: Context
) : RecyclerView.Adapter<MyCollectionRecyclerViewAdapter.ViewHolder>() {

    private val inflater: LayoutInflater = LayoutInflater.from(context)
    private var collections = emptyList<CollectionModel>() // Cached copy of words

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = inflater.inflate(R.layout.fragment_collection, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = collections[position]
        holder.mIdView.text = item.id.toString()
        holder.mContentView.text = item.name

        with(holder.mView) {
            tag = item
            // setOnClickListener(mOnClickListener)
        }
    }

    internal fun setCollections(collections: List<CollectionModel>) {
        this.collections = collections
        notifyDataSetChanged()
    }

    override fun getItemCount(): Int = collections.size

    inner class ViewHolder(val mView: View) : RecyclerView.ViewHolder(mView) {
        val mIdView: TextView = mView.card_title
        val mContentView: TextView = mView.card_subtitle

        override fun toString(): String {
            return super.toString() + " '" + mContentView.text + "'"
        }
    }
}
