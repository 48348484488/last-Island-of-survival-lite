package com.oxide.bridgeprobe;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.widget.TextView;

/**
 * Diagnostic app: only loads the standalone ESP math bridge.
 * Does not start, alter, read or inject into Oxide: Survival Island.
 */
public final class MainActivity extends Activity {
    private static final String TAG = "OXIDE_BRIDGE_TEST";
    static {
        System.loadLibrary("oxide_esp_probe");
    }

    private static native int runNativeSmoke();

    @Override public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        int result = runNativeSmoke();
        String verdict = result == 0 ? "PASS" : "FAIL";
        String marker = "RESULT=" + verdict + " CODE=" + result;
        Log.i(TAG, marker);
        TextView label = new TextView(this);
        label.setText("Android ARM64 native bridge: " + marker);
        label.setTextSize(20.0f);
        setContentView(label);
    }
}
