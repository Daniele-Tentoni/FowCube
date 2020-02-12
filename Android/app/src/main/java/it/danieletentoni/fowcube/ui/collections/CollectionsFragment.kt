package it.danieletentoni.fowcube.ui.collections

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.DialogFragment
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.floatingactionbutton.FloatingActionButton
import com.google.android.material.snackbar.Snackbar
import it.danieletentoni.fowcube.OnListFragmentInteractionListener
import it.danieletentoni.fowcube.R
import it.danieletentoni.fowcube.viewModels.CollectionViewModel

/**
 * A fragment representing a list of Items.
 * Activities containing this fragment MUST implement the
 * [OnListFragmentInteractionListener] interface.
 */
class CollectionsFragment : Fragment() {

    private var listener: OnListFragmentInteractionListener? = null

    private lateinit var collectionViewModel: CollectionViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_collection_list, container, false)

        // Set the adapter and live data observer.
        if (view is RecyclerView) {
            val context = this.context!!.applicationContext
            val adapter = MyCollectionRecyclerViewAdapter(context, listener)
            view.adapter = adapter
            view.layoutManager = LinearLayoutManager(context)

            collectionViewModel = ViewModelProvider(this).get(CollectionViewModel::class.java)
            collectionViewModel.allCollections.observe(viewLifecycleOwner, Observer { collections ->
                collections?.let { adapter.setCollections(collections)}
            })
        }

        // Make fab visible and add a listener.
        val fab: FloatingActionButton? = activity?.findViewById(R.id.fab)
        fab?.visibility = View.VISIBLE
        fab?.setOnClickListener {
            // Open the activity to add new collection.
            val newFragment = NewCollectionFragment()
            newFragment.show(childFragmentManager, "missiles")
            // Snackbar.make(view, "Use me to add new collections.", Snackbar.LENGTH_LONG).setAction("Action", null).show()
        }

        return view
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        if (context is OnListFragmentInteractionListener)
            listener = context
        else
            throw RuntimeException(context.toString() + " must implement OnListFragmentInteractionListener")
    }

    override fun onDetach() {
        super.onDetach()
        listener = null
    }
}
