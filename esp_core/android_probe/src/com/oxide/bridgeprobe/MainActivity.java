package com.oxide.bridgeprobe;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.content.Context;
import android.view.View;

/**
 * Isolated Android graphics integration test. Never reads or modifies Oxide.
 * Explicitly uses a single deterministic test vector generated in JNI.
 */
public final class MainActivity extends Activity {
    private static final String TAG = "OXIDE_BRIDGE_TEST";
    static { System.loadLibrary("oxide_esp_probe"); }

    private static native int runNativeSmoke();
    private static native float[] nativeDebugRects();

    private static final class NativeRectView extends View {
        private final float[] rectangles;
        private final Paint paint = new Paint(Paint.ANTI_ALIAS_FLAG);
        private final String mode;
        NativeRectView(Context context, float[] rectangles, String mode) {
            super(context);
            this.rectangles = rectangles;
            this.mode = mode;
            setLayerType(View.LAYER_TYPE_SOFTWARE, null);
        }
        @Override protected void onDraw(Canvas canvas) {
            super.onDraw(canvas);
            canvas.drawColor(Color.rgb(12, 16, 23));
            paint.setColor(Color.WHITE);
            paint.setStyle(Paint.Style.FILL);
            paint.setTextSize(28.0f);
            canvas.drawText("DIAGNOSTIC ONLY - NOT GAME DATA", 24, 42, paint);
            canvas.drawText(mode, 24, 80, paint);

            if (rectangles == null || rectangles.length == 0) return;
            if (rectangles.length % 4 != 0 || rectangles.length > 4 * 256) return;
            canvas.save();
            canvas.scale(getWidth() / 1080.0f, getHeight() / 1920.0f);
            paint.setColor(Color.rgb(38, 255, 110));
            paint.setStyle(Paint.Style.STROKE);
            paint.setStrokeWidth(5.0f);
            for (int i=0; i < rectangles.length; i += 4) {
                float l=rectangles[i], t=rectangles[i+1];
                float r=rectangles[i+2], b=rectangles[i+3];
                if (!Float.isFinite(l) || !Float.isFinite(t) ||
                    !Float.isFinite(r) || !Float.isFinite(b)) continue;
                if (l < 0 || t < 0 || r > 1080 || b > 1920 || l >= r || t >= b) continue;
                canvas.drawRect(l,t,r,b,paint);
            }
            canvas.restore();
        }
    }

    @Override public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        final int result = runNativeSmoke();
        final String verdict = result == 0 ? "PASS" : "FAIL";
        final boolean noSamples = getIntent().getBooleanExtra("no_boxes", false);
        float[] rects = null;
        if (result == 0 && !noSamples) rects = nativeDebugRects();
        String mode = result != 0 ? "NATIVE FAIL" : noSamples ? "NO_SAMPLES" :
                rects != null && rects.length >= 4 ? "TEST_RECT_PRESENT" : "NO_SAMPLES";
        Log.i(TAG, "RESULT=" + verdict + " CODE=" + result + " MODE=" + mode);
        setContentView(new NativeRectView(this, rects, mode));
    }
}
