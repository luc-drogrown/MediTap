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
import com.example.nfcreaderapp.R
class MainActivity : ComponentActivity() {

    private var nfcAdapter: NfcAdapter? = null
    private lateinit var textView: TextView
    private lateinit var editText: android.widget.EditText
    private lateinit var writeButton: android.widget.Button
    private val IP = "192.168.100.152";
    private val serverUrl = "http://$IP:5115/Nfc/Scan"
    private val pendingWriteUrl = "http://$IP:5115/Nfc/GetPendingWrite"
    private val confirmWriteUrl = "http://$IP:5115/Nfc/ConfirmWrite"
    private var writeMode = false
    private var currentTag: Tag? = null
    private var pendingPatientUname: String? = null
    private val handler = android.os.Handler(android.os.Looper.getMainLooper())
    private lateinit var pollRunnable: Runnable

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val layout = android.widget.LinearLayout(this).apply {
            orientation = android.widget.LinearLayout.VERTICAL
            setPadding(60, 100, 60, 60)
            setBackgroundColor(android.graphics.Color.parseColor("#EAF6F6"))
        }

        textView = TextView(this).apply {
            text = "Scan a card"
            textSize = 28f
            setTextColor(android.graphics.Color.parseColor("#0F8B8D"))
            setTypeface(null,android.graphics.Typeface.BOLD)
            gravity=android.view.Gravity.CENTER
            setPadding(0, 0, 0, 80)
        }

        editText = android.widget.EditText(this).apply {
            hint = "patient:test-001"
        }

        writeButton = android.widget.Button(this).apply {
            text = "Write patient card"
            isEnabled = false
            setBackgroundResource(R.drawable.bg_primary_button)
            setTextColor(android.graphics.Color.WHITE)
            isAllCaps=false
            textSize=16f
        }

        editText.visibility = android.view.View.GONE
        writeButton.visibility = android.view.View.VISIBLE

        writeButton.setOnClickListener {
            if (pendingPatientUname != null) {
                writeMode = true
                textView.text = "Tap NFC card to write patient data"
            } else {
                Toast.makeText(this, "No patient waiting for card writing", Toast.LENGTH_SHORT).show()
            }
        }

        layout.addView(textView)
        layout.addView(editText)
        layout.addView(writeButton)

        setContentView(layout)

        nfcAdapter = NfcAdapter.getDefaultAdapter(this)

        handleNfcIntent(intent)
        pollRunnable = Runnable{
            if(pendingPatientUname==null&& !writeMode){
                checkPendingWrite()
            }
            handler.postDelayed(pollRunnable, 3000)
        }
        handler.post(pollRunnable)

    }

    override fun onResume() {
        super.onResume()
        checkNfcState()

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


        if (writeMode && currentTag != null) {
            val dataToWrite = pendingPatientUname

            if (!dataToWrite.isNullOrEmpty()) {
                writeToTag(dataToWrite, currentTag)

                writeMode = false
                pendingPatientUname = null
                writeButton.isEnabled = false
                writeButton.text = "Write patient card"
            } else {
                Toast.makeText(this, "No patient data available", Toast.LENGTH_SHORT).show()
            }

            return
        }


        if (rawMessages != null && rawMessages.isNotEmpty()) {
            val message = rawMessages[0] as NdefMessage
            val record = message.records[0]
            val payload = record.payload

            val text = if (payload.isNotEmpty()) {
                String(payload.copyOfRange(3, payload.size), Charsets.UTF_8)
            } else {
                "Empty tag"
            }

            textView.text = "Scanned: $text"
            triggerHapticFeedback("SUCCESS")
            sendToServer(text)
        }else if (currentTag != null){
            textView.text = "Scanned: Blank Card"
            triggerHapticFeedback("ERROR")
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
                confirmWriteToServer()

                runOnUiThread {
                    Toast.makeText(this, "Written successfully", Toast.LENGTH_SHORT).show()
                    textView.text = "Scan a card"
                    triggerHapticFeedback("WRITE")
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

    private fun checkPendingWrite() {
        /*   pendingPatientUname="Teiubesc"

           runOnUiThread {
               textView.text = "Ready to write patient card"
               writeButton.isEnabled=true
           }*/
           thread {
               try {
                   val url = URL(pendingWriteUrl)
                   val conn = url.openConnection() as HttpURLConnection

                   conn.requestMethod = "GET"

                   if (conn.responseCode == 200) {
                       val response = conn.inputStream.bufferedReader().readText()

                       val json = org.json.JSONObject(response)
                       val patientUname = json.getString("patientUname")

                       pendingPatientUname = patientUname

                       runOnUiThread {
                           textView.text = "Ready to write patient card"
                           writeButton.isEnabled = true
                       }
                   } else {
                       pendingPatientUname = null

                       runOnUiThread {
                           textView.text = "No patient waiting for card writing"
                           writeButton.isEnabled = false
                       }
                   }

               } catch (e: Exception) {
                   e.printStackTrace()
               }
           }


    }

    private fun confirmWriteToServer() {
        thread {
            try {
                val url = URL(confirmWriteUrl)
                val conn = url.openConnection() as HttpURLConnection

                conn.requestMethod = "POST"
                conn.doOutput = true

                conn.responseCode

            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }
    private fun checkNfcState() {
        val nfcManager = getSystemService(android.content.Context.NFC_SERVICE) as android.nfc.NfcManager
        val defaultAdapter = nfcManager.defaultAdapter

        if (defaultAdapter == null) {
            // The phone literally doesn't have an NFC chip
            runOnUiThread {
                textView.text = "ERROR: This device does not support NFC."
                textView.setTextColor(android.graphics.Color.RED)
                writeButton.isEnabled = false
            }
        } else if (!defaultAdapter.isEnabled) {
            // NFC is turned off in settings. Prompt them to turn it on!
            android.app.AlertDialog.Builder(this)
                .setTitle("NFC is Disabled")
                .setMessage("MediTap requires NFC to be turned on. Would you like to go to your settings to enable it now?")
                .setPositiveButton("Go to Settings") { _, _ ->
                    val intent = android.content.Intent(android.provider.Settings.ACTION_NFC_SETTINGS)
                    startActivity(intent)
                }
                .setNegativeButton("Cancel") { dialog, _ ->
                    dialog.dismiss()
                    textView.text = "NFC must be enabled to use MediTap."
                }
                .setCancelable(false)
                .show()
        }
    }
    private fun triggerHapticFeedback(type: String) {
        val vibrator = getSystemService(android.content.Context.VIBRATOR_SERVICE) as android.os.Vibrator
        if (!vibrator.hasVibrator()) return

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            val effect = when (type) {
                "SUCCESS" -> android.os.VibrationEffect.createOneShot(150, android.os.VibrationEffect.DEFAULT_AMPLITUDE)
                "WRITE" -> android.os.VibrationEffect.createOneShot(500, 255) // Longer, stronger buzz
                "ERROR" -> android.os.VibrationEffect.createWaveform(longArrayOf(0, 100, 50, 100), -1) // Double buzz
                else -> android.os.VibrationEffect.createOneShot(100, android.os.VibrationEffect.DEFAULT_AMPLITUDE)
            }
            vibrator.vibrate(effect)
        } else {
            // Fallback for older phones
            @Suppress("DEPRECATION")
            when (type) {
                "SUCCESS" -> vibrator.vibrate(150)
                "WRITE" -> vibrator.vibrate(500)
                "ERROR" -> vibrator.vibrate(longArrayOf(0, 100, 50, 100), -1)
            }
        }
    }
}

