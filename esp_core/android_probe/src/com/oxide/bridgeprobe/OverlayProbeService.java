package com.oxide.bridgeprobe;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.os.SystemClock;
import android.provider.Settings;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Color;
import android.graphics.PixelFormat;
import android.util.Log;
import android.view.View;
import android.view.Gravity;
import android.view.WindowManager;

/**
 * Controlled graphical overlay test on the Android home screen.
 * Never reads Oxide, other processes, or game state.
 */
public final class OverlayProbeService extends Service {
    private static final String TAG="OXIDE_BRIDGE_OVERLAY";
    private WindowManager manager;
    private OverlayView view;
    private final Handler sequenceHandler = new Handler(Looper.getMainLooper());
    private boolean sequenceEnabled;
    private boolean feedEnabled;
    private long lastFeedSequence=-1;
    private static OverlayProbeService activeFeed;
    private static final long FEED_IDLE_LIMIT_MS=4000;
    private final Runnable feedIdleTimeout=() -> {
        if(feedEnabled && view!=null) {
            view.setRects(new float[0]);
            Log.i(TAG,"RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0");
        }
    };
    private void resetFeedExpiry() {
        sequenceHandler.removeCallbacks(feedIdleTimeout);
        sequenceHandler.postDelayed(feedIdleTimeout,FEED_IDLE_LIMIT_MS);
    }

    @Override public IBinder onBind(Intent intent) { return null; }

    @Override public int onStartCommand(Intent intent, int flags, int startId) {
        if (view != null) return START_NOT_STICKY;
        if (Build.VERSION.SDK_INT >= 23 && !Settings.canDrawOverlays(this)) {
            Log.e(TAG,"RESULT=FAIL PERMISSION_DENIED");
            stopSelf(startId);
            return START_NOT_STICKY;
        }

        // Deterministic test modes exercise the actual overlay Service,
        // not just MainActivity's canvas. No other app data is read.
        final boolean noSamples=intent!=null &&
                intent.getBooleanExtra("overlay_no_boxes",false);
        final boolean staleSamples=intent!=null &&
                intent.getBooleanExtra("overlay_stale_boxes",false);
        sequenceEnabled=intent!=null && intent.getBooleanExtra("overlay_sequence",false);
        feedEnabled=intent!=null && intent.getBooleanExtra("overlay_feed",false);
        if(sequenceEnabled && feedEnabled) {
            Log.e(TAG,"RESULT=FAIL CONFLICTING_OVERLAY_SEQUENCE_AND_FEED");
            stopSelf(startId);
            return START_NOT_STICKY;
        }
        if ((noSamples && staleSamples) || (sequenceEnabled && (noSamples || staleSamples))) {
            Log.e(TAG,"RESULT=FAIL CONTRADICTORY_TEST_MODES");
            stopSelf(startId);
            return START_NOT_STICKY;
        }
        final String mode=noSamples?"OVERLAY_NO_SAMPLES":
                staleSamples?"OVERLAY_STALE_SAMPLES":"OVERLAY_DIAGNOSTIC";
        long now=SystemClock.elapsedRealtime();
        float[] coords=MainActivity.nativeProjectSnapshot(
                MainActivity.testCamera(),1080,1920,now,now-5,250,0,
                noSamples?new long[0]:new long[]{1},
                noSamples?new float[0]:new float[]{0,1,-5,0,-1,-5},
                noSamples?new long[0]:new long[]{now-(staleSamples?600:5)});
        final int expectedCount=(noSamples || staleSamples)?0:4;
        if (coords==null || coords.length!=expectedCount) {
            Log.e(TAG,"RESULT=FAIL NATIVE_PROJECTION MODE="+mode);
            stopSelf(startId);
            return START_NOT_STICKY;
        }
        view = new OverlayView(coords);
        WindowManager.LayoutParams p=new WindowManager.LayoutParams(
                WindowManager.LayoutParams.MATCH_PARENT,
                WindowManager.LayoutParams.MATCH_PARENT,
                WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY,
                WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE |
                WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE |
                WindowManager.LayoutParams.FLAG_LAYOUT_IN_SCREEN,
                PixelFormat.TRANSLUCENT);
        p.gravity=Gravity.TOP | Gravity.START;
        manager=(WindowManager)getSystemService(WINDOW_SERVICE);
        if (manager==null) {
            Log.e(TAG,"RESULT=FAIL WINDOW_SERVICE");
            view=null;
            stopSelf(startId);
            return START_NOT_STICKY;
        }
        try {
            manager.addView(view,p);
            Log.i(TAG,"RESULT=PASS MODE="+mode+" RECT_COUNT="+
                  (coords.length/4)+" SOURCE=CONTROLLED_SYNTHETIC_ONLY");
            if(sequenceEnabled || feedEnabled) {
                Log.i(TAG,"RESULT=PASS PHASE=INITIAL RECT_COUNT=1 SOURCE=SYNTHETIC_JNI");
            }
            if(feedEnabled) {
                activeFeed=this;
                lastFeedSequence=-1;
                Log.i(TAG,"RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY");
                resetFeedExpiry();
            }
            if(sequenceEnabled) {
                sequenceHandler.postDelayed(() -> projectSequence("MOVED",1f,false),3000);
                sequenceHandler.postDelayed(() -> projectSequence("STALE",1f,true),6000);
                sequenceHandler.postDelayed(() -> projectSequence("RESTORED",-1f,false),9000);
                sequenceHandler.postDelayed(() -> {
                    Log.i(TAG,"PHASE=STOP_REQUEST SOURCE=SYNTHETIC_JNI");
                    stopSelf();
                },12000);
            }
        } catch (RuntimeException e) {
            Log.e(TAG,"RESULT=FAIL ADD_VIEW "+e.getClass().getSimpleName());
            view=null;
            stopSelf(startId);
        }
        return START_NOT_STICKY;
    }

    static void supplySyntheticFeed(long sequence, float x, long ageMs, boolean stop,
                                    boolean hasSecond, float x2, long age2Ms) {
        OverlayProbeService service=activeFeed;
        if(service==null || !service.feedEnabled || service.view==null) {
            Log.w(TAG,"RESULT=DROP FEED_NOT_ACTIVE");
            return;
        }
        // The diagnostic receiver is permission-restricted to Android DUMP
        // (ADB shell / privileged callers). These are NEVER real game players.
        if(sequence<0 || sequence<=service.lastFeedSequence) {
            Log.w(TAG,"RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ="+sequence);
            return;
        }
        if(stop) {
            service.lastFeedSequence=sequence;
            Log.i(TAG,"RESULT=PASS FEED_STOP_REQUEST SEQ="+sequence);
            service.stopSelf();
            return;
        }
        if(!Float.isFinite(x) || x < -1.5f || x > 1.5f ||
                ageMs<0 || ageMs>10000 ||
                (hasSecond && (!Float.isFinite(x2) || x2 < -1.5f || x2 > 1.5f ||
                               age2Ms<0 || age2Ms>10000))) {
            Log.w(TAG,"RESULT=DROP INVALID_INPUT SEQ="+sequence);
            return;
        }
        final long now=SystemClock.elapsedRealtime();
        if(now<ageMs+10 || (hasSecond && now<age2Ms+10)) {
            Log.w(TAG,"RESULT=DROP CLOCK_UNDERFLOW SEQ="+sequence);
            return;
        }
        final float[] result=MainActivity.nativeProjectSnapshot(
                MainActivity.testCamera(),1080,1920,now,now-5,250,0,
                hasSecond?new long[]{1,2}:new long[]{1},
                hasSecond?new float[]{x,1,-5,x,-1,-5,
                                      x2,1,-5,x2,-1,-5}
                         :new float[]{x,1,-5,x,-1,-5},
                hasSecond?new long[]{now-ageMs,now-age2Ms}
                         :new long[]{now-ageMs});
        final int expected=(ageMs>250?0:4)+(hasSecond && age2Ms<=250?4:0);
        if(result==null || result.length!=expected) {
            Log.e(TAG,"RESULT=FAIL FEED_INVALID_PROJECTION SEQ="+sequence);
            return;
        }
        service.lastFeedSequence=sequence;
        service.view.setRects(result);
        service.resetFeedExpiry();
        final String phase=ageMs>250?"STALE":
                           sequence==1?"MOVED":"RESTORED";
        Log.i(TAG,"RESULT=PASS PHASE="+phase+" RECT_COUNT="+
              (result.length/4)+" SOURCE=SYNTHETIC_JNI");
        Log.i(TAG,"RESULT=PASS FEED_SEQ="+sequence+" AGE_MS="+ageMs+
                   " RECT_COUNT="+(result.length/4)+" SYNTHETIC_ONLY");
        if(hasSecond) {
            Log.i(TAG,"RESULT=PASS FEED_MULTI_SEQ="+sequence+
                      " RECT_COUNT="+(result.length/4)+" SYNTHETIC_ONLY");
        }
    }

    private void projectSequence(String phase, float x, boolean stale) {
        if (!sequenceEnabled || view==null) return;
        final long now=SystemClock.elapsedRealtime();
        final float[] result=MainActivity.nativeProjectSnapshot(
                MainActivity.testCamera(),1080,1920,now,now-5,250,0,
                new long[]{1},
                new float[]{x,1,-5,x,-1,-5},
                new long[]{now-(stale?600:5)});
        if (result==null || result.length!=(stale?0:4)) {
            Log.e(TAG,"RESULT=FAIL PHASE="+phase+" INVALID_NATIVE_SNAPSHOT");
            stopSelf();
            return;
        }
        view.setRects(result);
        Log.i(TAG,"RESULT=PASS PHASE="+phase+" RECT_COUNT="+
              (result.length/4)+" SOURCE=SYNTHETIC_JNI");
    }

    @Override public void onDestroy() {
        sequenceHandler.removeCallbacksAndMessages(null);
        if(activeFeed==this) activeFeed=null;
        if(manager!=null && view!=null) {
            try { manager.removeViewImmediate(view); }
            catch(RuntimeException e) { Log.w(TAG,"REMOVE_VIEW_ERROR",e); }
        }
        manager=null;
        view=null;
        if(sequenceEnabled || feedEnabled) Log.i(TAG,"RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE");
        feedEnabled=false;
        sequenceEnabled=false;
        super.onDestroy();
    }

    private final class OverlayView extends View {
        private float[] coords;
        private final Paint paint=new Paint(Paint.ANTI_ALIAS_FLAG);
        OverlayView(float[] c) { super(OverlayProbeService.this);coords=c.clone(); }
        void setRects(float[] updated) {
            coords=updated.clone();
            invalidate();
        }
        @Override protected void onDraw(Canvas canvas) {
            super.onDraw(canvas);
            if(coords.length%4!=0 || coords.length>256*4) return;
            canvas.save();
            canvas.scale(getWidth()/1080f,getHeight()/1920f);
            paint.setStyle(Paint.Style.STROKE);
            paint.setStrokeWidth(5);
            paint.setColor(Color.rgb(38,255,110));
            for(int i=0;i<coords.length;i+=4) {
                canvas.drawRect(coords[i],coords[i+1],coords[i+2],coords[i+3],paint);
            }
            paint.setStyle(Paint.Style.FILL);
            paint.setColor(Color.WHITE);
            paint.setTextSize(28);
            canvas.drawText("DIAGNOSTIC OVERLAY - NO GAME DATA",25,125,paint);
            canvas.restore();
        }
    }
}
