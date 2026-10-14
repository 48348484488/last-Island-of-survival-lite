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
            // Typed batched protocol: caller supplies the complete matrix,
            // entity IDs and 3D head/feet coordinates in a single snapshot.
            // Legacy x/x2 single-target messages continue unchanged.
            boolean batch=!stop && (extras.containsKey("batch_ids") ||
                    extras.containsKey("batch_xyz") || extras.containsKey("batch_ages") ||
                    extras.containsKey("batch_camera"));
            if(batch) {
                if(!(extras.get("batch_ids") instanceof long[]) ||
                        !(extras.get("batch_xyz") instanceof float[]) ||
                        !(extras.get("batch_ages") instanceof long[]) ||
                        !(extras.get("batch_camera") instanceof float[]) ||
                        !(extras.get("batch_camera_age") instanceof Long) ||
                        !(extras.get("batch_width") instanceof Integer) ||
                        !(extras.get("batch_height") instanceof Integer) ||
                        (extras.containsKey("batch_flags") &&
                         !(extras.get("batch_flags") instanceof Integer))) {
                    Log.w(TAG,"RESULT=DROP MALFORMED_BATCH");
                    return;
                }
                OverlayProbeService.supplySyntheticBatch(
                        seq,intent.getFloatArrayExtra("batch_camera"),
                        intent.getIntExtra("batch_width",0),
                        intent.getIntExtra("batch_height",0),
                        intent.getIntExtra("batch_flags",0),
                        intent.getLongExtra("batch_camera_age",-1),
                        intent.getLongArrayExtra("batch_ids"),
                        intent.getFloatArrayExtra("batch_xyz"),
                        intent.getLongArrayExtra("batch_ages"));
                return;
            }
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
