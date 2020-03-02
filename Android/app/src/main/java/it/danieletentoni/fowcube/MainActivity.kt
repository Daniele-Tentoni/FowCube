package it.danieletentoni.fowcube

import android.os.Bundle
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.core.view.GravityCompat
import androidx.drawerlayout.widget.DrawerLayout
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.findNavController
import androidx.navigation.ui.AppBarConfiguration
import androidx.navigation.ui.navigateUp
import androidx.navigation.ui.setupActionBarWithNavController
import androidx.navigation.ui.setupWithNavController
import com.google.android.material.floatingactionbutton.FloatingActionButton
import com.google.android.material.navigation.NavigationView
import it.danieletentoni.fowcube.models.collection.CollectionModel
import it.danieletentoni.fowcube.ui.collections.NewCollectionFragment
import it.danieletentoni.fowcube.ui.home.HomeViewModel
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity(), NewCollectionFragment.NoticeDialogListener {

    private lateinit var appBarConfiguration: AppBarConfiguration

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)

        // Hide the fab button for next uses.
        val fab: FloatingActionButton = findViewById(R.id.fab)
        fab.visibility = View.GONE

        val drawerLayout: DrawerLayout = findViewById(R.id.drawer_layout)
        val navView: NavigationView = findViewById(R.id.nav_view)
        val navController = findNavController(R.id.nav_host_fragment)
        // Passing each menu ID as a set of Ids because each
        // menu should be considered as top level destinations.
        appBarConfiguration = AppBarConfiguration(
            setOf(
                R.id.nav_home, R.id.nav_cards
            ), drawerLayout
        )
        setupActionBarWithNavController(navController, appBarConfiguration)
        navView.setupWithNavController(navController)
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        menuInflater.inflate(R.menu.main, menu)
        return true
    }

    override fun onSupportNavigateUp(): Boolean {
        val navController = findNavController(R.id.nav_host_fragment)
        return navController.navigateUp(appBarConfiguration) || super.onSupportNavigateUp()
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        super.onOptionsItemSelected(item)
        when(item.itemId){
            R.id.collections_clear -> collectionClear()
        }
        return true
    }

    override fun onBackPressed() {
        if (drawer_layout.isDrawerOpen(GravityCompat.START)) {
            drawer_layout.closeDrawer(GravityCompat.START)
        } else {
            super.onBackPressed()
        }
    }

    /**
     * The positive result.
     */
    override fun onDialogPositiveClick(collectionName: String) {
        val homeViewModel = ViewModelProvider(this).get(HomeViewModel::class.java)
        val newCollectionModel = CollectionModel()
        newCollectionModel.name = collectionName
        homeViewModel.insert(newCollectionModel).invokeOnCompletion {
            Log.w("NEW_COLLECTION", "A new collection was correctly added.")
        }
    }

    /**
     * The negative result.
     */
    override fun onDialogNegativeClick() {
        Log.w("NEW_COLLECTION", "A new collection was not added.")
    }

    private fun collectionClear() {
        val homeViewModel = ViewModelProvider(this).get(HomeViewModel::class.java)
        homeViewModel.clearCollections().invokeOnCompletion {
            Log.w("NEW_COLLECTION", "All collection was cleared.")
        }
    }
}
