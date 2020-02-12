package it.danieletentoni.fowcube.ui.collections

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import it.danieletentoni.fowcube.OnListFragmentInteractionListener
import it.danieletentoni.fowcube.R
import it.danieletentoni.fowcube.models.collection.CollectionModel
import kotlinx.android.synthetic.main.fragment_card_list_item.view.*


/**
 * [RecyclerView.Adapter] that can display a [DummyItem] and makes a call to the
 * specified [OnListFragmentInteractionListener].
 * TODO: Replace the implementation with code for your data type.
 */
class MyCollectionRecyclerViewAdapter(
    context: Context,
    private val mListener: OnListFragmentInteractionListener?
) : RecyclerView.Adapter<MyCollectionRecyclerViewAdapter.ViewHolder>() {

    private val inflater: LayoutInflater = LayoutInflater.from(context)
    private var collections = emptyList<CollectionModel>() // Cached copy of words

    /*init {
        mOnClickListener = View.OnClickListener { v ->
            val item = v.tag as DummyContent.DummyItem
            // Notify the active callbacks interface (the activity, if the fragment is attached to
            // one) that an item has been selected.
            mListener?.onListFragmentInteraction(item)
        }
    }*/

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
        val mIdView: TextView = mView.item_number
        val mContentView: TextView = mView.content

        override fun toString(): String {
            return super.toString() + " '" + mContentView.text + "'"
        }
    }
}
