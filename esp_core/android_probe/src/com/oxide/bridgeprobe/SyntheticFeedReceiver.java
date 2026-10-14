package com.oxide.bridgeprobe;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;
import android.os.Bundle;

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
        final long seq;
        final boolean stop;
        final float x;
        final long age;
        final boolean second;
        final float x2;
        final long age2;
        try {
            Bundle extras=intent.getExtras();
            if(extras==null || !(extras.get("seq") instanceof Long) ||
                    (extras.containsKey("stop") && !(extras.get("stop") instanceof Boolean))) {
                Log.w(TAG,"RESULT=DROP MALFORMED_FIELDS");
                return;
            }
            seq=intent.getLongExtra("seq",-1);
            stop=intent.getBooleanExtra("stop",false);
            if(!stop && (!intent.hasExtra("x") || !intent.hasExtra("age_ms"))) {
                Log.w(TAG,"RESULT=DROP MISSING_REQUIRED_FIELDS SEQ="+seq);
                return;
            }
            if(!stop && (!(extras.get("x") instanceof Float) ||
                    !(extras.get("age_ms") instanceof Long))) {
                Log.w(TAG,"RESULT=DROP MALFORMED_FIELDS");
                return;
            }
            second=!stop && (extras.containsKey("x2") || extras.containsKey("age2_ms"));
            if(second && (!(extras.get("x2") instanceof Float) ||
                    !(extras.get("age2_ms") instanceof Long))) {
                Log.w(TAG,"RESULT=DROP MALFORMED_SECOND_TARGET");
                return;
            }
            x=stop?0f:intent.getFloatExtra("x",Float.NaN);
            age=stop?0L:intent.getLongExtra("age_ms",-1L);
            x2=second?intent.getFloatExtra("x2",Float.NaN):0f;
            age2=second?intent.getLongExtra("age2_ms",-1L):0L;
        } catch (ClassCastException | IllegalArgumentException e) {
            // A malformed broadcast cannot crash our diagnostic Service.
            Log.w(TAG,"RESULT=DROP MALFORMED_FIELDS");
            return;
        }
        OverlayProbeService.supplySyntheticFeed(seq,x,age,stop,second,x2,age2);
    }
}
