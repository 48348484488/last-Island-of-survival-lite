/* Minimal ARM64 JNI diagnostic. No Android hooks, no game code, no imports. */
__attribute__((visibility("default")))
int Java_org_oxideprobe_ProbeActivity_nativeCheck(void *jni_env, void *activity) {
    (void)jni_env;
    (void)activity;
    return 42;
}
