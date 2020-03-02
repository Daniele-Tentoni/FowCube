package it.danieletentoni.fowcube.ui.home

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.recyclerview.widget.LinearLayoutManager
import com.google.android.material.floatingactionbutton.FloatingActionButton
import it.danieletentoni.fowcube.R
import it.danieletentoni.fowcube.ui.collections.MyCollectionRecyclerViewAdapter
import it.danieletentoni.fowcube.ui.collections.NewCollectionFragment
import kotlinx.android.synthetic.main.fragment_home.view.*

class HomeFragment : Fragment() {

    private lateinit var homeViewModel: HomeViewModel

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val root = inflater.inflate(R.layout.fragment_home, container, false)
        val context = this.context!!.applicationContext
        val adapter = MyCollectionRecyclerViewAdapter(context)
        val recyclerView = root.collection_list_view
        recyclerView.adapter = adapter
        recyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.HORIZONTAL, false)

        homeViewModel = ViewModelProvider(this).get(HomeViewModel::class.java)
        homeViewModel.allCollections.observe(viewLifecycleOwner, Observer { collections ->
            collections?.let { adapter.setCollections(collections)}
        })

        // Make fab visible and add a listener.
        val fab: FloatingActionButton? = activity?.findViewById(R.id.fab)
        fab?.visibility = View.VISIBLE
        fab?.setOnClickListener {
            // Open the activity to add new collection.
            val newFragment = NewCollectionFragment()
            newFragment.show(childFragmentManager, "missiles")
        }

        return root
    }
}