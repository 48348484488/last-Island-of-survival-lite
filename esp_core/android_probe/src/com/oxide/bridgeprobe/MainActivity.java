package com.oxide.bridgeprobe;

import android.app.Activity;
import android.os.Bundle;
import android.content.Intent;
import android.os.SystemClock;
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
    private static native long nativeReadCpuIdPfr1();
    // Production-shaped JNI boundary: all values come from the Java caller.
    // This diagnostic app supplies CONTROLLED input, never game data.
    static native float[] nativeProjectSnapshot(
            float[] viewProjection, int width, int height,
            long nowMs, long cameraMs, int maxAgeMs, int matrixFlags,
            long[] entityIds, float[] headFeetXYZ, long[] entitySampleMs);

    static float[] testCamera() {
        final float near=0.1f, far=100.0f;
        return new float[]{
                1,0,0,0, 0,1,0,0,
                0,0,(far+near)/(near-far),(2*far*near)/(near-far),
                0,0,-1,0
        };
    }

    // Ensure invalid/stale data cannot accidentally create stale markers.
    private static boolean verifyInputBoundary(long now) {
        float[] camera = testCamera();
        long[] ids={1};
        float[] coords={0,1,-5,0,-1,-5};
        long[] samples={now-5};
        float[] valid=nativeProjectSnapshot(camera,1080,1920,now,now-5,
                                             250,0,ids,coords,samples);
        if (valid==null || valid.length!=4) return false;
        samples[0]=now-251;
        float[] stale=nativeProjectSnapshot(camera,1080,1920,now,now-5,
                                             250,0,ids,coords,samples);
        if (stale==null || stale.length!=0) return false;
        float[] empty=nativeProjectSnapshot(camera,1080,1920,now,now-5,
                                             250,0,new long[0],new float[0],new long[0]);
        if (empty==null || empty.length!=0) return false;
        if (nativeProjectSnapshot(camera,1080,1920,now,now+1,
                     250,0,ids,coords,samples)!=null) return false;
        camera[0]=Float.NaN;
        if (nativeProjectSnapshot(camera,1080,1920,now,now-5,
                     250,0,ids,coords,samples)!=null) return false;
        camera=testCamera();
        if (nativeProjectSnapshot(camera,1080,1920,now,now-5,
                     250,0,new long[]{1,2},coords,new long[]{now-5,now-5})!=null)
                     return false; // mismatched player and position array sizes
        if (nativeProjectSnapshot(camera,1080,1920,now,now-5,
                     250,0,new long[]{1,1},new float[]{0,1,-5,0,-1,-5, 0,1,-5,0,-1,-5},
                     new long[]{now-5,now-5})!=null) return false;
        return true;
    }

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
        // Isolated ARM64 CPU ID compatibility experiment. Never touches Oxide.
        if (getIntent().getBooleanExtra("cpu_id_probe", false)) {
            Log.i("OXIDE_CPU_ID_PROBE", "STAGE=BEFORE_MRS_ID_AA64PFR1_EL1");
            long value = nativeReadCpuIdPfr1();
            Log.i("OXIDE_CPU_ID_PROBE", "STAGE=MRS_RETURNED VALUE=0x" +
                  Long.toHexString(value));
            finish();
            return;
        }
        final int nativeResult=runNativeSmoke();
        final long now=SystemClock.elapsedRealtime();
        final boolean contractsOk=nativeResult==0 && verifyInputBoundary(now);
        final boolean noSamples=getIntent().getBooleanExtra("no_boxes",false);
        final boolean staleSamples=getIntent().getBooleanExtra("stale_boxes",false);

        final long[] ids=noSamples?new long[0]:new long[]{1};
        final float[] coordinates=noSamples?new float[0]:
                new float[]{0,1,-5,0,-1,-5};
        final long[] ages=noSamples?new long[0]:
                new long[]{now-(staleSamples?600:5)};
        final float[] rects=contractsOk ?
                nativeProjectSnapshot(testCamera(),1080,1920,now,now-5,250,0,
                                      ids,coordinates,ages) : null;
        final boolean expectEmpty=noSamples || staleSamples;
        final boolean correct=rects!=null &&
                (expectEmpty ? rects.length==0 : rects.length==4);
        final int result=nativeResult!=0?nativeResult:(contractsOk && correct?0:11);
        final String verdict=result==0?"PASS":"FAIL";
        final String mode=result!=0?"NATIVE_FAIL":
                noSamples?"NO_SAMPLES":staleSamples?"STALE_SAMPLES":"TEST_RECT_PRESENT";
        Log.i(TAG, "RESULT=" + verdict + " CODE=" + result + " MODE=" + mode);
        setContentView(new NativeRectView(this, rects, mode));
        if (getIntent().getBooleanExtra("overlay_probe",false)) {
            // Test only synthetic snapshots. Native renderer and overlay
            // Service must BOTH clear markers for stale/missing samples.
            Intent service=new Intent(this,OverlayProbeService.class);
            service.putExtra("overlay_no_boxes",
                    getIntent().getBooleanExtra("overlay_no_boxes",false));
            service.putExtra("overlay_stale_boxes",
                    getIntent().getBooleanExtra("overlay_stale_boxes",false));
            startService(service);
            finish(); // Return to home: overlay is independent of the activity.
        }
    }
}
