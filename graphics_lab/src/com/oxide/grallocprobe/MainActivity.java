package com.oxide.grallocprobe;

import android.app.Activity;
import android.os.Bundle;
import android.os.Build;
import android.hardware.HardwareBuffer;
import android.util.Log;

/** Standalone native Android graphics allocation compatibility experiment.
 *  Absolutely no Oxide game SDK, APK, JNI injection, or player telemetry.
 */
public final class MainActivity extends Activity {
    public static final String TAG = "OXIDE_GRALLOC_PROBE";
    private static final long EXACT_ORIGINAL_USAGE = 0xb00L;
    private static final long GPU_IMAGE_AND_RENDER = 0x300L;

    private static void one(String name, int format, long usage) {
        boolean supported = false;
        String result = "UNKNOWN";
        String failure = "NONE";
        HardwareBuffer handle = null;
        try {
            supported = HardwareBuffer.isSupported(4, 4, format, 1, usage);
            handle = HardwareBuffer.create(4, 4, format, 1, usage);
            result = "PASS";
        } catch (IllegalArgumentException e) {
            result = "FAIL";
            failure = "ILLEGAL_ARGUMENT";
        } catch (UnsupportedOperationException e) {
            result = "FAIL";
            failure = "UNSUPPORTED_OPERATION";
        } catch (RuntimeException e) {
            result = "FAIL";
            failure = e.getClass().getSimpleName();
        } finally {
            if (handle != null) {
                handle.close();
            }
        }
        Log.i(TAG, "CASE=" + name
                + " FORMAT=" + format
                + " WIDTH=4 HEIGHT=4 LAYERS=1"
                + " USAGE=0x" + Long.toHexString(usage)
                + " SUPPORTED=" + supported
                + " CREATE=" + result
                + " FAILURE=" + failure);
    }

    @Override public void onCreate(Bundle state) {
        super.onCreate(state);
        Log.i(TAG, "STAGE=START API=" + Build.VERSION.SDK_INT
                + " ABIS=" + java.util.Arrays.toString(Build.SUPPORTED_ABIS)
                + " ORIGINAL_GAME_INSTALLED=false");
        if (Build.VERSION.SDK_INT < 29) {
            Log.e(TAG, "STAGE=UNSUPPORTED_ANDROID_API");
            finish();
            return;
        }
        // Exact layout reported in the unmodified original Oxide's
        // Android 13 Redroid GraphicBufferAllocator logs.
        one("RGBA1010102_EXACT", 43, EXACT_ORIGINAL_USAGE);
        one("R8_EXACT", 56, EXACT_ORIGINAL_USAGE);
        // Control: whether this is generic allocator failure or
        // specifically sensitive to the format and/or usage flags.
        one("RGBA8888_EXACT_USAGE_CONTROL", 1, EXACT_ORIGINAL_USAGE);
        one("RGBA8888_GPU_CONTROL", 1, GPU_IMAGE_AND_RENDER);
        one("RGBA1010102_GPU_CONTROL", 43, GPU_IMAGE_AND_RENDER);
        one("R8_GPU_CONTROL", 56, GPU_IMAGE_AND_RENDER);
        Log.i(TAG, "STAGE=COMPLETE CASES=6 ORIGINAL_GAME_TOUCHED=false");
        finish();
    }
}
