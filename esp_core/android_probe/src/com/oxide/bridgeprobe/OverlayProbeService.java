package com.oxide.bridgeprobe;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;
import android.os.Build;
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
    private View view;

    @Override public IBinder onBind(Intent intent) { return null; }

    @Override public int onStartCommand(Intent intent, int flags, int startId) {
        if (view != null) return START_NOT_STICKY;
        if (Build.VERSION.SDK_INT >= 23 && !Settings.canDrawOverlays(this)) {
            Log.e(TAG,"RESULT=FAIL PERMISSION_DENIED");
            stopSelf(startId);
            return START_NOT_STICKY;
        }

        long now=SystemClock.elapsedRealtime();
        float[] coords=MainActivity.nativeProjectSnapshot(
                MainActivity.testCamera(),1080,1920,now,now-5,250,0,
                new long[]{1},new float[]{0,1,-5,0,-1,-5},new long[]{now-5});
        if (coords==null || coords.length!=4) {
            Log.e(TAG,"RESULT=FAIL NATIVE_PROJECTION");
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
            Log.i(TAG,"RESULT=PASS MODE=OVERLAY_DIAGNOSTIC");
        } catch (RuntimeException e) {
            Log.e(TAG,"RESULT=FAIL ADD_VIEW "+e.getClass().getSimpleName());
            view=null;
            stopSelf(startId);
        }
        return START_NOT_STICKY;
    }

    @Override public void onDestroy() {
        if(manager!=null && view!=null) {
            try { manager.removeViewImmediate(view); }
            catch(RuntimeException e) { Log.w(TAG,"REMOVE_VIEW_ERROR",e); }
        }
        manager=null;
        view=null;
        super.onDestroy();
    }

    private final class OverlayView extends View {
        private final float[] coords;
        private final Paint paint=new Paint(Paint.ANTI_ALIAS_FLAG);
        OverlayView(float[] c) { super(OverlayProbeService.this);coords=c.clone(); }
        @Override protected void onDraw(Canvas canvas) {
            super.onDraw(canvas);
            if(coords.length!=4) return;
            canvas.save();
            canvas.scale(getWidth()/1080f,getHeight()/1920f);
            paint.setStyle(Paint.Style.STROKE);
            paint.setStrokeWidth(5);
            paint.setColor(Color.rgb(38,255,110));
            canvas.drawRect(coords[0],coords[1],coords[2],coords[3],paint);
            paint.setStyle(Paint.Style.FILL);
            paint.setColor(Color.WHITE);
            paint.setTextSize(28);
            canvas.drawText("DIAGNOSTIC OVERLAY - NO GAME DATA",25,125,paint);
            canvas.restore();
        }
    }
}
