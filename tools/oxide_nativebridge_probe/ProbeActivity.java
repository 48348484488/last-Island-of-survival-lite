package org.oxideprobe;

import android.app.Activity;
import android.os.Bundle;
import android.os.Build;
import android.util.Log;
import android.widget.TextView;

/** Only tests whether ART can load and invoke one arm64-v8a JNI function. */
public final class ProbeActivity extends Activity {
    private static final String TAG = "OXIDE_ARM64_BRIDGE";
    private native int nativeCheck();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        TextView view = new TextView(this);
        String result;
        try {
            System.loadLibrary("oxideprobe");
            int actual = nativeCheck();
            if (actual == 42) {
                result = "ARM64_JNI_EXECUTION_VERIFIED";
                Log.i(TAG, result + " value=" + actual + " supported_abis="
                      + java.util.Arrays.toString(Build.SUPPORTED_ABIS));
            } else {
                result = "ARM64_JNI_WRONG_RESULT";
                Log.e(TAG, result + " value=" + actual);
            }
        } catch (Throwable error) {
            result = "ARM64_JNI_EXECUTION_FAILED";
            Log.e(TAG, result, error);
        }
        view.setText(result);
        setContentView(view);
    }
}
