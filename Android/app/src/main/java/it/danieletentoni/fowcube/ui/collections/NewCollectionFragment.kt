package it.danieletentoni.fowcube.ui.collections

import android.app.Dialog
import android.content.Context
import android.content.DialogInterface
import android.os.Bundle
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import it.danieletentoni.fowcube.R

class NewCollectionFragment: DialogFragment() {
    // Use this instance of the interface to deliver action events
    internal lateinit var listener: NoticeDialogListener

    /* The activity that creates an instance of this dialog fragment must
     * implement this interface in order to receive event callbacks.
     * Each method passes the DialogFragment in case the host needs to query it. */
    interface NoticeDialogListener {
        fun onDialogPositiveClick(dialog: DialogFragment)
        fun onDialogNegativeClick(dialog: DialogFragment)
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val inflater = requireActivity().layoutInflater
            builder.setMessage(R.string.collection)
                .setView(inflater.inflate(R.layout.fragment_new_collection, null))
                .setPositiveButton(R.string.create, DialogInterface.OnClickListener { dialog, id ->
                    // Add collection.
                    listener.onDialogPositiveClick(this)
                }).setNegativeButton(R.string.dismiss, DialogInterface.OnClickListener { dialog, id ->
                    // Don't add to collections.
                    listener.onDialogNegativeClick(this)
                })
            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }

    // Override the Fragment.onAttach() method to instantiate the NoticeDialogListener
    override fun onAttach(context: Context) {
        super.onAttach(context)

        // Verify that the host activity implements the callback interface
        try {
            // Instantiate the NoticeDialogListener so we can send events to the host
            listener = context as NoticeDialogListener
        } catch (e: ClassCastException) {
            // The activity doesn't implement the interface, throw exception
            throw ClassCastException((context.toString() +
                    " must implement NoticeDialogListener"))
        }
    }
}