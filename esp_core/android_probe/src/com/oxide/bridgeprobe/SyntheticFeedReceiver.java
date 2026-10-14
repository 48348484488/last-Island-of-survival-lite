package com.oxide.bridgeprobe;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

/** Privileged-shell-only synthetic test input. Not an Oxide data reader. */
public final class SyntheticFeedReceiver extends BroadcastReceiver {
    private static final String TAG="OXIDE_BRIDGE_FEED";
    static final String ACTION="com.oxide.bridgeprobe.SYNTHETIC_FEED";
    @Override public void onReceive(Context context, Intent intent) {
        if(intent==null || !ACTION.equals(intent.getAction()) ||
           !intent.hasExtra("seq")) {
            Log.w(TAG,"RESULT=DROP INVALID_ACTION");
            return;
        }
        final long seq=intent.getLongExtra("seq",-1);
        final boolean stop=intent.getBooleanExtra("stop",false);
        final float x=intent.getFloatExtra("x",0f);
        final long age=intent.getLongExtra("age_ms",0);
        OverlayProbeService.supplySyntheticFeed(seq,x,age,stop);
    }
}
