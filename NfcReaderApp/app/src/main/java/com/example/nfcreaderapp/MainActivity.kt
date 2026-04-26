package com.example.nfcreaderapp

import android.app.PendingIntent
import android.content.Intent
import android.nfc.NfcAdapter
import android.nfc.NdefMessage
import android.nfc.NdefRecord
import android.nfc.NfcAdapter.EXTRA_NDEF_MESSAGES
import android.os.Build
import android.os.Bundle
import android.widget.TextView
import androidx.activity.ComponentActivity
import java.io.OutputStreamWriter
import java.net.HttpURLConnection
import java.net.URL
import kotlin.concurrent.thread
import android.nfc.Tag
import android.nfc.tech.Ndef
import android.widget.Toast

class MainActivity : ComponentActivity() {

    private var nfcAdapter: NfcAdapter? = null
    private lateinit var textView: TextView
    private lateinit var editText: android.widget.EditText
    private lateinit var writeButton: android.widget.Button
    private val serverUrl = "htt
    private var writeMode = false
    private var currentTag: Tag? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val layout = android.widget.LinearLayout(this).apply {
            orientation = android.widget.LinearLayout.VERTICAL
            setPadding(40, 80, 40, 40)
        }

        textView = TextView(this).apply {
            text = "Scan a card"
            textSize = 24f
        }

        editText = android.widget.EditText(this).apply {
            hint = "patient:test-001"
        }

        writeButton = android.widget.Button(this).apply {
            text = "Write card"
        }

        editText.visibility = android.view.View.GONE
        writeButton.visibility = android.view.View.VISIBLE

        writeButton.setOnClickListener {
            writeMode = !writeMode

            if (writeMode) {
                textView.text = "Enter data, then tap card"
                editText.visibility = android.view.View.VISIBLE
                writeButton.text = "Read card"
            } else {
                textView.text = "Scan a card"
                editText.visibility = android.view.View.GONE
                editText.text.clear()
                writeButton.text = "Write card"
            }
        }

        layout.addView(textView)
        layout.addView(editText)
        layout.addView(writeButton)

        setContentView(layout)

        nfcAdapter = NfcAdapter.getDefaultAdapter(this)

        handleNfcIntent(intent)
    }

    override fun onResume() {
        super.onResume()

        val intent = Intent(this, javaClass).addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP)

        val pendingIntent = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            PendingIntent.getActivity(
                this,
                0,
                intent,
                PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_MUTABLE
            )
        } else {
            PendingIntent.getActivity(
                this,
                0,
                intent,
                PendingIntent.FLAG_UPDATE_CURRENT
            )
        }

        nfcAdapter?.enableForegroundDispatch(this, pendingIntent, null, null)
    }

    override fun onPause() {
        super.onPause()
        nfcAdapter?.disableForegroundDispatch(this)
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        handleNfcIntent(intent)
    }

    private fun handleNfcIntent(intent: Intent) {
        currentTag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG)
        val rawMessages = intent.getParcelableArrayExtra(EXTRA_NDEF_MESSAGES)

        if (rawMessages != null && rawMessages.isNotEmpty()) {
            val message = rawMessages[0] as NdefMessage
            val record = message.records[0]
            val payload = record.payload

            val text = if (payload.isNotEmpty()) {
                String(payload.copyOfRange(3, payload.size), Charsets.UTF_8)
            } else {
                "Empty tag"
            }

            if (writeMode) {
                val dataToWrite = editText.text.toString()

                if (dataToWrite.isNotEmpty()) {
                    writeToTag(dataToWrite, currentTag)

                    writeMode = false
                    editText.visibility = android.view.View.GONE
                    editText.text.clear()
                    writeButton.text = "Write card"
                } else {
                    Toast.makeText(this, "Enter data first", Toast.LENGTH_SHORT).show()
                }

                return
            }

            textView.text = "Scanned: $text"
            sendToServer(text)
        }
    }

    private fun sendToServer(data: String) {
        thread {
            try {
                val url = URL(serverUrl)
                val conn = url.openConnection() as HttpURLConnection

                conn.requestMethod = "POST"
                conn.doOutput = true
                conn.setRequestProperty("Content-Type", "text/plain")

                val writer = OutputStreamWriter(conn.outputStream)
                writer.write(data)
                writer.flush()
                writer.close()

                conn.responseCode // trigger request

            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    private fun writeToTag(text: String, tag: Tag?) {
        try {
            val ndef = Ndef.get(tag)
            if (ndef != null) {
                ndef.connect()

                val record = NdefRecord.createTextRecord("en", text)
                val message = NdefMessage(arrayOf(record))

                ndef.writeNdefMessage(message)
                ndef.close()

                runOnUiThread {
                    Toast.makeText(this, "Written successfully", Toast.LENGTH_SHORT).show()
                    textView.text = "Scan a card"
                }
            } else {
                runOnUiThread {
                    Toast.makeText(this, "Tag not supported", Toast.LENGTH_SHORT).show()
                }
            }
        } catch (e: Exception) {
            e.printStackTrace()
            runOnUiThread {
                Toast.makeText(this, "Write failed", Toast.LENGTH_SHORT).show()
            }
        }
    }
}